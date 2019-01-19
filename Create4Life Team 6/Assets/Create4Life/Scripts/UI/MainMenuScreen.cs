using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScreen : UIScreen
{
    
    public AnimatedElementController[] animatedControllers;
    public bool isUIAnimationDone = false;
    public bool wasUpdatedAtLeastOnce = false;
    
    public static bool matchEnded = false;


    public override void Activate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
        isUIAnimationDone = false;
        if (!isUIAnimationDone && wasUpdatedAtLeastOnce)
        {
            RunAnimations();
        }
        base.Activate(screenChangeCallback);
        
        matchEnded = false;
    }

    public override void UpdateScreen(UIScreenController.ScreenUpdatedEventHandler screenUpdatedCallBack)
    {
        base.UpdateScreen(screenUpdatedCallBack);
        if(!isUIAnimationDone)
        {
            wasUpdatedAtLeastOnce = true;
            RunAnimations();
        }
    }

    public override void Deactivate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
        for (int i = 0; i < animatedControllers.Length; ++i)
        {
            animatedControllers[i].ResetToStartingPoint();
        }
        base.Deactivate(screenChangeCallback);
    }

    public void OnBackButtonPressed()
    {
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sMainMenuScreen);
    }

    private void RunAnimations()
    {
        StartCoroutine("RunAnimationsRoutine");
    }

    private IEnumerator RunAnimationsRoutine()
    {
       
        yield return 0;
        for (int i = 0; i < animatedControllers.Length; ++i)
        {
            animatedControllers[i].SwitchAnimation(true);
        }
        isUIAnimationDone = true;
    }

    public void OnStartPressed()
    {
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sGameSelectionScreen);
    }

    public void OnCreditsPressed()
    {
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sCreditsScreen);
    }

    public void OnTempPausePressed()
    {
        ServiceLocator.Instance.GetServiceOfType<PopUpsManager>(SERVICE_TYPE.POPUPSMANAGER).ShowPopUpWithId(PopUpIds.sPausePopUp);
    }

    public void OnTempBlueWinsPressed()
    {
        ResultsScreen.lastWinner = 1;
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sResultsScreen);
    }

    public void OnTempRedWinsPressed()
    {
        ResultsScreen.lastWinner = 2;
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sResultsScreen);
    }

}
