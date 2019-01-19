using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceTest : MonoBehaviour {

    public bool calledService = false;
    public bool updateService = false;
    public Service askedService; 


	// Update is called once per frame
	void Update ()
    {
		if(!calledService)
        {
            askedService = ServiceLocator.Instance.GetServiceOfType<ExampleBaseService>(SERVICE_TYPE.EXAMPLE);
            ((ExampleBaseService)askedService).Foo();
            calledService = true;
        }

        if(updateService)
        {
            updateService = false;
            ServiceLocator.Instance.UpdateCurrentServices();
        }
	}
}
