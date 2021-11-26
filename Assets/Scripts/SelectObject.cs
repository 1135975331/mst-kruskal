using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public List<GameObject> selectedVertices = new List<GameObject>();
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new Vector2(mousePos.x, mousePos.y);

            var hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            
            if (hit.collider == null)  return;  //hit.collider.gameObject = clickedObject
            
            
            var clickedObject = hit.collider.gameObject;
            
            if(!clickedObject.CompareTag("Vertex")) return;
            
            var vertexComp = clickedObject.GetComponent<Vertex>();
            clickedObject.GetComponent<Vertex>().isSelected = !vertexComp.isSelected;

            if(selectedVertices.Count >= 2 && vertexComp.isSelected) //코드 직관성 개선 필요
                return;
                
                
            if(clickedObject.GetComponent<Vertex>().isSelected)
                ObjectSelected(clickedObject);
            else
                ObjectDeselected(clickedObject);
        }
    }
    
    private void ObjectSelected(GameObject pObject)
    {
        pObject.GetComponent<SpriteRenderer>().color = Color.green;

        selectedVertices.Add(pObject);
    }

    private void ObjectDeselected(GameObject pObject)
    {
        pObject.GetComponent<SpriteRenderer>().color = Color.white;

        for(var i = 0; i < selectedVertices.Count; i++) {
            if(selectedVertices[i] != pObject)  continue;
            selectedVertices.RemoveAt(i);
            break;
        }
    }
}
