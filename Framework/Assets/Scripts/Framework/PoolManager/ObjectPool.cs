namespace Frameworks
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ObjectPool<T> : AbstractObjectPool
    {
        public List<T> spawned      { get; protected set; } // 正在被使用的对象列表
        public List<T> despawned    { get; protected set; } // 闲置对象列表

        public int totalCount { get { return spawned.Count + despawned.Count; } }

        public ObjectPool()
        {
            Name = "[ObjectPool]" + typeof(T);
            preloaded = false;
        }

        public override void Initialize()
        {
            spawned = new List<T>();
            despawned = new List<T>();
            preloaded = false;
        }

        public override void SelfDestruct()
        {
            for (int i = despawned.Count - 1; i >= 0; i--)
                ItemDestruct(despawned[i]);

            for (int i = spawned.Count - 1; i >= 0; i--)
                ItemDestruct(spawned[i]);

            spawned.Clear();
            despawned.Clear();
            preloaded = false;
        }

        /// <summary>
        /// 设置对象为闲置
        /// </summary>
        public bool DespawnInstance(T instance)
        {
            return DespawnInstance(instance, true);
        }

        private bool DespawnInstance(T instance, bool sendEventMessage)
        {
            if (despawned.Contains(instance)) return false;

            spawned.Remove(instance);
            despawned.Add(instance);

            if (sendEventMessage) ItemOnDespawned(instance);

            ItemSetActive(instance, false);

            if (!cullingActive && cullDespawned && totalCount > cullAbove)
            {
                cullingActive = true;
                this.StartCoroutine(CullDespawned());
            }
            return true;
        }

        //清理闲置对象协程是否已经启动
        private bool cullingActive = false;
        private IEnumerator CullDespawned()
        {
            yield return new WaitForSeconds(cullDelay);
            while (totalCount > cullAbove)
            {
                for (int i = 0; i < cullMaxPerPass; i++)
                {
                    if (totalCount <= cullAbove) break;

                    if (despawned.Count > 0)
                    {
                        T instance = despawned[0];
                        despawned.RemoveAt(0);
                        ItemDestruct(instance);
                    }
                }
                yield return new WaitForSeconds(cullDelay);
            }

            cullingActive = false;
            yield return null;
        }

        /// <summary>
        /// 获取一个实例对象
        /// </summary>
        /// <returns>The instance</returns>
        public T SpawnInstance(params object[] args)
        {
            //如果限制了实例对象数量，且开启了limitFIFO，且正在使用的对象数量大于限制的数量，那么就把spawned[0]设置为闲置状态
            if (limitInstances && limitFIFO && spawned.Count >= limitAmount)
            {
                T firstInstance = spawned[0];
                DespawnInstance(firstInstance);
            }

            T instance;
            if (despawned.Count == 0)
            {
                instance = SpawnNew(args);
                ItemSetActive(instance, true);
            }
            else
            {
                instance = despawned[0];
                despawned.RemoveAt(0);
                spawned.Add(instance);

                ItemSetArg(instance, args);
                ItemSetActive(instance, true);
            }
            return instance;
        }

        /// <summary>
        /// 创建一个实例对象
        /// </summary>
        public T SpawnNew(params object[] arg)
        {
            // 如果限制了实例对象数量，且对象总数大于限制的数量,就返回一个空对象
            if (limitInstances && totalCount >= limitAmount) return default(T);

            T instance = Instantiate(arg);
            RenameInstance(instance);

            spawned.Add(instance);

            return instance;
        }

        /// <summary>
        /// 将一个实例对象添加到缓存池
        /// </summary>preloaded
        /// <param name="instance"></param>
        /// <param name="despawn">true为闲置状态;false为使用状态</param>
        public void AddToPool(T instance, bool despawn)
        {
            RenameInstance(instance);

            if (despawn)
            {
                ItemSetActive(instance, false);
                despawned.Add(instance);
            }
            else spawned.Add(instance);
        }

        /// <summary>
        /// 预加载缓存池
        /// </summary>
        /// <returns></returns>
        public void PreloadInstances()
        {
            if (preloaded) return;

            // 预加载数量超过限制数量
            if (limitInstances && preloadAmount > limitAmount)
                preloadAmount = limitAmount;

            // 预加载数量超过自动清理的基数
            if (cullDespawned && preloadAmount > cullAbove) { }

            if (preloadAsync)
            {
                if (preloadFrames > preloadAmount)
                    preloadFrames = preloadAmount;

                StartCoroutine(PreloadAsync());
            }
            else
            {
                T inst;
                while (totalCount < preloadAmount)
                {
                    inst = SpawnNew();
                    DespawnInstance(inst, false);
                }
            }
        }

        /// <summary>
        /// 异步预加载池内对象
        /// </summary>
        /// <returns></returns>
        private IEnumerator PreloadAsync()
        {
            yield return new WaitForSeconds(preloadDelay);

            T inst;
            // 防止其他脚本可能生成过对象
            int amount = preloadAmount - totalCount;
            if (amount <= 0) yield break;

            // 最后一帧实例的数量
            int remainder = amount % preloadFrames;
            // 每帧实例的数量
            int numPerFrame = amount / preloadFrames;
            
            int numThisFrame;//当前帧实例的数量
            for (int i = 0; i < preloadFrames; i++)
            {
                numThisFrame = numPerFrame;
                if (i == preloadFrames - 1)
                    numThisFrame += remainder;

                for (int n = 0; n < numThisFrame; n++)
                {
                    inst = SpawnNew();
                    if (inst != null) DespawnInstance(inst, false);
                }
                yield return null;

                if (totalCount > preloadAmount) yield break;
            }
        }

        /// <summary>
        /// 检查spawned、despawned是否包含了该对象
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        private bool Contains(T inst)
        {
            bool contains;

            contains = spawned.Contains(inst);
            if (contains) return true;

            contains = despawned.Contains(inst);
            if (contains) return true;

            return false;
        }

        /// <summary>
        /// T是否是实现IPoolItem接口
        /// </summary>
        private bool IsImplementIPoolItem
        {
            get { return typeof(IPoolItem).IsAssignableFrom(typeof(T)); }
        }

        /// <summary>
        /// T是否继承了ScriptableObject
        /// </summary>
        private bool IsScriptableObject
        {
            get { return typeof(ScriptableObject).IsAssignableFrom(typeof(T)); }
        }

        /// <summary>
        /// T是否继承了MonoBehaviour
        /// </summary>
        private bool IsMonoBehaviour
        {
            get { return typeof(MonoBehaviour).IsAssignableFrom(typeof(T)); }
        }

        /// <summary>
        /// T是否是GameObject
        /// </summary>
        private bool IsGameObject
        {
            get { return typeof(GameObject).IsAssignableFrom(typeof(T)); }
        }

        /// <summary>
        /// T是否是Transform
        /// </summary>
        private bool IsTransform
        {
            get { return typeof(Transform).IsAssignableFrom(typeof(T)); }
        }

        /// <summary>
        /// 调用对象方法--销毁
        /// </summary>
        /// <param name="instance"></param>
        protected virtual void ItemDestruct(T instance)
        {
            if (instance == null) return;

            if (IsImplementIPoolItem)
            {
                IPoolItem item = (IPoolItem)instance;
                item.Destruct();
            }
        }

        /// <summary>
        /// 调用对象方法--设置为闲置状态消息
        /// </summary>
        /// <param name="instance"></param>
        protected virtual void ItemOnDespawned(T instance)
        {
            if (instance == null) return;

            if (IsImplementIPoolItem)
            {
                IPoolItem item = (IPoolItem)instance;
                item.OnDespawned(this);
            }
        }

        /// <summary>
        /// 调用对象方法--设置为使用状态消息
        /// </summary>
        /// <param name="instance"></param>
        public virtual void ItemOnSpawned(T instance)
        {
            if (instance == null) return;

            if (IsImplementIPoolItem)
            {
                IPoolItem item = (IPoolItem)instance;
                item.OnSpawned(this);
            }
        }

        /// <summary>
        /// 调用对象方法--设置是否激活
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        protected virtual void ItemSetActive(T instance, bool value)
        {
            if (instance == null) return;

            if (IsImplementIPoolItem)
            {
                IPoolItem item = (IPoolItem)instance;
                item.SetActive(value);
            }
        }

        /// <summary>
        /// 调用对象方法--实例对象重设参数
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="args"></param>
        protected virtual void ItemSetArg(T instance, params object[] args)
        {
            if (instance == null) return;

            if (IsImplementIPoolItem)
            {
                IPoolItem item = (IPoolItem)instance;
                item.SetArg(args);
            }
        }

        /// <summary>
        /// 给实例对象重命名
        /// </summary>
        /// <param name="instance"></param>
        protected virtual void RenameInstance(T instance)
        {
            if (instance == null) return;

            if (IsImplementIPoolItem)
            {
                IPoolItem item = (IPoolItem)instance;
                item.Name += (totalCount + 1).ToString("#000");
            }
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual T Instantiate(params object[] arg)
        {
            T instance = System.Activator.CreateInstance<T>();
            ItemSetArg(instance, arg);
            return instance;
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public virtual Coroutine StartCoroutine(IEnumerator routine)
        {
            return GameManager.Instance.StartCoroutine(Name, routine);
        }

        public override string ToString()
        {
            return string.Format("[{0}: spawned={1}, despawned={2}, totalCount={3}]",
                Name, spawned.Count, despawned.Count, totalCount);
        }
    }
}