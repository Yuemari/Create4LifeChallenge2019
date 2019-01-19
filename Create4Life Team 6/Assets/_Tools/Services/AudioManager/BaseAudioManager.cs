using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************************/
/* AudioManager                                                   */
/* This manager is in charge of facilitating music and sound FX   */
/* easy to use and control through the hole game.                 */
/******************************************************************/
public class BaseAudioManager : Service
{
    #region Variables
    public delegate void AudioChangedPlayingStatus(string id);

    protected bool _isLogged;
    /*Id for every AudioObject controlled by this AudioItem*/
    public static int audioObjInstanceCounter = 0;
    /*Will enable Pooling Capacity on this Audio Object*/
    public bool usePooledAudioObjects = true;
    /*Global Volume that affects all Audios*/
    [Range(0.0f, 1.0f)]
    public float globalVolume = 1.0f;
    /*AudioObject Refab in case the Category doesnt have one.*/
    public AudioObject audioObjDefaultPrefab;
    /*Audio Categories List, is used for tunning porpouses*/
    public List<AudioCategory> categoriesList = new List<AudioCategory>();
    /*Dictionary to every Audio Category registrated in this Manager*/
    protected Dictionary<string, AudioCategory> cateogriesMap = new Dictionary<string, AudioCategory>();
    #endregion

    #region ServiceImp
    public override SERVICE_TYPE GetServiceType()
    {
        return SERVICE_TYPE.AUDIOMANAGER;
    }

    public override bool IsLoggedService()
    {
        return false;
    }

    public override bool IsServiceNull()
    {
       return true;
    }

    public override Service TransformService(bool isLogged)
    {
        _isLogged = isLogged;
        if (_isLogged)
        {
            LoggedAudioManager loggedService = gameObject.AddComponent<LoggedAudioManager>();
            loggedService.SetService(this);
            return loggedService;
        }
        return this;
    }
    #endregion

    #region GENERAL

    
    ///Function: Creates an AudioObject that will play inside a parent if provided and in the Local position provided
    ///Parameter: audioItemId the Id of AudioItem to be played
    ///Parameter: parent the transform to which the AudioObject will be child of
    ///Parameter: point the local position of the created AudioObject
    ///Return: The AudioObject created if successful
    public virtual AudioObject Play(string audioItemId, Transform parent = null, Vector3 point = default(Vector3))
    {
        return null;
    }

    public virtual AudioObject PlayLast(string categoryId, Transform parent = null, Vector3 point = default(Vector3))
    {
        return null;
    }

    /// <summary>
    /// Determines whether this instance is playing audio with identifier the specified audioItemId.
    /// </summary>
    /// <returns><c>true</c> if this instance is playing audio with identifier the specified audioItemId;
    /// otherwise, <c>false</c>.</returns>
    /// <param name="audioItemId">String audio item identifier.</param>
    /// <param name="onlyActiveStatus">If set to <c>true</c> it will ignore if it is paused.</param>
    public virtual bool IsPlayingAudioWithId(string audioItemId, bool onlyActiveStatus = true)
    {
        return false;
    }

    /*
    *  Function: Pause AudioItem with Passed Id
    *  Parameter: AudioItem to be Paused
    *  Return: None
    */
    public virtual void Pause(string audioItemId)
    {
        
    }

    /*
     *  Function: Stop AudioItem with Passed Id
     *  Parameter: AudioItem to be Stopped
     *  Return: None
     */
    public virtual void Stop(string audioItemId)
    {
    }

    /*
    *  Function: Resume AudioItem with Passed Id
    *  Parameter: AudioItem to be Resumed
    *  Return: None
    */
    public virtual void Resume(string audioItemId)
    {
       
    }

    /*
    *  Function: Resume all paused Audios
    *  Parameter: None
    *  Return: None
    */
    public virtual void ResumeAllAudios()
    {
       
    }

    /*
    *  Function: Pause All playing Audios
    *  Parameter: None
    *  Return: None
    */
    public virtual void PauseAllAudios()
    {
    }

    /*
     *  Function: Stop All playing Audios
     *  Parameter: None
     *  Return: None
     */
    public virtual void StopAllAudios()
    {
       
    }

