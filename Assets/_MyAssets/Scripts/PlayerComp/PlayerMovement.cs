using Cysharp.Threading.Tasks;
using Engine.PlayerComp.Interfaces;
using Engine.UnitComp.Consts;
using System;
using UnityEngine;

namespace Engine.PlayerComp
{
    public class PlayerMovement : MonoBehaviour, IPlayerMovement, IPlayerJump
    {
        public event Action OnStartMoving;
        public event Action OnStopMoving;
        public event Action<MovementData> OnMove;

        public event Action OnPlayerJump;

        public bool CanDoAnyAction { get; private set; }
        public bool IsJumping { get; private set; }
        public bool Grounded { get; private set; }
        public bool IsMoving { get; private set; }
        public bool IsSliding { get; private set; }
        public bool EdgeGrabbed { get; private set; }
        public bool IsFalling => _player.RigidBody.velocity.y < 0;

        [SerializeField]
        private float _colliderDetectDistance = .1f;

        [SerializeField]
        private LayerMask _solidLayer;

        private IPlayerUnit _player;
        private RaycastHit2D[] _hitColliders;
        private float _xDirection;
        private float _grabTime;

        private void Awake()
        {
            CanDoAnyAction = true;
            _hitColliders = new RaycastHit2D[3];
            _player = GetComponent<IPlayerUnit>();
        }
        private void Update()
        {
            if (!CanDoAnyAction)
                return;

            if (_player.AttackHandler.IsAttacking)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Slide().Forget();
                return;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                ReleaseGrab();
                return;
            }

        }
        private void FixedUpdate()
        {
            if (EdgeGrabbed)
                return;

            if (!CanDoAnyAction)
                return;

            if (_player.AttackHandler.IsAttacking)
                return;

            // IF FALLING DOWN.
            if (_player.RigidBody.velocity.y < 0)
            {
                Vector2 dir = new Vector2(_xDirection, 0);

                // IF THERE IS A COLLIDER IN FRONT OF THE PLAYER.
                Vector3 position = _player.RigidBody.position;
                int hitCount = Physics2D.RaycastNonAlloc(position, dir, _hitColliders, _colliderDetectDistance, _solidLayer);
                if (hitCount > 0)
                {
                    RaycastHit2D firstTouchedCollider = Array.Find(_hitColliders, x => x.transform != transform);

                    float playerMaxY = _player.SpriteBounds.max.y;
                    float boxY = firstTouchedCollider.collider.bounds.max.y;
                    if (playerMaxY >= boxY)
                    {
                        Vector2 currentPosition = _player.RigidBody.position;
                        currentPosition.x = firstTouchedCollider.point.x;
                        currentPosition.y = boxY - (_player.SpriteBounds.extents.y - .025f);

                        _player.RigidBody.position = currentPosition;

                        // WE TELL ALL THE LISTENERS PLAYER MOVED.
                        OnMove?.Invoke(new MovementData(
                            currentPosition: transform.position,
                            direction: dir
                        ));

                        Grab();
                    }

                    return;
                }
            }


            // IDLE STATE.
            float xAxis = Input.GetAxis("Horizontal");
            if (xAxis == 0)
            {
                PlayIdleAnimation();
                return;
            }

            if (IsSliding)
                return;

            _xDirection = Mathf.Sign(xAxis);

            // MOVE STATE.

            // IF THE NEXT POSITION IS OCCUPIED BY A COLLIDER.
            Vector3 direction = new Vector3(xAxis, 0, 0) * _player.PlayerData.MovementSpeed;

            Vector3 playerBottm = new Vector3(
                _player.RigidBody.position.x,
                _player.SpriteBounds.min.y
            );

            if (!IsPositionValid(direction))
            {
                // WE WILL PLAY SLIDE ANIMATION.
                PlaySlideAnimation();

                return;
            }

            // PLAYER MOVEMENT DIRECTION.
            Vector2 velocity = _player.RigidBody.velocity;
            velocity.x = direction.x;
            _player.RigidBody.velocity = velocity;

            PlayRunAnimation();

            // WE TELL ALL THE LISTENERS PLAYER MOVED.
            OnMove?.Invoke(new MovementData(
                currentPosition: transform.position,
                direction: direction
            ));
        }

        private void Jump()
        {
            if (!EdgeGrabbed)
            {
                if (Grounded == false)
                    return;
            }

            ReleaseGrab();

            // WE ADD A FORCE TO JUMP PLAYER.
            Vector2 currentVelocity = _player.RigidBody.velocity;
            currentVelocity.y = 0;
            _player.RigidBody.velocity = currentVelocity;

            _player.RigidBody.AddForce(
                Vector2.up * _player.PlayerData.JumpPower,
                ForceMode2D.Impulse
            );

            _player.UnitAnimation.ForcePlay(UnitConsts.Anims.JUMP);

            // TO PREVENT DOUBLE JUMPS.
            Grounded = false;
            IsJumping = true;
            OnPlayerJump?.Invoke();

        }

