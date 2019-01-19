using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggedAudioManager : BaseAudioManager
{

    private BaseAudioManager wrappedService;

    #region ServiceImp
    public override bool IsServiceNull()
    {
        if(wrappedService != null)
        {
            return wrappedService.IsServiceNull();
        }
        return true;
    }

    public override bool IsLoggedService()
    {
        return true;
    }

    public void SetService(Service serviceToWrap)
    {
        wrappedService = (BaseAudioManager)serviceToWrap;
    }

    public override Service TransformService(bool isLogged)
    {
        if (!isLogged)
        {
            Destroy(this);
            return wrappedService;
        }
        return this;
    }

    public override void InitializeService()
    {
        if (wrappedService != null)
        {
            Debug.Log("Initializing AudioManager Service.");
            wrappedService.InitializeService();
        }
    }
    #endregion

    #region GENERAL

    public override AudioObject Play(string audioItemId, Transform parent = null, Vector3 point = default(Vector3))
    {
        if(wrappedService != null)
        {
            Debug.LogFormat("Playing: {0} in {1} at {2}",audioItemId, parent, point);
            return wrappedService.Play(audioItemId, parent, point);
        }
        return null;
    }

    public override AudioObject PlayLast(string categoryId, Transform parent = null, Vector3 point = default(Vector3))
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Playing Last in category: {0} with parent {1} at {2}", categoryId, parent, point);
            return wrappedService.PlayLast(categoryId, parent, point);
        }
        return null;
    }

    public override bool IsPlayingAudioWithId(string audioItemId, bool onlyActiveStatus = true)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("IsPlaying: {0} OnlyActive:{1}", audioItemId, onlyActiveStatus);
            return wrappedService.IsPlayingAudioWithId(audioItemId, onlyActiveStatus);
        }
        return false;
    }


    public override void Pause(string audioItemId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Pause: {0}", audioItemId);
            wrappedService.Pause(audioItemId);
        }
    }

    public override void Stop(string audioItemId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Stop: {0}", audioItemId);
            wrappedService.Stop(audioItemId);
        }
    }


    public override void Resume(string audioItemId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Resume: {0}", audioItemId);
            wrappedService.Resume(audioItemId);
        }
    }


    public override void ResumeAllAudios()
    {
        if (wrappedService != null)
        {
            Debug.Log("Resume All");
            wrappedService.ResumeAllAudios();
        }
    }


    public override void PauseAllAudios()
    {
        if (wrappedService != null)
        {
            Debug.Log("Pause All");
            wrappedService.PauseAllAudios();
        }
    }

 
    public override void StopAllAudios()
    {
        if (wrappedService != null)
        {
            Debug.Log("Stop All");
            wrappedService.StopAllAudios();
        }
    }

  
    public override void UpdateVolume(float newVolume)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Update Global Vol: {0}", newVolume);
            wrappedService.UpdateVolume(newVolume);
        }
    }


    public override void RegisterToItemStart(string audioItemId, AudioChangedPlayingStatus listener)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Register To Start of {0}", audioItemId);
            wrappedService.RegisterToItemStart(audioItemId, listener);
        }
    }

    public override void RegisterToItemStopped(string audioItemId, AudioChangedPlayingStatus listener)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Register To Stop of {0}", audioItemId);
            wrappedService.RegisterToItemStopped(audioItemId, listener);
        }
    }

    public override void UnregisterToItemStart(string audioItemId, AudioChangedPlayingStatus listener)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Unregister To Start of {0}", audioItemId);
            wrappedService.UnregisterToItemStart(audioItemId, listener);
        }
    }

    public override void UnregisterToItemStopped(string audioItemId, AudioChangedPlayingStatus listener)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Unregister To Stop of {0}", audioItemId);
            wrappedService.UnregisterToItemStopped(audioItemId, listener);
        }
    }

    #endregion

    #region AUDIO_CATEGORY

    public override AudioCategory GetCategory(string categoryId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("GetCategory: {0}", categoryId);
            return wrappedService.GetCategory(categoryId);
        }
        return null;
    }

    public override bool CreateNewCategory(out AudioCategory acCategory, string categoryId, AudioObject aoPrefab, float volume = 1.0f, bool startEnabled = true)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Create New Category: {0} with prefab:{1} vol:{2} startStatus:{3}", categoryId, aoPrefab.name, volume, startEnabled);
            return wrappedService.CreateNewCategory(out acCategory, categoryId, aoPrefab, volume, startEnabled);
        }
        acCategory = null;
        return false;
    }

    public override bool AddAudioItemToCategory(ref AudioItem aiItem, string categoryId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Add Audio Item {0} To Category: {1}", aiItem.itemId, categoryId);
            return wrappedService.AddAudioItemToCategory(ref aiItem, categoryId);
        }
        return false;
    }

    public override void SwitchCategory(string categoryId, bool enable)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Switching Category {0} to {1}", categoryId, enable);
            wrappedService.SwitchCategory(categoryId, enable);
        }
    }

    public override void UpdateCategoryVolume(string categoryId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Update Category Volume {0}", categoryId);
            wrappedService.UpdateCategoryVolume(categoryId);
        }
    }

    public override float GetCategoryVolume(string categoryId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Get Category Volume {0}", categoryId);
            return wrappedService.GetCategoryVolume(categoryId);
        }
        return 0.0f;
    }

    public override bool IsCategoryEnabled(string categoryId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Is Category enabled {0}", categoryId);
            return wrappedService.IsCategoryEnabled(categoryId);
        }
        return false;
    }

    public override void ResumeAllAudiosInCategory(string categoryId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Resume All Audios In Category {0}", categoryId);
            wrappedService.ResumeAllAudiosInCategory(categoryId);
        }
    }

    public override void PauseAllAudiosInCategory(string categoryId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Pause All Audios In Category {0}", categoryId);
            wrappedService.PauseAllAudiosInCategory(categoryId);
        }
    }

    public override void StopAllAudiosInCategory(string categoryId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Stop All Audios In Category {0}", categoryId);
            wrappedService.StopAllAudiosInCategory(categoryId);
        }
    }
    #endregion

    #region AUDIOITEM
    public override AudioItem GetAudioItem(string audioItemId, out AudioCategory pCategory)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Get Audio Item {0}", audioItemId);
            return wrappedService.GetAudioItem(audioItemId, out pCategory);
        }
        pCategory = null;
        return null;
    }
    #endregion



}
