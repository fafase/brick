using Tools;


public class Player : Presenter, IPlayer
{
    private int m_level;
    public int Level => m_level;

    public Player()
    {
        m_level = 1;
    }

    public void IncreaseLevel () => m_level++;
    public void SetLevel(int level) => m_level = level;
  
}

