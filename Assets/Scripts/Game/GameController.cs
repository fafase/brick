using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameController
{
    public ReactiveProperty<int> BallAmount { get; private set; }
    public void Init(int ballAmount) 
    {
        BallAmount = new ReactiveProperty<int>(ballAmount > 0 ? ballAmount: 1);
    }

    public void DecreaseBallAmount() 
    {
        BallAmount.Value--;
    }
}