    /*
    *  Function: Updates the volume in all categories
    *  Parameter: new global volume
    *  Return: None
    */
    public virtual void UpdateVolume(float newVolume)
    {
        
    }

    public virtual void RegisterToItemStart(string audioItemId, AudioChangedPlayingStatus listener)
    {    
    }

    public virtual void RegisterToItemStopped(string audioItemId, AudioChangedPlayingStatus listener)
    {
    }

    public virtual void UnregisterToItemStart(string audioItemId, AudioChangedPlayingStatus listener)
    {
    }

    public virtual void UnregisterToItemStopped(string audioItemId, AudioChangedPlayingStatus listener)
    {
    }

    #endregion

    #region AUDIO_CATEGORY
  

    /*
     *  Function: Tries to find a Category with this Id.
     *  Parameter: Category id to be founded
     *  Return: AudioCategory with passed Id
     */
    public virtual AudioCategory GetCategory(string categoryId)
    {
        return null;
    }

    /*
     *  Function: Creates a new category from scratch
     *  Parameter: <out>acCategory is the AudioCategory filled with data if succesfully created
     *  Parameter: categoryId is the Id of this new AudioCategory
     *  Parameter: aoPrefab is the AudioObject that will be used in this Category
     *  Parameter: fVolume is the 
     *  Parameter: bStartEnabled is the
     *  Return: True if successfully created
     */
    public virtual bool CreateNewCategory(out AudioCategory acCategory, string categoryId, AudioObject aoPrefab, float fVolume = 1.0f, bool bStartEnabled = true)
    {
        acCategory = null;
        return false;
    }

    /*
    *  Function: Adds an AudioItem to a Category
    *  Parameter: aiItem is the AudioItem to be added to the category
    *  Parameter: categoryId is the Id of the category to which the AudioItem will be added to
    *  Return: True if added successfully, False otherwise
    */
    public virtual bool AddAudioItemToCategory(ref AudioItem aiItem, string categoryId)
    {
        return false;
    }

    /*
    *  Function: Switch the status of a AudioCategory
    *  Parameter: categoryId is the AudioCategory Id to be change
    *  Parameter: enable is the new status of this category
    *  Return: 
    */
    public virtual void SwitchCategory(string categoryId, bool enable)
    {
    }

    /*
     *  Function: Updates a Category with the global volume
     *  Parameter: Category to be updated with global volume
     *  Return: None
     */
    public virtual void UpdateCategoryVolume(string categoryId)
    {
    }

    /*
     *  Function: Retrives the volume of the Category with the Id passed
     *  Parameter: Category from which we want to know the Volume
     *  Return: Category volume
     */
    public virtual float GetCategoryVolume(string categoryId)
    {
        return 0.0f;
    }

    public virtual bool IsCategoryEnabled(string categoryId)
    {
        return false;
    }

    /*
     *  Function: Resume All paused Audios in a single AudioCategory
     *  Parameter: Category to be resumed
     *  Return: None
     */
    public virtual void ResumeAllAudiosInCategory(string categoryId)
    {
    }

    /*
     *  Function: Pause All playing Audios in a single AudioCategory
     *  Parameter: Category to be paused
     *  Return: None
     */
    public virtual void PauseAllAudiosInCategory(string categoryId)
    {
        
    }

    /*
     *  Function: Stop All playing Audios in a single AudioCategory
     *  Parameter: Category to be Stopped
     *  Return: None
     */
    public virtual void StopAllAudiosInCategory(string categoryId)
    {
        
    }
    #endregion

    #region AUDIOITEM
    /*
    *  Function: Gets the AudioItem with passed Id, as well as the category containing it
    *  Parameter: audioItemId the id of the AudioItem to be found
    *  Parameter: <out> pCategory is the Category containing the AudioItem  
    *  Return: 
    */
    public virtual AudioItem GetAudioItem(string audioItemId, out AudioCategory pCategory)
    {
        pCategory = null;
        return null;
    }
    #endregion



}

