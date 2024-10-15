using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class MetaLoader : MonoBehaviour
{
    [SerializeField] private List<UnityEngine.Object> m_loaders;

    private async void Start()
    {
        foreach (var loader in m_loaders)
        { 
            if(loader is IMetaLoader l) 
            {
                if (l.WaitForCompletion) 
                {
                    await l.OnMetaLoad();
                }
                else 
                {
                    l.OnMetaLoad().Forget();
                }
            }
        }
    }
}
