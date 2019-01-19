using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cached mono behaviour. Inherit from this class to obtain basci cached components
/// </summary>
public class CachedMonoBehaviour : MonoBehaviour 
{
	private GameObject	cachedGameObject; //cached GameObject if any.
	private Transform	cachedTransform; //cached Transform if any.
	private RectTransform	cachedRectTransform; //cached RectTransform if any.
	private Rigidbody	cachedRigidbody; //cached Rigidbody if any.
	private Rigidbody2D	cachedRigidbody2D; //cached Rigidbody2D if any.
	private Collider	cachedCollider; //cached Collider if any.
	private Collider2D	cachedCollider2D; //cached Collider2D if any.
	private	Renderer	cachedRenderer; //cached Renderer if any.
	private Material	cachedMaterial; //cached Material if any.
	private Dictionary<System.Type,Component>	cachedExtraComponents = new Dictionary<System.Type,Component>(); //extra cached components map by component type.

	/// <summary>
	/// Gets the cached game object.
	/// </summary>
	/// <value>The cached game object.</value>
	public GameObject CachedGameObject
	{
		get
		{
			if(cachedGameObject == null)
			{
				cachedGameObject = gameObject;
			}
			return cachedGameObject;
		}
	}

	/// <summary>
	/// Gets the cached transform.
	/// </summary>
	/// <value>The cached transform.</value>
	public Transform CachedTransform
	{
		get
		{
			if(cachedTransform == null)
			{
				cachedTransform = transform;
			}
			return cachedTransform;
		}
	}

	/// <summary>
	/// Gets the cached rect transform.
	/// </summary>
	/// <value>The cached rect transform.</value>
	public RectTransform CachedRectTransform
	{
		get
		{
			if(cachedRectTransform == null)
			{
				cachedRectTransform = GetComponent<RectTransform>();
			}
			return cachedRectTransform;
		}
	}

	/// <summary>
	/// Gets the cached rigidbody.
	/// </summary>
	/// <value>The cached rigidbody.</value>
	public Rigidbody CachedRigidbody
	{
		get
		{
			if(cachedRigidbody == null)
			{
				cachedRigidbody = GetComponent<Rigidbody>();
			}
			return cachedRigidbody;
		}
	}

	/// <summary>
	/// Gets the cached rigidbody if there is none it add it.
	/// </summary>
	/// <returns>The get rigidbody.</returns>
	public Rigidbody ForceGetRigidbody()
	{
		if(CachedRigidbody == null)
		{
			cachedRigidbody = CachedGameObject.AddComponent<Rigidbody>();
		}
		return cachedRigidbody;
	}

	/// <summary>
	/// Gets the cached rigidbody2 d.
	/// </summary>
	/// <value>The cached rigidbody2 d.</value>
	public Rigidbody2D CachedRigidbody2D
	{
		get
		{
			if(cachedRigidbody2D == null)
			{
				cachedRigidbody2D = GetComponent<Rigidbody2D>();
			}
			return cachedRigidbody2D;
		}
	}

	/// <summary>
	/// Gets the cached rigidbody2D if there is none it add it.
	/// </summary>
	/// <returns>The get rigidbody2 d.</returns>
	public Rigidbody2D ForceGetRigidbody2D()
	{
		if(CachedRigidbody2D == null)
		{
			cachedRigidbody2D = CachedGameObject.AddComponent<Rigidbody2D>();
		}
		return cachedRigidbody2D;
	}

	/// <summary>
	/// Gets the cached collider.
	/// </summary>
	/// <value>The cached collider.</value>
	public Collider CachedCollider
	{
		get
		{
			if(cachedCollider == null)
			{
				cachedCollider = GetComponent<Collider>();
			}
			return cachedCollider;
		}
	}

	/// <summary>
	/// Gets the cached collider2D.
	/// </summary>
	/// <value>The cached collider2 d.</value>
	public Collider2D CachedCollider2D
	{
		get
		{
			if(cachedCollider2D == null)
			{
				cachedCollider2D = GetComponent<Collider2D>();
			}
			return cachedCollider2D;
		}
	}

	/// <summary>
	/// Gets the cached renderer.
	/// </summary>
	/// <value>The cached renderer.</value>
	public Renderer CachedRenderer
	{
		get
		{
			if(cachedRenderer == null)
			{
				cachedRenderer = GetComponent<Renderer>();
			}
			return cachedRenderer;
		}
	}

	/// <summary>
	/// Gets the cached T component if there is none it add it.
	/// </summary>
	/// <returns>The get.</returns>
	/// <typeparam name="T">The component type parameter.</typeparam>
	public T ForceGet<T>() where T : Component
	{
		Component component;	

		if(cachedExtraComponents.TryGetValue(typeof(T),out component))
		{
			return (T)component;
		}
		else
		{
			component = CachedGameObject.GetComponent<T>();
			if(component == null)
			{
				component = CachedGameObject.AddComponent<T>();
			}
			cachedExtraComponents.Add(typeof(T),component);
			return (T)component;
		}
	}
		
	/// <summary>
	/// Gets the cached material. This method will create one copy of the material.
	/// </summary>
	/// <value>The cached material.</value>
	public Material CachedMaterial
	{
		get
		{
			if(CachedRenderer != null)
			{
				if(cachedMaterial == null)
				{
					cachedMaterial = Instantiate (CachedRenderer.sharedMaterial) as Material;
					CachedRenderer.sharedMaterial = cachedMaterial;
				}
				return cachedMaterial;
			}
			return null;
		}
	}

	/// <summary>
	/// Gets the cached shared material.
	/// </summary>
	/// <value>The cached shared material.</value>
	public Material CachedSharedMaterial
	{
		get
		{
			if(CachedRenderer != null)
			{
				return CachedRenderer.sharedMaterial;
			}
			return null;
		}
	}

	/// <summary>
	/// Prints the extra cached components.
	/// </summary>
	protected void PrintExtraCachedComponents()
	{
		foreach(KeyValuePair<System.Type,Component> pair in cachedExtraComponents)
		{
			Debug.Log("CachedExtraComponent of type ["+pair.Key.ToString()+"]");
		}
	}
}
