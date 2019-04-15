using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Core.Common;
using Game.Core.Util;
using PathologicalGames;

namespace Game.Core.Manager
{
    public enum PoolType
    {
        None = 0,
        Prefab = 1,
        Skill = 2,
    }

    public class PoolManager : Singleton<PoolManager>
    {
        public GameObject pool;
        public SpawnPool prefabPool;
        public SpawnPool skillPool;

        public PoolManager()
        {

        }

        public void Initialize()
        {
            if (pool == null)
            {
                pool = new GameObject("Pool");
                GameObject.DontDestroyOnLoad(pool);

                pool.SetActive(false);

                prefabPool = pool.AddComponent<SpawnPool>();
                prefabPool.poolName = "PrefabPool";

                skillPool = pool.AddComponent<SpawnPool>();
                skillPool.poolName = "SkillPool";

                pool.SetActive(true);
            }
        }

        /// <summary>
        /// 往对象池里面添加transform
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefab"></param>
        /// <param name="preloadAmount"></param>
        /// <param name="limitAmount"></param>
        /// <param name="cullAbove"></param>
        /// <param name="cullMaxPerPass"></param>
        public void AddPool(PoolType type, Transform prefab, int preloadAmount = 1, int limitAmount = 50, int cullAbove = 5, int cullMaxPerPass = 5)
        {
            SpawnPool tempPool = GetPool(type);
            if (tempPool != null)
            {
                if (tempPool.GetPrefabPool(prefab) == null)
                {
                    PrefabPool pool = new PrefabPool(prefab);
                    pool.preloadAmount = preloadAmount; // 预加载个数
                    pool.limitInstances = true; // 是否限制
                    pool.limitFIFO = true; // 关闭无限取prefab
                    pool.limitAmount = limitAmount; // 限制池子里最 大的Prefab数量
                    pool.cullDespawned = true; // 自动清除缓冲池
                    pool.cullAbove = cullAbove; // 最终保留
                    pool.cullDelay = 10; // 多久清理一次
                    pool.cullMaxPerPass = cullMaxPerPass; // 每次清理几个
                    tempPool.CreatePrefabPool(pool);
                }
            }
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public Transform Spawn(PoolType type, string prefabName)
        {
            SpawnPool tempPool = GetPool(type);
            if (tempPool != null)
            {
                return tempPool.Spawn(prefabName);
            }
            return null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefab"></param>
        public void Despawn(PoolType type, Transform prefab)
        {
            SpawnPool tempPool = GetPool(type);
            if (tempPool != null)
            {
                if (tempPool.IsSpawned(prefab))
                {
                    tempPool.Despawn(prefab, pool.transform);
                }
                else
                {
                    LogUtil.LogRed("资源不是对象池创建,无法释放--------->"+prefab.name);
                }
            }
        }

    
        /// <summary>
        /// 延迟释放资源
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefab"></param>
        /// <param name="seconds"></param>
        public void Despawn(PoolType type, Transform prefab, float seconds)
        {
            SpawnPool tempPool = GetPool(type);
            if (tempPool != null)
            {
                tempPool.Despawn(prefab, seconds, pool.transform);
            }
        }

        /// <summary>
        /// 释放所有资源
        /// </summary>
        /// <param name="type"></param>
        public void DespawnAll(PoolType type)
        {
            SpawnPool tempPool = GetPool(type);
            if (tempPool != null)
            {
                tempPool.DespawnAll();
            }
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        public void DestroyAll()
        {
            PathologicalGames.PoolManager.Pools.DestroyAll();
            pool = null;
            skillPool = null;
            prefabPool = null;
        }

        private SpawnPool GetPool(PoolType type)
        {
            switch (type)
            {
                case PoolType.Prefab:
                    return prefabPool;
                case PoolType.Skill:
                    return skillPool;
                default:
                    LogUtil.LogError("对象池类型错误--------------");
                    return null;
            }
        }

    }
}