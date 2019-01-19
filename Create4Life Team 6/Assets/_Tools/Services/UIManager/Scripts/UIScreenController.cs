using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// User interface screen controller.
/// </summary>
[System.Serializable]
public class UIScreenController
{
    private Service myManager;

    /// <summary>
    /// Screen changed event handler.It informs whether the screen is active or inactive.
    /// </summary>
    public delegate void ScreenChangedEventHandler(bool enabled);
    /// <summary>
    /// The on screen status changed.
    /// </summary>
    public ScreenChangedEventHandler OnScreenStatusChanged;
    /// <summary>
    /// Screen updated event handler.
    /// </summary>
    public delegate void ScreenUpdatedEventHandler();
    /// <summary>
    /// The on screen updated.
    /// </summary>
    public ScreenUpdatedEventHandler OnScreenUpdated;

    /// <summary>
    /// The user interface unique identifier.
    /// </summary>
    public string uiUniqueId;
    /// <summary>
    /// The do not destroy on deactivation flag.
    /// </summary>
    public bool doNotDestroyOnDeactivation = false;
    /// <summary>
    /// The is starting screen flag.
    /// </summary>
    public bool isStartingScreen;
    /// <summary>
    /// The is active screen status flag.
    /// </summary>
    public bool isActive;
    /// <summary>
    /// The is special pop up if it must skip the disponibility provider.
    /// </summary>
    public bool isSpecialPopUp;
    /// <summary>
    /// The user interface screen prefab.
    /// </summary>
    public UIScreenManager uiScreenPrefab;
    /// <summary>
    /// The current user interface screen object.
    /// </summary>
    public UIScreenManager uiScreenObject;
    /// <summary>
    /// The user interface screen position.
    /// </summary>
    public Vector3 uiScreenPosition;
    /// <summary>
    /// The complement screen identifiers.
    /// </summary>
    public List<string> complementScreenIds = new List<string>();

    public Service MyManager
    {
        get
        {
            return myManager;
        }
        set
        {
            myManager = value;
        }
    }

