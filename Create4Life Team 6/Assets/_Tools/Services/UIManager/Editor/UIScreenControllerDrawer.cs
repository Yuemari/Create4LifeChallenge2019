using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(UIScreenController))]
public class UIScreenControllerDrawer : PropertyDrawer 
{
	
	// Draw the property inside the given rect
	override public void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	{
		property.serializedObject.Update();
		EditorGUI.BeginProperty(position, label, property);
		// Don't make child fields be indented
		int indent = EditorGUI.indentLevel;

		SerializedProperty id = property.FindPropertyRelative("uiUniqueId");
		string idPath = id.propertyPath;
		bool isPopUpController = idPath.Contains("PopUp");
		SerializedProperty destroyFlag = property.FindPropertyRelative("doNotDestroyOnDeactivation");
		SerializedProperty startFlag = property.FindPropertyRelative("isStartingScreen");

		SerializedProperty activeFlag = property.FindPropertyRelative("isActive");
		SerializedProperty specialFlag = property.FindPropertyRelative("isSpecialPopUp");

		SerializedProperty prefab = property.FindPropertyRelative("uiScreenPrefab");
		SerializedProperty currentObject = property.FindPropertyRelative("uiScreenObject");
//		SerializedProperty screenPosition = property.FindPropertyRelative("_uiScreenPosition");
	//	SerializedProperty relatedIds = property.FindPropertyRelative("_complementScreenIds");

		if(property.isExpanded)
		{
			EditorGUI.indentLevel = indent+1;
			EditorGUILayout.PropertyField(id);
			EditorGUILayout.PropertyField(destroyFlag);
			if(!isPopUpController)
			{
				EditorGUILayout.PropertyField(startFlag);
			}
			EditorGUILayout.PropertyField(activeFlag);
			EditorGUILayout.PropertyField(specialFlag);
			EditorGUILayout.PropertyField(prefab);
			EditorGUILayout.PropertyField(currentObject);
			object manager = null;
			manager = EditorGUIHelper.GetParent(property);
			if(manager != null)
			{
				if(manager is UIManager)
				{
					UIManager uiManager = (UIManager)manager;
					if(uiManager.allScreenControllers.Count > 1)
					{
						SerializedProperty list = property.FindPropertyRelative("complementScreenIds");
						EditorGUILayout.PropertyField(list);
						if(list.isExpanded)
						{
							bool addNewComplement = false;
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("["+list.arraySize+"] Screens Related.");

							if((uiManager.allScreenControllers.Count - 1 - list.arraySize) > 0 )
							{
								addNewComplement = GUILayout.Button("+");
							}
							EditorGUILayout.EndHorizontal();
							if(!addNewComplement)
							{
								for (int i = 0; i < list.arraySize; i++) 
								{
									string complementId = list.GetArrayElementAtIndex(i).stringValue;
									int currentComplementIndex = GetComplementSelectedIndex(uiManager,complementId);
									bool removeComplement = false;
									EditorGUILayout.BeginHorizontal();
									int newComplementIndex = EditorGUIHelper.PopupWithStyle( property.serializedObject.targetObject,"PopUpsManager","Related Screen", currentComplementIndex, GetScreenIds(uiManager), new GUIStyle( EditorStyles.popup ) );
									if(newComplementIndex != currentComplementIndex)
									{
										//validate if it is a not assigned id index
										if(IsValidNewComplementAssigment(uiManager,list,newComplementIndex) &&
											uiManager.allScreenControllers[newComplementIndex].uiUniqueId != id.stringValue)
										{
											list.GetArrayElementAtIndex(i).stringValue = uiManager.allScreenControllers[newComplementIndex].uiUniqueId;
										}
										else
										{
											EditorUtility.DisplayDialog("Select different Screen Ids","Complement Screen Ids can not be repeated or be the same.\nPlease select a different one.","Ok");
										}
									}
									removeComplement = GUILayout.Button("-");
									EditorGUILayout.EndHorizontal();
									if(removeComplement)
									{
										list.DeleteArrayElementAtIndex(i);
										break;
									}
								}
							}
							else
							{
								if(uiManager.allScreenControllers.Count > list.arraySize)
								{
									list.InsertArrayElementAtIndex(list.arraySize);
									list.GetArrayElementAtIndex(list.arraySize-1).stringValue = GetNotAssignedComplementId(uiManager,list,id.stringValue);
								}
							}
							
						}
					}
				}
				else if(manager is PopUpsManager)
				{
					PopUpsManager popUpsManager = (PopUpsManager)manager;
					SerializedProperty list = property.FindPropertyRelative("complementScreenIds");
					string path = list.propertyPath;
					if(!path.Contains("Background"))
					{
						EditorGUILayout.PropertyField(list,new GUIContent("Background Ids"));
						if(list.isExpanded)
						{
							if(popUpsManager.allPopUpBackgroundsControllers.Count > 0)
							{
								bool addNewBackground = false;
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("["+list.arraySize+"] Backgrounds Related.");
								if((popUpsManager.allPopUpBackgroundsControllers.Count - list.arraySize) > 0 )
								{
									addNewBackground = GUILayout.Button("+");
								}
								EditorGUILayout.EndHorizontal();
								if(!addNewBackground)
								{
									for (int i = 0; i < list.arraySize; i++) 
									{
										string bkgId = list.GetArrayElementAtIndex(i).stringValue;
										int currentBkgIndex = GetBackgroundSelectedIndex(popUpsManager,bkgId);
										bool removeBkg = false;
										EditorGUILayout.BeginHorizontal();
										//EditorGUILayout.LabelField("Related Background->["+bkgId+"]");
										int newBkgIndex = EditorGUIHelper.PopupWithStyle( property.serializedObject.targetObject,"PopUpsManager","Related Background", currentBkgIndex, GetBackgroundsIds(popUpsManager), new GUIStyle( EditorStyles.popup ) );
										if(newBkgIndex != currentBkgIndex)
										{
											//validate if it is a not assigned id index
											if(IsValidNewAssigment(popUpsManager,list,newBkgIndex))
											{
												list.GetArrayElementAtIndex(i).stringValue = popUpsManager.allPopUpBackgroundsControllers[newBkgIndex].uiUniqueId;
											}
											else
											{
												EditorUtility.DisplayDialog("Select different Background Ids","Background Ids can not be repeated.\nPlease select a different one.","Ok");
											}
										}
										removeBkg = GUILayout.Button("-");
										EditorGUILayout.EndHorizontal();
										if(removeBkg)
										{
											list.DeleteArrayElementAtIndex(i);
											break;
										}
									}
								}
								else
								{
									if(popUpsManager.allPopUpBackgroundsControllers.Count > list.arraySize)
									{
										list.InsertArrayElementAtIndex(list.arraySize);
										list.GetArrayElementAtIndex(list.arraySize-1).stringValue = GetNotAssignedBkgId(popUpsManager,list);
									}
								}
							}
							else
							{
								EditorGUILayout.HelpBox("No Backgrounds Available, Register at least 1 background first.",MessageType.Info);
							}
						}
					}
				}
				else
				{
					SerializedProperty list = property.FindPropertyRelative("_complementScreenIds");
					EditorGUILayout.PropertyField(list);
					if(list.isExpanded)
					{
						EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
						for (int i = 0; i < list.arraySize; i++) 
						{
							EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
						}
					}
				}
			}
		}
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();

		if(property.serializedObject != null)
		{
			property.serializedObject.ApplyModifiedProperties();
		}

	}

