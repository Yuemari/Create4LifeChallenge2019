using UnityEngine;
using UnityEditor;
using System.Collections;

public class ResetPlayerPreferences
{

	[MenuItem("Tools/ResetPlayerPreferences")]
	static void Reset() 
	{
		if(EditorUtility.DisplayDialog("Are you sure to erase Player Preferences?","Press Ok to erase Player Preferences","Ok","Cancel"))
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}
	}
}
