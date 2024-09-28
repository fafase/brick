using System;
using Tools;
using UniRx;
using UnityEngine.Assertions;

public class CoinPresenter : Presenter, ICoin
{
    private const int m_initialCoins = 200;

    public IObservable<int> CoinsAsObservable => Coins.AsObservable();
    private ReactiveProperty<int> Coins = new ReactiveProperty<int>(m_initialCoins);

    public int CoinAmount => Coins.Value;

    public void AddCoins(int amount) 
    {
#if UNITY_EDITOR
        Assert.IsTrue(amount > 0, "Coin value to add should not be negative");
#endif
        Coins.Value += amount;
    }

    public Result UseCoins(int amount) 
    {
#if UNITY_EDITOR
        Assert.IsTrue(amount > 0, "Coin value to add should not be negative");
#endif
        if(amount > Coins.Value) 
        {
            return Result.Failure("Missing coins for purchase");
        }
        Coins.Value -= amount;
        return Result.Success();
    }
    public bool CanPurchase(int amount) => amount <= Coins.Value; 
}

public interface ICoin 
{
    IObservable<int> CoinsAsObservable { get; }
    public int CoinAmount { get; }
    void AddCoins(int amount);
    Result UseCoins(int amount);

    bool CanPurchase(int amount);
}
