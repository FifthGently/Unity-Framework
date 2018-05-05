namespace Frameworks
{
    using System.Collections.Generic;

    public class PoolManager
    {
        public static PoolManager Instance { get { return Singleton<PoolManager>.Instance; } }

        private Dictionary<string, PoolGroup> groups = new Dictionary<string, PoolGroup>();

        /// <summary>
        /// 创建PoolGroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public PoolGroup CreatePoolGroup(string groupName)
        {
            PoolGroup poolGroup = new PoolGroup(groupName);
            this.AddPoolGroup(poolGroup);

            return poolGroup;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="group"></param>
        public void AddPoolGroup(PoolGroup group)
        {
            if (ContainsPoolGroup(group.GroupName)) return;

            groups.Add(group.GroupName, group);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool RemovePoolGroup(PoolGroup group)
        {
            if (!ContainsPoolGroup(group.GroupName)) return false;

            groups.Remove(group.GroupName);
            return true;
        }

        /// <summary>
        /// 销毁对象池组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool DestroyPoolGroup(string groupName)
        {
            PoolGroup poolGroup;
            if (!TryGetPoolGroup(groupName, out poolGroup)) return false;

            this.RemovePoolGroup(poolGroup);
            poolGroup.Destroy();
            return true;
        }

        /// <summary>
        /// 销毁所有象池组
        /// </summary>
        public void DestroyAll()
        {
            foreach (PoolGroup poolGroup in groups.Values)
            { poolGroup.Destroy(); }

            groups.Clear();
        }

        /// <summary>
        /// 是否已经存在groupName的PoolGroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private bool ContainsPoolGroup(string groupName)
        {
            return groups.ContainsKey(groupName);
        }

        /// <summary>
        /// 尝试获取groupName的PoolGroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="poolGroup"></param>
        /// <returns></returns>
        private bool TryGetPoolGroup(string groupName, out PoolGroup poolGroup)
        {
            return groups.TryGetValue(groupName, out poolGroup);
        }

        public int Count { get { return groups.Count; } }

        public PoolGroup this[string key]
        {
            get
            {
                PoolGroup group;
                try { group = groups[key]; }
                catch (KeyNotFoundException)
                {
                    string msg = string.Format("A PoolGroup with the name '{0}' not found. ", key);
                    throw new KeyNotFoundException(msg);
                }
                return group;
            }
        }
    }
}
