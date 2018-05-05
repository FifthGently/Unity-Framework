using UnityEngine;
using Frameworks;

public class MYTEST : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("添加");

            System.Data.DataTable bt = new System.Data.DataTable();
            bt.Columns.Add("AAA");
            bt.Columns.Add("BBB");
            bt.Columns.Add("CCC");
            bt.Columns.Add("DDD");
            bt.Columns.Add("EEE");
            bt.Columns.Add("FFF");
            bt.Columns.Add("GGG");
            bt.Rows.Add("aaa,aaa", "bbb", "ccc", "ddd\"ddd", "eee");
            bt.Rows.Add(1, 2, 3, 4, 5, 6, 7);
            CSVFileHelper.DataTableToCSV(bt, @"E:\AAA.csv");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //string aaa = "12345/Resources/GUIPrefabs/CCC.prefab";
            //string bbb = GetLocalPath(aaa);
            //GameObject go = Resources.Load<GameObject>(bbb);

            GameObject go = ResourcesManager.Instance.LoadAsset<GameObject>("Resources/GUIPrefabs/CCC.prefab");
            //GameObject go = Resources.Load<GameObject>("CCC 1");
            Instantiate(go);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GameObject go = ResourcesManager.Instance.LoadAssetAsync<GameObject>("Resources/GUIPrefabs/CCC.prefab");
            //GameObject go = Resources.Load<GameObject>("CCC 1.prefab");
            Instantiate(go);
        }
    }
}
