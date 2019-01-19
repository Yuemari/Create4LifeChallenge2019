using UnityEngine;
using System.Collections;


/******************************************************************/
/* AudioObject                                                    */
/* Audio Object contains the information for every type of        */
/* sound that will be used through the game.And it will be used   */
/* as an instanced prefab in charge of playing stopping,          */
/* fading the AudioClips among other tasks.                       */
/******************************************************************/
[RequireComponent(typeof( AudioSource))]
public class AudioObject : CachedMonoBehaviour 
{
	public AudioManager.AudioChangedPlayingStatus OnAudioStarted;
	public AudioManager.AudioChangedPlayingStatus OnAudioStopped;

    /*If True this Item Wont be destroyed when scene loads*/
    public bool mustStopOnLevelLoading = true;
    /*This AudioObject Id*/
	private int instanceId = -1;
    /*Related AudioSubItem*/
	[System.NonSerialized]
	private AudioSubItem	currentSubItem;
    /*Reference to AudioSource component*/
    public AudioSource currentClip;
    /*Audio Pooled Object Reference*/
	private AudioPooledObject audioPooledObject;
    /*Prefab Volume for the AudioClip*/
	[Range(0.0f,1.0f)]
	public float 	volume = 1.0f;
    /*AudioVolume used to manipulate fades*/
	private float   realVolume = 1.0f;
    /*Marks if this Audio should loop*/
	public bool 	mustLoop = false;
    /*Marks AudioObject playing status*/
	private bool 	isPlaying = false;
    /*Marks if the AudioObject is Paused*/
	private bool    isPaused = false;
    /*Marks the Fade In Time*/
    public float    fadeInDuration = 1.0f;
    /*Marks the Fade Out Time*/
    public float    fadeOutDuration = 1.0f;
    /*Marks if the Audio object is doing Fade in*/
    private bool isDoingFadeIn = false;
    /*Reference to the playing fade in coroutine*/
	private IEnumerator fadeInCoroutine;
    /*Marks if the Audio object is doing Fade out*/
	private bool isDoingFadeOut = false;
    /*Reference to the playing Fade out coroutine*/
	private IEnumerator fadeOutCoroutine;

	private float stopAt = 0.0f;

    /*
    *  Function: Returns this instance id
    *  Parameters: None
    *  Return: AudioObject id
    */
    public int Id 
    {
        get 
        {
            return instanceId;
        }
    }

    /*
    *  Function: Provides acces to the reference of the AudioPooledObject for pooling porpouses
    *  Parameters: None
    *  Return: None
    */
    public AudioPooledObject    CachedAudioPooledObject
    {
        get 
        {
            if (audioPooledObject == null)
            {
                audioPooledObject = GetComponent<AudioPooledObject>();
            }
            return audioPooledObject;
        }
    }

    /*
    *  Function: Provides acces to the reference of the AudioSource in this GameObject
    *  Parameters: None
    *  Return: None
    */
    public AudioSource CachedAudioSource
    {
        get 
        {
            if (currentClip == null)
                currentClip = GetComponent<AudioSource>();
            return currentClip;
        }
    }

    /*
    *  Function: Initializes the AudioObject
    *  Parameters: None
    *  Return: None
    */
    void Awake()
    {
        
        if (!mustStopOnLevelLoading)
        {
            GameObject.DontDestroyOnLoad(CachedGameObject);
        }

        currentClip.playOnAwake = false;

    }

    /*
    *  Function: Sets starting data for this AudioObject
    *  Parameter: iInstanceId id of this instance
    *  Parameter: pAudioSubItem related AudioItem
    *  Parameter: fVolume to set in this AudioObject
    *  Return: None
    */
    public void InitializeAudioObject(int iInstanceId,AudioSubItem pAudioSubItem,float fVolume)
    {
        if (pAudioSubItem.RelatedAudioItem.mustShowDebugInfo)
            Debug.Log("Initialize AO with Volume[" + fVolume + "]");
		name = "AO_"+pAudioSubItem.RelatedAudioItem.relatedCategory.categoryId+"_"+pAudioSubItem.RelatedAudioItem.itemId;
        instanceId = iInstanceId;
        currentSubItem = pAudioSubItem;
		currentClip.clip = null;
        currentClip.clip = currentSubItem.audioClip;
        realVolume = fVolume * currentSubItem.volume;
        volume = realVolume;
        currentClip.volume = volume;
        currentClip.loop = mustLoop;
        isPaused = false;
        isPlaying = false;
    }

