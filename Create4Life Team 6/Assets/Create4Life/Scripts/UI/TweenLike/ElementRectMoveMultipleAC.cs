using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementRectMoveMultipleAC : AnimatedElementController
{
    public bool autoStartOnEnable = true;
    public bool setStartingAnchorAndPositionAtStart = true;
    public bool setTargetAnchorAndPositionAtStop = true;

    public Vector2 originMinAnchor = Vector3.zero;
    public Vector2 originMaxAnchor = Vector3.zero;

    public Vector2[] destinyMinAnchors;
    public Vector2[] destinyMaxAnchors;
    private Vector2 baseMinAnchor;
    private Vector2 baseMaxAnchor;
    private int currentDestinationIndex = 0;
    private Vector2 diffMinAnchor;
    private Vector2 diffMaxAnchor;

    public float duration = 1.0f;
    public float delay = 0.0f;
    private float speed = 1.0f;
    public AnimationCurve curve;
    private bool isPlayingBackwards = false;
    private bool isAnimationFinished = false;

    public bool setOriginAnchorsOnEnable = false;
    public bool setAnchorsFromOriginOnEnable = false;
    private RectTransform cachedRectTransform;

    public void SetStartAndEndPositions(Vector2 startMinAnchor, Vector2 startMaxAnchor, Vector2[] endMinAnchors, Vector2[] endMaxAnchors)
    {
        originMinAnchor = startMinAnchor;
        originMaxAnchor = startMaxAnchor;
        destinyMinAnchors = endMinAnchors;
        destinyMaxAnchors = endMaxAnchors;
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
        if (cachedRectTransform == null)
        {
            cachedRectTransform = GetComponent<RectTransform>();
        }

        if (setOriginAnchorsOnEnable)
        {
            originMinAnchor = cachedRectTransform.anchorMin;
            originMaxAnchor = cachedRectTransform.anchorMax;
        }
        if (setAnchorsFromOriginOnEnable)
        {
            cachedRectTransform.anchorMin = originMinAnchor;
            cachedRectTransform.anchorMax = originMaxAnchor;
            cachedRectTransform.ResetOffsets();
        }

        if (autoStartOnEnable)
        {
            SwitchAnimation(true);
        }
    }

    public override void OnDisable()
    {
        if (cachedRectTransform == null)
        {
            cachedRectTransform = GetComponent<RectTransform>();
        }

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
        baseMinAnchor = cachedRectTransform.anchorMin;
        baseMaxAnchor = cachedRectTransform.anchorMax;

        diffMinAnchor = destinyMinAnchors[currentDestinationIndex] - baseMinAnchor;
        diffMaxAnchor = destinyMaxAnchors[currentDestinationIndex] - baseMaxAnchor;
    }

    public override void ResetToStartingPoint()
    {
        cachedRectTransform.anchorMin = originMinAnchor;
        cachedRectTransform.anchorMax = originMaxAnchor;
        baseMinAnchor = originMinAnchor;
        baseMaxAnchor = originMaxAnchor;
        cachedRectTransform.ResetOffsets();
    }

    public override void ResetToEndingPoint()
    {
        cachedRectTransform.anchorMin = destinyMinAnchors[destinyMinAnchors.Length-1];
        cachedRectTransform.anchorMax = destinyMaxAnchors[destinyMinAnchors.Length - 1];
        baseMinAnchor = destinyMinAnchors[destinyMinAnchors.Length - 1];
        baseMaxAnchor = destinyMaxAnchors[destinyMinAnchors.Length - 1]; 
        cachedRectTransform.ResetOffsets();
    }

    public override void Play()
    {
        if (gameObject.activeInHierarchy)
        {
            speed = 1.0f / duration;
           
            isAnimationFinished = false;
            isPlayingBackwards = false;
            if (setStartingAnchorAndPositionAtStart)
            {
                cachedRectTransform.anchorMin = originMinAnchor;
                cachedRectTransform.anchorMax = originMaxAnchor;
                baseMinAnchor = originMinAnchor;
                baseMaxAnchor = originMaxAnchor;
                cachedRectTransform.ResetOffsets();
            }
            RecalculateDiff();
            StartCoroutine("Move");
        }
    }

    public override void Stop()
    {
        isAnimationFinished = true;
        if (setTargetAnchorAndPositionAtStop)
        {
            cachedRectTransform.anchorMin = destinyMinAnchors[currentDestinationIndex];
            cachedRectTransform.anchorMax = destinyMaxAnchors[currentDestinationIndex];
            cachedRectTransform.ResetOffsets();
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

                        Vector2 newMinAnchor = baseMinAnchor + diffMinAnchor * curve.Evaluate(progress);
                        Vector2 newMaxAnchor = baseMaxAnchor + diffMaxAnchor * curve.Evaluate(progress);

                        cachedRectTransform.anchorMin = newMinAnchor;
                        cachedRectTransform.anchorMax = newMaxAnchor;

                        if (progress >= 1.0f)
                        {
                            cachedRectTransform.anchorMin = destinyMinAnchors[currentDestinationIndex];
                            cachedRectTransform.anchorMax = destinyMaxAnchors[currentDestinationIndex];

                            progress = 0.0f;
                            currentDestinationIndex++;
                            if (currentDestinationIndex >= destinyMinAnchors.Length)
                            {
                                currentDestinationIndex = 0;
                            }
                            RecalculateDiff();
                        }
                        cachedRectTransform.ResetOffsets();
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.PINGPONG:
                    {
                        if (isPlayingBackwards)
                        {
                            progress -= Time.deltaTime * speed;

                            Vector2 newMinAnchor = baseMinAnchor + diffMinAnchor * curve.Evaluate(progress);
                            Vector2 newMaxAnchor = baseMaxAnchor + diffMaxAnchor * curve.Evaluate(progress);

                            cachedRectTransform.anchorMin = newMinAnchor;
                            cachedRectTransform.anchorMax = newMaxAnchor;

                            if (progress <= 0.0f)
                            {
                                cachedRectTransform.anchorMin = originMinAnchor;
                                cachedRectTransform.anchorMax = originMaxAnchor;

                                progress = 0.0f;
                                currentDestinationIndex--;
                                if (currentDestinationIndex == 0)
                                {
                                    isPlayingBackwards = false;
                                    currentDestinationIndex++;
                                }
                                RecalculateDiff();

                            }
                            cachedRectTransform.ResetOffsets();
                        }
                        else
                        {
                            progress += Time.deltaTime * speed;

                            Vector2 newMinAnchor = baseMinAnchor + diffMinAnchor * curve.Evaluate(progress);
                            Vector2 newMaxAnchor = baseMaxAnchor + diffMaxAnchor * curve.Evaluate(progress);

                            cachedRectTransform.anchorMin = newMinAnchor;
                            cachedRectTransform.anchorMax = newMaxAnchor;

                            if (progress >= 1.0f)
                            {
                                cachedRectTransform.anchorMin = destinyMinAnchors[currentDestinationIndex];
                                cachedRectTransform.anchorMax = destinyMaxAnchors[currentDestinationIndex];
                                progress = 1.0f;
                                currentDestinationIndex++;
                                if (currentDestinationIndex >= destinyMinAnchors.Length)
                                {
                                    isPlayingBackwards = true;
                                    currentDestinationIndex--;
                                }
                                RecalculateDiff();
                            }
                            cachedRectTransform.ResetOffsets();
                        }
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.PINGPONGONCE:
                    if (isPlayingBackwards)
                    {
                        progress -= Time.deltaTime * speed;

                        Vector2 newMinAnchor = baseMinAnchor + diffMinAnchor * curve.Evaluate(progress);
                        Vector2 newMaxAnchor = baseMaxAnchor + diffMaxAnchor * curve.Evaluate(progress);

                        cachedRectTransform.anchorMin = newMinAnchor;
                        cachedRectTransform.anchorMax = newMaxAnchor;

                        if (progress <= 0.0f)
                        {
                            cachedRectTransform.anchorMin = originMinAnchor;
                            cachedRectTransform.anchorMax = originMaxAnchor;

                            progress = 0.0f;
                            currentDestinationIndex--;
                            if (currentDestinationIndex == 0)
                            {
                                isPlayingBackwards = false;
                                isAnimationFinished = true;
                            }
                            RecalculateDiff();
                        }
                        cachedRectTransform.ResetOffsets();
                    }
                    else
                    {
                        progress += Time.deltaTime * speed;
                        Vector2 newMinAnchor = baseMinAnchor + diffMinAnchor * curve.Evaluate(progress);
                        Vector2 newMaxAnchor = baseMaxAnchor + diffMaxAnchor * curve.Evaluate(progress);

                        cachedRectTransform.anchorMin = newMinAnchor;
                        cachedRectTransform.anchorMax = newMaxAnchor;

                        if (progress >= 1.0f)
                        {
                            cachedRectTransform.anchorMin = destinyMinAnchors[currentDestinationIndex];
                            cachedRectTransform.anchorMax = destinyMaxAnchors[currentDestinationIndex];
                            progress = 1.0f;
                            currentDestinationIndex++;
                            if (currentDestinationIndex >= destinyMinAnchors.Length)
                            {
                                isPlayingBackwards = true;
                                currentDestinationIndex--;
                            }
                            RecalculateDiff();
                        }
                        cachedRectTransform.ResetOffsets();
                    }
                    break;
                case ELEMENT_ANIMATION_TYPE.SINGLE:
                    {
                        progress += Time.deltaTime * speed;

                        Vector2 newMinAnchor = originMinAnchor + diffMinAnchor * curve.Evaluate(progress);
                        Vector2 newMaxAnchor = originMaxAnchor + diffMaxAnchor * curve.Evaluate(progress);

                        cachedRectTransform.anchorMin = newMinAnchor;
                        cachedRectTransform.anchorMax = newMaxAnchor;

                        if (progress >= 1.0f)
                        {
                            cachedRectTransform.anchorMin = destinyMinAnchors[currentDestinationIndex];
                            cachedRectTransform.anchorMax = destinyMaxAnchors[currentDestinationIndex];
                            progress = 1.0f;
                            currentDestinationIndex++;
                            if (currentDestinationIndex >= destinyMinAnchors.Length)
                            {
                                isPlayingBackwards = true;
                                isAnimationFinished = true;
                            }
                            RecalculateDiff();
                        }
                        cachedRectTransform.ResetOffsets();
                    }
                    break;
            }
            yield return 0;
        }
        SwitchAnimation(false);
    }
}