    /// <summary>
    /// Updates the complement identifiers.
    /// </summary>
    /// <param name="oldId">Old identifier.</param>
    /// <param name="newId">New identifier.</param>
    public void UpdateComplementIds(string oldId, string newId)
    {
        if (complementScreenIds != null)
        {
            for (int i = 0; i < complementScreenIds.Count; i++)
            {
                if (complementScreenIds[i] == oldId)
                {
                    complementScreenIds[i] = newId;
                }
            }
        }
    }
    /// <summary>
    /// Removes the complement identifier.
    /// </summary>
    /// <param name="idToRemove">Identifier to remove.</param>
    public void RemoveComplementId(string idToRemove)
    {
        if (complementScreenIds != null)
        {
            for (int i = 0; i < complementScreenIds.Count;)
            {
                if (complementScreenIds[i] == idToRemove)
                {
                    if (ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).IsLoggedService())
                    {
                        Debug.Log("Removing [" + idToRemove + "] from [" + uiUniqueId + "]");
                    }
                    complementScreenIds.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    /// <summary>
    /// Switch the specified screen.
    /// </summary>
    /// <param name="enable">If set to <c>true</c> enable.</param>
    /// <param name="doNotDestroy">If set to <c>true</c> do not destroy on deactivation.</param>
    public bool Switch(bool enable, bool doNotDestroy = false, bool forceDestroy = false)
    {
        //Debug.Log("Switching ["+_uiUniqueId+"] to ["+enable+"] GralNotDestroy["+doNotDestroy+"] PartiNotDestroy["+_doNotDestroyOnDeactivation+"]");
        bool result = false;
        if (enable)
        {
            if (!isActive)
            {
                //Debug.Log("UIScreenObj["+(_uiScreenObject == null)+"] uiPrefab["+(_uiScreenPrefab != null)+"]");
                if (uiScreenObject == null && uiScreenPrefab != null)
                {
                    if (Application.isPlaying)
                    {
                        //Debug.LogWarning("Setting screen object to new instantiate");
                        // Create uiScreen
                        uiScreenObject = GameObject.Instantiate<UIScreenManager>(uiScreenPrefab);
                    }
                }
                if (uiScreenObject != null)
                {
                    // Init uiScreen
                    uiScreenObject.SetPosition(uiScreenPosition);
                    uiScreenObject.Init(OnScreenStatusChanged);
                    result = true;
                }
            }
            else
            {
                //just update
                if (uiScreenObject != null)
                {
                    uiScreenObject.UpdateScreen(OnScreenUpdated);
                }
            }
        }
        else
        {
            if (uiScreenObject != null)
            {
                if (uiScreenObject.mustShowDebugInfo)
                {
                    Debug.LogFormat("Switch OFF [{0}] UIScreenObj[{1}] uiPrefab[{2}] destroy[{3}] forceDestroy[{4}]",
                        uiUniqueId, (uiScreenObject != null), (uiScreenPrefab != null), !doNotDestroy, forceDestroy);
                }
                if (isActive )
                {
                    //Deactivate uiScreen
                    uiScreenObject.Deactivate(OnScreenStatusChanged);
                }

                if (uiScreenPrefab != null && (!doNotDestroy || forceDestroy))
                {
                    //Destroy uiScreen
                    if (Application.isPlaying)
                    {
                        if (!doNotDestroyOnDeactivation || forceDestroy)
                        {
                            GameObject.Destroy(uiScreenObject.CachedGameObject);
                            //Debug.LogWarning("Setting screen object to null");
                            uiScreenObject = null;
                        }
                    }
                    else
                    {
                        GameObject.DestroyImmediate(uiScreenObject.CachedGameObject);
                        //Debug.LogWarning("Setting screen object to null");
                        uiScreenObject = null;
                    }
                }
                result = isActive;
            }
        }
        isActive = enable;
        return result;
    }

    /// <summary>
    /// Updates the screen.
    /// </summary>
    public void UpdateScreen()
    {
        if (isActive && uiScreenObject != null)
        {
            uiScreenObject.UpdateScreen(OnScreenUpdated);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UIScreenController"/> class.
    /// </summary>
    /// <param name="uniqueId">Unique identifier.</param>
    /// <param name="prefab">Prefab.</param>
    /// <param name="currentObject">Current object.</param>
    /// <param name="position">Position.</param>
    public UIScreenController(Service manager, string uniqueId, UIScreenManager prefab, UIScreenManager currentObject, Vector3 position)
    {
        myManager = manager;
       
        uiUniqueId = uniqueId;
        uiScreenPrefab = prefab;
        if (manager.IsLoggedService())
        {
            Debug.LogWarningFormat("Setting screen object to [{0}]", currentObject);
        }
        uiScreenObject = currentObject;
        uiScreenPosition = position;
        isActive = uiScreenObject != null;
        if (isActive)
        {
            uiScreenObject.uniqueScreenId = uiUniqueId;
            uiScreenObject.SetPosition(uiScreenPosition);
        }
    }

    /// <summary>
    /// Resets the position to a new one.
    /// </summary>
    /// <param name="newPositionInWorld">New position in world.</param>
    public void ResetPosition(Vector3 newPositionInWorld)
    {
        uiScreenPosition = newPositionInWorld;
        if (isActive)
        {
            uiScreenObject.SetPosition(uiScreenPosition);
        }
    }
    /// <summary>
    /// Gets the current user interface screen.
    /// </summary>
    /// <returns>The current user interface screen.</returns>
    public UIScreen GetCurrentUIScreen()
    {
        if (uiScreenObject != null)
        {
            return uiScreenObject.GetUIScreenInstance();
        }
        return null;
    }

    /// <summary>
    /// Gets the current user interface screen casting it to the T type passed.
    /// </summary>
    /// <returns>The current user interface screen.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public T GetCurrentUIScreen<T>() where T : UIScreen
    {
        if (uiScreenObject != null)
        {
            return uiScreenObject.GetUIScreenInstance<T>();
        }
        return null;
    }
    /// <summary>
    /// Haves the complement screen with identifier.
    /// </summary>
    /// <returns><c>true</c>, if complement screen with identifier was had, <c>false</c> otherwise.</returns>
    /// <param name="otherScreenId">Other screen identifier.</param>
    public bool HaveComplementScreenWithId(string otherScreenId)
    {
        if (complementScreenIds != null)
        {
            for (int i = 0; i < complementScreenIds.Count; i++)
            {
                if (complementScreenIds[i] == otherScreenId)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the camera depth.
    /// </summary>
    /// <returns>The camera depth.</returns>
    public float GetCameraDepth()
    {
        if (uiScreenObject != null)
        {
            return uiScreenObject.screenCameraDepth;
        }
        else if (uiScreenPrefab != null)
        {
            return uiScreenPrefab.screenCameraDepth;
        }
        else
        {
            return -100;
        }
    }

    /// <summary>
    /// Sets the camera depth.
    /// </summary>
    /// <param name="newDepth">New depth.</param>
    public void SetCameraDepth(float newDepth)
    {
        if (uiScreenPrefab != null)
        {
            uiScreenPrefab.screenCameraDepth = newDepth;
        }
        if (uiScreenObject != null)
        {
            uiScreenObject.UpdateCameraDepth(newDepth);
        }
    }
}
