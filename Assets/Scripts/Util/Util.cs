using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
	public static class Util
	{
		public static string GetNumberStr(GameObject vertexOrEdge)
		{
			return vertexOrEdge.name.Split('_')[1];
		}
		
		public static int GetNumberInt(GameObject vertexOrEdge)
		{
			return int.Parse(vertexOrEdge.name.Split('_')[1]);
		}
		
		public static int[][] GetCombination(List<GameObject> conVertices, Vector3[] posArray)
		{
			int[] shortestCombination = {-1, -1};
			int[] otherCombination = {-1, -1};
        
			for(var a = 0; a < conVertices.Count; a++) {
				var shortestDist = Mathf.Infinity;
                

				for(var b = 0; b < posArray.Length; b++) {
					var distance = Vector3.Distance(conVertices[a].transform.position, posArray[b]);
                    
					if(!(distance < shortestDist)) continue;
					shortestDist = distance;
					shortestCombination[0] = a;
					shortestCombination[1] = b;
					otherCombination[0] = 1 - a;
					otherCombination[1] = 1 - b;
				}
			}

			int[][] combination = {shortestCombination, otherCombination};

			return combination;
		}
	}
}