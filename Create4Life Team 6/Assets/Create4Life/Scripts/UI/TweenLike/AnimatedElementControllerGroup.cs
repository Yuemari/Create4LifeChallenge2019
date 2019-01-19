using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct AnimatedElementData
{
    public string animatorId;
    public AnimatedElementController animatedController;
    public bool enableGO;
    public bool addRandomDelayAtPlay;
    public float maxRandomDelay;
    public bool disableGO;
    
}

public class AnimatedElementControllerGroup : MonoBehaviour
{
    public AnimatedElementData[] allAnimators;

    public void Reset()
    {
        if (allAnimators != null)
        {
            for (int i = 0; i < allAnimators.Length; ++i)
            {
                allAnimators[i].animatedController.Reset();
            }
        }
    }

    public void ResetToStartingPoint()
    {
        for (int i = 0; i < allAnimators.Length; ++i)
        {
            allAnimators[i].animatedController.ResetToStartingPoint();
        }
    }

    public void PlayGroup()
    {
        for (int i = 0; i < allAnimators.Length; ++i)
        {
            if (allAnimators[i].enableGO)
            {
                allAnimators[i].animatedController.gameObject.SetActive(true);
            }
            if(allAnimators[i].addRandomDelayAtPlay)
            {
                allAnimators[i].animatedController.SetDelay(Random.Range(0, allAnimators[i].maxRandomDelay));
            }
            allAnimators[i].animatedController.SwitchAnimation(true);
           
        }
    }

    public void StopGroup()
    {
        for (int i = 0; i < allAnimators.Length; ++i)
        {
            allAnimators[i].animatedController.SwitchAnimation(false);
            if (allAnimators[i].disableGO)
            {
                allAnimators[i].animatedController.gameObject.SetActive(false);
            }
        }
    }

    public bool IsAnyPlaying()
    {
        if (allAnimators.Length == 0)
            return true;
        for(int i = 0; i < allAnimators.Length; ++i)
        {
            if(allAnimators[i].animatedController.IsAnimating)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsPlayingWithId(string  animatorId)
    {
        for (int i = 0; i < allAnimators.Length; ++i)
        {
            if (allAnimators[i].animatorId == animatorId)
            {
                return allAnimators[i].animatedController.IsAnimating;
            }
        }
        return false;
    }

    public void SetTotalDuration(float totalDuration)
    {
        float duratonPerAnim = totalDuration / allAnimators.Length;
        for (int i = 0; i < allAnimators.Length; ++i)
        {
            allAnimators[i].animatedController.SetDuration(duratonPerAnim);
        }
    }
}
