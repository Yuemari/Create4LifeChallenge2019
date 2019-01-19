using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pop ups manager that works with the UIManager system.
/// </summary>
public class PopUpsManager : Service
{
    protected bool _isLogged;
    /// <summary>
    /// Pop up status changed event handler. Sends whether this pop up changed to active or inactive.
    /// </summary>
    public delegate void PopUpStatusChangedEventHandler(string popupId,bool enabled);
	/// <summary>
	/// The on pop up status changed.
	/// </summary>
	public static PopUpStatusChangedEventHandler OnPopUpStatusChanged;

	public delegate void AnyPopUpActiveChangedEventHandler(bool anyPopUpActive);

	public static AnyPopUpActiveChangedEventHandler OnAnyPopUpActiveChanged;

	public static bool s_isAnyPopUpActive = false;

	public PopUpsDisponibilityProvider disponibilityProvider;

	/// <summary>
	/// The minimum camera depth used for all pop ups, it will be override by the max depth in the UIScreens.
	/// </summary>
	public float minCameraDepth = 50.0f;
	/// <summary>
	/// The current camera depth the pop ups have.
	/// </summary>
	private float currentCameraDepth = 0.0f;
	/// <summary>
	/// The just disable on pop up deactivation instead of destroying it.
	/// </summary>
	public bool justDisableOnPopUpDeactivation = true;
	/// <summary>
	/// All pop up controllers.
	/// </summary>
	public List<UIScreenController>	allPopUpControllers;
	/// <summary>
	/// All pop up backgrounds controllers.
	/// </summary>
	public List<UIScreenController>	allPopUpBackgroundsControllers;
	/// <summary>
	/// The fast access pop up controllers including backgrounds.
	/// </summary>
	private Dictionary<string,UIScreenController> fastAccessPopUpControllers;
	/// <summary>
	/// The canvas scaler reference resolution to use when creatin a new pop up.
	/// </summary>
	public Vector2 	canvasScalerReferenceResolution = new Vector2(800,600);
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
	/// The canvas scaler pixels per unit.
	/// </summary>
	public int canvasScalerPixelsPerUnit = 100;
	/// <summary>
	/// The popups position number of columns.
	/// </summary>
	public int	popupsPositionNumberOfColumns = 3;
	/// <summary>
	/// The add help frame to newly created pop ups.
	/// </summary>
	public bool addHelpFrameToCreatedPopUps = true;
	/// <summary>
	/// The pop up help frame prefab.
	/// </summary>
	public GameObject	popUpHelpFramePrefab;
	/// <summary>
	/// The pop up bkg help frame prefab.
	/// </summary>
	public GameObject	popUpBkgHelpFramePrefab;
	/// <summary>
	/// The pop up screen separation offset.
	/// </summary>
	public Vector2	screenSeparation = new Vector2(19,11);//screen width with max aspect ratio(19),screen height with max aspect ratio(11)
	/// <summary>
	/// The system layer.
	/// </summary>
	public LayerMask	systemLayer = 0;

	public List<string> popUpQueue;

	public List<string> screenProhibitedForPopUps = new List<string>();

    #region ServiceImp
    public override SERVICE_TYPE GetServiceType()
    {
        return SERVICE_TYPE.POPUPSMANAGER;
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
    /// Awake this instance and register the instance in this gameObject.
    /// </summary>
    protected void Awake ()
	{
		popUpQueue = new List<string>();
		if(disponibilityProvider == null)
		{
			disponibilityProvider = GetComponent<PopUpsDisponibilityProvider>();
		}
		RegisterControllers();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void StartManager()
	{
		minCameraDepth = Mathf.Max(minCameraDepth,UIManager.GetBiggestCameraDepth());
		SetCameraDepth(minCameraDepth);
		SwitchAllPopUps(false);
	}

	void CheckIfAnyPopUpActiveChanged()
	{
		if(!Application.isPlaying)
			return;

		bool currentStatus = false;
		for(int i = 0; i < allPopUpControllers.Count; i++)
		{
			if(allPopUpControllers[i].isActive)
			{
				currentStatus = true;
				break;
			}
		}

		if(s_isAnyPopUpActive != currentStatus)
		{
			s_isAnyPopUpActive = currentStatus;
			if(OnAnyPopUpActiveChanged != null)
			{
				OnAnyPopUpActiveChanged(s_isAnyPopUpActive);
			}
		}
	}

	/// <summary>
	/// Sets the camera depth.
	/// </summary>
	/// <param name="newDepth">New depth.</param>
	void SetCameraDepth(float newDepth)
	{
		if(newDepth >= minCameraDepth && newDepth <= 100)
		{
			currentCameraDepth = newDepth;
		}
	}

	/// <summary>
	/// Updates the camera depth.
	/// </summary>
	void UpdateCameraDepth()
	{
		float minCurrentCameraDepth = minCameraDepth;
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			if(allPopUpControllers[i].isActive)
			{
				float depth = allPopUpControllers[i].GetCameraDepth();
				if(depth > minCurrentCameraDepth)
				{
					minCurrentCameraDepth = depth;
				}
			}
		}
		for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(allPopUpBackgroundsControllers[i].isActive)
			{
				float depth = allPopUpBackgroundsControllers[i].GetCameraDepth();
				if(depth > minCurrentCameraDepth)
				{
					minCurrentCameraDepth = depth;
				}
			}
		}

		SetCameraDepth(minCurrentCameraDepth);
	}
		
