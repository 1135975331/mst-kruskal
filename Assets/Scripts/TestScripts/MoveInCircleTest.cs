using UnityEngine;
using static DefaultNamespace.VectorUtil;

public class MoveInCircleTest : MonoBehaviour
{
	public GameObject target;
	public GameObject centerObject;
	public Vector3 centerPos;
	public float radius;
	public float theta;

	void Start()
	{
		centerPos = new Vector3(centerObject.transform.position.x, centerObject.transform.position.y, centerObject.transform.position.z);
	}
	
	void Update()
	{
		//theta += 100 * Time.deltaTime;

		MoveInCircle(target, centerPos, radius, theta);
	}

	private void MoveInCircle(GameObject target, Vector3 center, float radius, float deltaTheta) //radius = edge.localScale(가중치 * 로그값)
	{
		var position = target.transform.position;
		var targetPos = position;
		var angle = GetZRotation(center, targetPos);
        
		angle.z += deltaTheta;
        

		position = center + Get2DVectorComponents(angle.z, radius);
		target.transform.position = position;
	}
}
