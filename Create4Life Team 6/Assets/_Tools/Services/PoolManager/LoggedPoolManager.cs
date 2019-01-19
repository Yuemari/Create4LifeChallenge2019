using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggedPoolManager : BasePoolManager
{
    private BasePoolManager wrappedService;
    #region ServiceImp
    public override bool IsServiceNull()
    {
        if (wrappedService != null)
        {
            return wrappedService.IsServiceNull();
        }
        return true;
    }

    public override bool IsLoggedService()
    {
        return true;
    }

    public override bool IsPoolManagerInitialized
    {
        get
        {
            if(wrappedService != null)
            {
                return wrappedService.IsPoolManagerInitialized;
            }
            return poolManagerInitialized;
        }
    }

    public void SetService(Service serviceToWrap)
    {
        wrappedService = (BasePoolManager)serviceToWrap;
        poolManagerInitialized = wrappedService.IsPoolManagerInitialized;
    }

    public override Service TransformService(bool isLogged)
    {
        if (!isLogged)
        {
            Destroy(this);
            return wrappedService;
        }
        return this;
    }

    public override void InitializeService()
    {
        if (wrappedService != null)
        {
            Debug.Log("Initializing PoolManager Service.");
            wrappedService.InitializeService();
            
        }
    }
    #endregion

    /// <summary>
    /// Gets the pool with the id provided.
    /// </summary>
    /// <returns>The pool.</returns>
    /// <param name="poolId">Pool identifier.</param>
    public override PoolableObjectPool GetPool(string poolId)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Getting Pool: {0}", poolId);
            return wrappedService[poolId];
        }
        return null;
    }

    /// <summary>
    /// Creates the pool with the parameters provided.
    /// </summary>
    /// <returns><c>true</c>, if pool was created, <c>false</c> otherwise.</returns>
    /// <param name="poolId">Pool identifier.</param>
    /// <param name="spawnable">Spawnable.</param>
    /// <param name="preLoadedInstances">Pre loaded instances.</param>
    /// <param name="limitInstances">If set to <c>true</c> limit instances.</param>
    /// <param name="limitCounter">Limit counter.</param>
    public override bool CreatePool(string poolId, PoolableObject spawnable, int preLoadedInstances = 0, bool limitInstances = false, int limitCounter = 0)
    {
        if (wrappedService != null)
        {
            Debug.LogFormat("Creating Pool: {0} with Spwn:{1}, preload:{2} Limit:{3} to {4} instances", poolId,spawnable.name, preLoadedInstances, limitInstances, limitCounter);
            return wrappedService.CreatePool(poolId, spawnable, preLoadedInstances, limitInstances, limitCounter);
        }
        return false;
    }

    public override void AddPool(PoolableObjectPool objectPool)
    {
        if (wrappedService != null && objectPool != null)
        {
            Debug.LogFormat("Adding Pool: {0} with Spwn:{1}, preload:{2} Limit:{3} to {4} instances", objectPool.poolId, objectPool.poolableObjectPrefab.name, objectPool.numberOfPreloadedObjects,
              objectPool.limitInstances, 
              objectPool.spawnLimit);
            wrappedService.AddPool(objectPool);
        }
    }

    /// <summary>
    /// Spawns an instance of the specified poolId with the provided position, rotation and newParent.
    /// </summary>
    /// <param name="poolId">Pool identifier.</param>
    /// <param name="position">localPosition.</param>
    /// <param name="rotation">localRotation.</param>
    /// <param name="newParent">New parent.</param>
    public override PoolableObject Spawn(string poolId,
                                            Vector3 position = default(Vector3),
                                            Quaternion rotation = default(Quaternion),
                                            Transform newParent = null)
    {
        if (wrappedService != null)
        {
            //Debug.LogFormat("Spawning from Pool: {0} on Position:{1} Rotation:{2} NewParent:{3}", poolId, position, rotation, newParent);
            return wrappedService.Spawn(poolId, position, rotation, newParent);
        }
        return null;
    }

   

}
