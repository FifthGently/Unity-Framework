using UnityEditor;
using UnityEngine;

public class SimpleClothEditor : Editor
{
    [MenuItem("Framework/Tool/SimpleCloth/AddCollisionToCloth")]
    private static void AddCollision()
    {
        GameObject obj = Selection.objects[0] as GameObject;
        Cloth cloth = obj.GetComponent<Cloth>();
        if(cloth == null) cloth = obj.AddComponent<Cloth>();

        Mesh mesh = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        for (int i = 0; i < cloth.vertices.Length; i++)
        {
            GameObject go = new GameObject("CollisionSphera");
            go.transform.SetParent(cloth.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = cloth.vertices[i];
            go.AddComponent<SphereCollider>().radius = 0.02f;
            go.GetComponent<SphereCollider>().center = Vector3.zero;
            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            go.GetComponent<Collider>().isTrigger = true;

            ClothSphereCollision col = go.AddComponent<ClothSphereCollision>();
            col.VerticesIndex = i;
        }
    }
}
