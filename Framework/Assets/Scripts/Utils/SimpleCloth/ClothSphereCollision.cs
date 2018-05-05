using UnityEngine;

public class ClothSphereCollision : MonoBehaviour
{
    private Cloth cloth = null;
    private Mesh curMesh;

    /// <summary>
    /// 已经发生碰撞静止
    /// </summary>
    public bool BeSetted { get { return beSetted; } }
    private bool beSetted = false;

    public int verticesIndex = 0;
    /// <summary>
    /// 布料顶点上的触发器的索引
    /// </summary>
    public int VerticesIndex
    {
        get { return verticesIndex; }
        set { verticesIndex = value; }
    }

    public string clothKey { get { return this.transform.parent.name; } }

    public int VertexCount { get { return this.curMesh.vertexCount; } }

    public Mesh CurMesh { get { return this.curMesh; } }

    private void Start()
    {
        cloth = transform.parent.GetComponent<Cloth>();
        curMesh = transform.parent.GetComponent<SkinnedMeshRenderer>().sharedMesh;

        SimpleClothManager.Instance.SetResetPosList(this);
    }

    void Update()
    {
        if (cloth != null && !beSetted)
        {
            transform.localPosition = cloth.vertices[verticesIndex];
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!beSetted && other.transform != this.transform.parent && other.transform.parent != this.transform.parent)
        {
            Vector3[] meshVertices = curMesh.vertices;
            meshVertices[verticesIndex] = cloth.vertices[verticesIndex];
            curMesh.vertices = meshVertices;

            ClothSkinningCoefficient[] csc = cloth.coefficients;
            csc[verticesIndex].maxDistance = 0;
            cloth.coefficients = csc;

            beSetted = true;
        }
    }


    private void OnDisable()
    {
        Reset();
    }

    public void Reset()
    {
        if (curMesh != null)
            SimpleClothManager.Instance.ResetState(this);
    }
}
