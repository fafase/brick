using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpGrow : PowerUp
{
    protected override void ApplyEffect(Collider2D collider)
    {
        Debug.Log("Grow paddle");
    }
}
