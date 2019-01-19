using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;


public class DeletePlayerPrefs : EditorWindow
{
	[MenuItem("Create4Life/Delete Player Prefs")]
	public static void  DeletePlayerPrefs_Main()
	{
		//EditorWindow.GetWindow(typeof(DeletePlayerPrefs));
		PlayerPrefs.DeleteAll ();
       
		Debug.Log ("Player prefs deleted");
        
	}
		


}