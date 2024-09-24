using System;
using Tools;
using UniRx;


public class Player : IDisposable, IPlayer
{
    private bool m_disposed = false;
    public int Lives => 0;

    private CompositeDisposable m_disposables = new CompositeDisposable();

    public Player()
    {
       

    }
  

    public void Dispose()
    {
        if(m_disposed) return;
        m_disposed = true;
        m_disposables?.Dispose();
    }
}
public interface IPlayer 
{
    int Lives { get; }
}