#region AUDIO CATEGORY
/******************************************************************/
/* AudioCategory                                                  */
/* This class is in charge of manage a group of similar           */
/* audio items that uses a same goal(All music, all sfx,etc)      */
/* giving tunning options per Audio group                         */
/******************************************************************/

[System.Serializable]
public class AudioCategory
{
    /*Unique Id for this Category*/
    public string categoryId;
    public bool serializeStatus;
    /*Activates console printing*/
    public bool mustShowDebugInfo = false;
    /*Status of this category*/
    public bool isEnabled = true;
    /*Marks if only 1 Audio must be allowed to be played at the same time*/
    public bool onlyOneItemAllowed = true;
    /*AudioPrefab to be used by this Category, if null it will use the default*/
    public AudioObject audioObjPrefab;
    /*Volume of the Category*/
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    /*List of all AudioItems tunned on editor*/
    public List<AudioItem> itemsList = new List<AudioItem>();
    /*Dictionary containing all the owned AudioItems*/
    private Dictionary<string, AudioItem> itemsMap = new Dictionary<string, AudioItem>();

    public string lastPlayedId;

    /*
    *  Function: AudioCategory Contructor
    *  Parameter: iId is the Category Id
    *  Parameter: aoPrefab AudioObject prefab this will use for every audio created.
    *  Parameter: fVolum is the AudioCategory volume
    *  Parameter: bStartEnabled sets if the category should be created as enabled
    *  Return: new AudioCategory created.
    */
    public AudioCategory(string iId, AudioObject aoPrefab, float fVolume = 1.0f, bool bStartEnabled = true, bool bSerialize = true)
    {
        categoryId = iId;
        audioObjPrefab = aoPrefab;
        volume = fVolume;
        isEnabled = bStartEnabled;
        serializeStatus = bSerialize;
      
        InitializeCategory();
    }

    /*
    *  Function: Gives access to the category status.
    *  Parameter: None
    *  Return: True if the category is active
    */
    public bool IsActive
    {
        get { return isEnabled; }
    }

    /*
    *  Function: Switch the status of this category, stops audios if disabled
    *  Parameter: new status of this category 
    *  Return: None
    */
    public void SwitchCategory(bool enable)
    {
        if (isEnabled != enable)
        {
            if (isEnabled && !enable)
            {
                StopAllAudios();
            }
            isEnabled = enable;
            if(serializeStatus)
            {
                PlayerPrefs.SetInt(categoryId,isEnabled?1:0);
            }
        }
    }

    /*
    *  Function: Update the Volume in this Category playing audios.
    *  Parameter: the volume to be passed to every AudioItem in this category
    *  Return: None
    */
    public void UpdateVolume(float fVolume)
    {
        //recalculate volume for every item
        foreach (KeyValuePair<string, AudioItem> pair in itemsMap)
        {
            pair.Value.UpdateVolume(fVolume);
        }
    }

    /*
    *  Function: Gives acces to the volume setted in this category
    *  Parameter: None
    *  Return: Volume in this category
    */
    public float GetVolume()
    {
        return volume;
    }

    /*
    *  Function: Resume all paused Audios in this category
    *  Parameter: None
    *  Return: None
    */
    public void ResumeAllAudios()
    {
        foreach (KeyValuePair<string, AudioItem> pair in itemsMap)
        {
            pair.Value.ResumeAllSubItems();
        }
    }

    /*
    *  Function: Pause all playing Audios in this Category
    *  Parameter: None
    *  Return: None
    */
    public void PauseAllAudios()
    {
        foreach (KeyValuePair<string, AudioItem> pair in itemsMap)
        {
            pair.Value.PauseAllSubItems();
        }
    }

    /*
    *  Function: Stop all playing audios in this category
    *  Parameter: None
    *  Return: None
    */
    public void StopAllAudios()
    {
        foreach (KeyValuePair<string, AudioItem> pair in itemsMap)
        {
            pair.Value.StopAllSubItems();
        }
    }