	public bool IsPlaying()
	{
		return isPlaying && !isPaused;
	}

	void Update()
	{
		if(IsPlaying() && !mustLoop)
		{
			if(Time.realtimeSinceStartup >= stopAt)
			{
				AutoStop();
			}
		}
	}

    /*
    *  Function: Plays the AudioSource with the setted AudioClip related to the setted SubItem
    *  Parameter: None
    *  Return: None
    */
    public void Play()
    {
        if (currentClip.clip != null)
        {
            currentClip.Stop();
            if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
				Debug.Log("Play AO["+instanceId+"]["+currentSubItem.RelatedAudioItem.itemId+"] with FadeInTime[" + fadeInDuration + "]");
            if (fadeInDuration > 0.0f)
            {
                realVolume = 0.0f;
                if (fadeInCoroutine != null)
                {
                    StopCoroutine(fadeInCoroutine);
                    fadeInCoroutine = null;
                    isDoingFadeIn = false;
                }

                fadeInCoroutine = DoFadeIn();
                StartCoroutine(fadeInCoroutine);
            }
            else
            {
                currentClip.Play();
				if(OnAudioStarted != null)
				{
					if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
					{
						Debug.Log("Start event listeners["+OnAudioStarted.GetInvocationList().Length+"]");
					}
					OnAudioStarted(currentSubItem.RelatedAudioItem.itemId);
				}
            }
            isPlaying = currentClip.isPlaying;
            isPaused = false;
            if (!mustLoop)
            {
				float remainingTime = currentClip.clip.length - currentClip.time;
				float stopInSeconds = (remainingTime > fadeOutDuration ? remainingTime - fadeOutDuration : remainingTime);

				if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
				{
					Debug.Log("Invoking AutoStop in["+stopInSeconds+"] at["+(Time.realtimeSinceStartup + stopInSeconds )+"] " +
						"because [" + stopInSeconds + "] + now["+Time.realtimeSinceStartup+"] - fadeOut["+fadeOutDuration+"]");
				}
				//CancelInvoke("AutoStop");
				//Invoke("AutoStop", stopInSeconds);
				stopAt = stopInSeconds + Time.realtimeSinceStartup;
            }
        }
    }

    /*
    *  Function: Fade In Routine
    *  Parameter: None 
    *  Return: None
    */
    IEnumerator DoFadeIn()
    {
        currentClip.volume = 0.0f;
        float fDelta = 0.0f;
        float fLastTime = Time.realtimeSinceStartup;
        float fSpeed = volume/fadeInDuration;

        currentClip.Play();
		if(OnAudioStarted != null)
		{
			Debug.Log("Start event listeners["+OnAudioStarted.GetInvocationList().Length+"]");
			OnAudioStarted(currentSubItem.RelatedAudioItem.itemId);
		}
        if (currentSubItem.RelatedAudioItem.mustShowDebugInfo) 
			Debug.Log("Comenzar FadeIn de["+fadeInDuration+"] en [" + instanceId + "]");

        isDoingFadeIn = true;
        while(isDoingFadeIn)
        {
            if (currentSubItem.RelatedAudioItem.mustShowDebugInfo && !isPaused)
                Debug.Log("FADEIN Speed["+fSpeed+"] Volume["+volume+"] RealVol["+realVolume+"]");

            fDelta = Time.realtimeSinceStartup - fLastTime;
            fLastTime = Time.realtimeSinceStartup;
            if (!isPaused)
            {
                realVolume += fDelta * fSpeed;
                currentClip.volume = realVolume;
                if (realVolume > volume)
                {
                    realVolume = volume;
                    isDoingFadeIn = false;
                }

                if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
                    Debug.Log("CalculatedVolume[" + realVolume + "]");

               currentClip.volume = realVolume;

            }
            yield return new WaitForEndOfFrame();
        }
    }

