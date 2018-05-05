using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class SkinnedCollisionHelper : MonoBehaviour
{
    class CVertexWeight
    {
        public int index;
        public Vector3 localPosition;
        public float weight;

        public CVertexWeight(int i, Vector3 p, float w)
        {
            index = i;
            localPosition = p;
            weight = w;
        }
    }

    class CWeightList
    {
        public Transform transform;
        public List<CVertexWeight> weights;
        public CWeightList()
        {
            weights = new List<CVertexWeight>(4);
        }
    }
    /// <summary>
    /// 控制是否每侦更新碰撞数据,大多数情况不需要，剪切时也不需要
    /// </summary>
    [Tooltip("控制是否每侦更新碰撞数据,大多数情况不需要，剪切时也不需要")]
    public bool IsLoop = false;
    /// <summary>
    /// 控制是否用碰撞体功能，目前项目中冲洗时打开，剪切时可以不用打开
    /// </summary>
    [Tooltip("控制是否用碰撞体功能，目前项目中冲洗时打开，剪切时可以不用打开")]
    public bool UseMeshCollder = false;

    private CWeightList[] nodeWeights; // one per node
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshCollider meshCollider;

    /// <summary>
    ///  This basically translates the information about the skinned mesh into
    /// data that we can internally use to quickly update the collision mesh.
    /// </summary>
    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshCollider != null && skinnedMeshRenderer != null)
        {
            InitData();
            UpdateCollisionMesh();///极致情况下这个也可以省略
        }
        else
        {
            GameLog.LogError("[SkinnedCollisionHelper] " + gameObject.name + " is missing SkinnedMeshRenderer or MeshCollider!");
        }
    }
    public void Update()
    {
        if (IsLoop)
        {
            UpdateCollisionMesh();
        }
        if (UseMeshCollder != meshCollider.enabled)
        {
            if (IsLoop == false && UseMeshCollder == true) UpdateCollisionMesh();
            meshCollider.enabled = UseMeshCollder;
        }
    }


    private void InitData()
    {
        // Cache used values rather than accessing straight from the mesh on the loop below
        Vector3[] cachedVertices = skinnedMeshRenderer.sharedMesh.vertices;
        Matrix4x4[] cachedBindposes = skinnedMeshRenderer.sharedMesh.bindposes;
        BoneWeight[] cachedBoneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;

        // Make a CWeightList for each bone in the skinned mesh
        nodeWeights = new CWeightList[skinnedMeshRenderer.bones.Length];

        int length = skinnedMeshRenderer.bones.Length;

        for (int i = 0; i < length; i++)
        {
            nodeWeights[i] = new CWeightList();
            nodeWeights[i].transform = skinnedMeshRenderer.bones[i];
        }

        length = cachedVertices.Length;
        Vector3 localPt;
        BoneWeight bw;

        // Create a bone weight list for each bone, ready for quick calculation during an update...
        for (int i = 0; i < length; i++)
        {
            bw = cachedBoneWeights[i];
            if (bw.weight0 != 0.0f)
            {
                //GameLog.LogError("bw.boneIndex0==" + bw.boneIndex0);
                if (bw.boneIndex0 < 0 || bw.boneIndex0 > cachedBindposes.Length)
                {
                    GameLog.LogError("出错了==bw.boneIndex0===" + bw.boneIndex0);
                }
                else
                {
                    localPt = cachedBindposes[bw.boneIndex0].MultiplyPoint3x4(cachedVertices[i]);
                    nodeWeights[bw.boneIndex0].weights.Add(new CVertexWeight(i, localPt, bw.weight0));
                }
                
            }
            if (bw.weight1 != 0.0f)
            {
                  localPt = cachedBindposes[bw.boneIndex1].MultiplyPoint3x4(cachedVertices[i]);
                nodeWeights[bw.boneIndex1].weights.Add(new CVertexWeight(i, localPt, bw.weight1));
            }
            if (bw.weight2 != 0.0f)
            {
                localPt = cachedBindposes[bw.boneIndex2].MultiplyPoint3x4(cachedVertices[i]);
                nodeWeights[bw.boneIndex2].weights.Add(new CVertexWeight(i, localPt, bw.weight2));
            }
            if (bw.weight3 != 0.0f)
            {
                localPt = cachedBindposes[bw.boneIndex3].MultiplyPoint3x4(cachedVertices[i]);
                nodeWeights[bw.boneIndex3].weights.Add(new CVertexWeight(i, localPt, bw.weight3));
            }
        }
    }
   
    /// <summary>
    /// Manually recalculates the collision mesh of the skinned mesh on this object.
    /// </summary>
    /// 
    private Mesh collederMesh;
    public void UpdateCollisionMesh()
    {
        if (collederMesh == null) collederMesh = new Mesh();

        Vector3[] newVert = new Vector3[skinnedMeshRenderer.sharedMesh.vertices.Length];

        // Now get the local positions of all weighted indices...
        foreach (CWeightList wList in nodeWeights)
        {
            foreach (CVertexWeight vw in wList.weights)
            {
                newVert[vw.index] += wList.transform.TransformPoint(vw.localPosition) * vw.weight;
            }
        }
        // Now convert each point into local coordinates of this object.
        for (int i = 0; i < newVert.Length; i++)
        {
            newVert[i] = transform.InverseTransformPoint(newVert[i]);
           
        }
        // Update the mesh ( collider) with the updated vertices
        collederMesh.vertices = newVert;
        collederMesh.uv = skinnedMeshRenderer.sharedMesh.uv; // is this even needed here?
        collederMesh.triangles = skinnedMeshRenderer.sharedMesh.triangles;
        collederMesh.RecalculateBounds();
        collederMesh.MarkDynamic(); // says it should improve performance, but I couldn't see it happening
        meshCollider.sharedMesh = collederMesh;
    }

}
