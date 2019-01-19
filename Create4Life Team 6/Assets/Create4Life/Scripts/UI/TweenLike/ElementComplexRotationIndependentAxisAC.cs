using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementComplexRotationIndependentAxisAC : AnimatedElementController
{
    public bool autoStartOnEnable = true;
    public bool setStartingRotationAtStart = true;
    public bool setEndingRotationAtStop = true;
    public bool setZeroRotationOnStop = false;
    public Vector3 startRotation = Vector3.zero;
    public Vector3 endRotation = new Vector3(360,360,360);
    public float duration = 1.0f;//this represents the time it takes to do a full revolution
    public float delay = 0.0f;
    private float speed = 1.0f;
    private bool isAnimationFinished = false;
    private bool isPlayingBackwards = false;
   

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

    public override void ResetToStartingPoint()
    {
        transform.localEulerAngles = startRotation;
    }

    public override void ResetToEndingPoint()
    {
        transform.localEulerAngles = startRotation;
    }

    public override void Play()
    {
        if (gameObject.activeInHierarchy)
        {
            speed = 1.0f/duration ;
            isAnimationFinished = false;
            isPlayingBackwards = false;
            if (setStartingRotationAtStart)
            {
                transform.localEulerAngles = startRotation;
            }
            StartCoroutine("Rotate");
        }
    }
    
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }


    public override void Stop()
    {
        isAnimationFinished = true;
        if (setEndingRotationAtStop)
        {
            transform.localEulerAngles = endRotation;
        }
        if(setZeroRotationOnStop)
        {
            transform.SetLocalRotation(0,0,0);
        }
        StopCoroutine("Rotate");
        StopCoroutine("MustWaitOtherAnimatedControllers");
    }

    IEnumerator Rotate()
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
                case ELEMENT_ANIMATION_TYPE.SINGLE:
                    {
                        progress += Time.deltaTime * speed;
                        Vector3 newRotation = Vector3.Lerp(startRotation, endRotation, progress);
                        //transform.SetLocalRotationZ(newRotation);
                        transform.localEulerAngles = newRotation;
                        if (progress >= 1.0f)
                        {
                            // transform.SetLocalRotationZ(endRotation);
                            transform.localEulerAngles = endRotation;
                            progress = 1.0f;
                            isAnimationFinished = true;
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.PINGPONG:
                    {
                        if (isPlayingBackwards)
                        {
                            progress -= Time.deltaTime * speed;
                            Vector3 newRotation = Vector3.Lerp(startRotation, endRotation, progress);
                            //transform.SetLocalRotationZ(newRotation);
                            transform.localEulerAngles = newRotation;
                            if (progress <= 0.0f)
                            {
                                //transform.SetLocalRotationZ(startRotation);
                                transform.localEulerAngles = startRotation;
                                progress = 0.0f;
                                isPlayingBackwards = false;
                            }
                        }
                        else
                        {
                            progress += Time.deltaTime * speed;
                            Vector3 newRotation = Vector3.Lerp(startRotation, endRotation, progress);
                            //transform.SetLocalRotationZ(newRotation);
                            transform.localEulerAngles = newRotation;
                            if (progress >= 1.0f)
                            {
                                //transform.SetLocalRotationZ(endRotation);
                                transform.localEulerAngles = endRotation;
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
                        Vector3 newRotation = Vector3.Lerp(startRotation, endRotation, progress);
                        //transform.SetLocalRotationZ(newRotation);
                        transform.localEulerAngles = newRotation;
                        if (progress <= 0.0f)
                        {
                            //transform.SetLocalRotationZ(startRotation);
                            transform.localEulerAngles = startRotation;
                            progress = 0.0f;
                            isPlayingBackwards = false;
                            isAnimationFinished = true;
                        }
                    }
                    else
                    {
                        progress += Time.deltaTime * speed;
                        Vector3 newRotation = Vector3.Lerp(startRotation, endRotation, progress);
                        //transform.SetLocalRotationZ(newRotation);
                        transform.localEulerAngles = newRotation;
                        if (progress >= 1.0f)
                        {
                            //transform.SetLocalRotationZ(endRotation);
                            transform.localEulerAngles = endRotation;
                            progress = 1.0f;
                            isPlayingBackwards = true;
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.LOOP:
                    {
                        progress += Time.deltaTime * speed;
                        Vector3 newRotation = Vector3.Lerp(startRotation, endRotation, progress);
                        //transform.SetLocalRotationZ(newRotation);
                        transform.localEulerAngles = newRotation;
                        if (progress > 1.0f)
                        {
                            progress = 0.0f;
                            //transform.SetLocalRotationZ(startRotation);
                            transform.localEulerAngles = startRotation;

                        }
                    }
                    break;
            }
            yield return 0;
        }
        SwitchAnimation(false);
    }
}