using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementScaleAC : AnimatedElementController 
{
	public bool autoStartOnEnable = true;
	public bool setStartingScaleAtStart = true;

	public bool setStartingScaleAtStop = false;
	public bool setTargetScaleAtStop = true;

	public Vector3 startScale = Vector3.zero;
	public Vector3 targetScale = Vector3.one;
	private Vector3 diff;
	public float duration = 1.0f;
    public bool addDelayEveryTime = false;
    public float delay = 0.0f;
	private float speed = 1.0f;
	public AnimationCurve curve;
	private bool isPlayingBackwards = false;
	private bool isAnimationFinished = false;

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

	public void SetStartingScale()
	{
		transform.localScale = startScale;
	}

	public void SetEndingScale()
	{
		transform.localScale = targetScale;
	}

	public override void Reset ()
	{
		progress = 0.0f;
	}
    public override void ResetToStartingPoint()
    {
        transform.localScale = startScale;
    }

    public override void ResetToEndingPoint()
    {
        transform.localScale = targetScale;
    }

    public override void Play ()
	{
		if(gameObject.activeInHierarchy)
		{
			speed = 1.0f/duration;
			diff = targetScale - startScale;
			isAnimationFinished = false;
			isPlayingBackwards = false;
			if(setStartingScaleAtStart)
			{
				transform.localScale = startScale;
			}
            if (!isAnimationFinished)
            {
                StopCoroutine("ScaleUp");
            }
			StartCoroutine("ScaleUp");
		}
	}

	public override void Stop ()
	{
		isAnimationFinished = true;
		if(setStartingScaleAtStop)
		{
			transform.localScale = startScale;
		}
		else if(setTargetScaleAtStop)
		{
			transform.localScale = targetScale;
		}
		StopCoroutine("ScaleUp");
		StopCoroutine("MustWaitOtherAnimatedControllers");
	}

	IEnumerator ScaleUp()
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
			case ELEMENT_ANIMATION_TYPE.LOOP:
				{
					progress += Time.deltaTime*speed;

					Vector3 newScale = startScale + diff*curve.Evaluate(progress);
					transform.localScale = newScale;

					if(progress >= 1.0f)
					{
						transform.localScale = targetScale;
						progress = 0.0f;
                        if (addDelayEveryTime && delay > 0.0f)
                        {
                            yield return new WaitForSeconds(delay);
                        }
					}
				}
				break;
			case ELEMENT_ANIMATION_TYPE.PINGPONG:
				{
					if(isPlayingBackwards)
					{
						progress -= Time.deltaTime*speed;
						Vector3 newScale = startScale + diff*curve.Evaluate(progress);
						transform.localScale = newScale;

						if(progress <= 0.0f)
						{
							transform.localScale = startScale;
							progress = 0.0f;
							isPlayingBackwards = false;
                            if (addDelayEveryTime && delay > 0.0f)
                            {
                                yield return new WaitForSeconds(delay);
                            }
                        }
					}
					else
					{
						progress += Time.deltaTime*speed;
						Vector3 newScale = startScale + diff*curve.Evaluate(progress);
						transform.localScale = newScale;

						if(progress >= 1.0f)
						{
							transform.localScale = targetScale;
							progress = 1.0f;
							isPlayingBackwards = true;
                            if (addDelayEveryTime && delay > 0.0f)
                            {
                                yield return new WaitForSeconds(delay);
                            }
                        }
					}
				}
				break;
			case ELEMENT_ANIMATION_TYPE.PINGPONGONCE:
				if(isPlayingBackwards)
				{
					progress -= Time.deltaTime*speed;
					Vector3 newScale = startScale + diff*curve.Evaluate(progress);
					transform.localScale = newScale;

					if(progress <= 0.0f)
					{
						transform.localScale = startScale;
						progress = 0.0f;
						isPlayingBackwards = false;
						isAnimationFinished = true;
					}
				}
				else
				{
					progress += Time.deltaTime*speed;
					Vector3 newScale = startScale + diff*curve.Evaluate(progress);
					transform.localScale = newScale;

					if(progress >= 1.0f)
					{
						transform.localScale = targetScale;
						progress = 1.0f;
						isPlayingBackwards = true;
                        if (addDelayEveryTime && delay > 0.0f)
                        {
                            yield return new WaitForSeconds(delay);
                        }
                    }
				}
				break;
			case ELEMENT_ANIMATION_TYPE.SINGLE:
				{
					progress += Time.deltaTime*speed;

					Vector3 newScale = startScale + diff*curve.Evaluate(progress);
					transform.localScale = newScale;

					if(progress >= 1.0f)
					{
						transform.localScale = targetScale;
						progress = 1.0f;
						isAnimationFinished = true;
					}
				}
				break;
			}
			yield return 0;
		}
		SwitchAnimation(false);
	}

}
