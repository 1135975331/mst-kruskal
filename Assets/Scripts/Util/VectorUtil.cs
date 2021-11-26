using System;
using UnityEngine;

namespace DefaultNamespace
{
	public static class VectorUtil
	{
		public static Vector3 GetMidPoint2D(Vector3 aPos, Vector3 bPos)
		{
			var midPointX = (aPos.x + bPos.x) / 2;
			var midPointY = (aPos.y + bPos.y) / 2;

			return new Vector3(midPointX, midPointY, 0.1f);
		}

		public static Vector3 GetZRotation(Vector3 fromPos, Vector3 endPos)
		{
			var deltaX = endPos.x - fromPos.x;
			var deltaY = endPos.y - fromPos.y;
			var gradient = deltaY / deltaX;

			var zRotation = Mathf.Rad2Deg * Mathf.Atan(gradient);
        
			if(deltaX > 0 && deltaY >= 0) { //제1사분면, 0~90
				//does nothing
			}
			else if(deltaX < 0 && deltaY >= 0) //제2사분면, 90~180
				zRotation += 180;
			else if(deltaX < 0 && deltaY < 0) //제3사분면, 180~270
				zRotation += 180;
			else if(deltaX > 0 && deltaY < 0) //제4사분면, 270~360(0)
				zRotation += 360;
        
			//Debug.Log($"{deltaX}, {deltaY}, {gradient}, {zRotation}");
        
			return new Vector3(0, 0, zRotation);
		}
    
		public static Vector3 Get2DVectorComponents(float angle, float length)
		{
			var xComponent = Mathf.Cos(angle * Mathf.Deg2Rad) * length;
			var yComponent = Mathf.Sin(angle * Mathf.Deg2Rad) * length;

			return new Vector3(xComponent, yComponent);
		}
		
		public static Vector3[] GetCornerPosArray(Vector3 edgeEAngle, Vector3 edgeCenter, Vector3 edgeExtent)
		{
			/*var pos1 = new Vector3(edgeCenter.x + edgeExtent.x, edgeCenter.y + edgeExtent.y, 0);
			var pos2 = new Vector3(edgeCenter.x - edgeExtent.x, edgeCenter.y - edgeExtent.y, 0);
			
			Vector3[] posArray = {pos1, pos2};
			return posArray;*/

			Vector3 pos1;
			Vector3 pos2;
        
			if(0 <= edgeEAngle.z && edgeEAngle.z < 90) { //제1사분면  + +
				pos1 = new Vector3(edgeCenter.x + edgeExtent.x, edgeCenter.y + edgeExtent.y, 0);
				pos2 = new Vector3(edgeCenter.x - edgeExtent.x, edgeCenter.y - edgeExtent.y, 0);
			}
			else if(90 <= edgeEAngle.z && edgeEAngle.z < 180) { //제2사분면 - +
				pos1 = new Vector3(edgeCenter.x - edgeExtent.x, edgeCenter.y + edgeExtent.y, 0);
				pos2 = new Vector3(edgeCenter.x + edgeExtent.x, edgeCenter.y - edgeExtent.y, 0);
			}
			else if(180 <= edgeEAngle.z && edgeEAngle.z < 270) { //제3사분면 - -
				pos1 = new Vector3(edgeCenter.x - edgeExtent.x, edgeCenter.y - edgeExtent.y, 0);
				pos2 = new Vector3(edgeCenter.x + edgeExtent.x, edgeCenter.y + edgeExtent.y, 0);
			}
			else if(270 <= edgeEAngle.z && edgeEAngle.z < 360) { //제4사분면 + -
				pos1 = new Vector3(edgeCenter.x + edgeExtent.x, edgeCenter.y - edgeExtent.y, 0);
				pos2 = new Vector3(edgeCenter.x - edgeExtent.x, edgeCenter.y + edgeExtent.y, 0);
			}
			else {
				throw new Exception("pos1 and pos2 are not initialized");
			}

			Vector3[] posArray = {pos1, pos2};
			return posArray;
		}
		
		public static Vector3 GetNearestCornerPos(GameObject vertex, GameObject edge)
		{
			var edgeEAngle = edge.transform.eulerAngles;
			var edgeCenter = edge.transform.position;
			var edgeExtent = edge.GetComponent<SpriteRenderer>().bounds.extents;
			var cornerPosArray = GetCornerPosArray(edgeEAngle, edgeCenter, edgeExtent);
			var shortestPosIndex = -1;
        
			var shortestDist = Mathf.Infinity;

			for(var a = 0; a < cornerPosArray.Length; a++) {
				var cornerPos = cornerPosArray[a];
				var distance = Vector3.Distance(vertex.transform.position, cornerPos);

				if(!(distance < shortestDist)) continue;
            
				shortestDist = distance;
				shortestPosIndex = a;
			}

			return cornerPosArray[shortestPosIndex];
		}
		
		public static Vector3 GetScale(int weightedValue)
		{
			var length2Scale = Mathf.Pow(1.008f,-weightedValue) / 6;

			return new Vector3(weightedValue * length2Scale, 0.1f, 1f);
		}
	}
}