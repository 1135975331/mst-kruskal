using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static DefaultNamespace.Util;
using static DefaultNamespace.VectorUtil;
using static DefaultNamespace.GraphUtil;
using UnityEngine;

public class Kruskal : MonoBehaviour
{
	GameObject algorithmStartButton;
	
	private List<GameObject> vertices = new List<GameObject>();
	private List<GameObject> edges = new List<GameObject>();
	private List<int> parent;
	private int totalWeightedVal = 0;
	
	void Start()
	{
		algorithmStartButton = GameObject.Find("AlgorithmStartButton");
		parent = vertices.ToList().Select(GetNumberInt).ToList();
		//==> parent = vertices.ToList().Select(obj => Util.GetNumberInt(obj)).ToList();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void OnClickButton()
	{
		vertices = GameObject.FindGameObjectsWithTag("Vertex").ToList();
		edges = GameObject.FindGameObjectsWithTag("Edge").ToList();
		
		KruskalAlgorithm();
	}

	private void KruskalAlgorithm()
	{
		foreach(var vertex in vertices)
			parent.Add(GetNumberInt(vertex));

		parent.Sort();

		Debug.Log(edges);

		/*foreach(var edge in edges) {
			var connectedVertices = edge.GetComponent<Edge>().connectedVertices;
			UnionParent(parent, GetNumberInt(connectedVertices[0]), GetNumberInt(connectedVertices[1]));
		}


		for(var a = 0; a < vertices.Count; a++)
			for(var b = a; b < vertices.Count; b++) {
				Debug.Log($"{GetNumberInt(vertices[a])} {GetNumberInt(vertices[b])} ==> " +
				          $"{FindParent(parent, GetNumberInt(vertices[a]), GetNumberInt(vertices[b]))}");
			}*/


		var sortedEdges = edges.OrderBy(edge => edge.GetComponent<Edge>().weightedValue).ToList();
		foreach(var edge in sortedEdges)
			edge.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0.3f);
		
		StartCoroutine(AlgorithmStart(vertices, sortedEdges));
	}

	private int seq = 0;
	private IEnumerator AlgorithmStart(List<GameObject> pVertices, List<GameObject> pSortedEdges)
	{
		while(seq < pSortedEdges.Count) {
			var curEdge = pSortedEdges[seq];
			curEdge.GetComponent<SpriteRenderer>().color = Color.green;
			curEdge.GetComponent<Edge>().usedByKruskal = true;

			var curVertices = curEdge.GetComponent<Edge>().connectedVertices;
			foreach(var curVertex in curVertices) {
				curVertex.GetComponent<SpriteRenderer>().color = Color.yellow;
				curVertex.GetComponent<Vertex>().usedByKruskal = true;
			}

			yield return new WaitForSeconds(2f);

			
			/*Code from GenerateEdge.cs*/
			var edgeEAngle = curEdge.transform.localRotation.eulerAngles;
			var edgeCenter = curEdge.transform.position;
			var edgeExtent = curEdge.GetComponent<SpriteRenderer>().bounds.extents;

			var graph1 = GetConnectedGraph(curVertices[0], curEdge);
			var graph2 = GetConnectedGraph(curVertices[1], curEdge);

			const int SHORTEST = 0;
			const int OTHER = 1;

			var posArray = GetCornerPosArray(edgeEAngle, edgeCenter, edgeExtent);

			Debug.Log($"{edgeEAngle.z}, {curEdge.GetComponent<SpriteRenderer>().bounds.extents}, {curEdge.transform.localScale.x}, {edgeExtent.x}");
			Debug.Log($"{posArray[0]}, {posArray[1]}");

			var comb = GetCombination(curVertices, posArray);
			var nearestVertex = curVertices[comb[SHORTEST][0]];
			var posForNearestVertex = posArray[comb[SHORTEST][1]];
			var otherVertex = curVertices[comb[OTHER][0]];
			var posForOtherVertex = posArray[comb[OTHER][1]];

			var nearestGraph = GetGraphWhichTheVertexOrEdgeBelongsTo(nearestVertex, new List<List<GameObject>> {graph1, graph2});
			var otherGraph = GetGraphWhichTheVertexOrEdgeBelongsTo(otherVertex, new List<List<GameObject>> {graph1, graph2});
			/**/

			seq++;

			foreach(var curVertex in curVertices) {
				curVertex.GetComponent<SpriteRenderer>().color = Color.white;
				curVertex.GetComponent<Vertex>().usedByKruskal = true;
			}

			if(IsCircular(nearestGraph, otherGraph)) { //순환할떄, todo 여기서는 간선이 선택된 상태인지도 확인해줄것
				curEdge.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 0.3f);
				yield return new WaitForSeconds(0.5f);
				curEdge.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0.1f);
				yield return new WaitForSeconds(1f);
				curEdge.GetComponent<Edge>().usedByKruskal = true;
				continue;
			}
			
