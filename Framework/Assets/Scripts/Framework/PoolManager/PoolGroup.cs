namespace Frameworks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PoolGroup
    {
        public string GroupName { get; set; }

        private Dictionary<Type, AbstractObjectPool> typePools = new Dictionary<Type, AbstractObjectPool>();

        public PoolGroup(string groupName)
        {
            GroupName = groupName;
        }

        public void Destroy()
        {
            foreach (AbstractObjectPool pool in typePools.Values)
            {
                GameManager.Instance.StopAllCoroutines(pool.Name);
                pool.SelfDestruct();
            }

            typePools.Clear();
        }
        
        /// <summary>
        /// 添加一个对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectPool"></param>
        public void CreatePool<T>(ObjectPool<T> objectPool)
        {
            Type type = typeof(T);

            if (this.GetPool(type) == null)
            {
                objectPool.poolGroup = this;
                typePools.Add(type, objectPool);
            }

            objectPool.Initialize();
            if (!objectPool.preloaded)
                objectPool.PreloadInstances();
        }

        /// <summary>
        /// 添加一个实例到对应的Pool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="despawn">true为闲置状态;false为使用状态</param>
        public void Add<T>(T instance, bool despawn)
        {
            ObjectPool<T> pool = GetPool<T>();
            if (pool != null)
                pool.AddToPool(instance, despawn);
        }

        /// <summary>
        /// 获取对应类型的Pool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ObjectPool<T> GetPool<T>()
        {
            return (ObjectPool<T>)GetPool(typeof(T));
        }

        /// <summary>
        /// 获取对应类型的Pool
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AbstractObjectPool GetPool(Type type)
        {
            if (typePools.ContainsKey(type))
            {
                return typePools[type];
            }
            return null;
        }

        /// <summary>
        /// 获取一个对应类型的实例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public T Spawn<T>(params object[] args)
        {
            T inst;
            ObjectPool<T> pool = GetPool<T>();

            if (pool == null)
            {
                pool = new ObjectPool<T>();
                CreatePool<T>(pool);
            }

            inst = pool.SpawnInstance(args);
            if (inst == null) return default(T);

            pool.ItemOnSpawned(inst);
            return inst;
        }

        /// <summary>
        /// 将实例对象设置为闲置状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void Despawn<T>(T instance)
        {
            ObjectPool<T> pool = GetPool<T>();
            if (pool != null)
            {
                pool.DespawnInstance(instance);
            }
        }

        public void Despawn<T>(T instance, float seconds)
        {
            ObjectPool<T> pool = GetPool<T>();
            if (pool != null)
            {
                GameManager.Instance.StartCoroutine(pool.Name, this.DoDespawnAfterSeconds(instance, seconds));
            }
        }

        private IEnumerator DoDespawnAfterSeconds<T>(T instance, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Despawn<T>(instance);
        }
    }
}
