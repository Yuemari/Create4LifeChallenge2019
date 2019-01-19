using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBaseService : Service
{

    protected bool _isLogged;

    public override SERVICE_TYPE GetServiceType()
    {
        return SERVICE_TYPE.GAMESERVICE;
    }

    public override bool IsServiceNull()
    {
        return true;
    }

    public override bool IsLoggedService()
    {
        return false;
    }

    public override void InitializeService()
    {
        base.InitializeService();
        Load();
    }

    public override Service TransformService(bool isLogged)
    {
        _isLogged = isLogged;
        if (_isLogged)
        {
            GameServiceLogged loggedService = gameObject.AddComponent<GameServiceLogged>();
            loggedService.SetService(this);
            return loggedService;
        }
        return this;
    }

    public static Service GetNullService(bool isLogged)
    {
        if (isLogged)
        {
            Service nullService = Service.GenerateGameObjectWithService<GameBaseService>();
            return nullService.TransformService(true);
        }
        else
        {
            return Service.GenerateGameObjectWithService<GameBaseService>();
        }
    }

    public virtual void Load()
    {

    }

    public virtual bool IsLevelUnlocked(int levelId)
    {
        return false;
    }

    public virtual void StartGame(int selectedLevel)
    {
        //this is not implemented in the base service
    }

    public virtual void EndGame(bool win)
    {
        //this is not implemented in the base service
    }

    public virtual void PauseGame()
    {

    }

    public virtual void ResumeGame()
    {

    }
}
