using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentTransfer : MonoBehaviour
{
    [ContextMenu("Generate Child")]
    private void GenerateChild()
    {
        if(transform.childCount > 0) return;
        
        MeshFilter parentMF = GetComponent<MeshFilter>();
        MeshRenderer parentMR = GetComponent<MeshRenderer>();
        
        if(parentMF == null || parentMR == null)
        {
            return;
        }
        
        GameObject child = new GameObject(gameObject.name + "_Mesh");
        child.transform.SetParent(transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
        
        MeshFilter childMF = child.AddComponent<MeshFilter>();
        childMF.sharedMesh = parentMF.sharedMesh;
        
        MeshRenderer childMR = child.AddComponent<MeshRenderer>();
        childMR.sharedMaterials = parentMR.sharedMaterials;
        
        DestroyImmediate(parentMF);
        DestroyImmediate(parentMR);
        
        Debug.Log("Đã chuyển MeshFilter và MeshRenderer sang child object!");
    }
}