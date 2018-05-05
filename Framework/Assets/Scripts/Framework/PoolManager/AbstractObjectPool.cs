namespace Frameworks
{
    public abstract class AbstractObjectPool
    {
        public PoolGroup    poolGroup   { get; set; }       //
        public string       Name        { get; set; }       //
        public bool         preloaded   { get; set; }       //是否已经预加载

        public virtual void Initialize() { }

        public virtual void SelfDestruct() { }

        #region 配置属性
        /// <summary>
        /// 缓存池中这个Prefab的预加载数量
        /// </summary>
        public int preloadAmount = 1;

        /// <summary>
        /// 缓存池所有的gameobject是否可以“异步”加载
        /// </summary>
        public bool preloadAsync = false;

        /// <summary>
        /// 预加载共使用几帧
        /// </summary>
        public int preloadFrames = 2;

        /// <summary>
        /// 延迟多久开始加载
        /// </summary>
        public float preloadDelay = 0;

        /// <summary>
        /// 是否开启对象实例化的限制功能
        /// </summary>
        public bool limitInstances = false;

        /// <summary>
        /// 限制缓存池中对象的数量，与preloadAmount有冲突，如果同时开启则以limitAmout为准
        /// </summary>
        public int limitAmount = 100;

        /// <summary>
        /// 如果我们限制了缓存池里面只能有10个Prefab，如果不勾选它，那么你拿第11个的时候就会返回null。
        /// 如果勾选它在取第11个的时候他会返回给你前10个里最不常用的那个
        /// </summary>
        public bool limitFIFO = false;

        /// <summary>
        /// 是否开启缓存池智能自动清理模式
        /// </summary>
        public bool cullDespawned = false;

        /// <summary>
        /// 缓存池自动清理，但是始终保留几个对象不清理
        /// </summary>
        public int cullAbove = 50;

        /// <summary>
        /// 每过多久执行一遍自动清理(s),从上一次清理过后开始计时
        /// </summary>
        public int cullDelay = 60;

        /// <summary>
        /// 每次自动清理几个游戏对象
        /// </summary>
        public int cullMaxPerPass = 5;
        #endregion
    }
}
