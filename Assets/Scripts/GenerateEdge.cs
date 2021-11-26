using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static DefaultNamespace.Util;
using static DefaultNamespace.VectorUtil;
using static DefaultNamespace.GraphUtil;

public class GenerateEdge : MonoBehaviour
{
    GameObject genButton;
    SelectObject selectObject;
    InputField edgeAmountInputField;
    private int edgeCount = 0;

    void Start()
    {
        genButton = GameObject.Find("GenerateButton");
        selectObject = GameObject.Find("SelectManager").GetComponent<SelectObject>();
        edgeAmountInputField = GameObject.Find("EdgeAmountInputField").GetComponent<InputField>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
            ButtonClicked();
    }

    public bool isConfiguring = true; //간선을 만드는 중인지 여부
    private bool isValLegal = false;

    public void ButtonClicked()
    {
        ParseInput();

        if(!isValLegal) return;

        Generate();
    }

    private int weightedVal;

    private void ParseInput()
    {
        try {
            weightedVal = int.Parse(edgeAmountInputField.text);
        }
        catch(FormatException ignored) {
            //Debug.Log("Please input a number.");
            Debug.Log("Random number inserted.");
            weightedVal = UnityEngine.Random.Range(10, 70);
            isValLegal = true;
            return;
        }

        if(weightedVal <= 0) {
            Debug.Log("Weight value should be at least 1.");
            return;
        }

        isValLegal = true;
        Debug.Log(weightedVal + ", Weighted Value is Legal.");
    }

    public GameObject edgePrefab;

    private void Generate()
    {
        if(selectObject.selectedVertices.Count < 2) {
            Debug.Log("2 of Vertices should be selected.");
            return;
        }


        edgeCount++;
        //vertexPrefab = Resources.Load<GameObject>("Prefabs/Vertex");
        var edge = Instantiate(edgePrefab, transform.position, transform.rotation);
        edge.name = "Edge_" + edgeCount;

        var selVertex1 = selectObject.selectedVertices[0];
        var selVertex2 = selectObject.selectedVertices[1];

        var selVertex1Pos = selVertex1.transform.position;
        var selVertex2Pos = selVertex2.transform.position;
        var edgePos = GetMidPoint2D(selVertex1Pos, selVertex2Pos);
        edge.transform.position = edgePos;

        edge.transform.rotation = Quaternion.Euler(GetZRotation(selVertex1Pos, selVertex2Pos));

        edge.transform.localScale = GetScale(weightedVal);


        var textMesh = edge.GetComponentInChildren<TextMesh>();
        textMesh.text = $"{weightedVal}";

        SaveInfoToEdge(edge, selectObject.selectedVertices, weightedVal);

        ReDistanceVertices(edge);
    }

    private void SaveInfoToEdge(GameObject edge, List<GameObject> selVertices, int weightedValue)
    {
        foreach(var selVertex in selVertices) {
            edge.GetComponent<Edge>().connectedVertices.Add(selVertex);
            selVertex.GetComponent<Vertex>().connectedEdges.Add(edge);
        }

        edge.GetComponent<Edge>().weightedValue = weightedValue;
    }

    private void MoveInCircle(GameObject target, Vector3 center, float radius, float deltaTheta) //radius = edge.localScale(가중치 * 로그값)
    {
        //bug 간선을 각 정점의 중심이 아닌, 모서리와 연결하는 문제 
        var targetPos = target.transform.position;
        var angle = GetZRotation(center, targetPos);

        angle.z += deltaTheta;
        
        targetPos = center + Get2DVectorComponents(angle.z, radius);
        target.transform.position = targetPos;
        
        /*var position = target.transform.position;
        var targetPos = position;
        var angle = GetZRotation(center, targetPos);

        angle.z += deltaTheta;
        
        position = center + Get2DVectorComponents(angle.z, radius);
        target.transform.position = position;*/
    }

    private void IncreaseDistUntilMatch(GameObject v1, GameObject v2, GameObject centerObj)
    {
        var vertices = new List<GameObject> {v1, v2};

        GameObject edge = null;
        foreach(var e1 in v1.GetComponent<Vertex>().connectedEdges)
            foreach(var unused in v2.GetComponent<Vertex>().connectedEdges.Where(e2 => e1 == e2))
                edge = e1;

        if(edge == null)
            throw new NullReferenceException("Edge is Null.");


        var centerObjPos = centerObj.transform.position;
        var edgeLocalScale = edge.transform.localScale;


        var edgeEAngle = edge.transform.eulerAngles;
        var edgeCenter = edge.transform.position;
        var edgeExtent = edge.GetComponent<SpriteRenderer>().bounds.extents;
        var cornerPosArray = GetCornerPosArray(edgeEAngle, edgeCenter, edgeExtent);


        const int SHORTEST = 0;
        const int OTHER = 1;

        var comb = GetCombination(vertices, cornerPosArray);
        var oneVertex = vertices[comb[SHORTEST][0]];
        var oneCornerPos = cornerPosArray[comb[SHORTEST][1]];
        var otherVertex = vertices[comb[OTHER][0]];
        var otherCornerPos = cornerPosArray[comb[OTHER][1]];

        StartCoroutine(IncreaseDistUntilMatchExecution(oneVertex, otherVertex, edge, centerObjPos, edgeLocalScale));
        //IncreaseDistUntilMatchExecution(oneSign, oneVertex, otherVertex, centerObjPos, edgeLocalScale, oneAngle, oneCornerAngle)
    }

