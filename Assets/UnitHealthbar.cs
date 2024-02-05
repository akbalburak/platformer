using Engine.UnitComp;
using Engine.UnitComp.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthbar : MonoBehaviour
{
    [SerializeField]
    private Slider _healthbar;

    private IUnit _unit;

    // Start is called before the first frame update
    void Start()
    {
        _unit = GetComponentInParent<IUnit>();
        _unit.UnitHealth.OnHealthChange += OnHealthChanged;
    }

    private void OnHealthChanged(UnitHealth.HealthChangeData data)
    {
        _healthbar.value = data.Ratio;
        _healthbar.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
