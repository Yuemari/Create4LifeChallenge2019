using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class UIScreenManagerCreator : MonoBehaviour 
{
	[MenuItem ("GameObject/UI/UI Screen Manager")]
	static void CreateNewUIScreen() 
	{
		UIManager uiManager = FindObjectOfType<UIManager>();
		if(uiManager != null)
		{
			Undo.RegisterCreatedObjectUndo(uiManager,uiManager.name);
			GameObject newUIScreenManager = uiManager.CreateNewUIScreenManager();
			Selection.activeObject = newUIScreenManager;
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
		else
		{
			EditorUtility.DisplayDialog("Cant find UIManager","A UIManager gameobject in the scene is necessary for this feature to work correctly","Ok");
		}
	}
		
	[MenuItem ("CONTEXT/UIScreenManager/Delete")]
	static void RemoveUIScreen(MenuCommand command) 
	{
		UIScreenManager uiScreenManager = (UIScreenManager)command.context;
		UIManager uiManager = FindObjectOfType<UIManager>();
		if(uiManager != null)
		{
			uiManager.RemoveUIScreenManager(uiScreenManager);
		}
		else
		{
			DestroyImmediate(uiScreenManager.gameObject);
		}
	}
		

}
