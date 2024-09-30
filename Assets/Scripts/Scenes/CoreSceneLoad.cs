using System;
using Tools;
using UnityEngine;
using Zenject;
using UniRx;
using System.Collections.Generic;

public class CoreSceneLoad : MonoBehaviour
{
    [Header("Objects to load")]
    [SerializeField] private BallView m_ballPrefab;
    [SerializeField] private Transform m_brickContainer;

    [Inject] private BallView.Factory m_ballViewFactory;
    [Inject] private BrickView.Factory m_brickViewFactory;

    [Inject] private IPrefabContainer m_prefabContainer;
    [Inject] private ILevelManager m_levelManager;
    [Inject] private IPlayer m_player;
    [Inject] private IBrickSystem m_brickSystem;

    private void Start()
    {
        BallView ballInstance = m_ballViewFactory.Create(m_ballPrefab);
        ballInstance.Init();
        
        LevelConfig levelConfig = (LevelConfig)m_levelManager.CurrentLevelConfig(m_player.Level);
        ObservableSignal.Broadcast(new LoadLevelSignal(levelConfig));

        LoadLevel(levelConfig.levelContent)
            .Do(brickList => m_brickSystem.InitWithBricks(brickList))
            .Subscribe(
                _ => Debug.Log("Level successfully loaded"),
                ex => Debug.LogError($"Error loading level: {ex.Message}"), 
                () => Debug.Log("LoadLevel operation completed") 
            );
    }

    private IObservable<List<BrickView>> LoadLevel(List<List<int>> brickList) 
    {
        return Observable.Create<List<BrickView>>(observer =>
        {
            List<BrickView> list = new List<BrickView>();
            float xOffset = 1f;
            float yOffset = -0.5f;

            for (int i = 0; i < brickList.Count; i++)
            {
                for (int j = 0; j < brickList[i].Count; j++)
                {
                    int brickType = brickList[i][j];
                    if (brickType <= 0) 
                    {
                        continue; 
                    }

                    var prefab = m_prefabContainer.GetBrick(brickType - 1);
                    BrickView brick = m_brickViewFactory.Create(prefab);
                    brick.transform.SetParent(m_brickContainer, false);
                    float xPos = j * xOffset; 
                    float yPos = i * yOffset; 
                    brick.transform.localPosition = new Vector3(xPos, yPos, -1f);
                    list.Add(brick);
                }
            }
            observer.OnNext(list);
            observer.OnCompleted();
            return Disposable.Empty;
        });
    }
}

public class LoadLevelSignal : SignalData 
{
    public readonly LevelConfig LvlConfig;

    public LoadLevelSignal(LevelConfig lvlConfig)
    {
        LvlConfig = lvlConfig;
    }
}
