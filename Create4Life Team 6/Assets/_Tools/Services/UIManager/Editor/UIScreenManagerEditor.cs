using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(UIScreenManager))]
[CanEditMultipleObjects]
public class UIScreenManagerEditor : Editor 
{
	static UIScreenManager lastUIScreenManager;
	static bool lastUIScreenManagerWasDespawned = false;
	UIScreenManager uiScreenManager;
	SerializedObject serializedObj;
	MonoScript script;
	bool _isPopUp = false;

	protected void OnEnable()
	{
		uiScreenManager = (UIScreenManager)target;
		serializedObj = serializedObject;
		script = MonoScript.FromMonoBehaviour((UIScreenManager)target);
	}

	protected void OnDisable()
	{
		lastUIScreenManager = uiScreenManager;
	}

	protected void OnDestroy()
	{
		if (target == null && !Application.isPlaying)
		{
			if(_isPopUp)
			{
				//send remove instruction if possible
				PopUpsManager popUpManager = FindObjectOfType<PopUpsManager>();
				if(popUpManager != null)
				{
					popUpManager.RemoveUIPopUpBkgScreenManagerController(lastUIScreenManager.uniqueScreenId,lastUIScreenManagerWasDespawned);
					popUpManager.RemoveUIPopUpScreenManagerController(lastUIScreenManager.uniqueScreenId,lastUIScreenManagerWasDespawned);
				}
			}
			else
			{
				//send remove instruction if possible
				UIManager uiManager = FindObjectOfType<UIManager>();
				if(uiManager != null)
				{
					uiManager.RemoveUIScreenManagerController(lastUIScreenManager.uniqueScreenId,lastUIScreenManagerWasDespawned);
				}
			}

			lastUIScreenManagerWasDespawned = false;
			lastUIScreenManager = null;
		}
	}

