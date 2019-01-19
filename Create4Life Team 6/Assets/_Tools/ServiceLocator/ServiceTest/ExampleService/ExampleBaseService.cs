using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleBaseService : Service
{
 
    protected bool _isLogged;

    public override SERVICE_TYPE GetServiceType()
    {
        return SERVICE_TYPE.EXAMPLE;
    }

    public override bool IsServiceNull()
    {
        return true;
    }

    public override bool IsLoggedService()
    {
        return false;
    }

    public override Service TransformService(bool isLogged)
    {
        _isLogged = isLogged;
        if (_isLogged)
        {
            ExampleLoggedService loggedService = gameObject.AddComponent<ExampleLoggedService>();
            loggedService.SetService(this);
            return loggedService;
        }
        return this;
    }

    public static Service GetNullService(bool isLogged)
    {
        if (isLogged)
        {
            Service nullService = Service.GenerateGameObjectWithService<ExampleBaseService>();
            return nullService.TransformService(true);
        }
        else
        {
            return Service.GenerateGameObjectWithService<ExampleBaseService>();
        }
    }

   public virtual void Foo()
   {
        //this is not implemented in the base service
   }

    public virtual void Bar()
    {
         //this is not implemented in the base service
    }
}