        private void PlayRunAnimation()
        {
            // IF PLAYER IDLE WE WILL TRIGGER LISTENERS PLAYER STARTS MOVING.
            if (IsMoving == false)
            {
                IsMoving = true;
                OnStartMoving?.Invoke();
            }

            // IF PLAYER JUMPING RUN ANIMATION WONT PLAY.
            if (_player.Jump.IsJumping)
            {
                if (_player.RigidBody.velocity.y > 0)
                    _player.UnitAnimation.PlayAnim(UnitConsts.Anims.JUMP);
                else
                    _player.UnitAnimation.PlayAnim(UnitConsts.Anims.FALL);
            }
            else
            {
                _player.UnitAnimation.PlayAnim(UnitConsts.Anims.RUN);
            }

        }
        private void PlayIdleAnimation()
        {
            // IF PLAYER MOVING WWE WILL TELL ALL THE LISTENERS PLAYER STOPPED MOVING.
            if (IsMoving == true)
            {
                IsMoving = false;
                OnStopMoving?.Invoke();
            }

            // IF PLAYER JUMPING IDLE ANIMATION WONT PLAY.
            if (_player.Jump.IsJumping)
                return;

            _player.UnitAnimation.PlayAnim(UnitConsts.Anims.IDLE);
        }

        private void PlaySlideAnimation()
        {
            // IF GROUNDED WHILE SLIDING WE PLAY IDLE.
            if (_player.Jump.Grounded)
            {
                PlayIdleAnimation();
                return;
            }

            if (!IsFalling)
                return;

            _player.UnitAnimation.PlayAnim(UnitConsts.Anims.WALLSLIDE);
        }
        private async UniTask Slide()
        {
            if (Grounded == false || IsJumping == true)
                return;

            IsSliding = true;

            _player.RigidBody.velocity = Vector3.zero;
            _player.RigidBody.AddForce(
                _player.PlayerData.SlidePower * _xDirection * Vector2.right,
                ForceMode2D.Impulse
            );

            await _player.UnitAnimation.PlayAnimAsync(UnitConsts.Anims.SLIDE);

            IsSliding = false;
        }

        private void Grab()
        {
            EdgeGrabbed = true;

            _player.RigidBody.gravityScale = 0;
            _player.RigidBody.velocity = Vector3.zero;

            _player.UnitAnimation.ForcePlay(UnitConsts.Anims.EDGE_GRAB);

            _grabTime = Time.time;
        }
        private void ReleaseGrab()
        {
            if (!EdgeGrabbed)
                return;

            EdgeGrabbed = false;
            _player.RigidBody.gravityScale = 1;
            _player.UnitAnimation.PlayAnim(UnitConsts.Anims.WALLSLIDE);
        }

        public void ChangeCanDoActionState(bool newState)
        {
            this.CanDoAnyAction = newState;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // IF ALREADY GROUNDED RETURN.
            if (Grounded)
                return;

            // WE MAKE SURE WE TOUCH THE COLLIDER FROM THE TOP.
            Vector3 playerBottom = _player.SpriteBounds.min;
            if (collision.contacts[0].point.y > playerBottom.y + .01f)
                return;

            Grounded = true;
            IsJumping = false;
        }

        private bool IsPositionValid(Vector2 direction)
        {
            Vector3[] playerPositions = new Vector3[]
            {
                new Vector3(
                    _player.RigidBody.position.x,
                    _player.SpriteBounds.min.y
                ),
                new Vector3(
                    _player.RigidBody.position.x,
                    _player.SpriteBounds.max.y
                ),
                new Vector3(
                    _player.RigidBody.position.x,
                    _player.RigidBody.position.y
                )
            };

            foreach (var playerPosition in playerPositions)
            {
                int count = Physics2D.RaycastNonAlloc(playerPosition,
                    direction.normalized,
                    _hitColliders,
                    _colliderDetectDistance,
                    _solidLayer
                );

                for (int i = 0; i < count; i++)
                {
                    RaycastHit2D hitData = _hitColliders[i];
                    if (hitData.transform == null)
                        continue;

                    if (hitData.transform == transform)
                        continue;

                    return false;
                }
            }

            return true;
        }

        [Serializable]
        public struct MovementData
        {
            public Vector3 CurrentPosition { get; set; }
            public Vector3 Direction { get; set; }

            public MovementData(Vector3 currentPosition, Vector3 direction)
            {
                CurrentPosition = currentPosition;
                Direction = direction;
            }
        }
    }
}
