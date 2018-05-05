using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Frameworks
{
    public interface IDataBase<TKey>
    {
        TKey GetID();
        bool ParseData(BinaryReader reader);
    }

    public class DataTable<TKey, TValue> where TValue : IDataBase<TKey>, new()
    {
        Dictionary<TKey, TValue> mMapTable = new Dictionary<TKey, TValue>();
        bool mLoadRetValue;
        bool mIsLoaded;
        string mFileName;

        public DataTable(string fileName) { mFileName = fileName; }

        public TValue this[TKey key] { get { return Find(key); } }

        public bool Load()
        {
            if (mIsLoaded)
                return mLoadRetValue;
            mLoadRetValue = false;
            mIsLoaded = true;
            string strPath = ConfigManager.CONFIG_PATH_CONFIG_INFO + mFileName;
            TextAsset textAsset = ResourcesManager.Instance.LoadTextAsset(strPath);
            if (textAsset != null)
            {
                mLoadRetValue = true;
                MemoryStream ms = new MemoryStream(textAsset.bytes);
                BinaryReader reader = new BinaryReader(ms);
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    TValue record = new TValue();
                    if (!record.ParseData(reader))
                    {
                        mLoadRetValue = false;
                        break;
                    }

                    if (mMapTable.ContainsKey(record.GetID()))
                    {
                        mLoadRetValue = false;
                        break;
                    }
                    mMapTable.Add(record.GetID(), record);
                }
            }
            return mLoadRetValue;
        }

        public IEnumerator CoroutineLoad()
        {
            if (mIsLoaded) yield return false;

            mLoadRetValue = false;
            mIsLoaded = true;

        }

        public TValue Find(TKey key)
        {
            if (mMapTable.ContainsKey(key))
            {
                return mMapTable[key];
            }
            return default(TValue);
        }

        public int Size() { return mMapTable.Count; }

        public Dictionary<TKey, TValue> MapTable { get { return mMapTable; } }
    }
}
