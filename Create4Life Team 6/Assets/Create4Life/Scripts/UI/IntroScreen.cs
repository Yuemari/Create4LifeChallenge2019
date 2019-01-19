using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScreen : UIScreen
{
    public float waitBeforeFadingOut = 1.0f;
    public ElementCanvasGroupAplhaChangeAC canvasAnimator;

    public override void Activate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
        StartCoroutine("SwitchMainMenuAndFadeOut");
        base.Activate(screenChangeCallback);
    }

    public override void UpdateScreen(UIScreenController.ScreenUpdatedEventHandler screenUpdatedCallBack)
    {
        base.UpdateScreen(screenUpdatedCallBack);
    }

    public override void Deactivate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
        StopAllCoroutines();
        base.Deactivate(screenChangeCallback);
    }

    public void OnBackButtonPressed()
    {
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sMainMenuScreen);
    }

    IEnumerator SwitchMainMenuAndFadeOut()
    {
        //wait until all services have been initialized
        while(ServiceLocator.Instance.IsInitializing)
        {
            yield return 0;
        }
        yield return 0;
        UIManager uiManager = GetMyManager() as UIManager;
        if(uiManager != null)
        {
            uiManager.SwitchScreenById(ScreenIds.sMainMenuScreen,true,true);
        }
        else
        {
            Debug.LogWarning("CantGetMyManager");
        }
        yield return 0;
        yield return new WaitForSeconds(waitBeforeFadingOut);
        canvasAnimator.Play();
        yield return new WaitForSeconds(canvasAnimator.duration);
        if (uiManager != null)
        {
            //this will disable this screen
            uiManager.SwitchToScreenWithId(ScreenIds.sMainMenuScreen);
        }
    }

}
