using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Service : MonoBehaviour
{
    public bool autoInitialize = false;
    public bool dontDestroyOnLoad = false;

    public abstract SERVICE_TYPE GetServiceType();
    public abstract bool IsServiceNull();

    public abstract bool IsLoggedService();

    public abstract Service TransformService(bool isLogged);

    public static T GenerateGameObjectWithService<T>(bool initializeService = false) where T:Service
    {
        GameObject serviceGO = new GameObject(typeof(T).ToString());
        T service = serviceGO.AddComponent<T>();
        if (initializeService)
        {
            service.InitializeService();
        }
        return service;
    }

    public virtual void InitializeService()
    {
        DoDontDestroyOnLoadIfNecessary();
        ServiceLocator.Instance.RegisterService(this);
    }
	
    public void DoDontDestroyOnLoadIfNecessary()
    {
        if(dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        if (autoInitialize)
        {
            InitializeService();
        }
    }



}
