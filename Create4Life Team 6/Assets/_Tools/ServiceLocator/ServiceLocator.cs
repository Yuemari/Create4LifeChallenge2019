using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    [System.Serializable]
    public struct LoggedServiceStatus
    {
        public SERVICE_TYPE serviceType;
        public bool isLogged;
        public bool initializeIfFoundInScene;
    }

    public bool printLogs = false;
    public int framesToWaitBetweenServicesInitializations = 1;
    private bool isInitialized = false;
    private static ServiceLocator _serviceLocatorInstance;

    public List<LoggedServiceStatus> logStatusPerServiceType = new List<LoggedServiceStatus>();

    public List<Service> editorRegisteredServices;

    private Dictionary<SERVICE_TYPE, int> loggedServiceStatusMap = new Dictionary<SERVICE_TYPE, int>();
    private Dictionary<SERVICE_TYPE, Service> serviceMaps = new Dictionary<SERVICE_TYPE, Service>();

    public static ServiceLocator Instance
    {
        get
        {
            if(_serviceLocatorInstance == null)
            {
                _serviceLocatorInstance = FindObjectOfType<ServiceLocator>();
            }

            return _serviceLocatorInstance;
        }
    }

    public bool IsInitializing
    {
        get
        {
            return !isInitialized;
        }
    }

    public bool dontDestroyOnLoad = true;

    private void Awake()
    {
        if (_serviceLocatorInstance == null)
        {
            _serviceLocatorInstance = this;
            isInitialized = false;
            if (printLogs)
            {
                Debug.Log("Generating Log Status map and initialize services as needed.");
            }
            GenerateLoggedStatusMap();
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void GenerateLoggedStatusMap()
    {
        Service[] allServicesAvailableInScene = FindObjectsOfType<Service>();
        List<Service> orderedServicesToInitialize = new List<Service>();
        loggedServiceStatusMap.Clear();
        for (int i = 0; i < logStatusPerServiceType.Count; i++)
        {
            loggedServiceStatusMap.Add(logStatusPerServiceType[i].serviceType, i);
            if(logStatusPerServiceType[i].initializeIfFoundInScene  && allServicesAvailableInScene.Length > 0)
            {
                Service service = FindServiceOfType(logStatusPerServiceType[i].serviceType, 
                                                    ref allServicesAvailableInScene);
                if (service != null)
                {
                    orderedServicesToInitialize.Add(service);
                    service.autoInitialize = false;
                }
            }
        }
        if(orderedServicesToInitialize.Count > 0)
        {
            StartCoroutine("InitializeServicesInOrder",orderedServicesToInitialize);
        }
        else
        {
            isInitialized = true;
        }

    }

    private Service FindServiceOfType(SERVICE_TYPE type, ref Service[] services)
    {
        for(int i = 0; i < services.Length; ++i)
        {
            if(services[i].GetServiceType() == type)
            {
                return services[i];
            }
        }
        return null;
    }

    private IEnumerator InitializeServicesInOrder(List<Service> orderedServicesToInitialize)
    {
       
        while (orderedServicesToInitialize.Count > 0)
        {
            if(printLogs)
            {
                Debug.LogFormat("Intializing LoggedStatus Service: {0}", orderedServicesToInitialize[0].GetServiceType());
            }
            orderedServicesToInitialize[0].InitializeService();
            orderedServicesToInitialize.RemoveAt(0);
            yield return StartCoroutine("WaitFrames",framesToWaitBetweenServicesInitializations);
        }
        isInitialized = true;
    }

    private IEnumerator WaitFrames(int framesToWait)
    {
        for(int i = 0; i < framesToWait; ++i)
        {
            yield return 0;
        }
    }

    public static bool GetLogStatusPerType(SERVICE_TYPE serviceType)
    {
        if(_serviceLocatorInstance)
        {
            return _serviceLocatorInstance.GetLoggedStatusPerType(serviceType);
        }
        return false;
    }

    public bool GetLoggedStatusPerType(SERVICE_TYPE serviceType)
    {
        if (loggedServiceStatusMap.ContainsKey(serviceType))
        {
            return logStatusPerServiceType[loggedServiceStatusMap[serviceType]].isLogged;
        }
        return false;
    }

    public void UpdateCurrentServices()
    {
        Service[] allExistingServices = FindObjectsOfType<Service>();

        for (int i = 0; i < allExistingServices.Length; i++)
        {
            SetServiceAsNeeded(allExistingServices[i]);
        }
    }

    public void RegisterService(Service newService)
    {
        if (printLogs)
        {
            Debug.LogFormat("Registering Service: {0}", newService.GetServiceType());
        }
        SetServiceAsNeeded(newService);   
    }

    void SetServiceAsNeeded(Service newService)
    {
        if (loggedServiceStatusMap.ContainsKey(newService.GetServiceType()))
        {
            //check if its the same type I need
            if (logStatusPerServiceType[loggedServiceStatusMap[newService.GetServiceType()]].isLogged ==
                newService.IsLoggedService())
            {
                Service currentService = null;
                //check if we have it already
                if (serviceMaps.TryGetValue(newService.GetServiceType(), out currentService))
                {
                    //if we do, check if its null
                    if (currentService == null)
                    {
                        AddService(newService, false);
                    }
                }
                else
                {
                    AddService(newService, false);
                }

            }
            else
            {
                //transform it to the one I want
                newService = newService.TransformService(logStatusPerServiceType[loggedServiceStatusMap[newService.GetServiceType()]].isLogged);
                AddService(newService, false);
            }
        }
        else
        {
            //add it as it is and to the list
            AddService(newService, true);
        }
    }

    public T GetServiceOfType<T>(SERVICE_TYPE serviceType) where T:Service
    {
        return (T)GetServiceOfType(serviceType);
    }

    public Service GetServiceOfType(SERVICE_TYPE serviceType)
    {
        if (Application.isPlaying)
        {
            Service currentService = null;
            //check if we have it already
            if (serviceMaps.TryGetValue(serviceType, out currentService))
            {
                //if we do, check if its null
                if (currentService == null)
                {
                    CreateNullService(serviceType);
                }
            }
            else
            {
                CreateNullService(serviceType);
            }

            return serviceMaps[serviceType];
        }
        return null;

    }

    void CreateNullService(SERVICE_TYPE serviceType)
    {
        switch (serviceType)
        {
            case SERVICE_TYPE.EXAMPLE:
                {
                    serviceMaps[serviceType] = ExampleBaseService.GetNullService(GetLoggedStatusPerType(serviceType));
                    break;
                }
            case SERVICE_TYPE.NONE:
                {
                    serviceMaps[serviceType] = GetNullService();
                    break;
                }
            case SERVICE_TYPE.TOTAL:
                {
                    serviceMaps[serviceType] = GetServiceOfType(SERVICE_TYPE.NONE);
                    break;
                }
        }
    }

    private void AddService(Service service,bool addToControlList = false)
    {
        serviceMaps[service.GetServiceType()] = service;
        if(addToControlList)
        {
            LoggedServiceStatus newService = new LoggedServiceStatus();
            newService.isLogged = service.IsLoggedService();
            logStatusPerServiceType.Add(newService);
            GenerateLoggedStatusMap();
        }
    }

    public Service GetNullService()
    {
        return Service.GenerateGameObjectWithService<NullService>(false);
    }

    private void OnValidate()
    {
        if(editorRegisteredServices != null)
        {
            serviceMaps.Clear();
            for(int i = 0; i < editorRegisteredServices.Count; i++)
            {
                if (editorRegisteredServices[i] != null)
                {
                    AddService(editorRegisteredServices[i]);
                }
            }
        }
    }
}
