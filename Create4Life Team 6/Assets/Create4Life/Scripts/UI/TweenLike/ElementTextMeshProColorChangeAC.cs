using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElementTextMeshProColorChangeAC : AnimatedElementController 
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
    private TextMeshProUGUI text;

    public TextMeshProUGUI Text
    {
        get
        {
            GetText();
            return text;
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

    private void GetText()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
            if (text == null)
            {
                text = gameObject.AddComponent<TextMeshProUGUI>();
            }
        }
    }

    public override void ResetToStartingPoint()
    {
        text.color = startColor;
    }

    public override void ResetToEndingPoint()
    {
        text.color = targetColor;
    }

    public override void Play ()
	{
		if(gameObject.activeInHierarchy)
		{

            GetText();

            speed = 1.0f/duration;
			diff = targetColor - startColor;
			isAnimationFinished = false;
			isPlayingBackwards = false;
			if(setStartingColorAtStart)
			{
				text.color = startColor;
			}
			StartCoroutine("ChangeColor");
		}
	}

	public override void Stop ()
	{
        GetText();

        isAnimationFinished = true;
		if(setTargetColorAtStop)
		{
			text.color = targetColor;
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
					text.color = newColor;

					if(progress >= 1.0f)
					{
						text.color = targetColor;
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
						text.color = newColor;

						if(progress <= 0.0f)
						{
							text.color = startColor;
							progress = 0.0f;
							isPlayingBackwards = false;
						}
					}
					else
					{
						progress += Time.deltaTime*speed;
						Color newColor = startColor + diff*curve.Evaluate(progress);
						text.color = newColor;

						if(progress >= 1.0f)
						{
							text.color = targetColor;
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
					text.color = newColor;

					if(progress <= 0.0f)
					{
						text.color = startColor;
						progress = 0.0f;
						isPlayingBackwards = false;
						isAnimationFinished = true;
					}
				}
				else
				{
					progress += Time.deltaTime*speed;
					Color newColor = startColor + diff*curve.Evaluate(progress);
					text.color = newColor;

					if(progress >= 1.0f)
					{
						text.color = targetColor;
						progress = 1.0f;
						isPlayingBackwards = true;
					}
				}
				break;
			case ELEMENT_ANIMATION_TYPE.SINGLE:
				{
					progress += Time.deltaTime*speed;

					Color newColor = startColor + diff*curve.Evaluate(progress);
					text.color = newColor;

					if(progress >= 1.0f)
					{
						text.color = targetColor;
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