    private IEnumerator IncreaseDistUntilMatchExecution(GameObject oneVertex, GameObject otherVertex, GameObject edge, Vector3 centerObjPos, Vector3 edgeLocalScale)
    {
        var oneCornerPos = GetNearestCornerPos(oneVertex, edge);
        
        var oneAngle = GetZRotation(centerObjPos, oneVertex.transform.position).z;
        var oneCornerAngle = GetZRotation(centerObjPos, oneCornerPos).z;
        var oneSign = Mathf.Sign(oneCornerAngle - oneAngle);
        
        //edgeLocalScale.x -= Mathf.PI / 10;
        
        if(oneCornerAngle > oneAngle) {
            do {
                MoveInCircle(oneVertex, centerObjPos, edgeLocalScale.x, 0.1f * oneSign);
                MoveInCircle(otherVertex, centerObjPos, edgeLocalScale.x, 0.1f * -oneSign);
                Debug.Log(Vector3.Distance(centerObjPos, oneVertex.transform.position));
                
                oneCornerPos = GetNearestCornerPos(oneVertex, edge);
                oneAngle = GetZRotation(centerObjPos, oneVertex.transform.position).z;
                oneCornerAngle = GetZRotation(centerObjPos, oneCornerPos).z;
                
                yield return null;
            } while(oneCornerAngle >= oneAngle);
        }
        else if(oneCornerAngle < oneAngle) {
            do {
                MoveInCircle(oneVertex, centerObjPos, edgeLocalScale.x, 0.1f * oneSign);
                MoveInCircle(otherVertex, centerObjPos, edgeLocalScale.x, 0.1f * -oneSign);
                Debug.Log(Vector3.Distance(centerObjPos, oneVertex.transform.position));
                
                oneCornerPos = GetNearestCornerPos(oneVertex, edge);
                oneAngle = GetZRotation(centerObjPos, oneVertex.transform.position).z;
                oneCornerAngle = GetZRotation(centerObjPos, oneCornerPos).z;
                
                yield return null;
            } while(oneCornerAngle <= oneAngle);
        }
    }

    private GameObject GetCenterVertex(GameObject v1, GameObject v2)
    {
        foreach(var e1 in v1.GetComponent<Vertex>().connectedEdges)
            foreach(var e2 in v2.GetComponent<Vertex>().connectedEdges) {
                if(e1 == e2) continue;
                
                foreach(var ver1 in e1.GetComponent<Edge>().connectedVertices)
                    foreach(var ver2 in e2.GetComponent<Edge>().connectedVertices) {
                        if(ver1 == ver2)
                            return ver1;
                    }
            }

        throw new Exception("Center vertex not found");
    }

    private void ReDistanceVertices(GameObject edge)
    {
        if(edgeCount <= 1)
            ReDistanceSingle(edge);
        else 
            ReDistanceGraphs(edge);
        
        
        /*
         * 2번째 간선 부터는 선택한 정점 중 두번째 인덱스에 있는 것을 옮기는데,
         * 우선 간선을 생성하고 그 간선을 첫번째 정점에 맞춘 뒤,
         * 두번째 정점에 연결된 간선과 정점을 모두 리스트에 넣고 같이 옮긴다.
         */
    }

    private void ReDistanceSingle(GameObject edge)
    {
        var edgeEAngle = edge.transform.localRotation.eulerAngles;
        var edgeCenter = edge.transform.position;
        var edgeExtent = edge.GetComponent<SpriteRenderer>().bounds.extents;
        var conVertices = edge.GetComponent<Edge>().connectedVertices;
        
        
        const int SHORTEST = 0;
        const int OTHER = 1;
        //var vectorComponents = Get2DVectorComponents(edgeEAngle.z, edgeExtent.x);
            
        // var pos1 = Get2DVectorComponents(edgeEAngle.z, edgeExtent.x/2);
        // var pos2 = Get2DVectorComponents(edgeEAngle.z, -edgeExtent.x/2);
            
        var posArray = GetCornerPosArray(edgeEAngle, edgeCenter, edgeExtent);

        Debug.Log($"{edgeEAngle.z}, {edge.GetComponent<SpriteRenderer>().bounds.extents}, {edge.transform.localScale.x}, {edgeExtent.x}");
        Debug.Log($"PosArray :  {posArray[0]}, {posArray[1]}");

        var comb = GetCombination(conVertices, posArray);
        var nearestVertex = conVertices[comb[SHORTEST][0]];
        var posForNearestVertex = posArray[comb[SHORTEST][1]];
        var otherVertex = conVertices[comb[OTHER][0]];
        var posForOtherVertex = posArray[comb[OTHER][1]];
            
        nearestVertex.transform.position = posForNearestVertex;
        otherVertex.transform.position = posForOtherVertex;
    }

