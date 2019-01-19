using UnityEngine;
using System.Collections;

using System.Collections.Generic;


/******************************************************************/
/* AudioManager                                                   */
/* This manager is in charge of facilitating music and sound FX   */
/* easy to use and control through the hole game.                 */
/******************************************************************/

public class AudioManager : BaseAudioManager 
{


    #region ServiceImp
    public override bool IsServiceNull()
    {
        return false;
    }

    public override bool IsLoggedService()
    {
        return false;
    }

    public override void InitializeService()
    {
        base.InitializeService();
        _isLogged = ServiceLocator.GetLogStatusPerType(GetServiceType());
        CreateCategoriesDictionary();
        //Load categoryStatus
    }
    #endregion

    #region GENERAL
    public override AudioObject Play(string audioItemId, Transform parent = null, Vector3 point = default(Vector3))
    {
        AudioCategory ac;
        AudioItem ai = GetAudioItem(audioItemId, out ac);
        if (ai != null)
        {
            ac.lastPlayedId = audioItemId;
            if (ac.IsActive)
            {
                //create audio object from this audio Item and play it
                AudioObject AM_pAOPrefab = (ac.audioObjPrefab == null ? audioObjDefaultPrefab : ac.audioObjPrefab);
                AudioObject result = ai.Play(parent, point, ac.GetVolume() * globalVolume, ref AM_pAOPrefab);
                if (result != null)
                {
                    if (ac.onlyOneItemAllowed)
                    {
                        ac.StopAllAudiosExceptThis(ai.itemId, result.Id);
                    }
                   
                }
				return result;
            }
            else
            {
                if (_isLogged) Debug.LogFormat(this,"AudioCategory[{0}] for Item[{1}] is not Active!", ac.categoryId, audioItemId);
            }
            
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this,"AudioItem [{0}] not founded!",audioItemId);
        }
        return null;
    }

    public override AudioObject PlayLast(string categoryId, Transform parent = null, Vector3 point = default(Vector3))
    {
        AudioCategory ac = GetCategory(categoryId);
        if(ac != null)
        {
            return Play(ac.lastPlayedId, parent, point);
        }

        return null;
    }

    public override bool IsPlayingAudioWithId(string audioItemId,bool onlyActiveStatus = true)
	{
		AudioCategory ac;
		AudioItem ai = GetAudioItem(audioItemId, out ac);
		if (ai != null)
		{
			return ai.IsPlayingAudioWithId(audioItemId,onlyActiveStatus);
		}
		else
		{
            if (_isLogged) Debug.LogFormat(this, "AudioItem [{0}] not founded!", audioItemId);
			return false;
		}
	}

    public override void Pause(string audioItemId)
    {
        AudioCategory ac;
        AudioItem ai = GetAudioItem(audioItemId, out ac);
        if (ai != null)
        {
            ai.PauseAllSubItems();
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this, "AudioItem [{0}] not founded!", audioItemId);
        }
    }

    public override void Stop(string audioItemId)
    {
        AudioCategory ac;
        AudioItem ai = GetAudioItem(audioItemId, out ac);
        if (ai != null)
        {
            ai.StopAllSubItems();
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this,"AudioItem [{0}] not founded!", audioItemId);
        }
    }

    public override void Resume(string audioItemId)
    {
        AudioCategory ac;
        AudioItem ai = GetAudioItem(audioItemId, out ac);
        if (ai != null)
        {
            ai.ResumeAllSubItems();
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this,"AudioItem [{0}] not founded!",audioItemId);
        }
    }

    public override void ResumeAllAudios()
    {
        foreach (KeyValuePair<string, AudioCategory> pair in cateogriesMap)
        {
            pair.Value.ResumeAllAudios();
        }
    }

    public override void PauseAllAudios()
    {
        foreach (KeyValuePair<string, AudioCategory> pair in cateogriesMap)
        {
            pair.Value.PauseAllAudios();
        }
    }

    public override void StopAllAudios()
    {
        foreach (KeyValuePair<string, AudioCategory> pair in cateogriesMap)
        {
            pair.Value.StopAllAudios();
        }
    }

    public override void UpdateVolume(float newVolume)
    {
        globalVolume = newVolume;
        foreach (KeyValuePair<string, AudioCategory> pair in cateogriesMap)
        {
            pair.Value.UpdateVolume(globalVolume);
        }
    }

    public override void RegisterToItemStart(string audioItemId, AudioChangedPlayingStatus listener)
	{
		if (_isLogged) Debug.LogFormat(this,"Register start listener from [{0}]",audioItemId);
		AudioCategory ac;
		AudioItem ai = GetAudioItem(audioItemId, out ac);
		if (ai != null)
		{
			ai.OnAudioStarted += listener;
		}
		else
		{
			if (_isLogged) Debug.LogFormat(this,"AudioItem [{0}] not founded!",audioItemId);
		}
	}

    public override void RegisterToItemStopped(string audioItemId, AudioChangedPlayingStatus listener)
	{
		if (_isLogged) Debug.LogFormat(this,"Register stop listener from [{0}]",audioItemId);
		AudioCategory ac;
		AudioItem ai = GetAudioItem(audioItemId, out ac);
		if (ai != null)
		{
			ai.OnAudioStopped += listener;
		}
		else
		{
			if (_isLogged) Debug.LogFormat(this,"AudioItem [{0}] not founded!",audioItemId);
		}
	}

    public override void UnregisterToItemStart(string audioItemId, AudioChangedPlayingStatus listener)
	{
        if (_isLogged) Debug.LogFormat(this, "Unregister start listener from [{0}]",audioItemId);
		AudioCategory ac;
		AudioItem ai = GetAudioItem(audioItemId, out ac);
		if (ai != null)
		{
			ai.OnAudioStarted -= listener;
		}
		else
		{
            if (_isLogged) Debug.LogFormat(this, "AudioItem [{0}] not founded!", audioItemId);
		}
	}

    public override void UnregisterToItemStopped(string audioItemId, AudioChangedPlayingStatus listener)
	{
		if (_isLogged) Debug.LogFormat(this,"Unregister stop listener from [{0}]", audioItemId);
		AudioCategory ac;
		AudioItem ai = GetAudioItem(audioItemId, out ac);
		if (ai != null)
		{
			ai.OnAudioStopped -= listener;
		}
		else
		{
            if (_isLogged) Debug.LogFormat(this, "AudioItem [{0}] not founded!", audioItemId);
		}
	}

    #endregion

    #region AUDIO_CATEGORY

    /*
   *  Function: Initializes the Dictionary based on the Editor tunned list.
   *  Parameter: None
   *  Return: None
   */

    private void CreateCategoriesDictionary()
    {
        if (_isLogged) Debug.Log("Audio Manager: Creating Dictionaries",this);
        for (int i = 0; i < categoriesList.Count; i++)
        {
            if (!cateogriesMap.ContainsKey(categoriesList[i].categoryId))
            {
                categoriesList[i].InitializeCategory();
                cateogriesMap.Add(categoriesList[i].categoryId, categoriesList[i]);
                if (usePooledAudioObjects)
                {
                    if (! ServiceLocator.Instance.GetServiceOfType<BasePoolManager>(SERVICE_TYPE.POOLMANAGER).CreatePool("AM_"+categoriesList[i].categoryId,
                        (categoriesList[i].audioObjPrefab != null ? categoriesList[i].audioObjPrefab.CachedAudioPooledObject : audioObjDefaultPrefab.CachedAudioPooledObject ))
                        )
                    {
                        if (_isLogged) Debug.LogFormat(this, "Category [{0}] could not create pool.",categoriesList[i].categoryId); 
                    }
                }

            }
        }
    }

    public override AudioCategory GetCategory(string categoryId)
    {
        AudioCategory ac;
        if (cateogriesMap.TryGetValue(categoryId, out ac))
        {
            return ac;
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this,"Audio Manager Failed to find category[{0}]. Not founded!",categoryId);
            return null;
        }
    }

    public override bool CreateNewCategory(out AudioCategory acCategory, string categoryId, AudioObject aoPrefab, float fVolume = 1.0f, bool bStartEnabled = true)
    {
        if (_isLogged) Debug.LogFormat(this, "Audio Manager: Trying to create category[{0}]",categoryId);
        acCategory = null;
        if (cateogriesMap.ContainsKey(categoryId))
        {
            if (_isLogged) Debug.LogWarningFormat(this,"Audio Manager: A Category with Id[{0}] already exists!. Can't create new, try different Id.",categoryId);
        }
        else
        {
            if(aoPrefab == null)
                acCategory = new AudioCategory(categoryId, audioObjDefaultPrefab,fVolume,bStartEnabled);
            else
                acCategory = new AudioCategory(categoryId, aoPrefab, fVolume, bStartEnabled);

            cateogriesMap.Add(categoryId,acCategory);
            categoriesList.Add(acCategory);

            if (_isLogged) Debug.LogFormat(this, "Audio Manager: Category with Id[{0}] was succesfully created!",categoryId);
            return true;
        }
        return false;
    }

    public override bool AddAudioItemToCategory(ref AudioItem aiItem, string categoryId)
    {
        AudioCategory ac = GetCategory(categoryId);
        if (ac != null)
        {
            if (_isLogged) Debug.LogFormat(this, "Audio Manager: Trying to add item[{0}] to category[{1}]",aiItem.itemId,categoryId);
            return ac.AddAudioItem(ref aiItem);
        }
        else
        {
            if (_isLogged) Debug.LogWarningFormat(this,"Audio Manager: No category with Id[{0}] founded.",categoryId);
        }
        return false;
    }

    public override void SwitchCategory(string categoryId, bool enable)
    {
        AudioCategory ac = GetCategory(categoryId);
        if (ac != null)
        {
            ac.SwitchCategory(enable);
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this, "Audio Manager Failed to {0} category[{1}].", (enable ? "enabled" : "disabled"), categoryId);
        }
    }


    public override void UpdateCategoryVolume(string categoryId)
    {
        AudioCategory ac = GetCategory(categoryId);
        if (ac != null)
        {
            ac.UpdateVolume(globalVolume);
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this, "Audio Manager Failed to set volume to category[{0}].", categoryId);
        }
    }

    public override float GetCategoryVolume(string categoryId)
    {
        AudioCategory ac = GetCategory(categoryId);
        if (ac != null)
        {
            return ac.GetVolume();
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this, "Audio Manager Failed to set volume to category[{0}].", categoryId);
            return 0.0f;
        }
    }

    public override bool IsCategoryEnabled(string categoryId)
	{
		AudioCategory ac = GetCategory(categoryId);
		if (ac != null)
		{
			return ac.isEnabled;
		}
		else
		{
            if (_isLogged) Debug.LogFormat(this, "Audio Manager Failed to set volume to category[{0}].", categoryId);
			return false;
		}
	}

    public override void ResumeAllAudiosInCategory(string categoryId)
    {
        AudioCategory ac = GetCategory(categoryId);
        if (ac != null)
        {
            ac.ResumeAllAudios();
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this, "Audio Manager Failed to resume audios in category[{0}].", categoryId);
        }
    }

    public override void PauseAllAudiosInCategory(string categoryId)
    {
        AudioCategory ac = GetCategory(categoryId);
        if (ac != null)
        {
            ac.PauseAllAudios();
        }
        else
        {
            if (_isLogged) Debug.LogFormat(this, "Audio Manager Failed to pause audios in category[{0}].",categoryId);
        }
    }

    public override void StopAllAudiosInCategory(string categoryId)
    {
        AudioCategory ac = GetCategory(categoryId);
        if (ac != null)
        {
            ac.StopAllAudios();
        }
        else
        {
           if (_isLogged) Debug.LogFormat(this, "Audio Manager Failed to stop audios in category[{0}].", categoryId);
        }
    }
    #endregion

    #region AUDIOITEM
    public override AudioItem GetAudioItem(string audioItemId, out AudioCategory pCategory)
    {
        AudioItem ai = null;
        pCategory = null;
        foreach (KeyValuePair<string, AudioCategory> pair in cateogriesMap)
        {
            ai = pair.Value.GetAudioItem(audioItemId);
            if (ai != null)
            {
                pCategory = pair.Value;
                break;
            }
        }
        if (ai == null)
        {
            if(_isLogged)Debug.LogWarning("Audio Item Not Founded",this);
        }
        return ai;
    }
    #endregion

}
