namespace Frameworks
{
    using System.Collections.Generic;
    using UnityEngine;

    public sealed partial class ResourcesManager
    {
        public static ResourcesManager Instance { get { return Singleton<ResourcesManager>.Instance; } }

        public const string PATH_PREFAB_EFFECT = "Resources/Effects/";
        public const string PATH_PREFAB_SKILL = "Resources/Skills/";

        public const string PATH_TEXTURE_CARD = "Resources/Textures/Card/";
        public const string PATH_TEXTURE_CARD_MINI = "Resources/Textures/CardMini/";
        public const string PATH_TEXTURE_CARD_HEAD = "Resources/Textures/CardHead/";
        public const string PATH_TEXTURE_CARD_FEATURE = "Resources/Textures/CardFeature/";
        public const string PATH_TEXTURE_CARD_PROFESSION = "Resources/Textures/CardProfession/";
        public const string PATH_TEXTURE_CARD_BACKGROUND = "Resources/Textures/CardBackground/";
        public const string PATH_TEXTURE_BATTLE_BACKGROUND = "Resources/Textures/BattleBackground/";
        public const string PATH_TEXTURE_MAP_MISSION = "Resources/Textures/MissionMap/";
        public const string PATH_TEXTURE_EQUIP = "Resources/Textures/Equip/101_EquipIcon/zb_";
        public const string PATH_TEXTURE_ITEM = "Resources/Textures/Item/";
        public const string PATH_TEXTURE_INFO_CRAFT = "Resources/Textures/CraftInfo/";
        public const string PATH_TEXTURE_INFO_RANK = "Resources/Textures/BoaldIcon/";
        public const string PATH_TEXTURE_INFO_MISSION = "Resources/Textures/BoaldIcon/";

        private Dictionary<string, List<GameObject>> pool;
        public ResourcesManager()
        {
            pool = new Dictionary<string, List<GameObject>>();
            Initialize();
        }

        public GameObject InstantiateGUI(string prefabPath)
        {
            GameObject go = null;
            try
            {
                go = Object.Instantiate(LoadAsset<GameObject>(prefabPath));
            }
            catch
            {
                GameLog.LogError("InstantiateGUI error:" + prefabPath);
            }
            return go;
        }

        public GameObject InstantiateGameObject(string prefabPath)
        {
            GameObject go = null;
            try
            {
                go = UnityEngine.Object.Instantiate(LoadAsset<GameObject>(prefabPath));
            }
            catch
            {
                GameLog.LogError("InstantiateGameObject error:" + prefabPath);
            }
            return go;
        }

        public GameObject InstantiateGameObjectFromPool(string prefabPath)
        {
            GameObject go = null;
            try
            {
                go = UnityEngine.Object.Instantiate(LoadAsset<GameObject>(prefabPath));
            }
            catch
            {
                GameLog.LogError("InstantiateGameObject error:" + prefabPath);
            }
            return go;
        }

        public GameObject InstantiateEffect(string effectId)
        {
            return InstantiateGameObject(PATH_PREFAB_EFFECT + effectId);
        }

        public GameObject InstantiateGameObject(string prefPath, Vector3 pos)
        {
            GameObject go = null;
            try
            {
                go = GameObject.Instantiate(Resources.Load(prefPath), pos, Quaternion.identity) as GameObject;
            }
            catch
            {
                GameLog.LogError("InstantiateGameObject error:" + prefPath);
            }
            return go;
        }

        public GameObject InstantiateEffect(string effectId, Vector3 pos)
        {
            return InstantiateGameObject(PATH_PREFAB_EFFECT + effectId, pos);
        }

        public GameObject InstantiateSkill(string skillName)
        {
            return InstantiateGameObject(PATH_PREFAB_SKILL + skillName);
        }

        public GameObject LoadGameObject(string prefPath)
        {
            GameObject go = Resources.Load(prefPath) as GameObject;

            return go;
        }

        public TextAsset LoadTextAsset(string path)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            return textAsset;
        }
        public Texture2D LoadResourceTexture(string resPath)
        {
            //加载Resources下的纹理
            Texture2D tex = Resources.Load<Texture2D>(resPath);
            return tex;
        }

        public Sprite LoadResourceSprite(string resPath)
        {
            Sprite sprite = Resources.Load<Sprite>(resPath);
            return sprite;
        }

        public Sprite LoadCardIcon(string iconID)
        {
            Sprite sprite = Resources.Load<Sprite>(PATH_TEXTURE_CARD_HEAD + iconID);
            if (sprite == null) sprite = Resources.Load<Sprite>(PATH_TEXTURE_CARD_HEAD + "h0000");
            return sprite;
        }

        public GameObject LoadPool(string path)
        {
            GameObject obj;
            if (pool.ContainsKey(path) && pool[path].Count > 0)
            {
                obj = pool[path][0];
                pool[path].RemoveAt(0);
            }
            else
            {
                obj = Instance.InstantiateGameObject(path);
            }
            obj.SetActive(true);
            return obj;
        }

        public void BackPool(GameObject obj,string path)
        {
            obj.SetActive(false);
            if (!pool.ContainsKey(path))
            {
                pool[path] = new List<GameObject>();
            }
            pool[path].Add(obj);
        }
        //
    }
}