	private int GetBackgroundSelectedIndex(PopUpsManager manager,string currentBkgId)
	{
		for(int i = 0; i < manager.allPopUpBackgroundsControllers.Count; i++)
		{
			if(manager.allPopUpBackgroundsControllers[i].uiUniqueId == currentBkgId)
			{
				return i;
			}
		}
		return 0;	
	}

	private int GetComplementSelectedIndex(UIManager manager,string currentComplementId)
	{
		for(int i = 0; i < manager.allScreenControllers.Count; i++)
		{
			if(manager.allScreenControllers[i].uiUniqueId == currentComplementId)
			{
				return i;
			}
		}
		return 0;	
	}

	private string[] GetBackgroundsIds(PopUpsManager manager)
	{
		List<string> ids = new List<string>();
		for(int i = 0; i < manager.allPopUpBackgroundsControllers.Count; i++)
		{
			ids.Add( manager.allPopUpBackgroundsControllers[i].uiUniqueId );
		}
		return ids.ToArray();
	}

	private string[] GetScreenIds(UIManager manager)
	{
		List<string> ids = new List<string>();
		for(int i = 0; i < manager.allScreenControllers.Count; i++)
		{
			ids.Add( manager.allScreenControllers[i].uiUniqueId );
		}
		return ids.ToArray();
	}
		
	private string GetNotAssignedBkgId(PopUpsManager manager,SerializedProperty currentList)
	{
		string selectedId = string.Empty;
		for(int i = 0; i < manager.allPopUpBackgroundsControllers.Count; i++)
		{
			selectedId = manager.allPopUpBackgroundsControllers[i].uiUniqueId;
			//check if it is already assigned
			bool assigned = false;
			for(int j = 0; j < currentList.arraySize; j++)
			{
				if(selectedId == currentList.GetArrayElementAtIndex(j).stringValue)//is already assigned
				{
					assigned = true;
					break;
				}
			}
			if(!assigned)
			{
				break;
			}
		}
		return selectedId;
	}

	private string GetNotAssignedComplementId(UIManager manager,SerializedProperty currentList,string thisId)
	{
		string selectedId = string.Empty;
		for(int i = 0; i < manager.allScreenControllers.Count; i++)
		{
			if(manager.allScreenControllers[i].uiUniqueId != thisId)
			{
				selectedId = manager.allScreenControllers[i].uiUniqueId;
				//check if it is already assigned
				bool assigned = false;
				for(int j = 0; j < currentList.arraySize; j++)
				{
					if(selectedId == currentList.GetArrayElementAtIndex(j).stringValue)//is already assigned
					{
						assigned = true;
						break;
					}
				}
				if(!assigned)
				{
					break;
				}
			}
		}
		return selectedId;
	}

	private bool IsValidNewComplementAssigment(UIManager manager,SerializedProperty currentList,int newIndex)
	{
		for(int i = 0; i < currentList.arraySize; i++)
		{
			if(currentList.GetArrayElementAtIndex(i).stringValue == manager.allScreenControllers[newIndex].uiUniqueId)
			{
				return false;
			}
		}
		return true;
	}

	private bool IsValidNewAssigment(PopUpsManager manager,SerializedProperty currentList,int newIndex)
	{
		for(int i = 0; i < currentList.arraySize; i++)
		{
			if(currentList.GetArrayElementAtIndex(i).stringValue == manager.allPopUpBackgroundsControllers[newIndex].uiUniqueId)
			{
				return false;
			}
		}
		return true;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float spaceToRemove = 0;
		if(property.isExpanded)
		{
			spaceToRemove = 15;
		}

		return base.GetPropertyHeight(property, label) - spaceToRemove;
	}


}
	