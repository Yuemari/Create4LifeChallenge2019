using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Poolable Object pool. This class manages a single pool of objects, a pool of objects must be used for every poolable object. Is a 1:1 relation between poolable objects and pools.
/// </summary>
[System.Serializable]
public class PoolableObjectPool
{ 
	private static string appendedNameStart = "_(Instance_0"; //Controls the naming start of the instances spawned.
	private static string appendedNameEnd = ")"; //Controls the naming end of the instances spawned.

	public string poolId; //Pool Unique Id
	public PoolableObject poolableObjectPrefab; //Reference to the object this pool will control
	public int numberOfPreloadedObjects = 0; //Number of instances to preload of the controlled object
	public bool limitInstances = false; //True if the instancing of the objects in the pool should be limited by number of instances
	public int spawnLimit = 100; //Instancing limit for this controlled object
	public bool showDebugInfo = false; //True to activate console printing capabilities

    private List<PoolableObject> allPoolableObjectsList; //List containing a reference to every PoolableObject controlled by this pool.
	private List<PoolableObject> spawnedPoolableObjectsList; //List containing a reference to every SPAWNED PoolableObject controlled by this pool.
	private List<PoolableObject> despawnedPoolableObjectsList; //List containing a reference to every DESPAWNED PoolableObject controlled by this pool.

	/// <summary>
	/// Inits this pool.
	/// </summary>
	public void InitPool()
	{
		if (showDebugInfo)
		{
			Debug.Log("Init ObjectPool[" + poolId + "]");
		}
		allPoolableObjectsList = new List<PoolableObject>();
		spawnedPoolableObjectsList = new List<PoolableObject>();
		despawnedPoolableObjectsList = new List<PoolableObject>();

		RegisterPool();
		PreloadSpawns();
	}

	/// <summary>
	/// Registers the pool in the PoolManager.
	/// </summary>
	public void RegisterPool()
	{
		if (showDebugInfo)
		{
			Debug.Log("Registering ObjectPool[" + poolId + "]");
		}
        ServiceLocator.Instance.GetServiceOfType<BasePoolManager>(SERVICE_TYPE.POOLMANAGER).AddPool(this);
	}

	/// <summary>
	/// Preloads the instances of this pool.
	/// </summary>
	public void PreloadSpawns()
	{
		if (showDebugInfo)
		{
			Debug.Log("Preload ObjectPool[" + poolId + "]");
		}

		if (limitInstances)
		{
			numberOfPreloadedObjects = (numberOfPreloadedObjects > spawnLimit ? spawnLimit : numberOfPreloadedObjects);
		}


		for (int i = 0; i < numberOfPreloadedObjects; i++)
		{
			GameObject obj = GameObject.Instantiate(poolableObjectPrefab.CachedGameObject,
				Vector3.zero, Quaternion.identity) as GameObject;
			if (obj != null)
			{
				PoolableObject spwnObj = obj.GetComponent<PoolableObject>();

				if (spwnObj != null)
				{
					obj.name += appendedNameStart + allPoolableObjectsList.Count + appendedNameEnd;
					//set pool name on this object
					spwnObj.OriginObjectPool = poolId;
					//add to the main list
					allPoolableObjectsList.Add(spwnObj);
					if (spwnObj.callOnSpawnWhenPreloadedByPool)
					{
						spwnObj.OnSpawn(true);
					}
					Despawn(spwnObj);
				}
			}
		}
	}

