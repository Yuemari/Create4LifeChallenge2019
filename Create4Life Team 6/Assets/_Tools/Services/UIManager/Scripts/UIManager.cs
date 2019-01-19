using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// User interface manager.
/// </summary>
public class UIManager : Service 
{
    protected bool _isLogged;

    /// <summary>
    /// Screen status changed event handler.
    /// </summary>
    public delegate void ScreenStatusChangedEventHandler(string screenId,bool enabled);
	/// <summary>
	/// The on screen status changed.
	/// </summary>
	public static ScreenStatusChangedEventHandler OnScreenStatusChanged;
    /// <summary>
	/// The just disable on screen deactivation flag, 
	/// if true the Gameobjects of this screen will not be destroyed after swithing it off.
	/// </summary>
    public bool justDisableOnScreenDeactivation = true;
	/// <summary>
	/// All screen controllers.
	/// </summary>
	public List<UIScreenController>	allScreenControllers;
	/// <summary>
	/// The fast access ditionary with the screen controllers.
	/// </summary>
	private Dictionary<string,UIScreenController> fastAccessScreenControllers;
	/// <summary>
	/// The canvas scaler mode.
	/// </summary>
	public CanvasScaler.ScaleMode canvasScalerMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
	/// <summary>
	/// The canvas scaler screen match mode.
	/// </summary>
	public CanvasScaler.ScreenMatchMode canvasScalerScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
	/// <summary>
	/// The canvas match mode range.
	/// </summary>
	[Range(0,1)]
	public float canvasMatchModeRange = 0.5f;
	/// <summary>
	/// The canvas scaler reference resolution used when creating new screens.
	/// </summary>
	public Vector2 	canvasScalerReferenceResolution = new Vector2(800,600);
	/// <summary>
	/// The canvas scaler pixels per unit.
	/// </summary>
	public int canvasScalerPixelsPerUnit = 100;
	/// <summary>
	/// The screen position number of columns.
	/// </summary>
	public int	screenPositionNumberOfColumns = 3;
	/// <summary>
	/// The add help frame to created screens flag.
	/// </summary>
	public bool addHelpFrameToCreatedScreens = true;
	/// <summary>
	/// The help frame prefab.
	/// </summary>
	public GameObject	helpFramePrefab;
	/// <summary>
	/// The screen separation used to set screen positions.
	/// </summary>
	public Vector2	screenSeparation = new Vector2(19,11);//screen width with max aspect ratio(19),screen height with max aspect ratio(11)
	/// <summary>
	/// The Unity's layer this system will work with.
	/// </summary>
	public LayerMask systemLayer = 0;
	/// <summary>
	/// The current screen identifier.
	/// </summary>
	private string currentScreenId = string.Empty;

	private string lastScreenId = string.Empty;


