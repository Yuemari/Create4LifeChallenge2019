using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : UIScreen
{
    public static int lastWinner = 0;//0 is DRAW,1 is BLUE, 2 is RED

    public GameObject drawWinsGO;
    public GameObject blueWinsGO;
    public GameObject redWinsGO;

    //just temporary stuff so doesnt break current game implementation
    public GameObject gameCanvas;
   
    public override void Activate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
    {
       
        drawWinsGO.SetActive(lastWinner == 0);
        blueWinsGO.SetActive(lastWinner == 1);
        redWinsGO.SetActive(lastWinner == 2);


        base.Activate(screenChangeCallback);

        MainMenuScreen.matchEnded = true;
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
        gameCanvas.SetActive(false);
        ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).SwitchToScreenWithId(ScreenIds.sMainMenuScreen);
    }

    public void OnRetryPressed()
    {
        SwitchOffScreen();
      
    }

    public void SwitchOffScreen()
    {
        UIManager uiManager = GetMyManager() as UIManager;
        if (uiManager != null)
        {
            uiManager.SwitchScreenById(ScreenIds.sResultsScreen, false);
            gameCanvas.SetActive(true);
         
        }
    }

}