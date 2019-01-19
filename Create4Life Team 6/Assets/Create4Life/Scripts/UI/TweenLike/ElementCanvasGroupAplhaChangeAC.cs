using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementCanvasGroupAplhaChangeAC : AnimatedElementController
{
    public bool autoStartOnEnable = true;
    public bool setStartingColorAtStart = true;
    public bool setTargetColorAtStop = true;
    public float startAlpha = 0.0f;
    public float targetAlpha = 1.0f;
    private float diff;
    public float duration = 1.0f;
    public float delay = 0.0f;
    private float speed = 1.0f;
    public AnimationCurve curve;
    private bool isPlayingBackwards = false;
    private bool isAnimationFinished = false;
    private CanvasGroup canvasGroup;

    public override float GetDuration()
    {
        return duration;
    }

    public override void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    public override void OnEnable()
    {
        if (autoStartOnEnable)
        {
            SwitchAnimation(true);
        }
    }

    public override void OnDisable()
    {
        if (IsAnimating)
        {
            SwitchAnimation(false);
        }
    }

    public override void Reset()
    {
        progress = 0.0f;
    }

    private void GetCanvasGroup()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    public override void Play()
    {
        if (gameObject.activeInHierarchy)
        {

            GetCanvasGroup();

            speed = 1.0f / duration;
            diff = targetAlpha - startAlpha;
            isAnimationFinished = false;
            isPlayingBackwards = false;
            if (setStartingColorAtStart)
            {
                canvasGroup.alpha = startAlpha;
            }
            StartCoroutine("ChangeAlpha");
        }
    }

    public override void ResetToStartingPoint()
    {
        GetCanvasGroup();
        canvasGroup.alpha = startAlpha;
    }

    public override void ResetToEndingPoint()
    {
        GetCanvasGroup();
        canvasGroup.alpha = targetAlpha;
    }

    public override void Stop()
    {
        GetCanvasGroup();

        isAnimationFinished = true;
        if (setTargetColorAtStop)
        {
            canvasGroup.alpha = targetAlpha;
        }
        StopCoroutine("ChangeAlpha");
        StopCoroutine("MustWaitOtherAnimatedControllers");
    }

    IEnumerator ChangeAlpha()
    {
        //HACK to avoid first time playing on autostart
        yield return 0;

        yield return StartCoroutine("MustWaitOtherAnimatedControllers");

        if (delay > 0.0f)
        {
            yield return new WaitForSeconds(delay);
        }
        while (!isAnimationFinished)
        {
            switch (animationType)
            {
                case ELEMENT_ANIMATION_TYPE.LOOP:
                    {
                        progress += Time.deltaTime * speed;

                        float newAlpha = startAlpha + diff * curve.Evaluate(progress);
                        canvasGroup.alpha = newAlpha;

                        if (progress >= 1.0f)
                        {
                            canvasGroup.alpha = targetAlpha;
                            progress = 0.0f;
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.PINGPONG:
                    {
                        if (isPlayingBackwards)
                        {
                            progress -= Time.deltaTime * speed;
                            float newAlpha = startAlpha + diff * curve.Evaluate(progress);
                            canvasGroup.alpha = newAlpha;

                            if (progress <= 0.0f)
                            {
                                canvasGroup.alpha = startAlpha;
                                progress = 0.0f;
                                isPlayingBackwards = false;
                            }
                        }
                        else
                        {
                            progress += Time.deltaTime * speed;
                            float newAlpha = startAlpha + diff * curve.Evaluate(progress);
                            canvasGroup.alpha = newAlpha;

                            if (progress >= 1.0f)
                            {
                                canvasGroup.alpha = targetAlpha;
                                progress = 1.0f;
                                isPlayingBackwards = true;
                            }
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.PINGPONGONCE:
                    if (isPlayingBackwards)
                    {
                        progress -= Time.deltaTime * speed;
                        float newAlpha = startAlpha + diff * curve.Evaluate(progress);
                        canvasGroup.alpha = newAlpha;

                        if (progress <= 0.0f)
                        {
                            canvasGroup.alpha = startAlpha;
                            progress = 0.0f;
                            isPlayingBackwards = false;
                            isAnimationFinished = true;
                        }
                    }
                    else
                    {
                        progress += Time.deltaTime * speed;
                        float newAlpha = startAlpha + diff * curve.Evaluate(progress);
                        canvasGroup.alpha = newAlpha;

                        if (progress >= 1.0f)
                        {
                            canvasGroup.alpha = targetAlpha;
                            progress = 1.0f;
                            isPlayingBackwards = true;
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.SINGLE:
                    {
                        progress += Time.deltaTime * speed;

                        float newAlpha = startAlpha + diff * curve.Evaluate(progress);
                        canvasGroup.alpha = newAlpha;

                        if (progress >= 1.0f)
                        {
                            canvasGroup.alpha = targetAlpha;
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