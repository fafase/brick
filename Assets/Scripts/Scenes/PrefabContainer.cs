using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabContainer", menuName = "Tools/PrefabContainer")]
public class PrefabContainer : ScriptableObject, IPrefabContainer
{
    [SerializeField] private List<BrickView> m_bricks;
    [SerializeField] private List<PowerUp> m_powerUps;

    public PowerUp GetPowerUp(PowerUpType type) => m_powerUps.Find(pu => pu.PowerUpType.Equals(type));
    public BrickView GetBrick(int brick) 
    {
        if(brick < 0 || brick >= m_bricks.Count) { throw new ArgumentException(); }
        return m_bricks[brick]; 
    }
}

public  interface IPrefabContainer 
{
    PowerUp GetPowerUp(PowerUpType type);
    BrickView GetBrick(int brick);
}