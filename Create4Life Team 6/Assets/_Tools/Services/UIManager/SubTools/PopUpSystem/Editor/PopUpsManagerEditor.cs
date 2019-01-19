using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PopUpsManager))]
public class PopUpsManagerEditor : Editor 
{
	PopUpsManager uiPopUpsManager;
	SerializedObject serializedObj;
	MonoScript script;

	protected void OnEnable()
	{
		uiPopUpsManager = (PopUpsManager)target;
		serializedObj = serializedObject;
		script = MonoScript.FromMonoBehaviour((PopUpsManager)target);
	}

	public override void OnInspectorGUI () 
	{
		GUI.enabled = false;
		script = EditorGUILayout.ObjectField("Script:", script, typeof(MonoScript), false) as MonoScript;
		GUI.enabled = true;
		if(uiPopUpsManager != null && serializedObj != null)
		{
			// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
			serializedObj.Update();
			if(!Application.isPlaying)
			{
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("Spawn All Pop Ups"))
				{
					SpawnAllPopUps();
				}
				if(GUILayout.Button("Despawn All Pop Ups"))
				{
					DespawnAllPopUps();
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("Spawn All Backgrounds"))
				{
					SpawnAllPopUpBackgrounds();
				}
				if(GUILayout.Button("Despawn All Backgrounds"))
				{
					DespawnAllBackgrounds();
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("Create new Pop Up"))
				{
					CreateNewScreenPopUp();
				}
				if(GUILayout.Button("Create new Background"))
				{
					CreateNewScreenPopUpBackground();
				}
				GUILayout.EndHorizontal();
				if(GUILayout.Button("Create Controller from Selected Pop Up(Lock!)"))
				{
					CreatePopUpControllersFromSelected();
				}
				if(GUILayout.Button("Create Controller from Selected Background(Lock!)"))
				{
					CreatePopUpBkgControllersFromSelected();
				}
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("Update Pop Up Ids"))
				{
					UpdatePopUpIds();
				}
				if(GUILayout.Button("Update Background Ids"))
				{
					UpdatePopUpBkgIds();
				}
				GUILayout.EndHorizontal();
				if(GUILayout.Button("Update All References"))
				{
					UpdatePrefabs();
				}
			}
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Enable All"))
			{
				uiPopUpsManager.SwitchAllPopUps(true);
			}
			if(GUILayout.Button("Disable All"))
			{
				uiPopUpsManager.SwitchAllPopUps(false);
			}
			GUILayout.EndHorizontal();
			if(GUILayout.Button("Recalculate Positions"))
			{
				uiPopUpsManager.ResetAllPositions();
			}
			bool eliminated = false;
			if(!Application.isPlaying)
			{
				if(GUILayout.Button("Reset"))
				{
					ResetManager();
					eliminated = true;
				}
			}
			EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObj.FindProperty("autoInitialize"));
            EditorGUILayout.PropertyField(serializedObj.FindProperty("dontDestroyOnLoad"));
            EditorGUILayout.PropertyField(serializedObj.FindProperty("justDisableOnPopUpDeactivation"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("canvasScalerMode"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("canvasScalerScreenMatchMode"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("canvasMatchModeRange"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("canvasScalerPixelsPerUnit"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("canvasScalerReferenceResolution"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("popupsPositionNumberOfColumns"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("addHelpFrameToCreatedPopUps"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("popUpHelpFramePrefab"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("popUpBkgHelpFramePrefab"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("screenSeparation"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("disponibilityProvider"));
			SerializedProperty prohibitedList = serializedObj.FindProperty("screenProhibitedForPopUps");
			EditorGUILayout.PropertyField(prohibitedList,true);
			SerializedProperty mask =  serializedObj.FindProperty("systemLayer");
			mask.intValue = EditorGUILayout.LayerField("System Layer",mask.intValue);
			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
			SerializedProperty list = serializedObj.FindProperty("allPopUpControllers");
			EditorGUILayout.PropertyField(list);
			bool added = false;
			if(list.isExpanded)
			{
				if(GUILayout.Button("+"))
				{
					//list.InsertArrayElementAtIndex(list.arraySize);
					uiPopUpsManager.CreateNewEmptyPopUpControllerAlone();
					added = true;
				}
				else
				{
					//EditorGUILayout.SelectableLabel("Size   "+list.FindPropertyRelative("Array.size").intValue);
					if(!eliminated)
					{
						for (int i = 0; i < list.arraySize; i++) 
						{
							SerializedProperty element = list.GetArrayElementAtIndex(i);
							string elementId = element.FindPropertyRelative("uiUniqueId").stringValue;
							EditorGUILayout.BeginHorizontal();
							element.isExpanded = EditorGUILayout.Foldout(element.isExpanded,elementId);
							if(GUILayout.Button("-",EditorStyles.miniButton,GUILayout.MaxWidth(30)))
							{
								uiPopUpsManager.RemoveUIPopUpScreenManagerController(elementId,false);
								break;
							}
							EditorGUILayout.EndHorizontal();

							if(element.isExpanded)
							{
								EditorGUILayout.PropertyField(element);
								EditorGUIHelper.LineSeparator();
							}
							EditorGUIHelper.LineSeparator();
						}
					}
				}
			}
			if(!added)
			{
				SerializedProperty bkgList = serializedObj.FindProperty("allPopUpBackgroundsControllers");
				EditorGUILayout.PropertyField(bkgList);
				if(bkgList.isExpanded)
				{
					if(GUILayout.Button("+"))
					{
						//bkgList.InsertArrayElementAtIndex(bkgList.arraySize);
						uiPopUpsManager.CreateNewEmptyPopUpBackgroundControllerAlone();
					}
					else
					{
						//EditorGUILayout.SelectableLabel("Size   "+list.FindPropertyRelative("Array.size").intValue);
						if(!eliminated)
						{
							for (int i = 0; i < bkgList.arraySize; i++) 
							{
								SerializedProperty element = bkgList.GetArrayElementAtIndex(i);
								string elementId = element.FindPropertyRelative("uiUniqueId").stringValue;
								EditorGUILayout.BeginHorizontal();
								element.isExpanded = EditorGUILayout.Foldout(element.isExpanded,elementId);
								if(GUILayout.Button("-",EditorStyles.miniButton,GUILayout.MaxWidth(30)))
								{
									uiPopUpsManager.RemoveUIPopUpBkgScreenManagerController(elementId,false);
									break;
								}
								EditorGUILayout.EndHorizontal();

								if(element.isExpanded)
								{
									EditorGUILayout.PropertyField(element);
									EditorGUIHelper.LineSeparator();
								}
								EditorGUIHelper.LineSeparator();
							}

						}
					}
				}
			}
			if(serializedObject != null)
			{
				// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
				serializedObject.ApplyModifiedProperties();
			}
		}
	}

	void SpawnAllPopUps()
	{
		Undo.RegisterCreatedObjectUndo(uiPopUpsManager,uiPopUpsManager.name);
		for(int i = 0; i < uiPopUpsManager.allPopUpControllers.Count; i++)
		{
			if(uiPopUpsManager.allPopUpControllers[i].uiScreenObject == null && 
				uiPopUpsManager.allPopUpControllers[i].uiScreenPrefab != null)
			{						
				GameObject instancedGO = (GameObject)PrefabUtility.InstantiatePrefab(uiPopUpsManager.allPopUpControllers[i].uiScreenPrefab.gameObject);
				if(instancedGO != null )
				{
					UIScreenManager instancedUISM = instancedGO.GetComponent<UIScreenManager>();
					if(instancedUISM != null)
					{
						uiPopUpsManager.allPopUpControllers[i].uiScreenObject = instancedGO.GetComponent<UIScreenManager>();
						if(uiPopUpsManager.allPopUpControllers[i].uiScreenObject == null)//delete newly created object
						{
							DestroyImmediate(instancedGO);
							EditorUtility.DisplayDialog("Cant create.","Cant Spawn PopUp Screen["+uiPopUpsManager.allPopUpControllers[i].uiUniqueId+"].","Ok");
						}
						else
						{
							uiPopUpsManager.allPopUpControllers[i].uiScreenObject.CachedTransform.SetSiblingIndex( uiPopUpsManager.GetLastPopUpSiblingIndex());
							uiPopUpsManager.allPopUpControllers[i].Switch(false,true);
						}
					}
				}
				else
				{
					Debug.Log("Cant spawn["+uiPopUpsManager.allPopUpControllers[i].uiScreenPrefab.name+"]");
				}
			}	
		}
	}

	void SpawnAllPopUpBackgrounds()
	{
		Undo.RegisterCreatedObjectUndo(uiPopUpsManager,uiPopUpsManager.name);
		for(int i = 0; i < uiPopUpsManager.allPopUpBackgroundsControllers.Count; i++)
		{
			if(uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject == null && 
				uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab != null)
			{						
				GameObject instancedGO = (GameObject)PrefabUtility.InstantiatePrefab(uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab.gameObject);
				if(instancedGO != null )
				{
					UIScreenManager instancedUISM = instancedGO.GetComponent<UIScreenManager>();
					if(instancedUISM != null)
					{
						uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject = instancedGO.GetComponent<UIScreenManager>();
						if(uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject == null)//delete newly created object
						{
							DestroyImmediate(instancedGO);
							EditorUtility.DisplayDialog("Cant create.","Cant Spawn PopUp Screen["+uiPopUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId+"].","Ok");
						}
						else
						{
							uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject.CachedTransform.SetSiblingIndex( uiPopUpsManager.GetLastPopUpBkgSiblingIndex());
							uiPopUpsManager.allPopUpBackgroundsControllers[i].Switch(false,true);
						}
					}
				}
				else
				{
					Debug.Log("Cant spawn["+uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab.name+"]");
				}
			}	
		}
	}
		
	void DespawnAllPopUps()
	{
		Undo.RegisterCreatedObjectUndo(uiPopUpsManager,uiPopUpsManager.name);
		for(int i = 0; i < uiPopUpsManager.allPopUpControllers.Count; i++)
		{
			uiPopUpsManager.allPopUpControllers[i].Switch(false,false);
		}
	}

	void DespawnAllBackgrounds()
	{
		Undo.RegisterCreatedObjectUndo(uiPopUpsManager,uiPopUpsManager.name);
		for(int i = 0; i < uiPopUpsManager.allPopUpBackgroundsControllers.Count; i++)
		{
			uiPopUpsManager.allPopUpBackgroundsControllers[i].Switch(false,false);
		}
	}

	void CreateNewScreenPopUp()
	{
		Undo.RegisterCreatedObjectUndo(uiPopUpsManager,uiPopUpsManager.name);
		GameObject newUIScreenManager = uiPopUpsManager.CreateNewUIPopUpScreenManager();
		if(newUIScreenManager != null)
		{
			newUIScreenManager.transform.SetSiblingIndex( uiPopUpsManager.GetLastPopUpSiblingIndex());
			ArrayList sceneViews = UnityEditor.SceneView.sceneViews;
			if(sceneViews != null)
			{
				if(sceneViews.Count > 0)
				{
					UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[0];
					sceneView.AlignViewToObject(newUIScreenManager.transform);
				}
			}
		}
	}

	void CreateNewScreenPopUpBackground()
	{
		Undo.RegisterCreatedObjectUndo(uiPopUpsManager,uiPopUpsManager.name);
		GameObject newUIScreenManager = uiPopUpsManager.CreateNewUIPopUpBKGScreenManager();
		if(newUIScreenManager != null)
		{
			newUIScreenManager.transform.SetSiblingIndex( uiPopUpsManager.GetLastPopUpBkgSiblingIndex());
			ArrayList sceneViews = UnityEditor.SceneView.sceneViews;
			if(sceneViews != null)
			{
				if(sceneViews.Count > 0)
				{
					UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[0];
					sceneView.AlignViewToObject(newUIScreenManager.transform);
				}
			}
		}
	}

	void CreatePopUpControllersFromSelected()
	{
		if(Selection.gameObjects.Length > 0)
		{
			for(int i = 0; i < Selection.gameObjects.Length; i++)
			{
				UIScreenManager uism = Selection.gameObjects[i].GetComponent<UIScreenManager>();
				if(uism != null)
				{
					Undo.RegisterCreatedObjectUndo(uiPopUpsManager,uiPopUpsManager.name);

					PrefabType pType = PrefabUtility.GetPrefabType(uism.gameObject);
					bool isPrefab = pType == PrefabType.Prefab;
					UIScreenManager newUIScreenManager = null;
					//Debug.Log("Selection type ["+pType+"]");
					if(isPrefab)
					{
						//	Debug.Log("Creating screen from prefab reference");
						GameObject instancedGO = (GameObject)PrefabUtility.InstantiatePrefab(Selection.gameObjects[i]);
						if(instancedGO != null )
						{
							UIScreenManager instancedUISM = instancedGO.GetComponent<UIScreenManager>();
							if(instancedUISM != null)
							{
								newUIScreenManager = uiPopUpsManager.CreateNewUIPopUpScreenControllerFromSceneObject(instancedUISM);
								if(newUIScreenManager == null)//delete newly created object
								{
									DestroyImmediate(instancedGO);
									EditorUtility.DisplayDialog("Cant create.","For a new UIScreenManager to be registered/created it must have a unique Id.","Ok");
								}
							}
						}
					}
					else
					{
						newUIScreenManager = uiPopUpsManager.CreateNewUIPopUpScreenControllerFromSceneObject(uism);
					}

					if(newUIScreenManager != null)
					{
						newUIScreenManager.CachedTransform.SetSiblingIndex( uiPopUpsManager.GetLastPopUpSiblingIndex());
						Object prefab = PrefabUtility.GetCorrespondingObjectFromSource((Object)newUIScreenManager.gameObject);
						if(prefab != null)
						{
							//is a prefab
							string path = AssetDatabase.GetAssetPath(prefab);
							UIScreenManager prefabUIsm = AssetDatabase.LoadAssetAtPath<UIScreenManager>(path);
							if(prefabUIsm != null)
							{
								for(int j = 0; j < uiPopUpsManager.allPopUpControllers.Count; j++)
								{
									if(uiPopUpsManager.allPopUpControllers[j].uiUniqueId == newUIScreenManager.uniqueScreenId)
									{
										uiPopUpsManager.allPopUpControllers[j].uiScreenPrefab = prefabUIsm;
										break;
									}
								}	
							}
						}

						ArrayList sceneViews = UnityEditor.SceneView.sceneViews;
						if(sceneViews != null)
						{
							if(sceneViews.Count > 0)
							{
								UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[0];
								sceneView.AlignViewToObject(newUIScreenManager.transform);
							}
						}
					}

				}
			}
		}
	}

	void CreatePopUpBkgControllersFromSelected()
	{
		if(Selection.gameObjects.Length > 0)
		{
			for(int i = 0; i < Selection.gameObjects.Length; i++)
			{
				UIScreenManager uism = Selection.gameObjects[i].GetComponent<UIScreenManager>();
				if(uism != null)
				{
					Undo.RegisterCreatedObjectUndo(uiPopUpsManager,uiPopUpsManager.name);

					PrefabType pType = PrefabUtility.GetPrefabType(uism.gameObject);
					bool isPrefab = pType == PrefabType.Prefab;
					UIScreenManager newUIScreenManager = null;
					//Debug.Log("Selection type ["+pType+"]");
					if(isPrefab)
					{
						//	Debug.Log("Creating screen from prefab reference");
						GameObject instancedGO = (GameObject)PrefabUtility.InstantiatePrefab(Selection.gameObjects[i]);
						if(instancedGO != null )
						{
							UIScreenManager instancedUISM = instancedGO.GetComponent<UIScreenManager>();
							if(instancedUISM != null)
							{
								newUIScreenManager = uiPopUpsManager.CreateNewUIPopUpScreenBkgControllerFromSceneObject(instancedUISM);
								if(newUIScreenManager == null)//delete newly created object
								{
									DestroyImmediate(instancedGO);
									EditorUtility.DisplayDialog("Cant create.","For a new UIScreenManagerBKG to be registered/created it must have a unique Id.","Ok");
								}
							}
						}
					}
					else
					{
						newUIScreenManager = uiPopUpsManager.CreateNewUIPopUpScreenBkgControllerFromSceneObject(uism);
					}

					if(newUIScreenManager != null)
					{
						newUIScreenManager.CachedTransform.SetSiblingIndex( uiPopUpsManager.GetLastPopUpBkgSiblingIndex());
						Object prefab = PrefabUtility.GetCorrespondingObjectFromSource((Object)newUIScreenManager.gameObject);
						if(prefab != null)
						{
							//is a prefab
							string path = AssetDatabase.GetAssetPath(prefab);
							UIScreenManager prefabUIsm = AssetDatabase.LoadAssetAtPath<UIScreenManager>(path);
							if(prefabUIsm != null)
							{
								for(int j = 0; j < uiPopUpsManager.allPopUpBackgroundsControllers.Count; j++)
								{
									if(uiPopUpsManager.allPopUpBackgroundsControllers[j].uiUniqueId == newUIScreenManager.uniqueScreenId)
									{
										uiPopUpsManager.allPopUpBackgroundsControllers[j].uiScreenPrefab = prefabUIsm;
										break;
									}
								}	
							}
						}

						ArrayList sceneViews = UnityEditor.SceneView.sceneViews;
						if(sceneViews != null)
						{
							if(sceneViews.Count > 0)
							{
								UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[0];
								sceneView.AlignViewToObject(newUIScreenManager.transform);
							}
						}
					}

				}
			}
		}
	}

	void UpdatePopUpIds()
	{
		for(int i = 0; i < uiPopUpsManager.allPopUpControllers.Count; i++)
		{
			if(uiPopUpsManager.allPopUpControllers[i].uiScreenObject != null)
			{
				uiPopUpsManager.allPopUpControllers[i].uiScreenObject.uniqueScreenId = uiPopUpsManager.allPopUpControllers[i].uiUniqueId;
			}
			if(uiPopUpsManager.allPopUpControllers[i].uiScreenPrefab != null)
			{
				uiPopUpsManager.allPopUpControllers[i].uiScreenPrefab.uniqueScreenId = uiPopUpsManager.allPopUpControllers[i].uiUniqueId;
			}
			AssetDatabase.SaveAssets();
		}
	}

	void UpdatePopUpBkgIds()
	{
		for(int i = 0; i < uiPopUpsManager.allPopUpBackgroundsControllers.Count; i++)
		{
			bool popUpsUpdated = false;
			if(uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject != null)
			{
				popUpsUpdated = true;
				string oldId = uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject.uniqueScreenId;
				uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject.uniqueScreenId = uiPopUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId;
				//update on related popups
				for(int j = 0; j < uiPopUpsManager.allPopUpControllers.Count; j++)
				{
					uiPopUpsManager.allPopUpControllers[j].UpdateComplementIds(oldId,uiPopUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId);
				}
			}
			if(uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab != null)
			{
				if(!popUpsUpdated)
				{
					popUpsUpdated = true;
					string oldId = uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab.uniqueScreenId;
					//update on related popups
					for(int j = 0; j < uiPopUpsManager.allPopUpControllers.Count; j++)
					{
						uiPopUpsManager.allPopUpControllers[j].UpdateComplementIds(oldId,uiPopUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId);
					}
				}
				uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab.uniqueScreenId = uiPopUpsManager.allPopUpBackgroundsControllers[i].uiUniqueId;
			}
			AssetDatabase.SaveAssets();
		}
	}

	void UpdatePrefabs()
	{
		for(int i = 0; i < uiPopUpsManager.allPopUpControllers.Count; i++)
		{
			if(uiPopUpsManager.allPopUpControllers[i].uiScreenObject != null)
			{
				Object prefab = PrefabUtility.GetCorrespondingObjectFromSource((Object)uiPopUpsManager.allPopUpControllers[i].uiScreenObject);
				if(prefab != null)
				{
					//is a prefab
					string path = AssetDatabase.GetAssetPath(prefab);
					UIScreenManager uism = AssetDatabase.LoadAssetAtPath<UIScreenManager>(path);
					if(uism != null)
					{
						uiPopUpsManager.allPopUpControllers[i].uiScreenPrefab = uism;
					}
				}
			}
		}
		for(int i = 0; i < uiPopUpsManager.allPopUpBackgroundsControllers.Count; i++)
		{
			if(uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject != null)
			{
				Object prefab = PrefabUtility.GetCorrespondingObjectFromSource((Object)uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenObject);
				if(prefab != null)
				{
					//is a prefab
					string path = AssetDatabase.GetAssetPath(prefab);
					UIScreenManager uism = AssetDatabase.LoadAssetAtPath<UIScreenManager>(path);
					if(uism != null)
					{
						uiPopUpsManager.allPopUpBackgroundsControllers[i].uiScreenPrefab = uism;
					}
				}
			}
		}
	}

	void ResetManager()
	{
		bool result = EditorUtility.DisplayDialog("Reset PopUps Manager?","Reset this PopUps Manager?\nThis will clear all the controllers and destroy the current popups and backgrounds in scene.","Yes","No");
		if(result)
		{
			uiPopUpsManager.Reset();
		}
	}
}