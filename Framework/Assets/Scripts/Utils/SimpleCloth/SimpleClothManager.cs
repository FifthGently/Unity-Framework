using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleClothManager
{
    public static SimpleClothManager Instance { get { return Singleton<SimpleClothManager>.Instance; } }

    public SimpleClothManager()
    {
        resetPoslist = new Dictionary<string, Vector3[]>();
    }

    private Dictionary<string, Vector3[]> resetPoslist;

    public void SetResetPosList(string key, Vector3 pos, int index, int listCount)
    {
        if (index >= listCount) return;

        if(!resetPoslist.ContainsKey(key))
        {
            Vector3[] list = new Vector3[listCount];
            resetPoslist.Add(key, list);
        }
        else
        {
            if(resetPoslist[key] == null)
                resetPoslist[key] = new Vector3[listCount];
        }

        resetPoslist[key][index] = pos;
    }

    public void SetResetPosList(ClothSphereCollision collision)
    {
        if (collision.VerticesIndex >= collision.VertexCount) return;

        if (!resetPoslist.ContainsKey(collision.clothKey))
        {
            Vector3[] list = new Vector3[collision.VertexCount];
            resetPoslist.Add(collision.clothKey, list);
        }
        else
        {
            if (resetPoslist[collision.clothKey] == null)
                resetPoslist[collision.clothKey] = new Vector3[collision.VertexCount];
        }

        resetPoslist[collision.clothKey][collision.VerticesIndex] = collision.transform.localPosition;
    }

    public void ResetState(ClothSphereCollision collision)
    {
        if (collision.VerticesIndex >= collision.VertexCount) return;
        if (!resetPoslist.ContainsKey(collision.clothKey)) return;
        if (resetPoslist[collision.clothKey] == null) return;

        collision.CurMesh.vertices = resetPoslist[collision.clothKey];
    }

}