	public void AutoStop()
	{
		if (currentSubItem.RelatedAudioItem.mustShowDebugInfo && !isPaused)
		{
			Debug.Log("AutoStopping["+name+"]["+instanceId+"]["+currentSubItem.RelatedAudioItem.itemId+"] " +
				"at["+Time.realtimeSinceStartup+"]");
		}
		Stop();
	}

    /*
     *  Function: Fade Out Routine
     *  Parameter: None
     *  Return: None
     */
    IEnumerator DoFadeOut()
    {
        float fDelta = 0.0f;
        float fLastTime = Time.realtimeSinceStartup;
        float fSpeed = volume / fadeOutDuration;

        if (currentSubItem.RelatedAudioItem.mustShowDebugInfo) 
			Debug.Log("Comenzar FadeOut de["+fadeOutDuration+"] en [" + instanceId + "]");
        isDoingFadeOut = true;
        while (isDoingFadeOut)
        {
            if (currentSubItem.RelatedAudioItem.mustShowDebugInfo && !isPaused)
                Debug.Log("FADEOUT Speed[" + fSpeed + "] Volume[" + volume + "] RealVol[" + realVolume + "]");

            fDelta = Time.realtimeSinceStartup - fLastTime;
            fLastTime = Time.realtimeSinceStartup;
           
            if (!isPaused)
            {
                realVolume -= fDelta * fSpeed;
                if (realVolume < 0.0f)
                {
                    realVolume = 0.0f;
                    isDoingFadeOut = false;
                }

                if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
                    Debug.Log("CalculatedVolume[" + realVolume + "]");

                currentClip.volume = realVolume;
            }
            yield return new WaitForEndOfFrame();
        }
		if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
		{
        	Debug.Log("Stop after fadeOut");
		}

        Stop(true,true);
        
    }

    /*
    *  Function: Destroys AudioObject
    *  Parameter: None
    *  Return: None
    */
    public void DestroyAudioObject()
    {
		if (currentSubItem.RelatedAudioItem.mustShowDebugInfo) 
		{
			Debug.Log("Destroy AO[" + instanceId + "]["+currentSubItem.RelatedAudioItem.itemId+"] IsGOActive[" + CachedGameObject.activeSelf + "] IsPlaying[" + isPlaying + "] IsClipPlaying[" + currentClip.isPlaying + "]");
		}
        if (isPlaying || currentClip.isPlaying)
        {
            Stop();
            return;
        }
            
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            isDoingFadeIn = false;
        }

        if (fadeOutCoroutine != null) 
        {
            StopCoroutine(fadeOutCoroutine);
            isDoingFadeOut = false;
        }
        
        isDoingFadeIn = isDoingFadeOut = false;

		if(OnAudioStopped != null)
		{
			if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
			{
				Debug.Log("Stop event listeners["+OnAudioStopped.GetInvocationList().Length+"]");
			}
			OnAudioStopped(currentSubItem.RelatedAudioItem.itemId);
		}

