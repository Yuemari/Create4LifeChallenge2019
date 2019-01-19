using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementGOSwitchAC : AnimatedElementController
{
    public bool autoStartOnEnable = true;
    public bool setStartingStatusAtStart = true;
    public bool setTargetStatusAtStop = true;

    public bool initialStatus = false;
    public bool finalStatus = true;

    public GameObject[] relatedGOs;


    public float duration = 1.0f;
    public float delay = 0.0f;
    private float speed = 1.0f;
    private bool isPlayingBackwards = false;
    private bool isAnimationFinished = false;
    public bool setStartingStatusOnEnable = false;
    public bool setFinalStatusOnEnable = false;

    public override float GetDuration()
    {
        return duration;
    }
    public override void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    private void SwitchGOs(bool enable)
    {
        for(int i = 0; i < relatedGOs.Length; ++i)
        {
            relatedGOs[i].SetActive(enable);
        }
    }

    public override void OnEnable()
    {
       
        if (setStartingStatusOnEnable)
        {
            SwitchGOs(initialStatus);
        }
        if (setFinalStatusOnEnable)
        {
            SwitchGOs(finalStatus);
        }

       
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

    public override void ResetToStartingPoint()
    {
        Reset();
        SwitchGOs(initialStatus);
    }

    public override void ResetToEndingPoint()
    {
        progress = 1.0f;
        SwitchGOs(finalStatus);
    }

    public override void SetDelay(float newDelay)
    {
        delay = newDelay;
    }

    public override void Play()
    {
        if (gameObject.activeInHierarchy)
        {
            if (duration > 0)
            {
                speed = 1.0f / duration;
            }
            else
            {
                speed = float.MaxValue;
            }
            
            isAnimationFinished = false;
            isPlayingBackwards = false;
            if (setStartingStatusAtStart)
            {
                SwitchGOs(initialStatus);
            }
            StartCoroutine("Move");
        }
    }

    public override void Stop()
    {
        isAnimationFinished = true;
        if (setTargetStatusAtStop)
        {
            SwitchGOs(finalStatus);
        }
        StopCoroutine("Move");
        StopCoroutine("MustWaitOtherAnimatedControllers");
    }

    IEnumerator Move()
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

                        if (progress >= 1.0f)
                        {
                            SwitchGOs(finalStatus);

                            progress = 0.0f;
                        }
                        yield return 0;
                        if (delay > 0.0f)
                        {
                            yield return new WaitForSeconds(delay);
                        }
                        SwitchGOs(initialStatus);
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.PINGPONG:
                    {
                        if (isPlayingBackwards)
                        {
                            progress -= Time.deltaTime * speed;

                            if (progress <= 0.0f)
                            {
                                SwitchGOs(initialStatus);
                                progress = 0.0f;
                                isPlayingBackwards = false;
                            }
                        }
                        else
                        {
                            progress += Time.deltaTime * speed;

                            if (progress >= 1.0f)
                            {
                                SwitchGOs(finalStatus);
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

                        if (progress <= 0.0f)
                        {
                            SwitchGOs(initialStatus);

                            progress = 0.0f;
                            isPlayingBackwards = false;
                            isAnimationFinished = true;
                        }
                      
                    }
                    else
                    {
                        progress += Time.deltaTime * speed;

                        if (progress >= 1.0f)
                        {
                            SwitchGOs(finalStatus);
                            progress = 1.0f;
                            isPlayingBackwards = true;
                        }

                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.SINGLE:
                    {
                        progress += Time.deltaTime * speed;

                        if (progress >= 1.0f)
                        {
                            SwitchGOs(finalStatus);
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
