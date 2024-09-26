using System;
using Tools;
using UnityEngine;
using Zenject;
using UniRx;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;
using NSubstitute.Core;
public class PowerUpController : Presenter
{
    [Inject] private PowerUp.Factory m_powerUpFactory;
    [Inject] private IPrefabContainer m_container;

    private List<PowerUp> m_activePowerUps;

    public PowerUpController()
    {
        m_activePowerUps = new List<PowerUp>();

        ObservableSignal
            .AsObservable<BrickDestroyedSignal>()
            .Subscribe(ProcessBrickDestruction)
            .AddTo(m_compositeDisposable);

        ObservableSignal.AsObservable<PowerUpSignal>()
            .Where(data => data.IsStarting == false)
            .Select(data => data.PowerUp)
            .Subscribe(powerUp => m_activePowerUps.Remove(powerUp))
            .AddTo(m_compositeDisposable);
    }

    public virtual PowerUp CreatePowerUp(PowerUp prefab)
    {
        if (prefab == null)
        {
            throw new ArgumentNullException("Null prefab for PowerUp in CoreBase for Factory");
        }
        return m_powerUpFactory.Create(prefab);
    }

    private int m_empty = 0;
    private void ProcessBrickDestruction(BrickDestroyedSignal data)
    {
        int minEmpty = 3;
        int maxEmpty = 10;
        // Basic probability logic,
        // should be reviewed to avoid streak of creation and forced creation if too long without new power-up
        int prob = data.Type switch
        {
            BrickType.Blue => 10,
            BrickType.Orange => 20,
            _ => 0
        };
        m_empty++;

        
        if (m_empty > maxEmpty || m_empty > minEmpty && UnityEngine.Random.Range(0, 99) < prob)
        {
            try
            {
                List<PowerUpType> temp = new List<PowerUpType>();
                temp.AddRange(Enum.GetValues(typeof(PowerUpType)));
                if (m_activePowerUps.Find(p => p.PowerUpType.Equals(PowerUpType.ShrinkPad)))
                {
                    temp.Remove(PowerUpType.ShrinkPad);
                }
                if (m_activePowerUps.Find(p => p.PowerUpType.Equals(PowerUpType.GrowPad)))
                {
                    temp.Remove(PowerUpType.GrowPad);
                }
                int rand = UnityEngine.Random.Range(0, temp.Count - 1);
                PowerUpType powerUpType = temp[rand];
                PowerUp prefab = m_container.GetPowerUp(powerUpType);
                PowerUp powerUp = CreatePowerUp(prefab);
                powerUp.transform.position = data.Position;
                m_activePowerUps.Add(powerUp);
                m_empty = 0;
                return;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
