using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleService : ExampleBaseService
{

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
        if(ServiceLocator.GetLogStatusPerType(GetServiceType()))
        {
            //TODO: set gizmos and other debuggable things needed
        }
    }

    public override void Foo()
    {
        //this is implemented;
        Debug.Log("Running Foo real implementation");
    }

    public override void Bar()
    {
        //this is implemented;
        Debug.Log("Running Bar real implementation");
    }



}
