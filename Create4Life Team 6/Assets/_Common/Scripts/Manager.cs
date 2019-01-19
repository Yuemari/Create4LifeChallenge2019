using UnityEngine;
using System.Collections;

/// <summary>
/// Manager base class that contains the minimum requirements for every manager in the game.
/// </summary>
public abstract class Manager : CachedMonoBehaviour 
{
	public bool mustAutoStart = true; //makes this instance to auto start, i.e. not controlled by another class
	public bool	mustShowDebugInfo = false; //used to switch console logs
	public bool	mustPersistSceneChange = true;	//true if must persiste between scenes
	[HideInInspector]
	public bool isThisManagerValid = true; //used to know if after base class Awake this instance is valid or will be destroyed
	protected bool alreadystarted = false; //true if the StartManager function was alredy called

	/// <summary>
	/// Starts the manager. This function must be overrided and used instead of Unity's Start
	/// </summary>
	public virtual void StartManager()
	{
		alreadystarted = true;
	}

    protected void Log(string text,Object context)
    {
        if(mustShowDebugInfo)
        {
            Debug.Log(text, context);
        }
    }
    protected void LogFormat(Object context, string text, params object[] args)
    {
        if (mustShowDebugInfo)
        {
            Debug.LogFormat(context, text, args);
        }
    }
    protected void LogWarning(string text, Object context)
    {
        if (mustShowDebugInfo)
        {
            Debug.LogWarning(text, context);
        }
    }
    protected void LogWarningFormat(Object context, string text, params object[] args)
    {
        if (mustShowDebugInfo)
        {
            Debug.LogWarningFormat(context, text, args);
        }
    }
    protected void LogError(string text, Object context)
    {
        if (mustShowDebugInfo)
        {
            Debug.LogError(text, context);
        }
    }
    protected void LogErrorFormat(Object context, string text, params object[] args)
    {
        if (mustShowDebugInfo)
        {
            Debug.LogErrorFormat(context, text, args);
        }
    }
}

/// <summary>
/// Manager Template class that must be inherited to access the semi singleton pattern.
/// </summary>
[DisallowMultipleComponent]
public class Manager<T>: Manager where T : Manager
{
	protected static T cachedInstance; //unique instance of this class

	/// <summary>
	/// Sets the instance if there is none or destroy this instance if there is already another one.
	/// </summary>
	/// <param name="instance">Instance.</param>
	private static void SetInstance(T instance)
	{
		//check if there is an instance already registered
		if(cachedInstance == null)
		{
			//register this instance as the main one
			cachedInstance = instance;
			if(cachedInstance != null)
			{
				if(cachedInstance.mustPersistSceneChange)
				{
					GameObject root = cachedInstance.CachedGameObject.GetRootGameObject();
					GameObject.DontDestroyOnLoad(root);
				}
			}
		}
		else
		{
			if(cachedInstance.mustShowDebugInfo)
			{
				Debug.LogWarning("Destroying Manager["+instance.name+"] of type["+typeof(T)+"] because there is already one registered by the name["+cachedInstance.name+"].");
			}
			instance.isThisManagerValid = false;
			Destroy(instance.CachedGameObject);
		}
	}

	/// <summary>
	/// Gets the main instance.
	/// </summary>
	/// <returns>The main instance of this class if any.</returns>
	public static T GetInstance()
	{
		return cachedInstance;
	}

	/// <summary>
	/// Gets the main instance casted as needed.
	/// </summary>
	/// <returns>The casted instance.</returns>
	/// <typeparam name="R">The type to cast the instance parameter.</typeparam>
	public static R GetCastedInstance<R>() where R:T
	{
		return 	((R)cachedInstance);
	}

	/// <summary>
	/// Awake this instance and register it.
	/// </summary>
	protected virtual void Awake()
	{
		if(Application.isPlaying)
		{
			SetInstance(GetComponent<T>());
		}
	}

	/// <summary>
	/// Start this instance. This method is called by Unity and StartManager must be used instead Instead on Derived Classes
	/// </summary>
	protected void Start()
	{
		if(Application.isPlaying)
		{
			if(mustAutoStart)
			{
				StartManager();	
			}
		}
	}
}
