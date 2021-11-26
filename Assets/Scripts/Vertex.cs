using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public bool isSelected;
    public List<GameObject> connectedEdges = new List<GameObject>();

    public bool isInvolved;  //GenerateEdge의 GetConnectedGraph함수에서 대상 그래프에 포함되었는가의 여부
    public bool usedByKruskal;
}
