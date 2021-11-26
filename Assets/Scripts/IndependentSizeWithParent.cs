using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class IndependentSizeWithParent : MonoBehaviour
{
    public Vector3 childScale;
    public Vector3 childRotation;
    void Start()
    {
        name = $"{name}_{Util.GetNumberInt(transform.parent.gameObject)}";
        transform.parent = null; 
        transform.localScale = childScale;
        transform.localEulerAngles = childRotation;
        //transform.parent = parent;
    }
}
