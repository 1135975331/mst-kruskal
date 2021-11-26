using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GenerateVertex : MonoBehaviour
{
    GameObject genButton;
    private InputField vertexAmountInputField;
    
    // Start is called before the first frame update
    void Start()
    { 
        genButton = GameObject.Find("StartGenerateButton");
        vertexAmountInputField = GameObject.Find("VertexAmountInputField").GetComponent<InputField>();
    }

    
    private bool isAmountLegal = false;
    public void ButtonClicked()
    {
        ParseInput();
        
        if(!isAmountLegal)  return;

        SceneManager.LoadScene("Scenes/Kruskal");
        Generate();
    }

    private int vertexAmount;
    private void ParseInput()
    {
        try {
            vertexAmount = int.Parse(vertexAmountInputField.text);
        }
        catch(FormatException ignored) {
            Debug.Log("Please input a number."); 
            return;
        }

        if(vertexAmount <= 0) {
            Debug.Log("Vertex amount should be at least 1.");
            return;
        }

        isAmountLegal = true;
        Debug.Log(vertexAmount + ", Amount is Legal.");
    }

    
    public GameObject vertexPrefab;
    List<GameObject> vertexList = new List<GameObject>();
    private void Generate()
    {
        //vertexPrefab = Resources.Load<GameObject>("Prefabs/Vertex");
        for(var i = 1; i <= vertexAmount; i++) {
            var vertex = Instantiate(vertexPrefab, transform.position, transform.rotation);
            vertex.name = "Vertex_" + i;
            vertex.transform.localPosition = new Vector3(UnityEngine.Random.Range(-6f, 6f), UnityEngine.Random.Range(-4.5f, 4.5f));
            
            var textMesh = vertex.GetComponentInChildren<TextMesh>();
            textMesh.text = $"{i}";
            
            DontDestroyOnLoad(vertex);
        }
    }
}
