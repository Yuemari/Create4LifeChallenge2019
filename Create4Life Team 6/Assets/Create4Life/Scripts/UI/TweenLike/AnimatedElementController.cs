using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimatedElementController : MonoBehaviour 
{
	public enum ELEMENT_ANIMATION_TYPE
	{
		SINGLE,
		LOOP,
		PINGPONG,
		PINGPONGONCE
	}

	public ELEMENT_ANIMATION_TYPE animationType = ELEMENT_ANIMATION_TYPE.SINGLE;
	private bool isAnimating = false;
	protected float progress = 0.0f;
	public abstract void OnEnable();
	public abstract void OnDisable();
	public abstract void Play();
	public abstract void Stop();
	public abstract void Reset();

    public abstract void ResetToStartingPoint();
    public abstract void ResetToEndingPoint();

    public virtual void SetDelay(float newDelay)
    {

    }

    public virtual void ResetToProgress(float _progress)
    {
        progress = _progress;
    }

    public abstract float GetDuration();

    public virtual void SetDuration(float newDuration)
    {

    }

	public RelatedAC[] relatedAnimatedControllers;

	public bool IsAnimating
	{
		get
		{
			return isAnimating;
		}
	}

	public void SwitchAnimation(bool enable)
	{
		isAnimating = enable;
		if(enable)
		{
			Reset();
			Play();
		}
		else
		{
			Stop();
			Reset();
		}
	}

	public float GetAnimationProgress()
	{
		return progress;
	}

	protected IEnumerator MustWaitOtherAnimatedControllers()
	{
		bool mustWait = MustWaitOtherACs();
		while(mustWait)
		{
			yield return 0;
			mustWait = MustWaitOtherACs();
		}
	}

	private bool MustWaitOtherACs()
	{
		bool result = false;
		for(int i = 0; i < relatedAnimatedControllers.Length ; i++)
		{
			result |= !relatedAnimatedControllers[i].IsRelatedControllerReady();
		}
		return result;
	}

}

[System.Serializable]
public class RelatedAC
{
	public AnimatedElementController relatedController;
	public float advanceNeeded = 0.0f;
    public bool needToFinish = false;

	public bool IsRelatedControllerReady()
	{
		if(relatedController != null)
		{
            if(needToFinish)
            {
                return !relatedController.IsAnimating;
            }
			return relatedController.GetAnimationProgress() >= advanceNeeded;		
		}
		return true;
	}
}