	/// <summary>
	/// Registers the controllers.
	/// </summary>
	void RegisterControllers()
	{
		fastAccessPopUpControllers = new Dictionary<string, UIScreenController>();
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			if(!fastAccessPopUpControllers.ContainsKey(allPopUpControllers[i].uiUniqueId))
			{
				fastAccessPopUpControllers.Add(allPopUpControllers[i].uiUniqueId,allPopUpControllers[i]);
                allPopUpControllers[i].MyManager = this;
                //set controllers on UI screens if they exist
                UIScreen uiScreen = allPopUpControllers[i].GetCurrentUIScreen();
                if (uiScreen != null)
                {
                    uiScreen.SetController(allPopUpControllers[i]);
                }
            }
			else
			{
				Debug.LogWarningFormat(this,"Theres already a UIPopUpScreen with Id[{0}]",allPopUpControllers[i].uiUniqueId);
				
			}
		}
		for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(!fastAccessPopUpControllers.ContainsKey(allPopUpBackgroundsControllers[i].uiUniqueId))
			{
				fastAccessPopUpControllers.Add(allPopUpBackgroundsControllers[i].uiUniqueId,allPopUpBackgroundsControllers[i]);
                allPopUpBackgroundsControllers[i].MyManager = this;
                //set controllers on UI screens if they exist
                UIScreen uiScreen = allPopUpBackgroundsControllers[i].GetCurrentUIScreen();
                if (uiScreen != null)
                {
                    uiScreen.SetController(allPopUpBackgroundsControllers[i]);
                }
            }
			else
			{
				Debug.LogWarningFormat(this,"Theres already a UIPopUpScreen Background with Id[{0}]", allPopUpBackgroundsControllers[i].uiUniqueId);
			}
		}
	}
		
	/// <summary>
	/// Gets the biggest camera depth.
	/// </summary>
	/// <returns>The biggest camera depth.</returns>
	public static float GetBiggestCameraDepth()
	{
		//Min value for camera depth in Unity
		float biggestCameraDepth = -100;
        PopUpsManager cachedInstance = ServiceLocator.Instance.GetServiceOfType<PopUpsManager>(SERVICE_TYPE.POPUPSMANAGER);
        if (cachedInstance != null)
		{
			for(int i = 0; i < cachedInstance.allPopUpControllers.Count; i++)
			{
				float tempDepth = cachedInstance.allPopUpControllers[i].GetCameraDepth();
				if(tempDepth > biggestCameraDepth)
				{
					tempDepth = biggestCameraDepth;
				}
			}
		}
		return biggestCameraDepth;
	}

	private bool IsPopUpWithIdSpecial(string popUpId)
	{
		if(fastAccessPopUpControllers != null)
		{
			UIScreenController popUpController;
			if(fastAccessPopUpControllers.TryGetValue(popUpId,out popUpController))
			{
				return popUpController.isSpecialPopUp;
			}
		}
		else
		{
			int popUpIndex = -1;
			for(int  i = 0; i < allPopUpControllers.Count; i++)
			{
				if(popUpId == allPopUpControllers[i].uiUniqueId)	
				{
					popUpIndex = i;
					break;
				}
			}
			if(popUpIndex >= 0)
			{
				return allPopUpControllers[popUpIndex].isSpecialPopUp;
			}
		}
		return false;
	}

	public void QueueOrShowPopUpWithId(string popUpId)
	{
		bool canShow = true;
		if(disponibilityProvider != null)
		{
			canShow = disponibilityProvider.CanShowNonSpecialPopUps() || IsPopUpWithIdSpecial(popUpId);
		}
			
		//second check if there is any pop up active
		//if it is not, show this pop up
		if(!s_isAnyPopUpActive && canShow)
		{
			ShowPopUpWithId(popUpId);
		}
		//else, add to queue if it is not already active
		else if(!popUpQueue.Contains(popUpId))
		{
			if(!IsPopUpActive(popUpId))
			{
				popUpQueue.Add(popUpId);
			}
		}
	}

	public bool IsPopUpWithIdAlreadyQueued(string popUpId)
	{
		for(int i = 0; popUpQueue.Count > 0; i++)
		{
			if(popUpQueue[i] == popUpId)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Shows the pop up with identifier.
	/// </summary>
	/// <param name="popUpId">Pop up identifier.</param>
	/// <param name="closeOtherPopUps">If set to <c>true</c> close other pop ups.</param>
	public void ShowPopUpWithId(string popUpId,bool closeOtherPopUps = false)
	{
		StartCoroutine(ShowPopUpIfAble(popUpId,closeOtherPopUps));
	}

	private IEnumerator ShowPopUpIfAble(string popUpId,bool closeOtherPopUps)
	{
		while(!IsPopUpAllowed())
		{
			yield return 0;
			Debug.LogFormat(this,"Wait for blocking screen before showing popup with id[{0}]", popUpId);
			
		}

		if(fastAccessPopUpControllers != null)
		{
			UIScreenController popUpController;
			if(fastAccessPopUpControllers.TryGetValue(popUpId,out popUpController))
			{
				SwitchPopUpBackgroundsForPopUp(popUpController,true);
				//activates pop up
				SetCameraDepth(currentCameraDepth + 1);
				popUpController.SetCameraDepth(currentCameraDepth);
				if(popUpController.Switch(true))
				{
					if(OnPopUpStatusChanged != null)
					{
						OnPopUpStatusChanged(popUpController.uiUniqueId,true);
					}
					CheckIfAnyPopUpActiveChanged();
				}
				if(closeOtherPopUps)
				{
					HideAllBut(popUpId);
					HideAllBackgroundsUnrelated(popUpController);

				}
			}
		}
		else
		{
			int indexToSwitchOn = -1;
			for(int  i = 0; i < allPopUpControllers.Count; i++)
			{
				if(popUpId == allPopUpControllers[i].uiUniqueId)	
				{
					indexToSwitchOn = i;
					break;
				}
			}
			if(indexToSwitchOn >= 0)
			{
				SwitchPopUpBackgroundsForPopUp(allPopUpControllers[indexToSwitchOn],true);
				//activates pop up
				SetCameraDepth(currentCameraDepth + 1);
				allPopUpControllers[indexToSwitchOn].SetCameraDepth(currentCameraDepth);
				if(allPopUpControllers[indexToSwitchOn].Switch(true))
				{
					if(OnPopUpStatusChanged != null)
					{
						OnPopUpStatusChanged(allPopUpControllers[indexToSwitchOn].uiUniqueId,true);
					}
					CheckIfAnyPopUpActiveChanged();
				}
			}
		}
	}

	private bool IsPopUpAllowed()
	{
		for(int i = 0; i < screenProhibitedForPopUps.Count; i++)
		{
			if(UIManager.IsScreenWithIdActive(screenProhibitedForPopUps[i]))
			{
				return false;
			}
		}
		return true;
	}

	private void HideAllBut(string popUpId)
	{
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			if(popUpId != allPopUpControllers[i].uiUniqueId)	
			{
				allPopUpControllers[i].Switch(false, justDisableOnPopUpDeactivation || !Application.isPlaying);
				SwitchPopUpBackgroundsForPopUp(allPopUpControllers[i],false);
			}
		}
	}

	private void HideAllBackgroundsUnrelated(UIScreenController popUp)
	{
		for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			//if it is unrelated
			if(!popUp.HaveComplementScreenWithId(allPopUpBackgroundsControllers[i].uiUniqueId))
			{
				allPopUpBackgroundsControllers[i].Switch(false, justDisableOnPopUpDeactivation || !Application.isPlaying);
			}
		}
	}

	/// <summary>
	/// Hides the pop up with identifier.
	/// </summary>
	/// <param name="popUpId">Pop up identifier.</param>
	public void HidePopUpWithId(string popUpId)
	{
		if(fastAccessPopUpControllers != null)
		{
			UIScreenController popUpController;
			if(fastAccessPopUpControllers.TryGetValue(popUpId,out popUpController))
			{
				SwitchPopUpBackgroundsForPopUp(popUpController,false);
				//Deactivates pop up
				if(popUpController.Switch(false,justDisableOnPopUpDeactivation))
				{
					if(OnPopUpStatusChanged != null)
					{
						OnPopUpStatusChanged(popUpController.uiUniqueId,false);
					}
					UpdateCameraDepth();
					CheckIfAnyPopUpActiveChanged();
				}
			}
		}
		else
		{
			int indexToSwitchOn = -1;
			for(int  i = 0; i < allPopUpControllers.Count; i++)
			{
				if(popUpId == allPopUpControllers[i].uiUniqueId)	
				{
					indexToSwitchOn = i;
					break;
				}
			}
			if(indexToSwitchOn >= 0)
			{
				SwitchPopUpBackgroundsForPopUp(allPopUpControllers[indexToSwitchOn],false);
				//deactivates pop up
				if(allPopUpControllers[indexToSwitchOn].Switch(false,justDisableOnPopUpDeactivation))
				{
					if(OnPopUpStatusChanged != null)
					{
						OnPopUpStatusChanged(allPopUpControllers[indexToSwitchOn].uiUniqueId,false);
					}
					UpdateCameraDepth();
					CheckIfAnyPopUpActiveChanged();
				}
			}
		}
	}

	/// <summary>
	/// Switchs the pop up backgrounds for pop up. A background can't be disable if a nother pop up is using it.
	/// </summary>
	/// <param name="controller">Controller.</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	private void SwitchPopUpBackgroundsForPopUp(UIScreenController controller,bool enable)
	{
		Debug.LogFormat(this,"Switching [{0}] PopUpBkgs for[{1}] to [{2}]. Fast[{3}]", 
            controller.complementScreenIds.Count, controller.uiUniqueId, enable, (fastAccessPopUpControllers != null));
		
		if(fastAccessPopUpControllers != null)
		{
			for(int  i = 0; i < controller.complementScreenIds.Count; i++)
			{
				UIScreenController backgroundController;
				if(fastAccessPopUpControllers.TryGetValue(controller.complementScreenIds[i],out backgroundController))
				{
					Debug.LogFormat(this,"Try Switching PopUpBkg [{0}] to [{1}]", backgroundController.uiUniqueId,enable);
					
					if(enable)
					{
						SetCameraDepth(currentCameraDepth + 1);
						backgroundController.SetCameraDepth(currentCameraDepth);
						backgroundController.Switch(true,justDisableOnPopUpDeactivation || !Application.isPlaying);	
						if(!backgroundController.HaveComplementScreenWithId(controller.uiUniqueId))
						{
							backgroundController.complementScreenIds.Add(controller.uiUniqueId);
						}
					}
					else
					{
						if(backgroundController.HaveComplementScreenWithId(controller.uiUniqueId))
						{
							backgroundController.complementScreenIds.Remove(controller.uiUniqueId);
							Debug.LogFormat(this,"After Removed[{0}]", backgroundController.complementScreenIds.Count);
							
						}
						if(backgroundController.complementScreenIds.Count == 0)
						{
							Debug.Log("Effectively switching background controller to false",this);
							
							if(backgroundController.Switch(false,justDisableOnPopUpDeactivation || !Application.isPlaying))
							{	
								UpdateCameraDepth();
							}
						}
					}
				}
				else
				{
                    Debug.LogWarningFormat(this, "PopUpBkg [{0}] Not Founded", controller.complementScreenIds[i]);
				}
			}
		}
		else
		{
			//Activates/deactivates all related backgrounds
			for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
			{
				if(controller.HaveComplementScreenWithId(allPopUpBackgroundsControllers[i].uiUniqueId))
				{
					Debug.LogFormat(this,"Try Switching PopUpBkg [{0}] to [{1}]", allPopUpBackgroundsControllers[i].uiUniqueId,enable);
					
					if(enable)
					{
						SetCameraDepth(currentCameraDepth + 1);
						allPopUpBackgroundsControllers[i].SetCameraDepth(currentCameraDepth);
						allPopUpBackgroundsControllers[i].Switch(true,justDisableOnPopUpDeactivation || !Application.isPlaying);	
						if(!allPopUpBackgroundsControllers[i].HaveComplementScreenWithId(controller.uiUniqueId))
						{
							allPopUpBackgroundsControllers[i].complementScreenIds.Add(controller.uiUniqueId);
						}
					}
					else
					{
						if(allPopUpBackgroundsControllers[i].HaveComplementScreenWithId(controller.uiUniqueId))
						{
							allPopUpBackgroundsControllers[i].complementScreenIds.Remove(controller.uiUniqueId);
						}
						if(allPopUpBackgroundsControllers[i].complementScreenIds.Count == 0)
						{
							allPopUpBackgroundsControllers[i].Switch(false,justDisableOnPopUpDeactivation || !Application.isPlaying);	
							UpdateCameraDepth();
						}
					}
				}				
			}
		}
	}

	/// <summary>
	/// Updates the pop up by identifier.
	/// </summary>
	/// <param name="popUpId">Pop up identifier.</param>
	public void UpdatePopUpById(string popUpId)
	{
		UIScreenController controller;
		if(fastAccessPopUpControllers.TryGetValue(popUpId,out controller))
		{
			controller.UpdateScreen();
			//Updates all related backgrounds
			for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
			{
				if(controller.HaveComplementScreenWithId(allPopUpBackgroundsControllers[i].uiUniqueId))
				{
					allPopUpBackgroundsControllers[i].UpdateScreen();
				}
			}
		}
	}

	/// <summary>
	/// Switchs all pop ups.
	/// </summary>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public void SwitchAllPopUps(bool enable)
	{
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			if(allPopUpControllers[i].Switch(enable, justDisableOnPopUpDeactivation || !Application.isPlaying))
			{
				if(OnPopUpStatusChanged != null)
				{
					OnPopUpStatusChanged(allPopUpControllers[i].uiUniqueId,enable);
				}
			}
		}

		bool isQueuedBackground = false;
		UIScreenController queuedPopUp = null;
		if(popUpQueue.Count > 0)
		{
			fastAccessPopUpControllers.TryGetValue(popUpQueue[0],out queuedPopUp);
			popUpQueue.RemoveAt(0);
		}

		for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(queuedPopUp != null)
			{
				isQueuedBackground = queuedPopUp.HaveComplementScreenWithId(allPopUpBackgroundsControllers[i].uiUniqueId);
			}
			if(allPopUpBackgroundsControllers[i].Switch(enable || (!enable && isQueuedBackground), justDisableOnPopUpDeactivation || !Application.isPlaying))
			{
				if(OnPopUpStatusChanged != null)
				{
					OnPopUpStatusChanged(allPopUpBackgroundsControllers[i].uiUniqueId,enable);
				}
			}
		}
		if(queuedPopUp != null && !enable)
		{
			ShowPopUpWithId(queuedPopUp.uiUniqueId);
		}
		CheckIfAnyPopUpActiveChanged();
	}

	/// <summary>
	/// Updates all active pop ups.
	/// </summary>
	public void UpdateAllActivePopUps()
	{
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			if(allPopUpControllers[i].isActive)
			{
				UpdatePopUpById(allPopUpControllers[i].uiUniqueId);
			}
		}
	}

	/// <summary>
	/// Gets the UIScreen on the pop up with identifier.
	/// </summary>
	/// <returns>The user interface screen pop up for identifier.</returns>
	/// <param name="popUpId">Pop up identifier.</param>
	public UIScreen GetUIScreenPopUpForId(string popUpId)
	{
		UIScreenController controller;
		if(fastAccessPopUpControllers.TryGetValue(popUpId,out controller))
		{
			return controller.GetCurrentUIScreen();
		}
		else
		{
			Debug.LogWarningFormat(this,"UIControllerNotFound by Id[{0}]",popUpId);
			return null;
		}
	}

	/// <summary>
	/// Gets the UIScreen on the pop up with identifier.
	/// </summary>
	/// <returns>The user interface screen pop up for identifier.</returns>
	/// <param name="popUpId">Pop up identifier.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T GetUIScreenPopUpForId<T>(string popUpId) where T : UIScreen
	{
		UIScreenController controller;
		if(fastAccessPopUpControllers.TryGetValue(popUpId,out controller))
		{
			return controller.GetCurrentUIScreen<T>();
		}
		else
		{
			Debug.LogWarningFormat(this,"UIControllerNotFound by Id[{0}]",popUpId);
			
			return null;
		}
	}

	/// <summary>
	/// Gets the pop up position.
	/// </summary>
	/// <returns>The pop up position.</returns>
	/// <param name="popUpId">Pop up identifier.</param>
	public Vector3 GetPopUpPosition(string popUpId)
	{
		bool founded = false;
		Vector3 screenWorldPosition = Vector3.zero;
		for(int i = 0; i < allPopUpControllers.Count; i++)
		{
			if(allPopUpControllers[i].uiUniqueId == popUpId)
			{
				screenWorldPosition = allPopUpControllers[i].uiScreenPosition;
				founded = true;
				break;
			}
		}
		if(!founded)
		{
			screenWorldPosition = CalculateNewPopUpPositionInWorld(allPopUpControllers.Count);
		}
		return screenWorldPosition;
	}

	/// <summary>
	/// Gets the pop up bkg position.
	/// </summary>
	/// <returns>The pop up bkg position.</returns>
	/// <param name="popUpBkgId">Pop up bkg identifier.</param>
	public Vector3 GetPopUpBkgPosition(string popUpBkgId)
	{
		bool founded = false;
		Vector3 screenWorldPosition = Vector3.zero;
		for(int i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(allPopUpBackgroundsControllers[i].uiUniqueId == popUpBkgId)
			{
				screenWorldPosition = allPopUpBackgroundsControllers[i].uiScreenPosition;
				founded = true;
				break;
			}
		}
		if(!founded)
		{
			screenWorldPosition = CalculateNewPopUpBkgPositionInWorld(allPopUpControllers.Count);
		}
		return screenWorldPosition;
	}

	/// <summary>
	/// Resets all PopUps positions.
	/// </summary>
	public void ResetAllPositions()
	{
		for(int i = 0; i < allPopUpControllers.Count; i++)
		{
			allPopUpControllers[i].ResetPosition(CalculateNewPopUpPositionInWorld(i));
		}
		for(int i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			allPopUpBackgroundsControllers[i].ResetPosition(CalculateNewPopUpBkgPositionInWorld(i));
		}
	}

	/// <summary>
	/// Registers to change event for pop up with identifier.
	/// </summary>
	/// <param name="popUpId">Pop up identifier.</param>
	/// <param name="listener">Listener.</param>
	public void RegisterToChangeEventForPopUpWithId(string popUpId,UIScreenController.ScreenChangedEventHandler listener)
	{
		UIScreenController controller;
		if(fastAccessPopUpControllers.TryGetValue(popUpId,out controller))
		{
			controller.OnScreenStatusChanged += listener;
		}
	}

	/// <summary>
	/// Unregisters to change event for pop up with identifier.
	/// </summary>
	/// <param name="popUpId">Pop up identifier.</param>
	/// <param name="listener">Listener.</param>
	public void UnregisterToChangeEventForPopUpWithId(string popUpId,UIScreenController.ScreenChangedEventHandler listener)
	{
		UIScreenController controller;
		if(fastAccessPopUpControllers.TryGetValue(popUpId,out controller))
		{
			controller.OnScreenStatusChanged -= listener;
		}
	}

	/// <summary>
	/// Registers to update event for pop up with identifier.
	/// </summary>
	/// <param name="popUpId">Pop up identifier.</param>
	/// <param name="listener">Listener.</param>
	public void RegisterToUpdateEventForPopUpWithId(string popUpId,UIScreenController.ScreenUpdatedEventHandler listener)
	{
		UIScreenController controller;
		if(fastAccessPopUpControllers.TryGetValue(popUpId,out controller))
		{
			controller.OnScreenUpdated += listener;
		}
	}

	/// <summary>
	/// Unregisters to update event for pop up with identifier.
	/// </summary>
	/// <param name="popUpId">Pop up identifier.</param>
	/// <param name="listener">Listener.</param>
	public void UnregisterToUpdateEventForPopUpWithId(string popUpId,UIScreenController.ScreenUpdatedEventHandler listener)
	{
		UIScreenController controller;
		if(fastAccessPopUpControllers.TryGetValue(popUpId,out controller))
		{
			controller.OnScreenUpdated -= listener;
		}
	}

	/// <summary>
	/// Determines whether the matched pop up is active.
	/// </summary>
	/// <returns><c>true</c> if this instance matching pop up is active; otherwise, <c>false</c>.</returns>
	/// <param name="popUpId">Pop up identifier.</param>
	public bool IsPopUpActive(string popUpId)
	{
		UIScreenController controller;
		if(fastAccessPopUpControllers.TryGetValue(popUpId,out controller))
		{
			return controller.isActive;
		}
		return false;
	}

	/// <summary>
	/// Switchs the pop up by identifier.
	/// </summary>
	/// <param name="uniqueId">Unique identifier.</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public void SwitchById(string popUpId,bool enable,bool removeFromQueueIfFirst = false)
	{
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			if(allPopUpControllers[i].uiUniqueId == popUpId)
			{
				allPopUpControllers[i].Switch(enable, justDisableOnPopUpDeactivation || !Application.isPlaying);
			}
		}
		for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(allPopUpBackgroundsControllers[i].uiUniqueId == popUpId)
			{
				allPopUpBackgroundsControllers[i].Switch(enable, justDisableOnPopUpDeactivation || !Application.isPlaying);
			}
		}
		if(popUpQueue.Count > 0 && removeFromQueueIfFirst)
		{
			//if it was the next pop up in the queue, remove it
			if(popUpQueue[0] == popUpId)
			{
				popUpQueue.RemoveAt(0);
			}
		}
		CheckIfAnyPopUpActiveChanged();
	}

	/// <summary>
	/// Switchs on the pop up with the passed id and switchs off the others.
	/// </summary>
	/// <param name="uniqueId">Unique identifier.</param>
	public void SwitchSolo(string uniqueId)
	{
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			allPopUpControllers[i].Switch(allPopUpControllers[i].uiUniqueId == uniqueId, justDisableOnPopUpDeactivation || !Application.isPlaying);
		}
		for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			allPopUpBackgroundsControllers[i].Switch(allPopUpBackgroundsControllers[i].uiUniqueId == uniqueId, justDisableOnPopUpDeactivation || !Application.isPlaying);
		}
	}

	#region EDITOR HELPING FUNCTIONS
	//EDITOR HELPING FUNCTIONS
	/// <summary>
	/// Calculates the new pop up position in world.
	/// </summary>
	/// <returns>The new pop up position in world.</returns>
	/// <param name="screenIndex">Screen index.</param>
	Vector3 CalculateNewPopUpPositionInWorld(int screenIndex)
	{
		Vector3 finalPosition = Vector3.zero;
		int xCoord = screenIndex%popupsPositionNumberOfColumns;
		int yCoord = screenIndex/popupsPositionNumberOfColumns;
		finalPosition = new Vector3(-1*screenSeparation.x*(xCoord+1), screenSeparation.y*yCoord, 0);
		return finalPosition;
	}

	/// <summary>
	/// Calculates the new pop up bkg position in world.
	/// </summary>
	/// <returns>The new pop up bkg position in world.</returns>
	/// <param name="screenIndex">Screen index.</param>
	Vector3 CalculateNewPopUpBkgPositionInWorld(int screenIndex)
	{
		Vector3 finalPosition = Vector3.zero;
		int xCoord = screenIndex%popupsPositionNumberOfColumns;
		int yCoord = screenIndex/popupsPositionNumberOfColumns;
		finalPosition = new Vector3(-1*screenSeparation.x*(xCoord+1), -1*screenSeparation.y*yCoord, 0);
		return finalPosition;
	}

	/// <summary>
	/// Gets an unique identifier.
	/// </summary>
	/// <returns>The unique identifier.</returns>
	/// <param name="proposedIdComplement">Proposed identifier complement.</param>
	/// <param name="isPopUp">If set to <c>true</c> is pop up.</param>
	private string GetUniqueId(int proposedIdComplement,bool isPopUp = true)
	{
		string id = (isPopUp ? "UIPOP_":"UIPOPBG_")+proposedIdComplement;
		for(int i = 0; i < allPopUpControllers.Count; i++)
		{
			if(id == allPopUpControllers[i].uiUniqueId)
			{
				int newProposal = proposedIdComplement+1;
				id = GetUniqueId(newProposal,isPopUp);
			}
		}
		for(int i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(id == allPopUpBackgroundsControllers[i].uiUniqueId)
			{
				int newProposal = proposedIdComplement+1;
				id = GetUniqueId(newProposal,isPopUp);
			}
		}
		return id;
	}

	/// <summary>
	/// Creates a new UIScreenManager along with its controller for a pop up.
	/// </summary>
	/// <returns>The new user interface pop up screen manager.</returns>
	/// <param name="newPopUpId">New pop up identifier.</param>
	public GameObject CreateNewUIPopUpScreenManager(string newPopUpId = "")
	{
		int systemLayerValue = UIManager.GetUISystemLayer();
		if(systemLayerValue == 0)
		{
			systemLayerValue = systemLayer.value;
		}
		Debug.LogFormat(this,"Creating new UIPopUp with Layer[{0}][{1}]", LayerMask.LayerToName(systemLayerValue), systemLayerValue);
		
		GameObject go = new GameObject((newPopUpId == "" ? GetUniqueId(allPopUpControllers.Count) : newPopUpId));
		go.layer = systemLayerValue;
		Camera cam = go.AddComponent<Camera>();
		if(cam != null)
		{
			cam.orthographic = true;
			cam.orthographicSize = 5;
			cam.allowHDR = false;
			cam.useOcclusionCulling = true;
			cam.clearFlags = CameraClearFlags.Depth;
			cam.cullingMask = 1 << systemLayerValue;
			cam.farClipPlane = 200.0f;
			//add child Canvas
			GameObject canvasGO = new GameObject("Canvas");
			canvasGO.transform.SetParent(go.transform);
			canvasGO.layer = systemLayerValue;
			canvasGO.transform.localPosition = Vector3.zero;
			Canvas canvas = canvasGO.AddComponent<Canvas>();
			if(canvas != null)
			{
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = cam;
				canvas.planeDistance = 100;
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
					screenManager.isPopUp = true;
					Vector3 newPosition = CalculateNewPopUpPositionInWorld(allPopUpControllers.Count);
					//create controller for this screenManager
					UIScreenController controller = new UIScreenController(this,go.name,null,screenManager,newPosition);
					allPopUpControllers.Add(controller);
				}

				if(addHelpFrameToCreatedPopUps && popUpHelpFramePrefab != null)
				{
					GameObject helpFrame = GameObject.Instantiate(popUpHelpFramePrefab);
                    helpFrame.name = popUpHelpFramePrefab.name;
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
	/// Creates a new UIScreenManager along with its controller for a pop up background.
	/// </summary>
	/// <returns>The new user interface pop up BKG screen manager.</returns>
	/// <param name="newPopUpBkgId">New pop up bkg identifier.</param>
	public GameObject CreateNewUIPopUpBKGScreenManager(string newPopUpBkgId = "")
	{
		int systemLayerValue = UIManager.GetUISystemLayer();
		if(systemLayerValue == 0)
		{
			systemLayerValue = systemLayer.value;
		}
		Debug.Log("Creating new UIPopUpBG",this);
		
		GameObject go = new GameObject((newPopUpBkgId == "" ? GetUniqueId(allPopUpBackgroundsControllers.Count,false) : newPopUpBkgId));
		go.layer = systemLayerValue;
		Camera cam = go.AddComponent<Camera>();
		if(cam != null)
		{
			cam.orthographic = true;
			cam.orthographicSize = 5;
			cam.allowHDR = false;
			cam.useOcclusionCulling = true;
			cam.clearFlags = CameraClearFlags.Depth;
			cam.cullingMask = 1 << systemLayerValue;
			cam.farClipPlane = 200.0f;
			//add child Canvas
			GameObject canvasGO = new GameObject("Canvas");
			canvasGO.transform.SetParent(go.transform);
			canvasGO.layer = systemLayerValue;
			canvasGO.transform.localPosition = Vector3.zero;
			Canvas canvas = canvasGO.AddComponent<Canvas>();
			if(canvas != null)
			{
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = cam;
				canvas.planeDistance = 100;
				CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
				if(scaler != null)
				{
					scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
					scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
					scaler.referenceResolution = canvasScalerReferenceResolution;
					scaler.matchWidthOrHeight = 0.5f;
					scaler.referencePixelsPerUnit = 100;
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
					screenManager.isPopUp = true;
					Vector3 newPosition = CalculateNewPopUpBkgPositionInWorld(allPopUpControllers.Count);
					//create controller for this screenManager
					UIScreenController controller = new UIScreenController(this,go.name,null,screenManager,newPosition);
					allPopUpBackgroundsControllers.Add(controller);
				}

				if(addHelpFrameToCreatedPopUps && popUpBkgHelpFramePrefab != null)
				{
					GameObject helpFrame = GameObject.Instantiate(popUpBkgHelpFramePrefab);
                    helpFrame.name = popUpBkgHelpFramePrefab.name;
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
	/// Gets an unique pop up identifier from a suggestion.
	/// </summary>
	/// <returns>The unique pop up identifier suggestion.</returns>
	/// <param name="currentId">Current identifier.</param>
	private string GetUniquePopUpIdFrom(string currentId)
	{
		for(int i = 0; i < allPopUpControllers.Count; i++)
		{
			if(currentId == allPopUpControllers[i].uiUniqueId)
			{
				currentId = GetUniqueId(allPopUpControllers.Count);
			}
		}
		return currentId;
	}

	/// <summary>
	/// Gets an unique pop up bkg identifier from a suggestion.
	/// </summary>
	/// <returns>The unique pop up bkg identifier suggestion.</returns>
	/// <param name="currentId">Current identifier.</param>
	private string GetUniquePopUpBkgIdFrom(string currentId)
	{
		for(int i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(currentId == allPopUpBackgroundsControllers[i].uiUniqueId)
			{
				currentId = GetUniqueId(allPopUpBackgroundsControllers.Count,false);
			}
		}
		return currentId;
	}

	/// <summary>
	/// Creates a new controller for a UIScreenManager pop up in scene.
	/// </summary>
	/// <returns>The new user interface pop up screen controller from scene object.</returns>
	/// <param name="screenManager">Screen manager.</param>
	public UIScreenManager CreateNewUIPopUpScreenControllerFromSceneObject(UIScreenManager screenManager)
	{
		bool alreadyExist = false;	
		for(int i = 0; i < allPopUpControllers.Count; i++)
		{
			if(allPopUpControllers[i].uiUniqueId == screenManager.uniqueScreenId)
			{
				alreadyExist = true;
				break;
			}
		}
		Debug.LogFormat(this,"Creating new UIPopUpScreenController. AlreadyExist?[{0}]", alreadyExist);
		
		if(!alreadyExist)
		{
			string id = GetUniquePopUpIdFrom(screenManager.uniqueScreenId);
			Vector3 newPosition = CalculateNewPopUpPositionInWorld(allPopUpControllers.Count);
			//create controller for this screenManager
			UIScreenController controller = new UIScreenController(this,id,null,screenManager,newPosition);
			allPopUpControllers.Add(controller);
			screenManager.isPopUp = true;
			return screenManager;
		}
		else
		{
			Debug.LogWarningFormat(this,"A Controller with Id[{0}] already exist! It cannot be created in this manager.", screenManager.uniqueScreenId);
			
			return null;
		}
	}

	/// Creates a new controller for a UIScreenManager pop up background in scene.
	public UIScreenManager CreateNewUIPopUpScreenBkgControllerFromSceneObject(UIScreenManager screenManager)
	{
		bool alreadyExist = false;	
		for(int i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(allPopUpBackgroundsControllers[i].uiUniqueId == screenManager.uniqueScreenId)
			{
				alreadyExist = true;
				break;
			}
		}
		Debug.LogFormat(this,"Creating new UIPopUpBkgScreenController. AlreadyExist?[{0}]", alreadyExist);
		
		if(!alreadyExist)
		{
			string id = GetUniquePopUpBkgIdFrom(screenManager.uniqueScreenId);
			Vector3 newPosition = CalculateNewPopUpBkgPositionInWorld(allPopUpBackgroundsControllers.Count);
			//create controller for this screenManager
			UIScreenController controller = new UIScreenController(this,id,null,screenManager,newPosition);
			allPopUpBackgroundsControllers.Add(controller);
			screenManager.isPopUp = true;
			return screenManager;
		}
		else
		{
			Debug.LogWarningFormat(this,"A Controller with Id[{0}] already exist! It cannot be created in this manager.", screenManager.uniqueScreenId);
			
			return null;
		}
	}

	/// <summary>
	/// Gets the lowest sibling index of all the pop ups.
	/// </summary>
	/// <returns>The last pop up sibling index.</returns>
	public int GetLastPopUpSiblingIndex()
	{
		int lastSiblingIndex = transform.GetSiblingIndex();
		for(int i = 0; i < allPopUpControllers.Count; i++)
		{
			if(allPopUpControllers[i].uiScreenObject != null)
			{
				int newSiblingIndex = allPopUpControllers[i].uiScreenObject.CachedTransform.GetSiblingIndex();
				if(newSiblingIndex > lastSiblingIndex)
				{
					lastSiblingIndex = newSiblingIndex;
				}
			}
		}
		return lastSiblingIndex;
	}

	/// <summary>
	/// Gets the lowest sibling index of all the pop up backgrounds.
	/// </summary>
	/// <returns>The last pop up bkg sibling index.</returns>
	public int GetLastPopUpBkgSiblingIndex()
	{
		int lastSiblingIndex = transform.GetSiblingIndex();
		for(int i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			if(allPopUpBackgroundsControllers[i].uiScreenObject != null)
			{
				int newSiblingIndex = allPopUpBackgroundsControllers[i].uiScreenObject.CachedTransform.GetSiblingIndex();
				if(newSiblingIndex > lastSiblingIndex)
				{
					lastSiblingIndex = newSiblingIndex;
				}
			}
		}
		return lastSiblingIndex;
	}

	/// <summary>
	/// Removes the pop up passed from the PopUpsManager.
	/// </summary>
	/// <param name="popUpToRemove">Pop up to remove.</param>
	/// <param name="mustDestroyGameObject">If set to <c>true</c> must destroy game object.</param>
	public void RemoveUIPopUpScreenManager(UIScreenManager popUpToRemove,bool mustDestroyGameObject = true)
	{
		if(popUpToRemove != null)
		{
			int indexToRemove = -1;
			for(int  i = 0; i < allPopUpControllers.Count; i++)
			{
				bool hasSameGO = allPopUpControllers[i].uiScreenObject == popUpToRemove;
				bool hasSameId = allPopUpControllers[i].uiUniqueId == popUpToRemove.uniqueScreenId;
				if(hasSameGO || hasSameId)
				{
					Debug.LogFormat(this,"Removing UIPopUpScreenManager[{0}] with Id[{1}] by Object?[{2}] by Id?[{3}]",
                        popUpToRemove.name, popUpToRemove.uniqueScreenId, hasSameGO, hasSameId);
					
					indexToRemove = i;
					break;
				}
			}
			if(indexToRemove >= 0)
			{
				allPopUpControllers.RemoveAt(indexToRemove);
				if(mustDestroyGameObject)
				{
					DestroyImmediate(popUpToRemove.gameObject);
				}
				ResetAllPositions();
			}
		}
	}

	/// <summary>
	/// Removes the pop up background passed from the PopUpsManager.
	/// </summary>
	/// <param name="popUpBkgToRemove">Pop up bkg to remove.</param>
	/// <param name="mustDestroyGameObject">If set to <c>true</c> must destroy game object.</param>
	public void RemoveUIPopUpBkgScreenManager(UIScreenManager popUpBkgToRemove,bool mustDestroyGameObject = true)
	{
		if(popUpBkgToRemove != null)
		{
			int indexToRemove = -1;
			for(int  i = 0; i < allPopUpControllers.Count; i++)
			{
				bool hasSameGO = allPopUpControllers[i].uiScreenObject == popUpBkgToRemove;
				bool hasSameId = allPopUpControllers[i].uiUniqueId == popUpBkgToRemove.uniqueScreenId;
				if(hasSameGO || hasSameId)
				{
					Debug.LogFormat(this,"Removing UIPopUpBkgScreenManager[{0}] with Id[{1}] by Object?[{2}] by Id?[{3}]",
                        popUpBkgToRemove.name, popUpBkgToRemove.uniqueScreenId, hasSameGO, hasSameId);
					
					indexToRemove = i;
					break;
				}
			}
			if(indexToRemove >= 0)
			{
				allPopUpControllers.RemoveAt(indexToRemove);
				if(mustDestroyGameObject)
				{
					DestroyImmediate(popUpBkgToRemove.gameObject);
				}
				ResetAllPositions();
			}
		}
	}

	/// <summary>
	/// Removes the user interface pop up screen manager controller.
	/// </summary>
	/// <param name="uniqueId">Unique identifier.</param>
	/// <param name="justSetAsNull">If set to <c>true</c> just set as null and do not remove.</param>
	public void RemoveUIPopUpScreenManagerController(string uniqueId, bool justSetAsNull)
	{
		int indexToRemove = -1;
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			bool hasSameId = allPopUpControllers[i].uiUniqueId == uniqueId;
			if(hasSameId)
			{
				Debug.LogFormat(this,"Removing UIPopUpScreenManagerController with Id[{0}]", uniqueId);
				
				indexToRemove = i;
				break;
			}
		}
		if(indexToRemove >= 0)
		{
			if(justSetAsNull)
			{
				allPopUpControllers[indexToRemove].uiScreenObject = null;
			}
			else
			{
				allPopUpControllers.RemoveAt(indexToRemove);
				ResetAllPositions();
			}
		}
	}

	/// <summary>
	/// Removes the user interface pop up bkg screen manager controller.
	/// </summary>
	/// <param name="uniqueId">Unique identifier.</param>
	/// <param name="justSetAsNull">If set to <c>true</c> just set as null and do not remove.</param>
	public void RemoveUIPopUpBkgScreenManagerController(string uniqueId, bool justSetAsNull)
	{
		int indexToRemove = -1;
		if(!justSetAsNull)
		{
			for(int  i = 0; i < allPopUpControllers.Count; i++)
			{
				allPopUpControllers[i].RemoveComplementId(uniqueId);
			}
		}
		for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			bool hasSameId = allPopUpBackgroundsControllers[i].uiUniqueId == uniqueId;
			if(hasSameId)
			{
				Debug.LogFormat(this,"Removing UIPopUpBkgScreenManagerController with Id[{0}]", uniqueId);
				
				indexToRemove = i;
				break;
			}
		}
		if(indexToRemove >= 0)
		{
			if(justSetAsNull)
			{
				allPopUpBackgroundsControllers[indexToRemove].uiScreenObject = null;
			}
			else
			{
				allPopUpBackgroundsControllers.RemoveAt(indexToRemove);
				ResetAllPositions();
			}
		}
	}

	/// <summary>
	/// Creates the new empty pop up controller alone.
	/// </summary>
	public void CreateNewEmptyPopUpControllerAlone()
	{
		//create controller for this screenManager
		Vector3 newPosition = CalculateNewPopUpPositionInWorld(allPopUpControllers.Count);
		UIScreenController controller = new UIScreenController(this,GetUniqueId(allPopUpControllers.Count,true),null,null,newPosition);
		allPopUpControllers.Add(controller);
	}

	/// <summary>
	/// Creates the new empty pop up background controller alone.
	/// </summary>
	public void CreateNewEmptyPopUpBackgroundControllerAlone()
	{
		//create controller for this screenManager
		Vector3 newPosition = CalculateNewPopUpBkgPositionInWorld(allPopUpBackgroundsControllers.Count);
		UIScreenController controller = new UIScreenController(this,GetUniqueId(allPopUpBackgroundsControllers.Count,false),null,null,newPosition);
		allPopUpBackgroundsControllers.Add(controller);
	}


	/// <summary>
	/// Reset this instance.
	/// </summary>
	public void Reset()
	{
		if(allPopUpControllers != null)
		{
			while(allPopUpControllers.Count > 0)
			{
				if(allPopUpControllers[0].uiScreenObject != null)
				{
					if(Application.isPlaying)
					{
						Destroy(allPopUpControllers[0].uiScreenObject.gameObject);
					}
					else
					{
						DestroyImmediate(allPopUpControllers[0].uiScreenObject.gameObject);
					}
				}
				allPopUpControllers.RemoveAt(0);
			}
			allPopUpControllers.Clear();
		}
		if(allPopUpBackgroundsControllers != null)
		{
			while(allPopUpBackgroundsControllers.Count > 0)
			{
				if(allPopUpBackgroundsControllers[0].uiScreenObject != null)
				{
					if(Application.isPlaying)
					{
						Destroy(allPopUpBackgroundsControllers[0].uiScreenObject.gameObject);
					}
					else
					{
						DestroyImmediate(allPopUpBackgroundsControllers[0].uiScreenObject.gameObject);
					}
				}
				allPopUpBackgroundsControllers.RemoveAt(0);
			}
			allPopUpBackgroundsControllers.Clear();
			ResetAllPositions();
		}
	}

	/// <summary>
	/// Despawns all pop ups on scene.
	/// </summary>
	public void DespawnAllPopUpsOnScene()
	{
		for(int  i = 0; i < allPopUpControllers.Count; i++)
		{
			allPopUpControllers[i].Switch(false);
		}
	}
	/// <summary>
	/// Despawns all pop up backgrounds on scene.
	/// </summary>
	public void DespawnAllPopUpBkgsOnScene()
	{
		for(int  i = 0; i < allPopUpBackgroundsControllers.Count; i++)
		{
			allPopUpBackgroundsControllers[i].Switch(false);
		}
	}


	#endregion


}
