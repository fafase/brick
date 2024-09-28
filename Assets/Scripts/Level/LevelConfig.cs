// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.Collections.Generic;

public class Reward
{
    public string rewardType { get; set; }
    public int amount { get; set; }
}

public class LevelConfig
{
    public string name { get; set; }
    public string version { get; set; }
    public int bundle { get; set; }
    public int difficulty { get; set; }
    public List<List<int>> levelContent { get; set; }
    public List<object> boosterProbability { get; set; }
    public int tutorial { get; set; }
    public int time { get; set; }
    public List<object> objective { get; set; }
    public bool enableMultiplier { get; set; }
    public int ballAttempt { get; set; }
    public List<Reward> reward { get; set; }
}

