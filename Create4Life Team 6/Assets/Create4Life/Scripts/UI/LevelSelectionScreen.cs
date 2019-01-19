using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UILevelSelector
{
    public int levelId;
    public GameObject levelLockedGO;
}

public class LevelSelectionScreen : UIScreen
{

    public UILevelSelector[] levelSelectors;


    public override void Activate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
        base.Activate(screenChangeCallback);
        UpdateLevelSelectors();

    }

    public override void UpdateScreen(UIScreenController.ScreenUpdatedEventHandler screenUpdatedCallBack)
    {
        base.UpdateScreen(screenUpdatedCallBack);
    }

    public override void Deactivate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
        base.Deactivate(screenChangeCallback);
    }

    public void UpdateLevelSelectors()
    {
        GameBaseService gs = ServiceLocator.Instance.GetServiceOfType<GameBaseService>(SERVICE_TYPE.GAMESERVICE);
        if (gs != null)
        {
            for (int i = 0; i < levelSelectors.Length; ++i)
            {
                levelSelectors[i].levelLockedGO.SetActive(!gs.IsLevelUnlocked(levelSelectors[i].levelId));
            }
        }
    }

    public void OnLevelSelected(int levelId)
    {
        GameBaseService gs = ServiceLocator.Instance.GetServiceOfType<GameBaseService>(SERVICE_TYPE.GAMESERVICE);
        if(gs != null)
        {
            if(gs.IsLevelUnlocked(levelId))
            {
                gs.StartGame(levelId);
            }
        }
    }

    public void OnBackButtonPressed()
    {
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sMainMenuScreen);
    }
}