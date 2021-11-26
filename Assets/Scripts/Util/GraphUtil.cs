using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public static class GraphUtil
    {
        public static List<GameObject> GetConnectedGraph(GameObject vertexOrEdge, GameObject recentGeneratedEdge)
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

        public static void FindNearestVertexOrEdge(List<GameObject> graph, GameObject vertexOrEdge, GameObject recentGeneratedEdge)
        {
            if(vertexOrEdge.CompareTag("Vertex")) {
                var vertex = vertexOrEdge;

                var conEdges = vertex.GetComponent<Vertex>().connectedEdges.Where(edge => edge != recentGeneratedEdge).ToList();
                foreach(var edge in conEdges.Where(edge => !edge.GetComponent<Edge>().isInvolved)) {
                    edge.GetComponent<Edge>().isInvolved = true;
                    graph.Add(edge);
                    FindNearestVertexOrEdge(graph, edge, recentGeneratedEdge);
                }
            }
            else if(vertexOrEdge.CompareTag("Edge")) {
                var edge = vertexOrEdge;

                var conVertices = edge.GetComponent<Edge>().connectedVertices;
                foreach(var vertex in conVertices.Where(vertex => !vertex.GetComponent<Vertex>().isInvolved)) {
                    vertex.GetComponent<Vertex>().isInvolved = true;
                    graph.Add(vertex);
                    FindNearestVertexOrEdge(graph, vertex, recentGeneratedEdge);
                }
            }
            else
                throw new Exception("Given GameObject is neither a vertex nor an edge.");
        }

        public static List<GameObject> GetGraphWhichTheVertexOrEdgeBelongsTo(GameObject vertexOrEdge, List<List<GameObject>> graphs)
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

        public static bool IsCircular(List<GameObject> graph1, List<GameObject> graph2)
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
}