    private void ReDistanceGraphs(GameObject edge)
    {
        var edgeEAngle = edge.transform.localRotation.eulerAngles;
        var edgeCenter = edge.transform.position;
        var edgeExtent = edge.GetComponent<SpriteRenderer>().bounds.extents;
        var conVertices = edge.GetComponent<Edge>().connectedVertices;
        
        var graphToMove1 = GetConnectedGraph(conVertices[0], edge);
        var graphToMove2 = GetConnectedGraph(conVertices[1], edge);
            
            

        const int SHORTEST = 0;
        const int OTHER = 1;

        var posArray = GetCornerPosArray(edgeEAngle, edgeCenter, edgeExtent);
            
        Debug.Log($"{edgeEAngle.z}, {edge.GetComponent<SpriteRenderer>().bounds.extents}, {edge.transform.localScale.x}, {edgeExtent.x}");
        Debug.Log($"{posArray[0]}, {posArray[1]}");
            
        var comb = GetCombination(conVertices, posArray);
        var nearestVertex = conVertices[comb[SHORTEST][0]];
        var posForNearestVertex = posArray[comb[SHORTEST][1]];
        var otherVertex = conVertices[comb[OTHER][0]];
        var posForOtherVertex = posArray[comb[OTHER][1]];

        var nearestGraph = GetGraphWhichTheVertexOrEdgeBelongsTo(nearestVertex, new List<List<GameObject>> {graphToMove1, graphToMove2});
        var otherGraph = GetGraphWhichTheVertexOrEdgeBelongsTo(otherVertex, new List<List<GameObject>> {graphToMove1, graphToMove2});


        if(IsCircular(nearestGraph, otherGraph)) {  //두 그래프가 동일할 때 그 그래프는 순환한다.
            var centerVertex = GetCenterVertex(nearestVertex, otherVertex);
            IncreaseDistUntilMatch(nearestVertex, otherVertex, centerVertex);
            return;
        }
        
        
        
        var nearestTransference = posForNearestVertex - nearestVertex.transform.position;
        var otherTransference = posForOtherVertex - otherVertex.transform.position;
        

        foreach(var graphComponent in nearestGraph)
            graphComponent.transform.position += nearestTransference;
        foreach(var graphComponent in otherGraph)
            graphComponent.transform.position += otherTransference;


        var graphComponents = GameObject.FindGameObjectsWithTag("Vertex").ToList();
        graphComponents.AddRange(GameObject.FindGameObjectsWithTag("Edge"));
        foreach(var obj in graphComponents) {
            if(obj.CompareTag("Vertex"))
                obj.GetComponent<Vertex>().isInvolved = false;
            else if(obj.CompareTag("Edge"))
                obj.GetComponent<Edge>().isInvolved = false;
            else 
                throw new Exception("Given GameObject is neither a vertex nor an edge.");
        }
        
        //bug 순환간선이 되는 간선을 생성할 시 그래프가 움직이지 않음
        //순환간선이 되면 그래프가 움직이지 않는 원인을 조사한 뒤 방법을 모색할것.
        //그래프를 움츠리거나 벌려야 함  sin cos를 이용한 오브젝트 원형이동
    }
    

    /*NoLongerUsedMethods
    
    
    private GameObject FindNearestObjectByTag(Vector3 basePosition, string tag)
    {
        // 탐색할 오브젝트 목록을 List 로 저장합니다.
        var objects = GameObject.FindGameObjectsWithTag(tag).ToList();

        // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
        var nearestObject = objects
                             .OrderBy(obj => Vector3.Distance(basePosition, obj.transform.position))
                             .FirstOrDefault();

        return nearestObject;
    }
    
    private GameObject FindNearestObjectInSpecificObject(Vector3 basePosition, List<GameObject> targets)
    {
        // 탐색할 오브젝트 목록을 List 로 저장합니다.
        var objects = targets.ToList();

        // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
        var nearestObject = objects
                            .OrderBy(obj => Vector3.Distance(basePosition, obj.transform.position))
                            .FirstOrDefault();

        return nearestObject;
    }*/

    
    
}