	public override void OnInspectorGUI () 
	{
		bool wasDestroyed = false;
		if(uiScreenManager != null && serializedObj != null)
		{
			GUI.enabled = false;
			script = EditorGUILayout.ObjectField("Script:", script, typeof(MonoScript), false) as MonoScript;
			GUI.enabled = true;
			// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
			serializedObj.Update();
			_isPopUp = serializedObj.FindProperty("isPopUp").boolValue;
			if(Selection.objects.Length == 1)
			{
				PrefabType pType = PrefabUtility.GetPrefabType(uiScreenManager.gameObject);
				bool isPrefab = pType == PrefabType.Prefab;
				if(!isPrefab)
				{
					if(GUILayout.Button("Update Id In Manager"))
					{
						if(_isPopUp)
						{
							PopUpsManager popUpsManager = FindObjectOfType<PopUpsManager>();
							if(popUpsManager != null)
							{
								UpdateIdOnPopUpsManager(popUpsManager);
							}
						}
						else
						{
							UIManager uiManager = FindObjectOfType<UIManager>();
							if(uiManager != null)
							{
								UpdateIdOnUIManager(uiManager);
							}
						}
					}
					if(GUILayout.Button("Switch "+(uiScreenManager.gameObject.activeSelf ? "OFF" : "ON" )))
					{
						if(_isPopUp)
						{
							PopUpsManager popUpManager = FindObjectOfType<PopUpsManager>();
							if(popUpManager != null)
							{
								popUpManager.SwitchById(uiScreenManager.uniqueScreenId,!uiScreenManager.gameObject.activeSelf);
							}
						}
						else
						{
							UIManager uiManager = FindObjectOfType<UIManager>();
							if(uiManager != null)
							{
								uiManager.SwitchScreenById(uiScreenManager.uniqueScreenId,!uiScreenManager.gameObject.activeSelf);
							}
						}
					}
					if(GUILayout.Button("Enable as SOLO"))
					{
						if(_isPopUp)
						{
							PopUpsManager popUpManager = FindObjectOfType<PopUpsManager>();
							if(popUpManager != null)
							{
								popUpManager.SwitchSolo(uiScreenManager.uniqueScreenId);
							}
						}
						else
						{
							UIManager uiManager = FindObjectOfType<UIManager>();
							if(uiManager != null)
							{
								uiManager.SwitchToScreenWithId(uiScreenManager.uniqueScreenId);
								ArrayList sceneViews = UnityEditor.SceneView.sceneViews;
								if(sceneViews != null)
								{
									if(sceneViews.Count > 0)
									{
										UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[0];
										sceneView.AlignViewToObject(uiScreenManager.transform);
									}
								}
							}
						}

					}
					if(GUILayout.Button("Despawn"))
					{
						wasDestroyed = true;
						lastUIScreenManagerWasDespawned = true;
						DestroyImmediate(uiScreenManager.gameObject);
					}
					if(GUILayout.Button("Destroy And Remove"))
					{
						wasDestroyed = true;
						DestroyImmediate(uiScreenManager.gameObject);
					}
				}
			}
		}
		if(target != null && !wasDestroyed)
		{
			//DrawDefaultInspector();
			EditorGUILayout.PropertyField(serializedObj.FindProperty("mustShowDebugInfo"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("mustRegisterForBackOperations"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("mustActiveRecursively"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("mustSurviveSceneChange"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("uniqueScreenId"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("isPopUp"));
			EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(serializedObj.FindProperty("screenCameraDepth"));
			if(EditorGUI.EndChangeCheck())
			{
				//try to update camera depth
				Camera camera = uiScreenManager.gameObject.GetComponent<Camera>();
				if(camera != null)
				{
					float depth = serializedObj.FindProperty("screenCameraDepth").floatValue;
					camera.depth = depth;
				}
			}
		}
		if(serializedObject != null && !wasDestroyed)
		{
			// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
			serializedObject.ApplyModifiedProperties();
		}
	}

	void UpdateIdOnUIManager(UIManager uiManager)
	{
		for(int i = 0; i < uiManager.allScreenControllers.Count; i++)
		{
			if(uiManager.allScreenControllers[i].uiScreenObject == uiScreenManager)
			{
				string oldId = uiManager.allScreenControllers[i].uiUniqueId;
				uiManager.allScreenControllers[i].uiUniqueId = uiScreenManager.uniqueScreenId;
				for(int j = 0; j < uiManager.allScreenControllers.Count; j++)
				{
					if(j != i)
					{
						uiManager.allScreenControllers[j].UpdateComplementIds(oldId,uiManager.allScreenControllers[i].uiUniqueId);
					}
				}
			}
			if(uiManager.allScreenControllers[i].uiScreenPrefab != null)
			{
				uiManager.allScreenControllers[i].uiScreenPrefab.uniqueScreenId = uiManager.allScreenControllers[i].uiUniqueId;
			}
			AssetDatabase.SaveAssets();
		}
	}

	void UpdateIdOnPopUpsManager(PopUpsManager popUpsManager)
	{
		for(int i = 0; i < popUpsManager.allPopUpControllers.Count; i++)
		{
			if(popUpsManager.allPopUpControllers[i].uiScreenObject == uiScreenManager)
			{
				popUpsManager.allPopUpControllers[i].uiUniqueId = uiScreenManager.uniqueScreenId;
			}
			if(popUpsManager.allPopUpControllers[i].uiScreenPrefab != null)
			{
				popUpsManager.allPopUpControllers[i].uiScreenPrefab.uniqueScreenId = popUpsManager.allPopUpControllers[i].uiUniqueId;
			}
		}

		for(int i = 0; i < popUpsManager.allPopUpBackgroundsControllers.Count; i++)
		{
			bool popUpsUpdated = false;
			string oldId = popUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId;
			if(popUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject == uiScreenManager)
			{
				popUpsUpdated = true;
				popUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId = uiScreenManager.uniqueScreenId;
				//update on related popups
				for(int j = 0; j < popUpsManager.allPopUpControllers.Count; j++)
				{
					popUpsManager.allPopUpControllers[j].UpdateComplementIds(oldId,popUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId);
				}
			}
			if(popUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab != null)
			{
				if(!popUpsUpdated)
				{
					popUpsUpdated = true;
					//update on related popups
					for(int j = 0; j < popUpsManager.allPopUpControllers.Count; j++)
					{
						popUpsManager.allPopUpControllers[j].UpdateComplementIds(oldId,popUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId);
					}
				}
				popUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab.uniqueScreenId = popUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId;
			}
		}
		AssetDatabase.SaveAssets();
	}

}
