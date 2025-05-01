using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class Stats : CoreComponent
{
    [field: SerializeField] public StatComponent level { get; protected set; }
    [field: SerializeField] public StatComponent health { get; protected set; }
    [field: SerializeField] public StatComponent posture { get; protected set; }
    // [field: SerializeField, PreventAdd, Tooltip("Warning: Status effects system is based on enum. You should NEVER change the order of the list!!!!!!")] public List<StatComponent> statusEffects { get; protected set; }
    // [field: EnumFlags] public StatusEffect currentlyAppliedStatusEffect { get; protected set; }

    private IEnumerable<PropertyInfo> statComponentProperties;

    private void OnValidate()
    {
        /*if (statusEffects == null)
        {
            statusEffects = new List<StatComponent>();
        }

        int statusEffectCount = Enum.GetValues(typeof(StatusEffect)).Length;

        if (statusEffects.Count != statusEffectCount)
        {
            if (statusEffects.Count < statusEffectCount)
            {
                for (int index = statusEffects.Count; index < statusEffectCount; index++)
                {
                    StatComponent statusEffect = new StatComponent();
                    statusEffect.name = Enum.GetName(typeof(StatusEffect), index);
                    statusEffects.Add(statusEffect);
                }
            }
            else
            {
                statusEffects.RemoveRange(statusEffectCount, statusEffects.Count - statusEffectCount);
            }
        }*/
    }

    protected override void Awake()
    {
        base.Awake();

        #region Stat Initialization
        statComponentProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.Equals(typeof(StatComponent)));

        foreach (PropertyInfo property in statComponentProperties)
        {
            StatComponent statComponent = property.GetValue(this) as StatComponent;
            MethodInfo initMethod = typeof(StatComponent).GetMethod("Init");
            object[] parameters = new object[] { entity, property.Name };
            initMethod.Invoke(statComponent, parameters);
        }

        /*foreach (StatComponent statComponent in statusEffects)
        {
            statComponent.entity = entity;
            statComponent.Init();
        }*/
        #endregion
    }

    protected virtual void Start()
    {
        health.SetMaxValue(health.graph.accumulationPerLevel.Evaluate(level.currentValue));
        health.SetCurrentValue(health.graph.accumulationPerLevel.Evaluate(level.currentValue));
        
        posture.SetMaxValue(posture.graph.accumulationPerLevel.Evaluate(level.currentValue));
    }

    protected virtual void Update()
    {
        foreach (PropertyInfo property in statComponentProperties)
        {
            StatComponent statComponent = property.GetValue(this) as StatComponent;
            MethodInfo initMethod = typeof(StatComponent).GetMethod("Recovery");
            initMethod.Invoke(statComponent, null);
        }
    }
}
