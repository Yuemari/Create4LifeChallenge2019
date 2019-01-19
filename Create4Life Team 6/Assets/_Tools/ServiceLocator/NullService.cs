using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullService : Service
{
    public override SERVICE_TYPE GetServiceType()
    {
        return SERVICE_TYPE.NONE;
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
        return this;
    }

}