    /*
    *  Function: Stop all SubItems playing in this category
    *  Parameter: audioItemId is the AudioItem that should not be stopped
    *  Parameter: iAudioObjectId is the AudioObject instance id that should not be stopped
    *  Return: None
    */
    public void StopAllAudiosExceptThis(string audioItemId, int iAudioObjectId)
    {
        AudioItem ai;
        if (itemsMap.TryGetValue(audioItemId, out ai))
        {
            //stop all other audio items
            foreach (KeyValuePair<string, AudioItem> aItem in itemsMap)
            {
                if (aItem.Key != audioItemId)
                    aItem.Value.StopAllSubItems();
            }

            if (mustShowDebugInfo)
                Debug.Log("Audio Manager: Audio Item with Id[" + ai.itemId + "] stopping all subitems except[" + iAudioObjectId + "]!");
            ai.StopAllSubItemsExceptThis(iAudioObjectId);

        }
        else
        {
            if (mustShowDebugInfo)
                Debug.LogWarning("Audio Manager: Audio Item with [" + ai.itemId + "] not founded exist!");
        }
    }

    /*
    *  Function: Adds an AudioItem to this category and sets references as needed.
    *  Parameter: The audioItem to be added to the category
    *  Return: True if the AudioItem was added successfully
    */
    public bool AddAudioItem(ref AudioItem aiItem)
    {

        if (itemsMap.ContainsKey(aiItem.itemId))
        {
            itemsMap.Add(aiItem.itemId, aiItem);
            itemsList.Add(aiItem);
            aiItem.SetRelatedCategory(this);
            if (mustShowDebugInfo)
                Debug.Log("Audio Manager: Audio Item with Id[" + aiItem.itemId + "] succesfully added!");
            return true;
        }
        else
        {
            if (mustShowDebugInfo)
                Debug.LogWarning("Audio Manager: Audio Item with [" + aiItem.itemId + "] already exist!");
        }
        return false;
    }

    /*
    *  Function: Tries to find an AudioItem with passed Id
    *  Parameter: Id of the AudioItem we are looking for
    *  Return: AudioItem founded with passed Id
    */
    public AudioItem GetAudioItem(string strAudioItem)
    {
        AudioItem ai;
        if (!itemsMap.TryGetValue(strAudioItem, out ai))
        {
            if (mustShowDebugInfo)
                Debug.Log("AudioItem [" + strAudioItem + "] founded in this category!");
        }
        return ai;
    }

    /*
    *  Function: Register all the Categories founded in the Editor list inside the class dictionary and sets needed references
    *  Parameter: None
    *  Return: None
    */
    public void InitializeCategory()
    {
        if (itemsMap == null)
            itemsMap = new Dictionary<string, AudioItem>();

        for (int i = 0; i < itemsList.Count; i++)
        {
            itemsMap.Add(itemsList[i].itemId, itemsList[i]);
            itemsList[i].SetRelatedCategory(this);
        }

        if(serializeStatus)
        {
            isEnabled = PlayerPrefs.GetInt(categoryId, isEnabled ? 1 : 0) == 1;
        }

    }

}
#endregion

#region AUDIO ITEM

/*Types of AudioSubItems Methods*/
public enum SubItemPickMode
{
    /*First in list*/
    Unique,
    /*Random between all elements*/
    Random,
    /*Ordered as Listed*/
    Ordered,
    /*Ordered as Inverse Listed*/
    InverseOrdered
}
/******************************************************************/
/* AudioItem                                                      */
/* This class is in charge of manage a group of AudioSubItems     */
/* that are related to the same sound (having multiple sounds     */
/* under one Id) and give options like selection method.          */
/******************************************************************/

[System.Serializable]
public class AudioItem
{
    public AudioManager.AudioChangedPlayingStatus OnAudioStarted;
    public AudioManager.AudioChangedPlayingStatus OnAudioStopped;

