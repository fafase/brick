using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameController
{
    public ReactiveProperty<int> BallAmount { get; private set; }

    public GameController()
    {
        BallAmount = new ReactiveProperty<int>();
    }
    public void Init(int ballAmount) 
    {
        BallAmount.Value = ballAmount > 0 ? ballAmount : 1;
    }

    public void DecreaseBallAmount() 
    {
        BallAmount.Value--;
    }
}
