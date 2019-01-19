using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementMoveMultipleAC : AnimatedElementController
{
    public bool useLocalPosition = true;
    public bool autoStartOnEnable = true;
    public bool setStartingPositionAtStart = true;
    public bool setTargetPositionAtStop = true;
    private int currentDestinationIndex = 0;
    public Vector3 originPosition = Vector2.zero;
    public Vector3[] destinyPositions;
    private Vector3 basePosition;
    private Vector3 diff;
    public float duration = 1.0f;
    public float delay = 0.0f;
    private float speed = 1.0f;
    public AnimationCurve curve;
    private bool isPlayingBackwards = false;
    private bool isAnimationFinished = false;

    public bool setOriginPositionOnEnable = false;

    public void SetStartAndEndPositions(Vector3 start, Vector3[] destinations)
    {
        originPosition = start;
        destinyPositions = destinations;
    }

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
        if (setOriginPositionOnEnable)
        {
            if(useLocalPosition)
            {
                originPosition = transform.localPosition;
            }
            else
            { 
                originPosition = transform.position;
            }
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

    private void RecalculateDiff()
    {
        diff = destinyPositions[currentDestinationIndex] - basePosition;
    }

    public override void ResetToStartingPoint()
    {
        SetPosition(originPosition);

        UpdateBasePosition();
    }

    public override void ResetToEndingPoint()
    {
        SetPosition(destinyPositions[destinyPositions.Length - 1]);
        UpdateBasePosition();
    }

    public override void Play()
    {
        if (gameObject.activeInHierarchy)
        {
            currentDestinationIndex = 0;
            speed = 1.0f / duration;
            if (setStartingPositionAtStart)
            {
                SetPosition(originPosition);
                basePosition = originPosition;
            }

            RecalculateDiff();
            isAnimationFinished = false;
            isPlayingBackwards = false;
            
            StartCoroutine("Move");
        }
    }

    public override void Stop()
    {
        isAnimationFinished = true;
        if (setTargetPositionAtStop)
        {
            SetPosition(destinyPositions[currentDestinationIndex]);
        }
        StopCoroutine("Move");
        StopCoroutine("MustWaitOtherAnimatedControllers");
    }

    private void UpdateBasePosition()
    {
        if (useLocalPosition)
        {
            basePosition = transform.localPosition;
        }
        else
        {
            basePosition = transform.position;
        }
        
    }

    private void SetPosition(Vector3 newPos)
    {
        if (useLocalPosition)
        {
            transform.localPosition = newPos;
        }
        else
        {
            transform.position = newPos;
        }
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

                        Vector3 newPosition = basePosition + diff * curve.Evaluate(progress);
                        SetPosition(newPosition);

                        if (progress >= 1.0f)
                        {
                            SetPosition(destinyPositions[currentDestinationIndex]);
                            UpdateBasePosition();
                            progress = 0.0f;
                            currentDestinationIndex++;
                            if(currentDestinationIndex >= destinyPositions.Length)
                            {
                                currentDestinationIndex = 0;
                            }
                            RecalculateDiff();
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.PINGPONG:
                    {
                        if (isPlayingBackwards)
                        {
                            progress -= Time.deltaTime * speed;
                            Vector3 newPosition = basePosition + diff * curve.Evaluate(progress);
                            SetPosition(newPosition);

                            if (progress <= 0.0f)
                            {
                                SetPosition(originPosition);
                                UpdateBasePosition();
                                progress = 0.0f;
                                currentDestinationIndex--;
                                if (currentDestinationIndex == 0)
                                {
                                    isPlayingBackwards = false;
                                    currentDestinationIndex++;
                                }
                                RecalculateDiff();
                                
                            }
                        }
                        else
                        {
                            progress += Time.deltaTime * speed;
                            Vector3 newPosition = basePosition + diff * curve.Evaluate(progress);
                            SetPosition(newPosition);

                            if (progress >= 1.0f)
                            {
                                transform.position = destinyPositions[currentDestinationIndex];
                                UpdateBasePosition();
                                progress = 1.0f;
                                currentDestinationIndex++;
                                if (currentDestinationIndex >= destinyPositions.Length)
                                {
                                    isPlayingBackwards = true;
                                    currentDestinationIndex--;
                                }
                                RecalculateDiff();
                            }
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.PINGPONGONCE:
                    if (isPlayingBackwards)
                    {
                        progress -= Time.deltaTime * speed;
                        Vector3 newPosition = basePosition + diff * curve.Evaluate(progress);
                        SetPosition(newPosition);

                        if (progress <= 0.0f)
                        {
                            SetPosition(originPosition);
                            UpdateBasePosition();
                            progress = 0.0f;
                            currentDestinationIndex--;
                            if (currentDestinationIndex == 0)
                            {
                                isPlayingBackwards = false;
                                isAnimationFinished = true;
                            }
                            RecalculateDiff();

                        }
                    }
                    else
                    {
                        progress += Time.deltaTime * speed;
                        Vector3 newPosition = basePosition + diff * curve.Evaluate(progress);
                        SetPosition(newPosition);

                        if (progress >= 1.0f)
                        {
                            SetPosition(destinyPositions[currentDestinationIndex]);
                            UpdateBasePosition();
                            progress = 1.0f;
                            currentDestinationIndex++;
                            if (currentDestinationIndex >= destinyPositions.Length)
                            {
                                isPlayingBackwards = true;
                                currentDestinationIndex--;
                            }
                            RecalculateDiff();
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.SINGLE:
                    {
                        progress += Time.deltaTime * speed;
                        Vector3 newPosition = basePosition + diff * curve.Evaluate(progress);
                        SetPosition(newPosition);

                        if (progress >= 1.0f)
                        {
                            SetPosition(destinyPositions[currentDestinationIndex]);
                            UpdateBasePosition();
                            progress = 0.0f;
                            currentDestinationIndex++;
                            if (currentDestinationIndex >= destinyPositions.Length)
                            {
                                isPlayingBackwards = true;
                                isAnimationFinished = true;
                                currentDestinationIndex = destinyPositions.Length - 1;
                            }
                            RecalculateDiff();
                            
                        }
                    }
                    break;
            }
            yield return 0;
        }
        SwitchAnimation(false);
    }
}