    /*Reference to the owner AudioCategory*/
    [System.NonSerialized]
    public AudioCategory relatedCategory;
    /*Id of this AudioItem, this is used to play any sound with the AudioManager*/
    public string itemId;
    /*Activates console printing*/
    public bool mustShowDebugInfo = true;
    /*Volume that affects all AudioSubItems of this AudioItem*/
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    /*Minimun time to be able to play this AudioItem again*/
    public float minTimeBetweenPlay = 0.0f;
    /*Last time this AudioItem was played*/
    private float lastTimePlayed = 0.0f;
    /*Current pick mode for the AudioSubItems*/
    public SubItemPickMode subItemPickMode = SubItemPickMode.Unique;
    /*List that contains every AudioSubItem On this AudioItem*/
    public List<AudioSubItem> subItemsList = new List<AudioSubItem>();
    /*Current AudioSubItem picked Index*/
    private int currentIndex = 0;
    /*Dictionary containing all playing AudioObjects related to this AudioItem*/
    private Dictionary<int, AudioObject> playingAudioObjsMap = new Dictionary<int, AudioObject>();

    /*
    *  Function: Picks AudioSubItem to play, and creates the AudioObject based on the prefab passed
    *  Parameter: transformParent is the new parent of the AudioObject if provided
    *  Parameter: v3Point is the local position of the created AudioObject
    *  Parameter: fVolume to be passed to the AudioObject
    *  Parameter: pPrefab that will be used as instance for this AudioObject
    *  Return: AudioObject created to play this sound if successfull
    */
    public AudioObject Play(Transform transformParent, Vector3 v3Point, float fVolume, ref AudioObject pPrefab)
    {
        float fTimeElapsedSinceLastPlay = Time.realtimeSinceStartup - lastTimePlayed;
        if (mustShowDebugInfo)
            Debug.Log("LastTimePlayed[" + lastTimePlayed + "] TimeELapsed[" + fTimeElapsedSinceLastPlay + "] MinTime[" + minTimeBetweenPlay + "] Volume[" + fVolume + "]");
        if (fTimeElapsedSinceLastPlay >= minTimeBetweenPlay || minTimeBetweenPlay <= 0.0f)
        {
            AudioObject ao = null;
            if (subItemsList.Count > 0)
            {
                if (pPrefab != null)
                {
                    AudioSubItem asiSubItem = null;
                    //pick subitem
                    switch (subItemPickMode)
                    {
                        case SubItemPickMode.Unique:
                            asiSubItem = subItemsList[0];
                            break;
                        case SubItemPickMode.Random:
                            asiSubItem = subItemsList[UnityEngine.Random.Range(0, subItemsList.Count)];
                            break;
                        case SubItemPickMode.Ordered:
                            asiSubItem = subItemsList[currentIndex];
                            AdvanceIndex(true);
                            break;
                        case SubItemPickMode.InverseOrdered:
                            asiSubItem = subItemsList[currentIndex];
                            AdvanceIndex(false);
                            break;
                        default: break;
                    }

                    if (asiSubItem != null)
                    {
                        asiSubItem.SetAudioItem(this);
                    }

                    if (relatedCategory.onlyOneItemAllowed)
                    {
                        if (mustShowDebugInfo)
                        {
                            Debug.Log("Stopping all audios in category[" + relatedCategory.categoryId + "] because only one is allowed and will give chance for a new [" + itemId + "] audio.");
                        }
                        relatedCategory.StopAllAudios();
                    }

                    float itemVolume = fVolume * volume;
                    //substitute for pool manager function
                    if (ServiceLocator.Instance.GetServiceOfType<BaseAudioManager>(SERVICE_TYPE.AUDIOMANAGER).usePooledAudioObjects)
                    {
                        PoolableObject apo = ServiceLocator.Instance.GetServiceOfType<BasePoolManager>(SERVICE_TYPE.POOLMANAGER).Spawn("AM_" + relatedCategory.categoryId);
                        if (apo != null)
                        {
                            ao = ((AudioPooledObject)apo).audioObjReference;
                            if (ao != null)
                            {
                                ao.OnAudioStarted = OnAudioStarted;
                                ao.OnAudioStopped = OnAudioStopped;
                                ao.InitializeAudioObject(AudioManager.audioObjInstanceCounter, asiSubItem, itemVolume);

                                AudioManager.audioObjInstanceCounter++;
                                ao.CachedTransform.parent = transformParent;
                                ao.CachedTransform.localPosition = v3Point;
                                lastTimePlayed = Time.realtimeSinceStartup;
                                ao.Play();
                                RegisterAudioObject(ao);
                            }
                            else
                            {
                                if (mustShowDebugInfo)
                                    Debug.LogWarning("There are no AudioObject reference on PooledAudioObject when playing Item[" + itemId + "]");
                            }
                        }
                        else
                        {
                            if (mustShowDebugInfo)
                                Debug.LogWarning("There are no AudioObject Prefab instance availbale in the pool to play Item[" + itemId + "] in category[AM_" + relatedCategory.categoryId + "]");
                        }

                    }
                    else
                    {
                        ao = GameObject.Instantiate(pPrefab) as AudioObject;
                        if (ao != null)
                        {
                            ao.InitializeAudioObject(AudioManager.audioObjInstanceCounter, asiSubItem, itemVolume);
                            AudioManager.audioObjInstanceCounter++;
                            ao.CachedTransform.parent = transformParent;
                            ao.CachedTransform.localPosition = v3Point;
                            lastTimePlayed = Time.realtimeSinceStartup;
                            RegisterAudioObject(ao);
                            ao.Play();
                        }
                        else
                        {
                            if (mustShowDebugInfo)
                                Debug.LogWarning("There are no AudioObject Prefab instance to play Item[" + itemId + "]");
                        }
                    }
                }
                else
                {
                    if (mustShowDebugInfo)
                        Debug.LogWarning("There are no AudioObject Prefab to play Item[" + itemId + "]");
                }
            }
            else
            {
                if (mustShowDebugInfo)
                    Debug.LogWarning("There are no SubItems to play in Item[" + itemId + "]");
            }
            return ao;
        }
        else
        {
            if (mustShowDebugInfo)
                Debug.LogWarning("Time elpased since last[" + fTimeElapsedSinceLastPlay + "] is not enough to play Item[" + itemId + "]");
        }

        return null;
    }

