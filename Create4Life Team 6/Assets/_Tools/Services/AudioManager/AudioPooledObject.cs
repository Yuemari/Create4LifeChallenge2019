using UnityEngine;
using System.Collections;


/******************************************************************/
/* AudioPooledObject                                              */
/* Audio Pooled Object contains the information needed for added  */
/* pool functionality with Audio Object and the Object Manager    */
/* without compromising the Audio Manager functionality.          */
/******************************************************************/
public class AudioPooledObject : PoolableObject
{
	
	public AudioManager.AudioChangedPlayingStatus OnAudioStarted;
	public AudioManager.AudioChangedPlayingStatus OnAudioStopped;

    /*Reference to the Audio Object in this GameObject*/
    public AudioObject audioObjReference;
    /*Activates and deactivates console printing*/
    public bool mustShowDebugInfo = false;

    /*
    *  Function: Activates this gameobject. It is mandatory to have this method for pooling porpouses
    *  Parameters: isFirstTime is true when the object is preloaded in the pool
    *  Return: None
    */
    public override void OnSpawn(bool isFirstTime)
    {
        if (mustShowDebugInfo)
        {
            Debug.Log("OnObjectPooledAudioObject at First Time["+isFirstTime+"]");
        }
        CachedGameObject.SetActive(true);
    }

    /*
    *  Function: DeActivates this gameobject and cleans its clip reference. It is mandatory to have this method for pooling porpouses
    *  Parameters: None
    *  Return: None
    */
    public override void OnDespawn() 
    { 
        if (mustShowDebugInfo)
        {
            Debug.Log("OnDespawnObjectPooledAudioObject");
        }
			
        audioObjReference.currentClip.clip = null;
        CachedGameObject.SetActive(false);
    }
}