        if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
        {
            Debug.Log("Destroy AO[" + instanceId + "] as pooled?"+ ServiceLocator.Instance.GetServiceOfType<BaseAudioManager>(SERVICE_TYPE.AUDIOMANAGER).usePooledAudioObjects);
        }
        if (ServiceLocator.Instance.GetServiceOfType<BaseAudioManager>(SERVICE_TYPE.AUDIOMANAGER).usePooledAudioObjects)
        {
            ServiceLocator.Instance.GetServiceOfType<BasePoolManager>(SERVICE_TYPE.POOLMANAGER).Despawn(CachedAudioPooledObject);
        }
        else
        {
            Destroy(CachedTransform);
        }
    }

    /*
    *  Function: Resume AudioClip
    *  Parameter: None
    *  Return: None
    */
    public void Resume()
    {
        if (isPlaying && isPaused)
        {
            currentClip.Play();
            if (!mustLoop)
            {
                if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
                    Debug.Log("When resumed time[" + currentClip.time + "] timesamples[" + currentClip.timeSamples + "] length[" + currentClip.clip.length + "]");
                float remainingTime = currentClip.clip.length - currentClip.time;
                float destroyInSeconds = (remainingTime > fadeOutDuration ? remainingTime - fadeOutDuration : remainingTime);
				//Invoke("AutoStop", destroyInSeconds);
				stopAt = destroyInSeconds  + Time.realtimeSinceStartup;;
            }
            isPaused = false;
        }
    }

    /*
    *  Function: Pause AudioClip
    *  Parameter: None
    *  Return: None
    */
    public void Pause()
    {
        if (isPlaying && !isPaused)
        {
            currentClip.Pause();
            if (!mustLoop)
            {
				//CancelInvoke("AutoStop");
                if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
                    Debug.Log("When paused time[" + currentClip.time + "] timesamples[" + currentClip.timeSamples+ "] length[" + currentClip.clip.length + "]");
            }
            isPaused = true;
        }
    }

    /*
    *  Function: Stop AudioClip
    *  Parameter: None
    *  Return: None
    */
	public void Stop(bool forceStop = false,bool fromFadeOut = false)
    {
		//CancelInvoke("AutoStop");
		if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
			Debug.Log("STOP AO["+instanceId+"]Force["+forceStop+"]isPlaying["+(isPlaying || currentClip.isPlaying)+"]");	
        if (fadeOutDuration > 0.0f)
        {
            if (forceStop)
            {
                if (isPlaying || currentClip.isPlaying)
                {
                    if (currentSubItem.RelatedAudioItem.mustShowDebugInfo) 
                        Debug.Log("Stopping Id[" + instanceId + "]");
                    currentClip.Stop();
					currentClip.clip = null;
                    isPlaying = false;
                    isPaused = false;
                    if (currentSubItem.RelatedAudioItem.mustShowDebugInfo) 
                        Debug.Log("valid subitem[" + (currentSubItem != null) + "]");
                    currentSubItem.RelatedAudioItem.UnRegisterAudioObject(this);
                }
                if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
                    Debug.Log("Force Destroying AO");

                DestroyAudioObject();
            }
            else
            {

                if (fadeInCoroutine != null)
                {
					if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
					{
                    	Debug.Log("Stopping fadein");
					}
                    StopCoroutine(fadeInCoroutine);
                    fadeInCoroutine = null;
                    isDoingFadeIn = false;
                }
                if (fadeOutCoroutine != null)
                {
					if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
					{
                    	Debug.Log("Stopping fadeout");
					}
                    
                    StopCoroutine(fadeOutCoroutine);
                    fadeOutCoroutine = null;
                    isDoingFadeOut = false;
                }
               
                if (isPaused)
                {
                    if (isPlaying || currentClip.isPlaying)
                    {
						if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
						{
                        	Debug.Log("Stopping Id[" + instanceId + "]");
						}
                        currentClip.Stop();
						currentClip.clip = null;
                        isPlaying = false;
                        isPaused = false;
						if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
						{
                        	Debug.Log("[" + (currentSubItem != null) + "]");
						}
                        currentSubItem.RelatedAudioItem.UnRegisterAudioObject(this);
                    }
                    if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
                        Debug.Log("Pause Destroying AO");
                    DestroyAudioObject();
                }
                else
                {
                    //start fade out coroutine
                    fadeOutCoroutine = DoFadeOut();
                    StartCoroutine(fadeOutCoroutine);
                }
                
            }
        }
        else
        {
            if (isPlaying || currentClip.isPlaying)
            {
				if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
	                Debug.Log("Stopping Id[" + instanceId + "]");
                currentClip.Stop();
				currentClip.clip = null;
                isPlaying = false;
                isPaused = false;
				if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
	                Debug.Log("can unregister from item[" + (currentSubItem != null) + "]");
                currentSubItem.RelatedAudioItem.UnRegisterAudioObject(this);
            }
            if (currentSubItem.RelatedAudioItem.mustShowDebugInfo)
				Debug.Log("Fadeless Destroying AO["+instanceId+"]");

            DestroyAudioObject();
        }
        
    }

    /*
    *  Function: Sets Volume on AudioObject
    *  Parameter: None
    *  Return: None
    */
    public void SetVolume(float fVolume)
    {
        if (isPlaying)
        {
            volume = fVolume;
            realVolume = fVolume;
            currentClip.volume = fVolume;
        }
    }

}