			totalWeightedVal += curEdge.GetComponent<Edge>().weightedValue;
			curEdge.GetComponent<SpriteRenderer>().color = Color.black;
			curEdge.GetComponent<Edge>().usedByKruskal = true;
			yield return new WaitForSeconds(1.5f);
		}
	}
		

	//Union-Find
	private static int GetParent(List<int> parent, int vertexNum)
	{
		if(parent[vertexNum] == vertexNum)  return vertexNum;
		return parent[vertexNum] = GetParent(parent, parent[vertexNum]);
	}

	private void UnionParent(List<int> parent, int vertexNum1, int vertexNum2)  //부모 정점 합치기
	{
		vertexNum1 = GetParent(parent, vertexNum1);	
		vertexNum2 = GetParent(parent, vertexNum2);
		if(vertexNum1 < vertexNum2) parent[vertexNum2] = vertexNum1;
		else parent[vertexNum1] = vertexNum2;
	}

	private bool FindParent(List<int> parent, int vertexNum1, int vertexNum2)
	{
		vertexNum1 = GetParent(parent, vertexNum1);
		vertexNum2 = GetParent(parent, vertexNum2);

		return vertexNum1 == vertexNum2;
	}
	
	//https://m.blog.naver.com/ndb796/221230967614


	private static List<GameObject> GetConnectedGraph(GameObject vertexOrEdge, GameObject recentGeneratedEdge)
	{
		var graph = new List<GameObject>();

		if(vertexOrEdge.CompareTag("Vertex")) {
			vertexOrEdge.GetComponent<Vertex>().isInvolved = true;
			graph.Add(vertexOrEdge);
		}
		else if(vertexOrEdge.CompareTag("Edge")) {
			vertexOrEdge.GetComponent<Edge>().isInvolved = true;
			graph.Add(vertexOrEdge);
		}

		FindNearestVertexOrEdge(graph, vertexOrEdge, recentGeneratedEdge);
		
		foreach(var vOrE in graph) {
			if(vOrE.CompareTag("Vertex"))
				vOrE.GetComponent<Vertex>().isInvolved = false;
			else if(vOrE.CompareTag("Edge"))
				vOrE.GetComponent<Edge>().isInvolved = false;
		}

		return graph;
	}

	private static void FindNearestVertexOrEdge(List<GameObject> graph, GameObject vertexOrEdge, GameObject recentGeneratedEdge)
	{
		if(vertexOrEdge.CompareTag("Vertex")) {
			var vertex = vertexOrEdge;

			var conEdges = vertex.GetComponent<Vertex>().connectedEdges.Where(edge => edge != recentGeneratedEdge).ToList();
			foreach(var edge in conEdges.Where(edge => !edge.GetComponent<Edge>().isInvolved && edge.GetComponent<Edge>().usedByKruskal)) {
				edge.GetComponent<Edge>().isInvolved = true;
				graph.Add(edge);
				FindNearestVertexOrEdge(graph, edge, recentGeneratedEdge);
			}
		}
		else if(vertexOrEdge.CompareTag("Edge")) {
			var edge = vertexOrEdge;

			var conVertices = edge.GetComponent<Edge>().connectedVertices;
			foreach(var vertex in conVertices.Where(vertex => !vertex.GetComponent<Vertex>().isInvolved && vertex.GetComponent<Vertex>().usedByKruskal)) {
				vertex.GetComponent<Vertex>().isInvolved = true;
				graph.Add(vertex);
				FindNearestVertexOrEdge(graph, vertex, recentGeneratedEdge);
			}
		}
		else
			throw new Exception("Given GameObject is neither a vertex nor an edge.");
	}
	
	private static List<GameObject> GetGraphWhichTheVertexOrEdgeBelongsTo(GameObject vertexOrEdge, List<List<GameObject>> graphs)
	{
		//정점이나 간선 그리고 그래프 두 개를 매개변수로 받고 그래프 속 오브젝트들을 foreach로 돌아다니며 같은지 확인하고 같은게 발견되면 그 그래프를 반환
		if(vertexOrEdge.CompareTag("Vertex")) {
			var selVertex = vertexOrEdge;
			foreach(var graph in graphs) {
				if(graph.Any(graphVertex => graphVertex.CompareTag("Vertex") && graphVertex == selVertex))
					return graph;
			}

			return new List<GameObject> {selVertex};
		} 

		if(vertexOrEdge.CompareTag("Edge")) {
			var selEdge = vertexOrEdge;
			foreach(var graph in graphs) {
				if(graph.Any(graphEdge => graphEdge.CompareTag("Vertex") && graphEdge == selEdge))
					return graph;
			}

			return new List<GameObject> {selEdge};
		}

		throw new Exception("Given GameObject is neither a vertex nor an edge.");
	}
	private static bool IsCircular(List<GameObject> graph1, List<GameObject> graph2)
	{
		//순환하는 경우는 주어진 두 그래프가 같은 원소를 포함하고 있을때
		if(graph1.Count == graph2.Count) {
			var identityCount = 0;

			foreach(var vertexOrEdge1 in graph1)
				foreach(var vertexOrEdge2 in graph2) {
					if(vertexOrEdge1 != vertexOrEdge2) continue;

					identityCount++;
					break;
				}

			return identityCount == graph1.Count;
		}
		
		return false;
	}
}


/*
 * 1. 정점 개수를 입력받는다
 * 2. 간선 갯수 만큼 간선의 가중치를 입력받는다.
 * 3. Scene에 정점과 간선을 시각화한다.
 * 3-1. 각 간선의 가중치 만큼 떨어져 있도록 하여 정점들을 랜덤한 위치에 배치한다.
 * 4. 간선 선택에 1초 정도의 시간을 두며 간선을 선택하고, 선택된 간선은 색을 바꾼다.
 * 4-1. 선택될 수 있는 간선들의 색은 초록색, 선택된 간선은 검은색으로 표시한다.
 * 5. 선택되지 않은 간선들을 보이지 않게 설정하여 최소 신장 트리를 보여준다.
 */