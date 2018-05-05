namespace Frameworks
{
    public interface IPoolItem
    {
        string Name { get; set; }

        void Destruct();

        /// <summary>
        /// 对象池设置--设置为使用状态消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pool"></param>
        void OnSpawned<T>(ObjectPool<T> pool);

        /// <summary>
        /// 对象池设置--设置为闲置状态消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pool"></param>
        void OnDespawned<T>(ObjectPool<T> pool);

        /// <summary>
        /// 对象池设置--该对象是否激活
        /// </summary>
        /// <param name="value"></param>
        void SetActive(bool value);

        /// <summary>
        /// 对象池设置--该对象重设参数
        /// </summary>
        /// <param name="args"></param>
        void SetArg(params object[] args);
    }
}