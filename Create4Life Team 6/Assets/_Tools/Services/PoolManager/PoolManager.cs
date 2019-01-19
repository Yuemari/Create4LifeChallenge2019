using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pool manager. Class that controls every Pool
/// </summary>
public class PoolManager : BasePoolManager
{

    private Dictionary<string, PoolableObjectPool> poolsMap = new Dictionary<string, PoolableObjectPool>(); //Dictionary that contains every managed pool in the PoolManager

    #region ServiceImp
    public override bool IsServiceNull()
    {
        return false;
    }

    public override bool IsLoggedService()
    {
        return false;
    }

    public override void InitializeService()
    {
        base.InitializeService();
        _isLogged = ServiceLocator.GetLogStatusPerType(GetServiceType());
        InitPools();
    }
    #endregion

    /// <summary>
    /// Gets the <see cref="Pool"/> with the specified poolId.
    /// </summary>
    /// <param name="poolId">Pool identifier.</param>
    public override PoolableObjectPool this[string poolId]
    {
        get
        {
            PoolableObjectPool op;
            if (poolsMap.TryGetValue(poolId, out op))
            {
                return op;
            }
            else
            {
                Debug.LogWarningFormat(this, "A pool with the id [{0}] doesn't exist!", poolId);

                return null;
            }
        }
    }

    /// <summary>
    /// Gets the pool with the id provided.
    /// </summary>
    /// <returns>The pool.</returns>
    /// <param name="poolId">Pool identifier.</param>
    public override PoolableObjectPool GetPool(string poolId)
    {
       return this[poolId];
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
        PoolableObjectPool op = GetPool(poolId);
        if (op == null)
        {
            if(_isLogged)
            Debug.Log("As this pool doesnt exist we will create it.", this);
			
            op = new PoolableObjectPool(poolId, spawnable, preLoadedInstances, limitInstances, limitCounter);

            op.showDebugInfo = _isLogged;
			
            op.InitPool();
            return true;
        }
        else
        {
            if (_isLogged)
                Debug.LogWarningFormat(this,"A pool with the name [{0}] already exist!", poolId);
        }
        return false;
    }

		
	/// <summary>
	/// Spawns an instance of the specified poolId with the provided position, rotation and newParent.
	/// </summary>
	/// <param name="poolId">Pool identifier.</param>
	/// <param name="position">localPosition.</param>
	/// <param name="rotation">localRotation.</param>
	/// <param name="newParent">New parent.</param>
    public override PoolableObject Spawn(     string poolId,
                                            Vector3 position = default(Vector3),
                                            Quaternion rotation = default(Quaternion),
                                            Transform newParent = null)
    {
        if (!poolManagerInitialized)
        {
            if (_isLogged)
                Debug.LogWarning("The Pool Manager Is not ready yet!",this);

            return null;
        }
        
        PoolableObjectPool sp = GetPool(poolId);
        if (sp != null)
        {
            return sp.Spawn(position, rotation, newParent);
        }
        else
        {
            if (_isLogged)
                Debug.LogWarningFormat(this, "A pool with the name [{0}] doesn't exist!. Can't spawn from there.",poolId);
			
            return null;
        }
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
	public override R SpawnLike<R>( 	string poolId,
									Vector3 position = default(Vector3),
									Quaternion rotation = default(Quaternion),
									Transform newParent = null)
	{
		PoolableObject obj = Spawn(poolId,position,rotation,newParent);
		if(obj != null)
		{
			return (R)obj;
		}
		return null;
	}

    /// <summary>
    /// Adds the pool provided to the manager.
    /// </summary>
    /// <param name="objectPool">Object pool.</param>
    public override void AddPool(PoolableObjectPool objectPool)
    {
        if (objectPool != null)
        {
            if (!poolsMap.ContainsKey(objectPool.poolId))
            {
                poolsMap.Add(objectPool.poolId, objectPool);
                if (poolManagerInitialized)
                {
                    poolsList.Add(objectPool);
                    objectPool.InitPool();
                }
            }
            else
            {
                Debug.LogWarningFormat(this, "A pool with the name [{0}] already exist!", objectPool.poolId);
            }
        }
    }

    /// <summary>
    /// Destroys the pool with the id provided.
    /// </summary>
    /// <param name="poolId">Pool identifier.</param>
    public override void DestroyPool(string poolId)
    {
        PoolableObjectPool op = GetPool(poolId);
        if (op == null)
        {
            op.DestroyPool();
            poolsList.Remove(op);
            poolsMap.Remove(poolId);
        }
        else
        {
            Debug.LogWarningFormat(this, "A pool with the name [{0}] doesn't exist!", poolId);
        }
    }




    /// <summary>
    /// Inits the pools registered in this manager at its start. Only once.
    /// </summary>
    private void InitPools()
    {
		if(!poolManagerInitialized)
		{
            if (_isLogged)
            {
                Debug.Log("Initializing pools", this);
            }
            //poolsMap = new Dictionary<string, PoolableObjectPool>();
            for (int i = 0; i < poolsList.Count; i++)
	        {
	            poolsList[i].InitPool();
	        }
		}
        poolManagerInitialized = true;
    }

}
