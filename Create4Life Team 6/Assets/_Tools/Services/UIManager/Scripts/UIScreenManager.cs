using UnityEngine;
using System.Collections;

/// <summary>
/// User interface screen manager.
/// </summary>
[RequireComponent(typeof(Camera))]
public class UIScreenManager : CachedMonoBehaviour 
{
	/// <summary>
	/// The must show debug info flag.
	/// </summary>
	public bool mustShowDebugInfo = true;
	/// <summary>
	/// True if must change last id when changing to this screen, false otherwise.
	/// </summary>
	public bool mustRegisterForBackOperations = true;
	/// <summary>
	/// The must active recursively flag.
	/// </summary>
	public bool mustActiveRecursively = true;
	/// <summary>
	/// The must survive scene change flag.(DontDestroyOnSceneChange)
	/// </summary>
	public bool mustSurviveSceneChange = false;
	private bool _alreadyMarkedAsPerpetual = false;
	/// <summary>
	/// The unique screen identifier.
	/// </summary>
	public string 		uniqueScreenId;
	/// <summary>
	/// The screen camera depth.
	/// </summary>
	public float screenCameraDepth = 0.0f;
	/// <summary>
	/// The is pop up flag.
	/// </summary>
	public bool isPopUp = false;
	/// <summary>
	/// The current user interface screen.
	/// </summary>
	private UIScreen	_currentUIScreen;
	/// <summary>
	/// The camera used by this screen manager.
	/// </summary>
	private Camera		_camera;

	/// <summary>
	/// Init the screen and calls the specified screenChangeCallback.
	/// </summary>
	/// <param name="screenChangeCallback">Screen change callback.</param>
	public void Init(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
	{
		if(Application.isPlaying)
		{
			if(mustShowDebugInfo)
			{
				Debug.Log("Init UIScreenManager["+CachedGameObject.name+"] with Id["+uniqueScreenId+"]");
			}

			if(mustSurviveSceneChange && !_alreadyMarkedAsPerpetual)
			{
				UnityEngine.GameObject.DontDestroyOnLoad(CachedGameObject);
				_alreadyMarkedAsPerpetual = true;
			}

			//look for ui screen component
			if(_currentUIScreen == null)
			{
				_currentUIScreen = GetComponent<UIScreen>();
			}
			if(_camera == null)
			{
				_camera = GetComponent<Camera>();
			}

			UpdateCameraDepth(screenCameraDepth);
			if(_currentUIScreen == null)
			{
				if(mustShowDebugInfo)
				{
					Debug.LogWarning("UIManager["+CachedGameObject.name+"] could not found UIScreen component, it will not work properly");
				}
				if(mustActiveRecursively)
				{
					CachedGameObject.ForceActivateRecursively(true);
				}
			}
			else
			{
				if(mustActiveRecursively)
				{
					CachedGameObject.ForceActivateRecursively(true);
				}
				_currentUIScreen.Activate(screenChangeCallback);
			}
		}
		else
		{
			CachedGameObject.ForceActivateRecursively(true);
		}
	}

	/// <summary>
	/// Deactivate the screen and calls the specified screenChangeCallback.
	/// </summary>
	/// <param name="screenChangeCallback">Screen change callback.</param>
	public void Deactivate(UIScreenController.ScreenChangedEventHandler screenChangeCallback)
	{
		if(Application.isPlaying)
		{
			if(mustShowDebugInfo)
			{
				Debug.Log("Deactivating UIScreenManager["+CachedGameObject.name+"] with Id["+uniqueScreenId+"]");
			}
			if(_currentUIScreen != null)
			{
				_currentUIScreen.Deactivate(screenChangeCallback);
			}
			if(mustActiveRecursively)
			{
				CachedGameObject.ForceActivateRecursively(false);
			}
		}
		else
		{
			CachedGameObject.ForceActivateRecursively(false);
		}
	}

	/// <summary>
	/// Updates the screen.
	/// </summary>
	/// <param name="screenUpdatedCallback">Screen updated callback.</param>
	public void UpdateScreen(UIScreenController.ScreenUpdatedEventHandler screenUpdatedCallback)
	{
		if(_currentUIScreen != null)
		{
			_currentUIScreen.UpdateScreen(screenUpdatedCallback);
		}
	}

	/// <summary>
	/// Updates the camera depth.
	/// </summary>
	/// <param name="newDepth">New depth.</param>
	public void UpdateCameraDepth(float newDepth)
	{
		if(mustShowDebugInfo)
		{
			Debug.Log("Updating screen["+uniqueScreenId+"] depth to["+newDepth+"]");
		}
		if(newDepth >= -100 && newDepth <= 100)
		{
			screenCameraDepth = newDepth;
		}
		if(_camera != null)
		{
			_camera.depth = screenCameraDepth;
		}
	}

	/// <summary>
	/// Sets the position of this screen.
	/// </summary>
	/// <param name="screenPositionInWorld">Screen position in world.</param>
	public void SetPosition(Vector3 screenPositionInWorld)
	{
		if(Application.isPlaying)
		{
			if(CachedRectTransform != null)
			{
				CachedRectTransform.position = screenPositionInWorld;
			}
			else 
			{
				CachedTransform.position = screenPositionInWorld;
			}
		}
		else
		{
			transform.position = screenPositionInWorld;
		}
	}

	/// <summary>
	/// Gets the UIScreen instance.
	/// </summary>
	/// <returns>The user interface screen instance.</returns>
	public UIScreen GetUIScreenInstance()
	{
		if(_currentUIScreen != null)
		{
			return _currentUIScreen;	
		}
		else
		{
			return GetComponent<UIScreen>();
		}
	}

	/// <summary>
	/// Gets the UIScreen instance casted to the T type.
	/// </summary>
	/// <returns>The user interface screen instance.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T GetUIScreenInstance<T>() where T : UIScreen
	{
		if(_currentUIScreen != null)
		{
			return (T)_currentUIScreen;
		}
		else
		{
			return GetComponent<T>();
		}
	}

	/// <summary>
	/// Determines whether this instance GameObject screen is active in hierarchy.
	/// </summary>
	/// <returns><c>true</c> if this screen GO is active in hierarchy; otherwise, <c>false</c>.</returns>
	public bool IsScreenGOActiveInHierarchy()
	{
		return CachedGameObject.activeInHierarchy;
	}
}
