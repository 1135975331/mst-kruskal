using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using static DefaultNamespace.VectorUtil;
using UnityEngine;

public class Edge : MonoBehaviour
{
    private GenerateEdge generateEdge;
    private GameObject edgeWeightedValObj;
    public bool usedByKruskal;
    
    public int weightedValue;
    public List<GameObject> connectedVertices = new List<GameObject>();
    
    public bool isInvolved;  //GenerateEdge의 GetConnectedGraph함수에서 대상 그래프에 포함되었는가의 여부
    void Start()
    {
        generateEdge = GameObject.Find("GenerateEdgeButton").GetComponent<GenerateEdge>();
        
        Debug.Log(GameObject.FindGameObjectsWithTag("EdgeNumber").Length);
        
        edgeWeightedValObj = GameObject.FindGameObjectsWithTag("EdgeNumber")
                               .Where(obj => obj.name.Contains(Util.GetNumberStr(this.gameObject)))
                               .Select(obj => obj)
                               .FirstOrDefault(obj => obj != null);

        StartCoroutine(UpdateRotation());
    }

    // Update is called once per frame
    IEnumerator UpdateRotation()
    {
        while(generateEdge.isConfiguring) {
            if(connectedVertices.Count >= 2) {
                var conVertex1Pos = connectedVertices[0].transform.position;
                var conVertex2Pos = connectedVertices[1].transform.position;
                var zRotation = GetZRotation(conVertex1Pos, conVertex2Pos);
                var midPoint = GetMidPoint2D(conVertex1Pos, conVertex2Pos);

                transform.rotation = Quaternion.Euler(zRotation);
                transform.position = midPoint;
                edgeWeightedValObj.transform.position = midPoint;

                //Debug.Log(transform.eulerAngles);
            }
            
            var waitForMoment = new WaitForSeconds(0.2f); //0.2sec == 48fps
            yield return waitForMoment;
        }
    }
}
