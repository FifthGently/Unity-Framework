namespace Frameworks
{
    public enum EventEnum
    {
        NONE = 0,

        #region 《网络数据层》
        //===========================《网络数据层》========================

        #endregion 《网络数据层》

        #region 《逻辑层》
        //===========================《逻辑层》============================
        #region 场景相关
        /// <summary>
        /// 场景加载开始
        /// </summary>
        SCENE_LOAD_BEGIN,
        /// <summary>
        /// 场景加载百分比
        /// </summary>
        SCENE_LOAD_PRECENT,
        /// <summary>
        /// 场景加载结束
        /// </summary>
        SCENE_LOAD_END,
        /// <summary>
        /// 场景加载部件
        /// </summary>
        SCENE_LOAD_UNIT,
        /// <summary>
        /// 场景加载玩家池开始
        /// </summary>
        SCENE_LOAD_PLAYER_POOL_BEGIN,
        /// <summary>
        /// 场景加载玩家池结束
        /// </summary>
        SCENE_LOAD_PLAYER_POOL_END,
        /// <summary>
        /// 场景加载怪物池开始
        /// </summary>
        SCENE_LOAD_MONSTER_POOL_BEGIN,
        /// <summary>
        /// 场景加载怪物池结束
        /// </summary>
        SCENE_LOAD_MONSTER_POOL_END,
        /// <summary>
        /// 场景加载特效池开始
        /// </summary>
        SCENE_LOAD_EFFECT_POOL_BEGIN,
        /// <summary>
        /// 场景加载特效池结束
        /// </summary>
        SCENE_LOAD_EFFECT_POOL_END,
        /// <summary>
        /// 主角加载完成
        /// </summary>
        MAIN_PLAYER_LOAD_FINISH,
        /// <summary>
        /// 对象回池
        /// </summary>
        OBJECT_BACK_POOL,
        /// <summary>
        /// 点击NPC
        /// </summary>
        CLICK_NPC,
        /// <summary>
        /// 场景对象清空
        /// </summary>
        SCENE_ALL_OBJ_CLEAR,

        #endregion

        #region 动作相关
        /// <summary>
        /// 动作开始
        /// </summary>
        ANIMATION_START_EVENT,
        /// <summary>
        /// 动作结束
        /// </summary>
        ANIMATION_END_EVENT,
        /// <summary>
        /// 攻击命中
        /// </summary>
        ANIMATION_HIT_EVENT,
        /// <summary>
        /// 怪物普通攻击
        /// </summary>
        MONSTER_COMMON_ATTACK,
        /// <summary>
        /// 玩家普通攻击
        /// </summary>
        PLAYER_COMMON_ATTACK,

        #endregion

        #region NPC剧情对话
        //开始对话
        DIALOGUE_START_EVENT,
        //结束对话
        DIALOGUE_END_EVENT,

        #endregion

        #region 缓存池
        //创建PoolGroup
        POOL_GROUP_CREATE,

        #endregion

        #endregion 《逻辑层》

        #region 《UI层》
        //===========================《UI层》==============================
        /// <summary>
        /// 打开UI
        /// </summary>
        OPEN_UI_PANEL,

        /// <summary>
        /// 关闭UI
        /// </summary>
        CLOSE_UI_PANEL,

        /// <summary>
        /// 金钱变化
        /// </summary>
        CHANGE_GOLD_VALUE,

        /// <summary>
        /// 小地图
        /// </summary>
        CHANGE_SMALL_MAP,

        /// <summary>
        /// 生命值变化
        /// </summary>
        CHANGE_HP_VALUE,

        /// <summary>
        /// 魔法值变化
        /// </summary>
        CHANGE_MP_VALUE,

        /// <summary>
        /// 攻击值变化
        /// </summary>
        CHANGE_ATTACK_VALUE,

        /// <summary>
        /// 防御值变化
        /// </summary>
        CHANGE_DEFENSE_VALUE,

        /// <summary>
        /// 经验值变化
        /// </summary>
        CHANGE_EXP_VALUE,

        /// <summary>
        /// 等级变化attribute
        /// </summary>
        CHANGE_LEVEL_VALUE,

        /// <summary>
        /// 属性变化
        /// </summary>
        CHANGE_ATTRIBUTE_VALUE,

        /// <summary>
        /// 使用技能
        /// </summary>
        CHANGE_USE_SKILL,

        /// <summary>
        /// 使用道具
        /// </summary>
        CHANGE_USE_ITEM,

        /// <summary>
        /// 获得技能
        /// </summary>
        STUDY_SKILL,

        /// <summary>
        /// 获得道具
        /// </summary>
        GET_ITEM,

        /// <summary>
        /// 显示Tip
        /// </summary>
        SHOW_TIP,

        /// <summary>
        /// 隐藏Tip
        /// </summary>
        HIDE_TIP,

        /// <summary>
        /// 在聊天界面输出语句
        /// </summary>
        PRINT_IN_TALK_FACE,

        /// <summary>
        /// 更改主场景UI的场景名
        /// </summary>
        CHANGE_MAP_NAME,

        #endregion 《UI层》
    }
}