using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Services.CloudSave;

public static class CloudSave 
{
    private const string PLAYER_DATA = "playerData";

    public static async UniTask<Dictionary<string, string>> SendCloudSave(string json)
    {
        try
        {
            Dictionary<string, string> result = await CloudSaveService.Instance
                 .Data.Player
                 .SaveAsync(new Dictionary<string, object>
                 {
                    { "playerData", json }
                 });
            return result;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Direct SaveAsync error: {ex}");
            return new Dictionary<string, string>();
        }
    }

    public static async UniTask<string> GetPlayerData()
    {
        Dictionary<string, Unity.Services.CloudSave.Models.Item> playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { PLAYER_DATA });
        if (playerData.Count > 0)
        {
            if (playerData.TryGetValue(PLAYER_DATA, out var item))
            {
                return item.Value.GetAsString();
            }
        }
        return string.Empty;
    }
}
