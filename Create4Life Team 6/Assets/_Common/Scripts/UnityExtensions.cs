using UnityEngine;
using System;
using System.Text;
using System.Collections;
using UnityEngine.EventSystems;

//Version: 1.0.0

/// <summary>
/// Extensions for Unity classes, this class contains functions that work as extensions and help to encapsulate
/// common used functionalities.
/// </summary>
public static class UnityExtensions
{
	/// <summary>
	/// Gets the root game object.
	/// </summary>
	/// <returns>The root game object.</returns>
	/// <param name="gameObject">Game object.</param>
	public static GameObject GetRootGameObject(this GameObject gameObject)
	{
		Transform t = gameObject.transform.GetRootTransform();
		return t.gameObject;
	}

	/// <summary>
	/// Gets the root transform.
	/// </summary>
	/// <returns>The root transform.</returns>
	/// <param name="transform">Transform.</param>
	public static Transform GetRootTransform(this Transform transform)
	{
		Transform root = transform.parent;
		if(root == null)
		{
			return transform;
		}
		else
		{
			while(root.parent != null)
			{
				root = root.parent;
			}
			return root;
		}
	}

	/// <summary>
	/// Forces the GameObject to activate/deactivate recursively itself and all its children.
	/// </summary>
	/// <param name="gameObject">Game objectransform.</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public static void ForceActivateRecursively(this GameObject gameObject, bool enable)
	{
		gameObject.transform.ForceActivateRecursively(enable);
	}