	/// <summary>
	/// Spawn the PoolableObject in the localposition, localrotation and newParent provided.
	/// </summary>
	/// <param name="position">localPosition.</param>
	/// <param name="rotation">localRotation.</param>
	/// <param name="newParent">Object New parent.</param>
	public PoolableObject Spawn(Vector3 position = default(Vector3),
		Quaternion rotation = default(Quaternion),
		Transform newParent = null)
	{
		PoolableObject spawnObj = null;
		if (despawnedPoolableObjectsList.Count > 0)
		{
			spawnObj = despawnedPoolableObjectsList[0];
			despawnedPoolableObjectsList.RemoveAt(0);
		}
		else if (!limitInstances || allPoolableObjectsList.Count < spawnLimit)
		{
			GameObject obj = GameObject.Instantiate(poolableObjectPrefab.CachedGameObject,
				Vector3.zero, Quaternion.identity) as GameObject;
			if (obj != null)
			{
				spawnObj = obj.GetComponent<PoolableObject>();
				if (spawnObj != null)
				{
					obj.name += appendedNameStart + allPoolableObjectsList.Count + appendedNameEnd;
					//set pool name on this object
					spawnObj.OriginObjectPool = poolId;
					//add to the main list
					allPoolableObjectsList.Add(spawnObj);
				}
			}
		}
		else
		{
			if (showDebugInfo) 
			{
				Debug.Log ("Can't Spawn [" + poolableObjectPrefab.CachedGameObject.name + "] from pool [" + poolId + "]");
			}
			return null;
		}
		if (showDebugInfo) 
		{
			Debug.Log ("Spawning [" + spawnObj.CachedGameObject.name + "] from pool [" + spawnObj.OriginObjectPool + "]");
		}
		if(spawnedPoolableObjectsList.Contains(spawnObj))
		{
			if(showDebugInfo)
			{
				Debug.Log("Already Spawned");
			}
		}
		else
		{
			spawnedPoolableObjectsList.Add(spawnObj);
		}

		spawnObj.CachedTransform.SetParent(newParent);
		spawnObj.CachedTransform.localPosition = position;
		spawnObj.CachedTransform.localRotation = rotation;
		spawnObj.OnSpawn(false);
		return spawnObj;
	}

	/// <summary>
	/// Despawn the specified PoolableObject.
	/// </summary>
	/// <param name="objectToDespawn">Object to despawn.</param>
	public void Despawn(PoolableObject objectToDespawn)
	{
		if (objectToDespawn != null)
		{
			if(objectToDespawn.OriginObjectPool == poolId)
			{
				if (showDebugInfo) 
				{
					Debug.Log ("Despawning [" + objectToDespawn.CachedGameObject.name + "] from pool [" + objectToDespawn.OriginObjectPool);
				}
				objectToDespawn.OnDespawn();
				objectToDespawn.CachedTransform.SetParent(ServiceLocator.Instance.GetServiceOfType<BasePoolManager>(SERVICE_TYPE.POOLMANAGER).transform);
				spawnedPoolableObjectsList.Remove(objectToDespawn);
				if(!despawnedPoolableObjectsList.Contains(objectToDespawn))
				{
					despawnedPoolableObjectsList.Add(objectToDespawn);
				}
				else
				{
					if(showDebugInfo)
					{
						Debug.Log("Already Despawned");
					}
				}
			}
		}
	}

	/// <summary>
	/// Despawns all the spawned instances of this pool
	/// </summary>
	public void DespawnAll()
	{
		if (showDebugInfo) 
		{
			Debug.Log ("Despawn ALL from [" + poolId + "]");
		}

		while (spawnedPoolableObjectsList.Count > 0)
		{
			Despawn(spawnedPoolableObjectsList[0]);
		}
	}


	/// <summary>
	/// Initializes a new instance of the <see cref="ObjectPool"/> class.
	/// </summary>
	/// <param name="poolUniqueId">Pool Id.</param>
	/// <param name="prefab">Prefab controlled by this pool.</param>
	/// <param name="preLoadedInstances">Instances to preload.</param>
	/// <param name="mustLimitInstances">If set to <c>true</c> must limit instances.</param>
	/// <param name="limitCounter">Limit counter.</param>
	public PoolableObjectPool(string poolUniqueId,PoolableObject prefab,int preLoadedInstances,bool mustLimitInstances,int limitCounter)
	{
		poolId = poolUniqueId;
		poolableObjectPrefab = prefab;
		limitInstances = mustLimitInstances;
		numberOfPreloadedObjects = preLoadedInstances;
		spawnLimit = limitCounter;
	}

	/// <summary>
	/// Destroys the pool after despawning all its PoolableObjects.
	/// </summary>
	public void DestroyPool()
	{
		DespawnAll();

		despawnedPoolableObjectsList.Clear();
		spawnedPoolableObjectsList.Clear();

		if (showDebugInfo) 
		{
			Debug.Log ("Destroying Pool contents from [" + poolId + "]");
		}
		while(allPoolableObjectsList.Count > 0)
		{
			PoolableObject obj = allPoolableObjectsList[0];
			allPoolableObjectsList.RemoveAt(0);
			MonoBehaviour.Destroy(obj);
		}
		allPoolableObjectsList.Clear();
	}

}


