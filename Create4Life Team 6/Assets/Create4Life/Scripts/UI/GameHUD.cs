using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : UIScreen
{




    public override void Activate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
        base.Activate(screenChangeCallback);
    }

    public override void UpdateScreen(UIScreenController.ScreenUpdatedEventHandler screenUpdatedCallBack)
    {
        base.UpdateScreen(screenUpdatedCallBack);
    }

    public override void Deactivate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
        base.Deactivate(screenChangeCallback);
    }

    public void OnBackButtonPressed()
    {
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sMainMenuScreen);
    }

    public bool IsGameLoadPresentationDone()
    {
        return true;
    }

    public void StartGameLoad()
    {

    }

    public bool IsGameLoadFinishDone()
    {
        return true;
    }

    public void FinishGameLoad()
    {

    }
}