	/// <summary>
	/// Forces the Transform to activate/deactivate recursively itself and all its children.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public static void ForceActivateRecursively(this Transform transform, bool enable)
	{
		transform.gameObject.SetActive(enable);
		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).ForceActivateRecursively(enable);
		}
	}

	/// <summary>
	/// Sets the position x.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">X coordinate New value.</param>
	public static void SetPositionX(this Transform transform,float newValue)
	{
		transform.position = new Vector3(newValue, transform.position.y, transform.position.z);
	}

	/// <summary>
	/// Sets the local position x.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">X coordinate New value.</param>
	public static void SetLocalPositionX(this Transform transform,float newValue)
	{
		transform.localPosition = new Vector3(newValue, transform.localPosition.y, transform.localPosition.z);
	}

	/// <summary>
	/// Sets the position y.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">Y coordinate New value.</param>
	public static void SetPositionY(this Transform transform,float newValue)
	{
		transform.position = new Vector3(transform.position.x, newValue, transform.position.z);
	}

	/// <summary>
	/// Sets the local position y.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">Y coordinate New value.</param>
	public static void SetLocalPositionY(this Transform transform,float newValue)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, newValue, transform.localPosition.z);
	}

	/// <summary>
	/// Sets the position z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">Z coordinate New value.</param>
	public static void SetPositionZ(this Transform transform,float newValue)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, newValue);
	}

	/// <summary>
	/// Sets the local position z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">Z coordinate New value.</param>
	public static void SetLocalPositionZ(this Transform transform,float newValue)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newValue);
	}

	/// <summary>
	/// Sets the position in X and Y.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	public static void SetPositionXY(this Transform transform,float newValueX, float newValueY)
	{
		transform.position = new Vector3(newValueX, newValueY, transform.position.z);
	}

	/// <summary>
	/// Sets the local position in X and Y.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	public static void SetLocalPositionXY(this Transform transform,float newValueX, float newValueY)
	{
		transform.localPosition = new Vector3(newValueX, newValueY, transform.localPosition.z);
	}

	/// <summary>
	/// Sets the position in X and Z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetPositionXZ(this Transform transform,float newValueX, float newValueZ)
	{
		transform.position = new Vector3(newValueX, transform.position.y, newValueZ);
	}

	/// <summary>
	/// Sets the local position in X and Z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalPositionXZ(this Transform transform,float newValueX, float newValueZ)
	{
		transform.localPosition = new Vector3(newValueX, transform.localPosition.y, newValueZ);
	}

	/// <summary>
	/// Sets the position in Y and Z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetPositionYZ(this Transform transform,float newValueY, float newValueZ)
	{
		transform.position = new Vector3(transform.position.x, newValueY, newValueZ);
	}

	/// <summary>
	/// Sets the local position in Y and Z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalPositionYZ(this Transform transform,float newValueY, float newValueZ)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, newValueY, newValueZ);
	}

	/// <summary>
	/// Sets the position in all axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetPosition(this Transform transform, float newValueX,float newValueY, float newValueZ)
	{
		transform.position = new Vector3(newValueX, newValueY, newValueZ);
	}

	/// <summary>
	/// Sets the local position in all axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalPosition(this Transform transform, float newValueX, float newValueY, float newValueZ)
	{
		transform.localPosition = new Vector3(newValueX, newValueY, newValueZ);
	}

	/// <summary>
	/// Sets the rotation for X axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetRotationX(this Transform transform,float newValue)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	newValue, 
													transform.rotation.eulerAngles.y, 
													transform.rotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the local rotation for X axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetLocalRotationX(this Transform transform,float newValue)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	newValue, 
														transform.localRotation.eulerAngles.y, 
														transform.localRotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the rotation for Y axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetRotationY(this Transform transform,float newValue)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	transform.rotation.eulerAngles.x, 
													newValue,
													transform.rotation.eulerAngles.z));
	}
		
	/// <summary>
	/// Sets the local rotation for Y axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetLocalRotationY(this Transform transform,float newValue)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	transform.localRotation.eulerAngles.x, 
														newValue,
														transform.localRotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the rotation for Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetRotationZ(this Transform transform,float newValue)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	transform.rotation.eulerAngles.x, 
													transform.rotation.eulerAngles.y,
													newValue));
	}

	/// <summary>
	/// Sets the local rotation for Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetLocalRotationZ(this Transform transform,float newValue)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	transform.localRotation.eulerAngles.x, 
														transform.localRotation.eulerAngles.y,
														newValue));
	}

	/// <summary>
	/// Sets the rotation for X and Y axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	public static void SetRotationXY(this Transform transform,float newValueX, float newValueY)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	newValueX, 
													newValueY, 
													transform.rotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the local rotation for X and Y axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	public static void SetLocalRotationXY(this Transform transform,float newValueX,float newValueY)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	newValueX, 
														newValueY, 
														transform.localRotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the rotation for X and Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetRotationXZ(this Transform transform,float newValueX, float newValueZ)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	newValueX, 
													transform.rotation.eulerAngles.y,
													newValueZ));
	}

	/// <summary>
	/// Sets the local rotation for X and Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalRotationXZ(this Transform transform,float newValueX,float newValueZ)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	newValueX, 
														transform.localRotation.eulerAngles.y,
														newValueZ));
	}

	/// <summary>
	/// Sets the rotation for Y and Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetRotationYZ(this Transform transform,float newValueY, float newValueZ)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	transform.rotation.eulerAngles.x,
													newValueY, 
													newValueZ));
	}

	/// <summary>
	/// Sets the local rotation for Y and Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalRotationYZ(this Transform transform,float newValueY,float newValueZ)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	transform.localRotation.eulerAngles.x,
														newValueY, 
														newValueZ));
	}

	/// <summary>
	/// Sets the rotation for all axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetRotation(this Transform transform,float newValueX, float newValueY, float newValueZ)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	newValueX, newValueY, newValueZ));
	}

	/// <summary>
	/// Sets the local rotation for all axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalRotation(this Transform transform, float newValueX,float newValueY,float newValueZ)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	newValueX, newValueY, newValueZ));
	}
		
	/// <summary>
	/// Reset the specified transform to 0's on Local position, rotation and scale of 1's.
	/// </summary>
	/// <param name="transform">Transform.</param>
	public static void Reset(this Transform transform)
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

    public static void AnchorsToCorners(this RectTransform t)
    {
        RectTransform pt = t.parent as RectTransform;

        if (t == null || pt == null) return;

        Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                            t.anchorMin.y + t.offsetMin.y / pt.rect.height);
        Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                            t.anchorMax.y + t.offsetMax.y / pt.rect.height);

        t.anchorMin = newAnchorsMin;
        t.anchorMax = newAnchorsMax;
        t.offsetMin = t.offsetMax = new Vector2(0, 0);
    }

    /// <summary>
    /// Resets to fill RectTransform parent.
    /// </summary>
    /// <param name="rTransform">R transform.</param>
    public static void  ResetToFillParent(this RectTransform rTransform)
	{
		if(rTransform.parent != null)
		{
			rTransform.anchorMin = Vector2.zero;
			rTransform.anchorMax = Vector2.one;
			rTransform.offsetMin = Vector2.zero;
			rTransform.offsetMax = Vector2.zero;
			rTransform.localScale = Vector3.one;
		}
	}

	/// <summary>
	/// Resets the velocity (also the angular velocity).
	/// </summary>
	/// <param name="rigidbody">Rigidbody.</param>
	public static void ResetVelocity(this Rigidbody rigidbody)
	{
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
	}

	/// <summary>
	/// Rect of the transform to screen space.
	/// </summary>
	/// <returns>The transform to screen space.</returns>
	/// <param name="transform">Transform.</param>
	public static Rect RectTransformToScreenSpace(this RectTransform transform )
	{
		Vector2 size = Vector2.Scale( transform.rect.size, transform.lossyScale );
		return new Rect( transform.position.x, Screen.height - transform.position.y, size.x, size.y );
	}

	/// <summary>
	/// Rect size of the transform in screen space.
	/// </summary>
	/// <returns>The transform size in screen space.</returns>
	/// <param name="transform">Transform.</param>
	/// <param name="cam">Cam.</param>
	public static Vector2 RectTransformSizeInScreenSpace(this RectTransform transform, Camera cam )
	{
		Vector3[] points = new Vector3[4];
		transform.GetWorldCorners(points);

		Vector2 dl =  RectTransformUtility.WorldToScreenPoint(cam,points[0]);
		Vector2 ur =  RectTransformUtility.WorldToScreenPoint(cam,points[2]);

		return new Vector2(ur.x-dl.x , ur.y-dl.y);
	}

	/// <summary>
	/// Size of the RectTransform in screen space from anchors.
	/// </summary>
	/// <returns>The transform size in screen space from anchors.</returns>
	/// <param name="transform">Transform.</param>
	public static Vector2 RectTransformSizeInScreenSpaceFromAnchors(this RectTransform transform )
	{
		Vector2 minAnchor = transform.GetRootMinAnchor();
		Vector2 maxAnchor = transform.GetRootMaxAnchor();

		//Debug.Log("MinA["+minAnchor.x+","+minAnchor.y+"] MaxA["+maxAnchor.x+","+maxAnchor.y+"]");

		Vector2 dl =  new Vector2(minAnchor.x*Screen.width,minAnchor.y*Screen.height);
		Vector2 ur =  new Vector2(maxAnchor.x*Screen.width,maxAnchor.y*Screen.height);

		return new Vector2(ur.x-dl.x , ur.y-dl.y);
	}

	/// <summary>
	/// Gets the root minimum anchor.
	/// </summary>
	/// <returns>The root minimum anchor.</returns>
	/// <param name="transform">Transform.</param>
	public static Vector2 GetRootMinAnchor(this RectTransform transform)
	{
		Vector2 anchor = transform.anchorMin;
		if(transform.parent != null)
		{
			if(transform.parent is RectTransform)
			{
				Vector2 parentAnchor = ((RectTransform)transform.parent).GetRootMinAnchor();
				//Debug.Log("Parent with MinA["+parentAnchor.x+","+parentAnchor.y+"]");
				if(parentAnchor.x != 0)
				{
					anchor.x *= parentAnchor.x;
				}
				if(parentAnchor.y != 0)
				{
					anchor.y *= parentAnchor.y;
				}
			}
		}
		return anchor;
	}

	/// <summary>
	/// Gets the root max anchor.
	/// </summary>
	/// <returns>The root max anchor.</returns>
	/// <param name="transform">Transform.</param>
	public static Vector2 GetRootMaxAnchor(this RectTransform transform)
	{
		Vector2 anchor = transform.anchorMax;
		if(transform.parent != null)
		{
			if(transform.parent is RectTransform)
			{
				Vector2 parentAnchor = ((RectTransform)transform.parent).GetRootMaxAnchor();
			//	Debug.Log("Parent with MaxA["+parentAnchor.x+","+parentAnchor.y+"]");
				if(parentAnchor.x != 0)
				{
					anchor.x *= parentAnchor.x;
				}
				if(parentAnchor.y != 0)
				{
					anchor.y *= parentAnchor.y;
				}
			}
		}
		return anchor;
	}

	/// <summary>
	/// Copies the rect transform.
	/// </summary>
	/// <param name="rTransform">R transform.</param>
	/// <param name="otherRT">Other R.</param>
	public static void  CopyRectTransform(this RectTransform rTransform, RectTransform otherRT)
	{
		rTransform.anchoredPosition = otherRT.anchoredPosition;
		rTransform.anchorMax = otherRT.anchorMax;
		rTransform.anchorMin = otherRT.anchorMin;
		rTransform.sizeDelta = otherRT.sizeDelta;
		rTransform.offsetMax = otherRT.offsetMax;
		rTransform.offsetMin = otherRT.offsetMin;
		rTransform.localScale = otherRT.localScale;
		rTransform.eulerAngles = otherRT.eulerAngles;

	}

	/// <summary>
	/// Sets the offsets to zero.
	/// </summary>
	/// <param name="rTransform">R transform.</param>
	public static void  SetOffsetsToZero(this RectTransform rTransform)
	{
		rTransform.offsetMin = Vector2.zero;
		rTransform.offsetMax = Vector2.zero;
	}

	/// <summary>
	/// Gets the number of digits before fixed point.
	/// </summary>
	/// <returns>The number of digits before fixed point.</returns>
	/// <param name="n">N.</param>
	public static int GetNumberOfDigitsBeforeFixedPoint(this float n)
	{
		return Mathf.FloorToInt(Mathf.Log10(n) + 1);
	}

	/// <summary>
	/// Gets the number of digits.
	/// </summary>
	/// <returns>The number of digits.</returns>
	/// <param name="n">N.</param>
	public static int GetNumberOfDigits(this int n)
	{
		if(n < 0)
		{
			n = -n;
		}
		// This bit could be optimised with a binary search
		return 	n < 10 ? 1
				: n < 100 ? 2
				: n < 1000 ? 3
				: n < 10000 ? 4
				: n < 100000 ? 5
				: n < 1000000 ? 6
				: n < 10000000 ? 7
				: n < 100000000 ? 8
				: n < 1000000000 ? 9
				: 10;
	}

	/// <summary>
	/// Gets the number of digits.
	/// </summary>
	/// <returns>The number of digits.</returns>
	/// <param name="n">N.</param>
	public static int GetNumberOfDigits(this uint n)
	{
		// This bit could be optimised with a binary search
		return 	n < 10 ? 1
				: n < 100 ? 2
				: n < 1000 ? 3
				: n < 10000 ? 4
				: n < 100000 ? 5
				: n < 1000000 ? 6
				: n < 10000000 ? 7
				: n < 100000000 ? 8
				: n < 1000000000 ? 9
				: 10;
	}

	/// <summary>
	/// Gets the number of digits.
	/// </summary>
	/// <returns>The number of digits.</returns>
	/// <param name="n">N.</param>
	public static int GetNumberOfDigits(this long n)
	{
		if(n < 0)
		{
			n = -n;
		}
		// This bit could be optimised with a binary search
		return 	n < 10 ? 1
				: n < 100 ? 2
				: n < 1000 ? 3
				: n < 10000 ? 4
				: n < 100000 ? 5
				: n < 1000000 ? 6
				: n < 10000000 ? 7
				: n < 100000000 ? 8
				: n < 1000000000 ? 9
				: n < 10000000000 ? 10
				: n < 100000000000 ? 11
				: n < 1000000000000 ? 12
				: n < 10000000000000 ? 13
				: n < 100000000000000 ? 14
				: n < 1000000000000000 ? 15
				: n < 10000000000000000 ? 16
				: n < 100000000000000000 ? 17
				: n < 1000000000000000000 ? 18
				: 19;
	}

	/// <summary>
	/// Gets the number of digits.
	/// </summary>
	/// <returns>The number of digits.</returns>
	/// <param name="n">N.</param>
	public static int GetNumberOfDigits(this ulong n)
	{
		// This bit could be optimised with a binary search
		return 	n < 10 ? 1
				: n < 100 ? 2
				: n < 1000 ? 3
				: n < 10000 ? 4
				: n < 100000 ? 5
				: n < 1000000 ? 6
				: n < 10000000 ? 7
				: n < 100000000 ? 8
				: n < 1000000000 ? 9
				: n < 10000000000 ? 10
				: n < 100000000000 ? 11
				: n < 1000000000000 ? 12
				: n < 10000000000000 ? 13
				: n < 100000000000000 ? 14
				: n < 1000000000000000 ? 15
				: n < 10000000000000000 ? 16
				: n < 100000000000000000 ? 17
				: n < 1000000000000000000 ? 18
				: n < 10000000000000000000 ? 19
				: 20;
	}

	/// <summary>
	/// Clears the stringBuilder provided.
	/// </summary>
	/// <param name="value">Value.</param>
	public static void Clear(this StringBuilder value)
	{
		value.Length = 0;
		value.Capacity = 0;
	}

	/// <summary>
	/// Gets the length, only works if is a playing clip.
	/// </summary>
	/// <returns><c>true</c>, if length if is playing clip was gotten, <c>false</c> otherwise.</returns>
	/// <param name="animator">Animator.</param>
	/// <param name="layerIndex">Layer index.</param>
	/// <param name="clipName">Clip name.</param>
	/// <param name="clipLength">Clip length.</param>
	public static bool GetLengthIfIsPlayingClip(this Animator animator,int layerIndex, string clipName,out float clipLength)
	{
		clipLength = 0;
		AnimatorClipInfo[] info = animator.GetCurrentAnimatorClipInfo(layerIndex);
		if(info != null)
		{
			for(int i = 0; i < info.Length; i++)
			{
				if(info[i].clip.name == clipName)
				{
					clipLength = info[i].clip.length;
					return true;
				}
			}
		}
		return false;
	}

    public static void ResetOffsets(this RectTransform rTransform)
    {
        rTransform.offsetMin = Vector2.zero;
        rTransform.offsetMax = Vector2.zero;
    }

    public static string GetTimeSpanToDHMSString(this TimeSpan span)
    {
        if (span.Ticks > 0)
        {
            DateTime date = new DateTime(span.Ticks);

            if (span.TotalDays >= 1)
            {
                return string.Format("{0}.{1}:{2}:{3}", span.Days, span.Hours, span.Minutes, span.Seconds);
            }
            else if (span.TotalHours >= 1)
            {
                return date.ToString("HH:mm:ss");
            }
            else
            {
                return date.ToString("mm:ss");
            }
        }
        else
        {
            return "00:00";
        }
    }

    public static string GetTimeSpanToDHMString(this TimeSpan span)
    {
        if (span.Ticks > 0)
        {
            DateTime date = new DateTime(span.Ticks);

            if (span.TotalDays >= 1)
            {
                return string.Format("{0}.{1}:{2}", span.Days, span.Hours, span.Minutes);
            }
            else if (span.TotalHours >= 1)
            {
                return date.ToString("HH:mm");
            }
            else
            {
                return date.ToString("mm");
            }
        }
        else
        {
            return "00:00";
        }
    }

    public static string GetTimeSpanToMinutesString(this TimeSpan span)
    {
        if (span.Ticks > 0)
        {
            int totalMinutes = span.Hours * 60 + span.Days * 1440 + span.Minutes;
            return totalMinutes + ":" + span.Seconds;
        }
        else
        {
            return "00:00";
        }
    }

    public static string GetTimeSpanToHoursString(this TimeSpan span)
    {
        if (span.Ticks > 0)
        {
            int totalHours = span.Days * 24 + span.Hours;
            return totalHours + ":" + span.Minutes;
        }
        else
        {
            return "00:00";
        }
    }

    public static string ToStringWithSeconds(this DateTime dt)
    {
        return dt.ToString("MM/dd/yyyy HH:mm:ss");
    }

    public static string ToStringWithHoursAndMinutes(this DateTime dt)
    {
        if (dt.Minute > 0)
        {
            return string.Format("{0} H {1}MIN", dt.Hour, dt.Minute);
        }
        else
        {
            return string.Format("{0} H", dt.Hour);
        }

    }

    public static bool EqualsDateTimeWithoutMilliSeconds(this DateTime dt, DateTime other)
    {
        return dt.Year == other.Year
            && dt.Month == other.Month
            && dt.Day == other.Day
            && dt.Hour == other.Hour
            && dt.Minute == other.Minute
            && dt.Second == other.Second;
    }

}
