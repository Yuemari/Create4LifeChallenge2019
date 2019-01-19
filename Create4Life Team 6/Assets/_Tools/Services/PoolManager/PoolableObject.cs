using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Poolable object class that must be inherited by any poolable thing in the game
/// </summary>
public abstract class PoolableObject : CachedMonoBehaviour
{
    public bool callOnSpawnWhenPreloadedByPool = false; //Should be true if on spawn should be called during the preloading of this object

    private string originPoolName = string.Empty; //String that cotains the pool Id that controls this object

	/// <summary>
	/// Gets or sets the origin pool of this object.
	/// </summary>
	/// <value>The origin object pool.</value>
	public string OriginObjectPool
	{
		get
		{
			return originPoolName;
		}
		set
		{
			originPoolName = value;
		}
	}

	/// <summary>
	/// Raises the spawn event. This must be overrided and reports to this class when the object has finished its spawn.
	/// </summary>
	/// <param name="isFirstTime">If set to <c>true</c> is first time.</param>
	abstract public void OnSpawn(bool isPreloading);

	/// <summary>
	/// Raises the despawn event. This must be overrided and reports to this class when the object has finished the despawning process
	/// </summary>
	abstract public void OnDespawn();
}
