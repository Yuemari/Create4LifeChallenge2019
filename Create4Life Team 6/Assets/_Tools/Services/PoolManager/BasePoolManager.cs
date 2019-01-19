using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePoolManager : Service
{

    protected bool poolManagerInitialized = false; //Marks if the pools in this manager have been initialized
    public List<PoolableObjectPool> poolsList; //List of editor setted pools
   

    protected bool _isLogged;

    public virtual bool IsPoolManagerInitialized
    {
        get
        {
            return poolManagerInitialized;
        }
    }

    #region ServiceImp
    public override SERVICE_TYPE GetServiceType()
    {
        return SERVICE_TYPE.POOLMANAGER;
    }

    public override bool IsLoggedService()
    {
        return false;
    }

    public override bool IsServiceNull()
    {
        return true;
    }

    public override Service TransformService(bool isLogged)
    {
        _isLogged = isLogged;
        if (_isLogged)
        {
            LoggedPoolManager loggedService = gameObject.AddComponent<LoggedPoolManager>();
            Debug.Log("Transforming service to Logged Type");
            loggedService.SetService(this);
            
            return loggedService;
        }
        return this;
    }
    #endregion


    /// <summary>
    /// Gets the <see cref="Pool"/> with the specified poolId.
    /// </summary>
    /// <param name="poolId">Pool identifier.</param>
    public virtual PoolableObjectPool this[string poolId]
    {
        get
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the pool with the id provided.
    /// </summary>
    /// <returns>The pool.</returns>
    /// <param name="poolId">Pool identifier.</param>
    public virtual PoolableObjectPool GetPool(string poolId)
    {
        return null;
    }


    /// <summary>
    /// Adds the pool provided to the manager.
    /// </summary>
    /// <param name="objectPool">Object pool.</param>
    public abstract void AddPool(PoolableObjectPool objectPool);


    /// <summary>
    /// Creates the pool with the parameters provided.
    /// </summary>
    /// <returns><c>true</c>, if pool was created, <c>false</c> otherwise.</returns>
    /// <param name="poolId">Pool identifier.</param>
    /// <param name="spawnable">Spawnable.</param>
    /// <param name="preLoadedInstances">Pre loaded instances.</param>
    /// <param name="limitInstances">If set to <c>true</c> limit instances.</param>
    /// <param name="limitCounter">Limit counter.</param>
    public virtual bool CreatePool(string poolId, PoolableObject spawnable, int preLoadedInstances = 0, bool limitInstances = false, int limitCounter = 0)
    {
        return false;
    }


    /// <summary>
    /// Destroys the pool with the id provided.
    /// </summary>
    /// <param name="poolId">Pool identifier.</param>
    public virtual void DestroyPool(string poolId)
    {
        
    }

    /// <summary>
    /// Spawns an instance of the specified poolId with the provided position, rotation and newParent.
    /// </summary>
    /// <param name="poolId">Pool identifier.</param>
    /// <param name="position">localPosition.</param>
    /// <param name="rotation">localRotation.</param>
    /// <param name="newParent">New parent.</param>
    public virtual PoolableObject Spawn(string poolId,
                                            Vector3 position = default(Vector3),
                                            Quaternion rotation = default(Quaternion),
                                            Transform newParent = null)
    {
        return null;
    }

    /// <summary>
    /// Spawn an instance of the specified poolId with the provided position, rotation and newParent, casted to the typeof(R).
    /// </summary>
    /// <returns>The R instance.</returns>
    /// <param name="poolId">Pool identifier.</param>
    /// <param name="position">Position.</param>
    /// <param name="rotation">Rotation.</param>
    /// <param name="newParent">New parent.</param>
    /// <typeparam name="R">The type of the PoolableObject parameter.</typeparam>
    public virtual R SpawnLike<R>(string poolId,
                                    Vector3 position = default(Vector3),
                                    Quaternion rotation = default(Quaternion),
                                    Transform newParent = null) where R : PoolableObject
    {
        return null;
    }

    /// <summary>
    /// Despawns the specified objectToDespawn.
    /// </summary>
    /// <param name="objectToDespawn">Object to despawn.</param>
    public void Despawn(PoolableObject objectToDespawn)
    {
        if (objectToDespawn == null)
        {
            Debug.LogWarning("Despawning empty obj...");
            return;
        }
        if (!IsPoolManagerInitialized)
        {
            Debug.LogWarning("The Pool manager has not been initialized");
            return;
        }
        if (objectToDespawn != null)
        {
            PoolableObjectPool sp = GetPool(objectToDespawn.OriginObjectPool);
            if (sp != null)
            {
                sp.Despawn(objectToDespawn);
            }
            else
            {
                Debug.LogWarningFormat( this, "A pool with the name [{0}] doesn't exist!. Can't despawn from there.", objectToDespawn.OriginObjectPool);
            }
        }
    }

    /// <summary>
    /// Despawns all objects from pool.
    /// </summary>
    /// <param name="poolId">Pool identifier.</param>
    public void DespawnAllObjectsFromPool(string poolId)
    {
        PoolableObjectPool sp = GetPool(poolId);
        if (sp != null)
        {
            sp.DespawnAll();
        }
    }

}
