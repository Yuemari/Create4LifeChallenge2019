using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServiceLogged : GameBaseService, IServiceLogger
{
    private GameBaseService wrappedService;

    public override bool IsServiceNull()
    {
        return false;
    }

    public override bool IsLoggedService()
    {
        return true;
    }


    public void SetService(Service serviceToWrap)
    {
        wrappedService = (GameBaseService)serviceToWrap;
    }

    public override Service TransformService(bool isLogged)
    {
        if (!isLogged)
        {
            Destroy(this);
            return wrappedService;
        }
        return this;
    }

    public override void InitializeService()
    {
        if (wrappedService != null)
        {
            Debug.Log("Initializing Battle Master Service.");
            wrappedService.InitializeService();
        }
    }

    public override void Load()
    {
        if (wrappedService != null)
        {
            Debug.Log("Will Load game service data");
            wrappedService.Load();
        }
    }

    public override bool IsLevelUnlocked(int levelId)
    {
        if (wrappedService != null)
        {
            Debug.Log("Is level unlocked for id:" + levelId);
            return wrappedService.IsLevelUnlocked(levelId);
        }
        return false;
    }

    public override void StartGame(int level)
    {
        if (wrappedService != null)
        {
            Debug.Log("Will Start game on level index:"+level);
            wrappedService.StartGame(level);
        }
    }

    public override void EndGame(bool win)
    {
        if (wrappedService != null)
        {
            Debug.Log("Will End game with result: win?"+win);
            wrappedService.EndGame(win);
        }
    }

}
