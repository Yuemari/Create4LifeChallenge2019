using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSimpleRotationAC : AnimatedElementController 
{
	public bool autoStartOnEnable = true;
	public bool setStartingRotationAtStart = true;
	public bool setStartingRotationAtStop = true;
	public float startRotation = 0.0f;
	public float duration = 1.0f;//this represents the time it takes to do a full revolution
    public bool addDelayEveryTime = false;
	public float delay = 0.0f;
	private float speed = 1.0f;
	private bool isAnimationFinished = false;
	public bool rotateClockwise = true;

    public override float GetDuration()
    {
        return duration;
    }

    public override void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    public override void OnEnable ()
	{
		if(autoStartOnEnable)
		{
			SwitchAnimation(true);
		}
	}

	public override void OnDisable ()
	{
		if(IsAnimating)
		{
			SwitchAnimation(false);
		}
	}

	public override void Reset ()
	{
		progress = 0.0f;
	}

    public override void ResetToStartingPoint()
    {
        transform.SetLocalRotationZ(startRotation);
    }

    public override void ResetToEndingPoint()
    {
        transform.SetLocalRotationZ(startRotation);
    }

    public override void Play ()
	{
		if(gameObject.activeInHierarchy)
		{
			speed = 360.0f/duration;
			isAnimationFinished = false;
			if(setStartingRotationAtStart)
			{
				transform.SetLocalRotationZ(startRotation);
			}
			StartCoroutine("Rotate");
		}
	}

	public override void Stop ()
	{
		isAnimationFinished = true;
		if(setStartingRotationAtStop)
		{
			transform.SetLocalRotationZ(startRotation);
		}
		StopCoroutine("Rotate");
		StopCoroutine("MustWaitOtherAnimatedControllers");
	}

	IEnumerator Rotate()
	{
		//HACK to avoid first time playing on autostart
		yield return 0;

		yield return StartCoroutine("MustWaitOtherAnimatedControllers");

		if(delay > 0.0f)
		{
			yield return new WaitForSeconds(delay);
		}
		while(!isAnimationFinished)
		{
			switch(animationType)
			{
			case ELEMENT_ANIMATION_TYPE.SINGLE:
			case ELEMENT_ANIMATION_TYPE.PINGPONG:
			case ELEMENT_ANIMATION_TYPE.PINGPONGONCE:
			case ELEMENT_ANIMATION_TYPE.LOOP:
				{
					progress += Time.deltaTime*speed;
					float newRotation = transform.localEulerAngles.z;
					if(rotateClockwise)
					{
						newRotation -= Time.deltaTime*speed;
					}
					else
					{
						newRotation += Time.deltaTime*speed;
					}
					transform.SetLocalRotationZ(newRotation);
				}
				break;
			}
			yield return 0;
            if(addDelayEveryTime && delay > 0.0f)
            {
                yield return new WaitForSeconds(delay);
            }
		}
		SwitchAnimation(false);
	}
}