    public bool IsPlayingAudioWithId(string audioItemId, bool onlyActiveStatus)
    {
        if (onlyActiveStatus)
        {
            return playingAudioObjsMap.Count > 0;
        }
        else
        {
            if (playingAudioObjsMap.Count > 0)
            {
                foreach (KeyValuePair<int, AudioObject> ao in playingAudioObjsMap)
                {
                    //if any is playing
                    if (ao.Value.IsPlaying())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /*
    *  Function: Adds the passed AudioObject to be responsability of this AudioItem
    *  Parameter: AudioObject to register
    *  Return: None
    */
    public void RegisterAudioObject(AudioObject pAudioObject)
    {
        playingAudioObjsMap.Add(pAudioObject.Id, pAudioObject);
        pAudioObject.OnAudioStarted = OnAudioStarted;
        pAudioObject.OnAudioStopped = OnAudioStopped;
        if (mustShowDebugInfo)
            Debug.Log("Register [" + pAudioObject.CachedGameObject.name + "] on [" + itemId + "] playing register");
    }

    /*
    *  Function: Removes the passed AudioObject of this AudioItem responsability
    *  Parameter: AudioObject to remove register from
    *  Return: None
    */
    public void UnRegisterAudioObject(AudioObject pAudioObject)
    {
        if (mustShowDebugInfo)
            Debug.Log("Removing [" + pAudioObject.CachedGameObject.name + "] from [" + itemId + "] playing register");
        pAudioObject.OnAudioStarted = OnAudioStarted;
        pAudioObject.OnAudioStopped = OnAudioStopped;
        playingAudioObjsMap.Remove(pAudioObject.Id);
    }

    /*
    *  Function: Move the index back or forward for picking AudioSubItems porpouses
    *  Parameter: True if wants to advance index, False to move back index
    *  Return: None
    */
    private void AdvanceIndex(bool forward)
    {
        if (forward)
        {
            currentIndex++;
            if (currentIndex >= subItemsList.Count)
                currentIndex = 0;
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = subItemsList.Count - 1;
        }
    }

    /*
    *  Function: Resume all paused playing audios of this AudioItem
    *  Parameter: None
    *  Return: None
    */
    public void ResumeAllSubItems()
    {
        foreach (KeyValuePair<int, AudioObject> pair in playingAudioObjsMap)
        {
            pair.Value.Resume();
        }
    }

    /*
    *  Function: Pause a;; the playing audios of this AudioItem
    *  Parameter: None
    *  Return: None
    */
    public void PauseAllSubItems()
    {
        if (mustShowDebugInfo)
            Debug.Log("Pausing [" + playingAudioObjsMap.Count + "] playing subitems.");
        foreach (KeyValuePair<int, AudioObject> pair in playingAudioObjsMap)
        {
            pair.Value.Pause();
        }
    }

    /*
    *  Function: Stop all playing audios of this AudioItem
    *  Parameter: None
    *  Return: None
    */
    public void StopAllSubItems()
    {
        List<AudioObject> listAO = new List<AudioObject>();
        listAO.AddRange(playingAudioObjsMap.Values);
        for (int i = 0; i < listAO.Count; i++)
        {
            listAO[i].Stop(true);
        }
    }

    /*
    *  Function: Stop all playing audios of this AudioItem except the one with the passed Id
    *  Parameter: the Id of the AudioObject that should not be stopped
    *  Return: None
    */
    public void StopAllSubItemsExceptThis(int iAudioObjectId)
    {
        List<AudioObject> listAO = new List<AudioObject>();
        listAO.AddRange(playingAudioObjsMap.Values);
        if (mustShowDebugInfo)
            Debug.Log("[" + listAO.Count + "] SubItems founded! StopAllBut[" + iAudioObjectId + "]");
        for (int i = 0; i < listAO.Count; i++)
        {
            if (listAO[i].Id != iAudioObjectId)
            {
                if (mustShowDebugInfo)
                    Debug.Log("Force stop[" + listAO[i].Id + "]");
                listAO[i].Stop(true);
            }
        }
    }

    /*
    *  Function: Updates the volume on every playing SubItems
    *  Parameter: new volume to set
    *  Return: None
    */
    public void UpdateVolume(float fVolume)
    {
        foreach (KeyValuePair<int, AudioObject> pair in playingAudioObjsMap)
        {
            pair.Value.SetVolume(volume * relatedCategory.GetVolume() * fVolume);
        }
    }

    /*
    *  Function: Sets a reference to the AudioCategory owner of this AudioItem
    *  Parameter: Reference to the AudioCategory owner of this AudioItem
    *  Return: None
    */
    public void SetRelatedCategory(AudioCategory pAudioCategory)
    {
        relatedCategory = pAudioCategory;
    }

}
#endregion

#region AUDIO SUBITEM
/*******************************************************************/
/* AudioSubItem                                                    */
/* This class is in charge of manage all the AudioClip             */
/* information to be use by the AudioItem                          */
/*******************************************************************/

[System.Serializable]
public class AudioSubItem
{
    /*Reference to the Owner of this AudioSubItem*/
    [System.NonSerialized]
    private AudioItem relatedAudioItem;
    /*Reference to the audio clip that will play this AudioSubItem*/
    public AudioClip audioClip;
    /*SubItem Volume*/
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;

    /*
    *  Function: Sets the reference of the AudioItem owner of this AudioSubItem
    *  Parameter: Reference to the AudioItem that will be responsible for this AudioSubItem
    *  Return: None
    */
    public void SetAudioItem(AudioItem pAudioItem)
    {
        relatedAudioItem = pAudioItem;
    }

    /*
    *  Function: Gives access to the AudioItem in which this AudioSubItem is registered
    *  Parameter: None
    *  Return: AudioItem to which this subitem belongs
    */
    public AudioItem RelatedAudioItem
    {
        get
        {
            return relatedAudioItem;
        }
    }

}
#endregion

#region HELPERS
[System.Serializable]
public class AudioItemsForRegistration
{
    public string relatedCategoryId = string.Empty;
    public AudioItem[] audioItems;

    public void RegisterToAudioManager()
    {
        if (audioItems != null)
        {
            for (int i = 0; i < audioItems.Length; i++)
            {
                ServiceLocator.Instance.GetServiceOfType<BaseAudioManager>(SERVICE_TYPE.AUDIOMANAGER).AddAudioItemToCategory(ref audioItems[i], relatedCategoryId);
            }
        }
    }
}
#endregion