    #region ServiceImp
    public override SERVICE_TYPE GetServiceType()
    {
        return SERVICE_TYPE.UIMANAGER;
    }

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
        _isLogged = ServiceLocator.GetLogStatusPerType(GetServiceType());
        StartManager();
    }

    public override Service TransformService(bool isLogged)
    {
        _isLogged = isLogged;
        return this;
    }
    #endregion


    /// <summary>
    /// Gets the current screen identifier.
    /// </summary>
    /// <value>The current screen identifier.</value>
    public string CurrentScreenId
	{
		get
		{
			return currentScreenId;
		}
	}

	public string LastScreenId
	{
		get
		{
			return lastScreenId;
		}
	}

	public static int GetUISystemLayer()
	{
		if(ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER) != null)
		{
			return ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER).systemLayer.value;
		}
		else
		{
			//try to find the object
			UIManager instance = GameObject.FindObjectOfType<UIManager>();
			if(instance != null)
			{
				return instance.systemLayer.value;
			}
			else
			{
				return 0;
			}
		}
	}
		
	/// <summary>
	/// Awake this instance and register the instance in this gameObject.
	/// </summary>
	private void StartManager()
	{
		RegisterControllers();
		SwitchToScreenWithIndex(GetStartingScreenIndex());
	}

	void OnDestroy()
	{
		if(Application.isPlaying)
		{
			SwitchAllScreens(false,false);
		}
	}

	/// <summary>
	/// Registers the controllers for fast access.
	/// </summary>
	void RegisterControllers()
	{
		fastAccessScreenControllers = new Dictionary<string, UIScreenController>();
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			if(!fastAccessScreenControllers.ContainsKey(allScreenControllers[i].uiUniqueId))
			{
				fastAccessScreenControllers.Add(allScreenControllers[i].uiUniqueId,allScreenControllers[i]);
                allScreenControllers[i].MyManager = this;
                //set controllers on UI screens if they exist
                UIScreen uiScreen = allScreenControllers[i].GetCurrentUIScreen();
                if(uiScreen != null)
                {
                    uiScreen.SetController(allScreenControllers[i]);
                }

            }
			else
			{
                if(_isLogged)
				    Debug.LogWarningFormat(this,"Theres already a UIScreen with Id[{0}]", allScreenControllers[i].uiUniqueId);
			}
		}
	}

	/// <summary>
	/// Gets the index of the starting screen.
	/// </summary>
	/// <returns>The starting screen index.</returns>
	int GetStartingScreenIndex()
	{
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			if(allScreenControllers[i].isStartingScreen)
			{
				return i;
			}
		}
		return 0;
	}

	/// <summary>
	/// Get the biggest camera depth used by the screens registered.
	/// </summary>
	/// <returns>The biggest camera depth.</returns>
	public static float GetBiggestCameraDepth()
	{
		//Min value for camera depth in Unity
		float biggestCameraDepth = -100;
        UIManager cachedInstance = ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER);

        if (cachedInstance != null)
		{
			for(int i = 0; i < cachedInstance.allScreenControllers.Count; i++)
			{
				float tempDepth = cachedInstance.allScreenControllers[i].GetCameraDepth();
				if(tempDepth > biggestCameraDepth)
				{
					tempDepth = biggestCameraDepth;
				}
			}
		}
		return biggestCameraDepth;
	}

	/// <summary>
	/// Switchs to screen with the index passed.
	/// </summary>
	/// <param name="screenIndex">Screen index.</param>
	public void SwitchToScreenWithIndex(int screenIndex,bool registerLast = true)
	{
		string tempCurrent = currentScreenId;
		if(screenIndex >= 0 && screenIndex < allScreenControllers.Count)
		{
			//activates screen
			if(allScreenControllers[screenIndex].Switch(true,justDisableOnScreenDeactivation || !Application.isPlaying))
			{
				currentScreenId = allScreenControllers[screenIndex].uiUniqueId;
				if(OnScreenStatusChanged != null && Application.isPlaying)
				{
					OnScreenStatusChanged(allScreenControllers[screenIndex].uiUniqueId,true);
				}
			}
			//deactivates all others
			for(int  i = 0; i < allScreenControllers.Count; i++)
			{
				if(screenIndex != i)
				{
					if(allScreenControllers[screenIndex].HaveComplementScreenWithId(allScreenControllers[i].uiUniqueId))
					{
						if(allScreenControllers[i].Switch(true,justDisableOnScreenDeactivation || !Application.isPlaying))
						{
							if(OnScreenStatusChanged != null && Application.isPlaying)
							{
								OnScreenStatusChanged(allScreenControllers[i].uiUniqueId,true);
							}
						}
							
					}
					else
					{
						if(allScreenControllers[i].Switch(false,justDisableOnScreenDeactivation || !Application.isPlaying))
						{
							if(allScreenControllers[i].uiScreenObject.mustRegisterForBackOperations)
							{
								lastScreenId = tempCurrent;
							}
							if(OnScreenStatusChanged != null && Application.isPlaying)
							{
								OnScreenStatusChanged(allScreenControllers[screenIndex].uiUniqueId,false);
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Switchs to screen with identifier.
	/// </summary>
	/// <param name="screenId">Screen identifier.</param>
	public void SwitchToScreenWithId(string screenId)
	{
        if (_isLogged)
            Debug.LogFormat(this,"Switching To Screen With Id[{0}]",screenId);
		
		int indexToSwitchOn = -1;
		string tempCurrent = currentScreenId;
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			if(screenId == allScreenControllers[i].uiUniqueId)	
			{
				indexToSwitchOn = i;
				break;
			}
		}
		if(indexToSwitchOn >= 0)
		{
			//activates screen
			if(allScreenControllers[indexToSwitchOn].Switch(true,justDisableOnScreenDeactivation || !Application.isPlaying))
			{
				currentScreenId = allScreenControllers[indexToSwitchOn].uiUniqueId;
				if(OnScreenStatusChanged != null && Application.isPlaying)
				{
					OnScreenStatusChanged(allScreenControllers[indexToSwitchOn].uiUniqueId,true);
				}
               
            }
            //activate complementary screens
            ActivateComplementaryScreensFor(allScreenControllers[indexToSwitchOn]);

           //deactivates all others
            for (int i = 0; i < allScreenControllers.Count; i++)
            {
                if (indexToSwitchOn != i)
                {
                    if (!allScreenControllers[indexToSwitchOn].HaveComplementScreenWithId(allScreenControllers[i].uiUniqueId))
                    {
                        if (allScreenControllers[i].Switch(false, justDisableOnScreenDeactivation || Application.isPlaying))
                        {
                            if (allScreenControllers[i].uiScreenObject != null)
                            {
                                if (allScreenControllers[i].uiScreenObject.mustRegisterForBackOperations)
                                {
                                    lastScreenId = tempCurrent;
                                }
                                if (OnScreenStatusChanged != null && Application.isPlaying)
                                {
                                    OnScreenStatusChanged(allScreenControllers[i].uiUniqueId, false);
                                }
                            }
                        }
                    }
                }
            }
           
		}
	}


    private void ActivateComplementaryScreensFor(UIScreenController controller)
    {
        if (controller != null && controller.complementScreenIds != null)
        {
            for (int i = 0; i < controller.complementScreenIds.Count; ++i)
            {
                SwitchScreenById(controller.complementScreenIds[i],true);
            }

        }
    }

	/// <summary>
	/// Switchs the screen by identifier.
	/// </summary>
	/// <param name="screenId">Screen identifier.</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public void SwitchScreenById(string screenId,bool enable, bool enableComplements = false)
	{
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			if(screenId == allScreenControllers[i].uiUniqueId)
			{
				if(allScreenControllers[i].Switch(enable, justDisableOnScreenDeactivation || !Application.isPlaying))
				{
					if(OnScreenStatusChanged != null && Application.isPlaying)
					{
						OnScreenStatusChanged(allScreenControllers[i].uiUniqueId,enable);
					}
                    if(enableComplements)
                    {
                        ActivateComplementaryScreensFor(allScreenControllers[i]);
                    }
				}
				break;
			}
		}
	}

	/// <summary>
	/// Updates the screen by identifier.
	/// </summary>
	/// <param name="screenId">Screen identifier.</param>
	public void UpdateScreenById(string screenId)
	{
		UIScreenController controller;
		if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
		{
			controller.UpdateScreen();
		}
	}

	/// <summary>
	/// Switchs the screen with index.
	/// </summary>
	/// <param name="screenIndex">Screen index.</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public void SwitchScreenByIndex(int screenIndex,bool enable)
	{
		if(screenIndex >= 0 && screenIndex < allScreenControllers.Count)
		{
			if(allScreenControllers[screenIndex].Switch(enable, justDisableOnScreenDeactivation || !Application.isPlaying))
			{
				if(OnScreenStatusChanged != null && Application.isPlaying)
				{
					OnScreenStatusChanged(allScreenControllers[screenIndex].uiUniqueId,enable);
				}
			}	
		}
	}

	/// <summary>
	/// Updates the screen by index.
	/// </summary>
	/// <param name="screenIndex">Screen index.</param>
	public void UpdateScreenByIndex(int screenIndex)
	{
		if(screenIndex >= 0 && screenIndex < allScreenControllers.Count)
		{
			allScreenControllers[screenIndex].UpdateScreen();
		}
	}

	/// <summary>
	/// Switchs all screens.
	/// </summary>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public void SwitchAllScreens(bool enable,bool forceDestroy = false)
	{
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			if(allScreenControllers[i].Switch(enable,  justDisableOnScreenDeactivation || !Application.isPlaying ,forceDestroy))
			{
				if(OnScreenStatusChanged != null && Application.isPlaying)
				{
					OnScreenStatusChanged(allScreenControllers[i].uiUniqueId,enable);
				}
			}
		}
	}

	/// <summary>
	/// Updates all active screens.
	/// </summary>
	public void UpdateAllActiveScreens()
	{
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			if(allScreenControllers[i].isActive)
			{
				allScreenControllers[i].UpdateScreen();
			}
		}
	}

	/// <summary>
	/// Gets the UIScreen for the screen with identifier.
	/// </summary>
	/// <returns>The user interface screen for identifier.</returns>
	/// <param name="screenId">Screen identifier.</param>
	public UIScreen GetUIScreenForId(string screenId)
	{
		UIScreenController controller;
		if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
		{
			return controller.GetCurrentUIScreen();
		}
		else
		{
            if (_isLogged)
                Debug.LogWarningFormat(this,"UIControllerNotFound by Id[{0}]",screenId);
			
			return null;
		}
	}

	/// <summary>
	/// Gets the UI controller for identifier.
	/// </summary>
	/// <returns>The user interface controller for identifier.</returns>
	/// <param name="screenId">Screen identifier.</param>
	public UIScreenController GetUIControllerForId(string screenId)
	{
		UIScreenController controller;
		if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
		{
			return controller;
		}
		else
		{
            if (_isLogged)
                Debug.LogWarningFormat(this, "UIControllerNotFound by Id[{0}]",screenId);
			
			return null;
		}
	}

	/// <summary>
	/// Determines whether this instance is screen active the specified screenId.
	/// </summary>
	/// <returns><c>true</c> if this instance is screen active the specified screenId; otherwise, <c>false</c>.</returns>
	/// <param name="screenId">Screen identifier.</param>
	public bool IsScreenActive(string screenId)
	{
		UIScreenController controller;
		if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
		{
			return controller.isActive;
		}
		else
		{
            if (_isLogged)
                Debug.LogWarningFormat(this, "UIControllerNotFound by Id[{0}]",screenId);
			
			return false;
		}
	}

	public static bool IsScreenWithIdActive(string screenId)
	{
        UIManager cachedInstance = ServiceLocator.Instance.GetServiceOfType<UIManager>(SERVICE_TYPE.UIMANAGER);
        if (cachedInstance != null)
		{
			return cachedInstance.IsScreenActive(screenId);
		}
		return false;
	}

	/// <summary>
	/// Gets the UIScreen casted to type T for the screen with identifier.
	/// </summary>
	/// <returns>The user interface screen for identifier.</returns>
	/// <param name="screenId">Screen identifier.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T GetUIScreenForId<T>(string screenId) where T : UIScreen
	{
		UIScreenController controller;
		if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
		{
			return controller.GetCurrentUIScreen<T>();
		}
		else
		{
            if (_isLogged)
                Debug.LogWarningFormat(this, "UIControllerNotFound by Id[{0}]",screenId);
			
			return null;
		}
	}

	/// <summary>
	/// Gets the position of this screen.
	/// </summary>
	/// <returns>The position.</returns>
	/// <param name="uniqueScreenId">Unique screen identifier.</param>
	public Vector3 GetPosition(string uniqueScreenId)
	{
		bool founded = false;
		Vector3 screenWorldPosition = Vector3.zero;
		for(int i = 0; i < allScreenControllers.Count; i++)
		{
			if(allScreenControllers[i].uiUniqueId == uniqueScreenId)
			{
				screenWorldPosition = allScreenControllers[i].uiScreenPosition;
				founded = true;
				break;
			}
		}
		if(!founded)
		{
			screenWorldPosition = CalculateNewPositionInWorld(allScreenControllers.Count);
		}
		return screenWorldPosition;
	}

	/// <summary>
	/// Resets all screen positions.
	/// </summary>
	public void ResetAllPositions()
	{
		for(int i = 0; i < allScreenControllers.Count; i++)
		{
			allScreenControllers[i].ResetPosition(CalculateNewPositionInWorld(i));
		}
	}

	/// <summary>
	/// Registers to status change event for screen with identifier.
	/// </summary>
	/// <param name="screenId">Screen identifier.</param>
	/// <param name="listener">Listener.</param>
	public void RegisterToChangeEventForScreenWithId(string screenId,UIScreenController.ScreenChangedEventHandler listener,bool sendCurrentstate = true)
	{
		if(listener != null)
		{
			UIScreenController controller;
			if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
			{
				controller.OnScreenStatusChanged += listener;
				if(sendCurrentstate)
				{
					listener(controller.isActive);
				}
			}
		}
	}

	/// <summary>
	/// Unregisters to status change event for screen with identifier.
	/// </summary>
	/// <param name="screenId">Screen identifier.</param>
	/// <param name="listener">Listener.</param>
	public void UnregisterToChangeEventForScreenWithId(string screenId,UIScreenController.ScreenChangedEventHandler listener)
	{
		UIScreenController controller;
		if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
		{
			controller.OnScreenStatusChanged -= listener;
		}
	}

	/// <summary>
	/// Registers to update event for screen with identifier.
	/// </summary>
	/// <param name="screenId">Screen identifier.</param>
	/// <param name="listener">Listener.</param>
	public void RegisterToUpdateEventForScreenWithId(string screenId,UIScreenController.ScreenUpdatedEventHandler listener)
	{
		UIScreenController controller;
		if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
		{
			controller.OnScreenUpdated += listener;
		}
	}

	/// <summary>
	/// Unregisters to update event for screen with identifier.
	/// </summary>
	/// <param name="screenId">Screen identifier.</param>
	/// <param name="listener">Listener.</param>
	public void UnregisterToUpdateEventForScreenWithId(string screenId,UIScreenController.ScreenUpdatedEventHandler listener)
	{
		UIScreenController controller;
		if(fastAccessScreenControllers.TryGetValue(screenId,out controller))
		{
			controller.OnScreenUpdated -= listener;
		}
	}



	#region EDITOR HELPING FUNCTIONS
	/// <summary>
	/// Tries to get the index of the UIScreen controller.
	/// </summary>
	/// <returns>The UIScreen controller index.</returns>
	/// <param name="screenId">Screen identifier.</param>
	public int TryGetUIScreenControllerIndex(string screenId)
	{
		for(int i = 0; i < allScreenControllers.Count; i++)
		{
			if(allScreenControllers[i].uiUniqueId == screenId)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Gets the complement managers for screen with identifier.
	/// </summary>
	/// <returns>The complement managers for identifier.</returns>
	/// <param name="screenId">Screen identifier.</param>
	List<int> GetComplementManagersForId(string screenId)
	{
		List<int> complements = new List<int>();
		int controller = TryGetUIScreenControllerIndex(screenId);
		if(controller >= 0)
		{
			for(int i = 0; i < allScreenControllers[controller].complementScreenIds.Count; i++)
			{
				int complementIndex = TryGetUIScreenControllerIndex(allScreenControllers[controller].complementScreenIds[i]);
				if(complementIndex >= 0)
				{
					complements.Add(complementIndex);
				}
			}
		}
		return complements;
	}

	/// <summary>
	/// Calculates the new position in world.
	/// </summary>
	/// <returns>The new position in world.</returns>
	/// <param name="screenIndex">Screen index.</param>
	Vector3 CalculateNewPositionInWorld(int screenIndex)
	{
		Vector3 finalPosition = Vector3.zero;
		int xCoord = screenIndex%screenPositionNumberOfColumns;
		int yCoord = screenIndex/screenPositionNumberOfColumns;
		finalPosition = new Vector3(screenSeparation.x*xCoord, screenSeparation.y*yCoord, 0);
		return finalPosition;
	}

	/// <summary>
	/// Gets an unique identifier.
	/// </summary>
	/// <returns>The unique identifier.</returns>
	/// <param name="proposedIdComplement">Proposed identifier complement.</param>
	private string GetUniqueId(int proposedIdComplement)
	{
		string id = "UISM_"+proposedIdComplement;
		for(int i = 0; i < allScreenControllers.Count; i++)
		{
			if(id == allScreenControllers[i].uiUniqueId)
			{
				int newProposal = proposedIdComplement+1;
				id = GetUniqueId(newProposal);
			}
		}
		return id;
	}

	/// <summary>
	/// Creates the a new UIScreen manager along with its controller.
	/// </summary>
	/// <returns>The new user interface screen manager.</returns>
	/// <param name="newScreenId">New screen identifier.</param>
	public GameObject CreateNewUIScreenManager(string newScreenId = "")
	{
        if (_isLogged)
            Debug.LogFormat(this,"Creating new UISCreen with layer[{0}][{1}]", LayerMask.LayerToName(systemLayer), systemLayer.value);
		
		GameObject go = new GameObject((newScreenId == "" ? GetUniqueId(allScreenControllers.Count) : newScreenId));
		go.layer = systemLayer.value;
		Camera cam = go.AddComponent<Camera>();
		if(cam != null)
		{
			cam.orthographic = true;
			cam.orthographicSize = 5;
			cam.allowHDR = false;
			cam.useOcclusionCulling = true;
			cam.clearFlags = CameraClearFlags.Depth;
			cam.cullingMask = 1 << systemLayer.value;
			cam.farClipPlane = 200.0f;
			//add child Canvas
			GameObject canvasGO = new GameObject("Canvas");
			canvasGO.transform.SetParent(go.transform);
			canvasGO.layer = systemLayer.value;
			canvasGO.transform.localPosition = Vector3.zero;
			Canvas canvas = canvasGO.AddComponent<Canvas>();
			if(canvas != null)
			{
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = cam;
				canvas.planeDistance = 100;
				canvas.sortingLayerID = systemLayer.value;
				CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
				if(scaler != null)
				{
					scaler.uiScaleMode =canvasScalerMode;
					scaler.screenMatchMode = canvasScalerScreenMatchMode;
					scaler.matchWidthOrHeight = canvasMatchModeRange;
					scaler.referenceResolution = canvasScalerReferenceResolution;
					scaler.referencePixelsPerUnit = canvasScalerPixelsPerUnit;
				}

				GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
				if(raycaster != null)
				{
					raycaster.ignoreReversedGraphics = true;
					raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
				}

				UIScreenManager screenManager = go.AddComponent<UIScreenManager>();
				if(screenManager != null)
				{
					Vector3 newPosition = CalculateNewPositionInWorld(allScreenControllers.Count);
					//create controller for this screenManager
					UIScreenController controller = new UIScreenController(this,go.name,null,screenManager,newPosition);
					allScreenControllers.Add(controller);
				}

				if(addHelpFrameToCreatedScreens && helpFramePrefab != null)
				{
					GameObject helpFrame = GameObject.Instantiate(helpFramePrefab);
                    helpFrame.name = helpFramePrefab.name;
					helpFrame.transform.SetParent(canvasGO.transform);
					helpFrame.transform.localScale = Vector3.one;
					RectTransform rect = helpFrame.GetComponent<RectTransform>();
					if(rect != null)
					{
						rect.anchorMin = Vector2.zero;
						rect.anchorMax = Vector2.one;
						rect.offsetMin = Vector2.zero;
						rect.offsetMax = Vector2.zero;
					}
                    helpFrame.transform.localPosition = Vector3.zero;
					Text textId = helpFrame.GetComponentInChildren<Text>();
					if(textId != null)
					{
						textId.text = go.name;
					}
				}
			}
		}
		return go;
	}

	/// <summary>
	/// Creates the new empty controller alone.
	/// </summary>
	public void CreateNewEmptyControllerAlone()
	{
		//create controller for this screenManager
		Vector3 newPosition = CalculateNewPositionInWorld(allScreenControllers.Count);
		UIScreenController controller = new UIScreenController(this,GetUniqueId(allScreenControllers.Count),null,null,newPosition);
		allScreenControllers.Add(controller);
	}

	/// <summary>
	/// Gets a unique identifier from the passed Id.
	/// </summary>
	/// <returns>The unique identifier from.</returns>
	/// <param name="currentId">Current identifier.</param>
	private string GetUniqueIdFrom(string currentId)
	{
		for(int i = 0; i < allScreenControllers.Count; i++)
		{
			if(currentId == allScreenControllers[i].uiUniqueId)
			{
				currentId = GetUniqueId(allScreenControllers.Count);
			}
		}
		return currentId;
	}

	/// <summary>
	/// Creates a new UIScreen Controller from an UIScreenManager in scene.
	/// </summary>
	/// <returns>The new user interface screen controller from scene object.</returns>
	/// <param name="screenManager">Screen manager.</param>
	public UIScreenManager CreateNewUIScreenControllerFromSceneObject(UIScreenManager screenManager)
	{
		bool alreadyExist = false;	
		for(int i = 0; i < allScreenControllers.Count; i++)
		{
			if(allScreenControllers[i].uiUniqueId == screenManager.uniqueScreenId)
			{
				alreadyExist = true;
				break;
			}
		}

        Debug.LogFormat(this,"Creating new UISCreenController. AlreadyExist?[{0}]", alreadyExist);
		
		if(!alreadyExist)
		{
			string id = GetUniqueIdFrom(screenManager.uniqueScreenId);
			Vector3 newPosition = CalculateNewPositionInWorld(allScreenControllers.Count);
			//create controller for this screenManager
			UIScreenController controller = new UIScreenController(this,id,null,screenManager,newPosition);
			allScreenControllers.Add(controller);
			return screenManager;
		}
		else
		{
			Debug.LogWarningFormat(this,"A Controller with Id[{0}] already exist! It cannot be created in this manager.", screenManager.uniqueScreenId);
			
			return null;
		}
	}

	/// <summary>
	/// Gets the sibling index in hierarchy for the lowest screen.
	/// </summary>
	/// <returns>The last screen sibling index.</returns>
	public int GetLastScreenSiblingIndex()
	{
		int lastSiblingIndex = transform.GetSiblingIndex();
		//Debug.Log("UIManagerSiblingIndex["+lastSiblingIndex+"]");
		for(int i = 0; i < allScreenControllers.Count; i++)
		{
			if(allScreenControllers[i].uiScreenObject != null)
			{
				int newSiblingIndex = allScreenControllers[i].uiScreenObject.CachedTransform.GetSiblingIndex();
				if(newSiblingIndex > lastSiblingIndex)
				{
					lastSiblingIndex = newSiblingIndex;
				}
			}
		}
		return lastSiblingIndex;
	}

	/// <summary>
	/// Removes the user interface screen manager.
	/// </summary>
	/// <param name="screenToRemove">Screen to remove.</param>
	/// <param name="mustDestroyGameObject">If set to <c>true</c> must destroy game object.</param>
	public void RemoveUIScreenManager(UIScreenManager screenToRemove,bool mustDestroyGameObject = true)
	{
		if(screenToRemove != null)
		{
			int indexToRemove = -1;
			for(int  i = 0; i < allScreenControllers.Count; i++)
			{
				
				bool hasSameGO = allScreenControllers[i].uiScreenObject == screenToRemove;
				bool hasSameId = allScreenControllers[i].uiUniqueId == screenToRemove.uniqueScreenId;
				if(hasSameGO || hasSameId)
				{
					Debug.LogFormat(this,"Removing UIscreenManager[{0}] with Id[{1}] by Object?[{2}] by Id?[{3}]", 
                        screenToRemove.name, screenToRemove.uniqueScreenId, hasSameGO, hasSameId);

					indexToRemove = i;
					break;
				}
			}
			if(indexToRemove >= 0)
			{
				allScreenControllers.RemoveAt(indexToRemove);
				if(mustDestroyGameObject)
				{
					DestroyImmediate(screenToRemove.gameObject);
				}
				ResetAllPositions();
			}
		}
	}

	/// <summary>
	/// Removes the user interface screen manager controller.
	/// </summary>
	/// <param name="uniqueId">Unique identifier.</param>
	/// <param name="justSetAsNull">If set to <c>true</c> just set as null and do not remove.</param>
	public void RemoveUIScreenManagerController(string uniqueId, bool justSetAsNull)
	{
        int indexToRemove = -1;
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			bool hasSameId = allScreenControllers[i].uiUniqueId == uniqueId;
			if(hasSameId && indexToRemove == -1)
			{
                if (IsLoggedService())
                {
                    Debug.LogFormat(this, "Removing UIscreenManagerController with Id[{0}]", uniqueId);
                }
				indexToRemove = i;
			}
			if(!justSetAsNull)
			{
				allScreenControllers[i].RemoveComplementId(uniqueId);
			}
		}
		if(indexToRemove >= 0)
		{
			if(justSetAsNull)
			{
                if (IsLoggedService())
                {
                    Debug.LogWarning("Setting screen object to null");
                }
				allScreenControllers[indexToRemove].uiScreenObject = null;	
			}
			else
			{
				allScreenControllers.RemoveAt(indexToRemove);
				ResetAllPositions();
			}
		}
	}

	/// <summary>
	/// Reset this instance.
	/// </summary>
	public void Reset()
	{
		if(allScreenControllers != null)
		{
			while(allScreenControllers.Count > 0)
			{
				if(allScreenControllers[0].uiScreenObject != null)
				{
					if(Application.isPlaying)
					{
						Destroy(allScreenControllers[0].uiScreenObject.gameObject);
					}
					else
					{
						DestroyImmediate(allScreenControllers[0].uiScreenObject.gameObject);
					}
				}
				allScreenControllers.RemoveAt(0);
			}
			allScreenControllers.Clear();
			ResetAllPositions();
		}
	}
		
	/// <summary>
	/// Despawns all screens on scene.
	/// </summary>
	public void DespawnAllScreensOnScene()
	{
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			allScreenControllers[i].Switch(false);
		}
	}

	/// <summary>
	/// Despawns all screens on scene that are not initial.
	/// </summary>
	public void DespawnAllScreensOnSceneThatAreNotInitial()
	{
		List<int> allInitials = new List<int>();
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			if(allScreenControllers[i].isStartingScreen)	
			{
				if(!allInitials.Contains(i))
				{
					allInitials.Add(i);
				}
				List<int> complements = GetComplementManagersForId(allScreenControllers[i].uiUniqueId);
				for(int j = 0; j < complements.Count; j++)
				{
					if(complements[j] >= 0)
					{
						if(!allInitials.Contains(complements[j]))
						{
							allInitials.Add(complements[j]);
						}
					}
				}
			}
		}
		for(int  i = 0; i < allScreenControllers.Count; i++)
		{
			if(!allInitials.Contains(i))
			{
				allScreenControllers[i].Switch(false,false);	
			}
		}

	}
		
	#endregion
}
