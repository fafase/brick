using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BrickController 
{
    public ReactiveProperty<int> HealthProperty;
    public BrickController(int health)
    {
        HealthProperty = new ReactiveProperty<int>(health);
    }
    public void ApplyDamage(int damage) 
    {
        if(damage <= 0) 
        {
            return;
        }
        HealthProperty.Value -= damage;
    }
}
