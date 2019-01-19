using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LevelData
{
    public int levelId;
    public bool isLevelUnlocked;
    public bool isTutorialLevel;
    public string worldName;
    public string levelName;

}

public class GameService: GameBaseService
{

    public LevelData[] allLevelsData;
    private int currentLevel = -1;

    public override bool IsServiceNull()
    {
        return false;
    }

    public override bool IsLoggedService()
    {
        return false;
    }


    public override void InitializeService()
    {
        base.InitializeService();
        if (ServiceLocator.GetLogStatusPerType(GetServiceType()))
        {
            //TODO: set gizmos and other debuggable things needed
        }
       
    }

    public override void Load()
    {
        for(int i = 0; i < allLevelsData.Length; ++i)
        {
            allLevelsData[i].isLevelUnlocked = PlayerPrefs.GetInt("Lvl" + allLevelsData[i].levelId, (allLevelsData[i].isTutorialLevel?1:0) ) == 1;
        }
    }

    public override void StartGame(int level)
    {
       
       
        Debug.LogFormat("Starting Level with Id:{0}",level);

        StartCoroutine("LoadLevel",level);
    }

    private IEnumerator LoadLevel(int levelIndex)
    {
        UIManager uiManager = ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER);

        GameHUD hud = uiManager.GetUIScreenForId<GameHUD>(ScreenIds.sGameScreen);
        if (hud != null)
        {
            //start level load transition HERE!
            hud.StartGameLoad();
            while(!hud.IsGameLoadPresentationDone())
            {
                yield return 0;
            }
            //load level scene
            string sceneToLoad = string.Format("{0} {1}",allLevelsData[levelIndex].worldName, allLevelsData[levelIndex].levelName);
            Debug.LogFormat("Scene to load:{0}",sceneToLoad);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

            while(!operation.isDone)
            {
                yield return 0;
            }

            //TODO: Init world HERE!

            //start level load transition END HERE!
            hud.FinishGameLoad();
            while (!hud.IsGameLoadFinishDone())
            {
                yield return 0;
            }
            //show GameHUD
            ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sGameScreen);
            
           

            currentLevel = levelIndex;
        }
    }



    public override void EndGame(bool win)
    {
        if(currentLevel >= 0 && win)
        {
            for (int i = 0; i < allLevelsData.Length; ++i)
            {
                if (allLevelsData[i].levelId == currentLevel)
                {
                    allLevelsData[i].isLevelUnlocked = true;
                    PlayerPrefs.SetInt("Lvl" + allLevelsData[i].levelId,1);
                    break;
                }
            }
        }
    }

    private IEnumerator LoadMainMenu()
    {
        UIManager uiManager = ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER);

        GameHUD hud = uiManager.GetUIScreenForId<GameHUD>(ScreenIds.sGameScreen);
        if (hud != null)
        {
            //start level load transition HERE!
            hud.StartGameLoad();
            while (!hud.IsGameLoadPresentationDone())
            {
                yield return 0;
            }
            //unload level scene
            string sceneToUnLoad = string.Format("{0} {1}", allLevelsData[currentLevel].worldName, allLevelsData[currentLevel].levelName);
            Debug.LogFormat("Scene to load:{0}", sceneToUnLoad);
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneToUnLoad);

            while (!operation.isDone)
            {
                yield return 0;
            }

            //TODO: Init world HERE!

            //start level load transition END HERE!
            hud.FinishGameLoad();
            while (!hud.IsGameLoadFinishDone())
            {
                yield return 0;
            }
            //show GameHUDS
            ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sMainMenuScreen);



            currentLevel = -1;
        }
    }

    public override bool IsLevelUnlocked(int levelId)
    {
        for (int i = 0; i < allLevelsData.Length; ++i)
        {
            if (allLevelsData[i].levelId == levelId)
            {
                return allLevelsData[i].isLevelUnlocked;
            }
        }
        Debug.LogWarningFormat("No level with Id[{0}] found",levelId);
        return false;
    }

}
