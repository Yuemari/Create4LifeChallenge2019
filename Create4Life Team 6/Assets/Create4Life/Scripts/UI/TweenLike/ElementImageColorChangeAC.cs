using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementImageColorChangeAC : AnimatedElementController 
{
	public bool autoStartOnEnable = true;
	public bool setStartingColorAtStart = true;
	public bool setTargetColorAtStop = true;
	public Color startColor = Color.white;
	public Color targetColor = Color.white;
	private Color diff;
	public float duration = 1.0f;
	public float delay = 0.0f;
	private float speed = 1.0f;
	public AnimationCurve curve;
	private bool isPlayingBackwards = false;
	private bool isAnimationFinished = false;
    private Image image;

    public Image Image
    {
        get
        {
            GetImage();
            return image;
        }
    }

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

    private void GetImage()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
            }
        }
    }

    public override void ResetToStartingPoint()
    {
        GetImage();
        image.color = startColor;
    }

    public override void ResetToEndingPoint()
    {
        GetImage();
        image.color = targetColor;
    }

    public override void Play ()
	{
		if(gameObject.activeInHierarchy)
		{

            GetImage();

            speed = 1.0f/duration;
			diff = targetColor - startColor;
			isAnimationFinished = false;
			isPlayingBackwards = false;
			if(setStartingColorAtStart)
			{
				image.color = startColor;
			}
			StartCoroutine("ChangeColor");
		}
	}

	public override void Stop ()
	{
        GetImage();

        isAnimationFinished = true;
		if(setTargetColorAtStop)
		{
			image.color = targetColor;
		}
		StopCoroutine("ChangeColor");
		StopCoroutine("MustWaitOtherAnimatedControllers");
	}

	IEnumerator ChangeColor()
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

					Color newColor = startColor + diff*curve.Evaluate(progress);
					image.color = newColor;

					if(progress >= 1.0f)
					{
						image.color = targetColor;
						progress = 0.0f;
					}
				}
				break;
			case ELEMENT_ANIMATION_TYPE.PINGPONG:
				{
					if(isPlayingBackwards)
					{
						progress -= Time.deltaTime*speed;
						Color newColor = startColor + diff*curve.Evaluate(progress);
						image.color = newColor;

						if(progress <= 0.0f)
						{
							image.color = startColor;
							progress = 0.0f;
							isPlayingBackwards = false;
						}
					}
					else
					{
						progress += Time.deltaTime*speed;
						Color newColor = startColor + diff*curve.Evaluate(progress);
						image.color = newColor;

						if(progress >= 1.0f)
						{
							image.color = targetColor;
							progress = 1.0f;
							isPlayingBackwards = true;
						}
					}
				}
				break;
			case ELEMENT_ANIMATION_TYPE.PINGPONGONCE:
				if(isPlayingBackwards)
				{
					progress -= Time.deltaTime*speed;
					Color newColor = startColor + diff*curve.Evaluate(progress);
					image.color = newColor;

					if(progress <= 0.0f)
					{
						image.color = startColor;
						progress = 0.0f;
						isPlayingBackwards = false;
						isAnimationFinished = true;
					}
				}
				else
				{
					progress += Time.deltaTime*speed;
					Color newColor = startColor + diff*curve.Evaluate(progress);
					image.color = newColor;

					if(progress >= 1.0f)
					{
						image.color = targetColor;
						progress = 1.0f;
						isPlayingBackwards = true;
					}
				}
				break;
			case ELEMENT_ANIMATION_TYPE.SINGLE:
				{
					progress += Time.deltaTime*speed;

					Color newColor = startColor + diff*curve.Evaluate(progress);
					image.color = newColor;

					if(progress >= 1.0f)
					{
						image.color = targetColor;
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