using Tools;
using UnityEngine;
using Zenject;

public class PowerUpExtraBall : PowerUp
{
    [Inject] IPopupManager popupManager;
    protected override void ApplyEffect(Collider2D collider)
    {
        
    }
}
