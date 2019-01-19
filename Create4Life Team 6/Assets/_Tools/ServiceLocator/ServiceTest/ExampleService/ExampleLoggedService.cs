using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleLoggedService : ExampleBaseService, IServiceLogger
{
    private ExampleBaseService wrappedService;

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
        wrappedService = (ExampleBaseService)serviceToWrap;
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

    public override void Foo()
    {
        if (wrappedService != null)
        {
            Debug.Log("Will Foo...");
            wrappedService.Foo();
        }
    }

    public override void Bar()
    {
        if (wrappedService != null)
        {
            Debug.Log("Will Bar...");
            wrappedService.Bar();
        }
    }

}
