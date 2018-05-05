namespace Frameworks
{
    using UnityEngine;

    /// <summary>
    /// 预设的<Transform>对象池
    /// </summary>
    public class PrefabPool : ObjectPool<Transform>
    {
        // 勾选后实例化的游戏对象的缩放比例将全是1，不勾选择用Prefab默认的
        public bool matchPoolScale = false;
        // 勾选后实例化的游戏对象的Layer将用Prefab默认的
        public bool matchPoolLayer = false;

        public Transform parent { get; set; }
        public Transform prefab { get; set; }
        public GameObject prefabGO { get { return prefab.gameObject; } }
        public bool dontDestroyOnLoad { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            if (parent == null)
                parent = new GameObject(prefabGO.name.Replace("(Clone)", "") + "Pool").transform;

            if (dontDestroyOnLoad)
                Object.DontDestroyOnLoad(parent.gameObject);
        }

        public override void SelfDestruct()
        {
            base.SelfDestruct();

            if (parent != null)
                Object.Destroy(parent.gameObject);
        }
        
        protected override void ItemDestruct(Transform instance)
        {
            Object.Destroy(instance.gameObject);
        }

        public override void ItemOnSpawned(Transform instance)
        {
            instance.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
        }

        protected override void ItemOnDespawned(Transform instance)
        {
            instance.gameObject.BroadcastMessage("OnDespawned", this, SendMessageOptions.DontRequireReceiver);
        }

        protected override void ItemSetActive(Transform instance, bool value)
        {
            instance.gameObject.SetActive(value);
        }

        protected override void ItemSetArg(Transform instance, params object[] args)
        {
            instance.gameObject.BroadcastMessage("SetArg", args, SendMessageOptions.DontRequireReceiver);
        }

        protected override void RenameInstance(Transform instance)
        {
            instance.gameObject.name = instance.name.Replace("(Clone)", string.Empty) + (this.totalCount + 1).ToString("#000");
        }

        protected override Transform Instantiate(params object[] args)
        {
            Transform instance = Object.Instantiate(prefabGO).transform;
            if (parent != null) instance.SetParent(parent, false);
            ItemSetArg(instance, args);
            return instance;
        }
    }
}