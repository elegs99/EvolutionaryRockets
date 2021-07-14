using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMovementScript : MonoBehaviour
{
    float rand;
    public int rotate;

    Vector3 screenPoint;
    bool obstacleMove = false;
    void Start()
    {
        obstacleMove = false;
        do {
            rand = UnityEngine.Random.Range(-0.2f, 0.2f);
        } while (rand > -0.1 && rand < 0.1);
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * rand * rotate);
    }
    void LateUpdate()
    {
        screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        screenPoint.z = 1f;
        if (obstacleMove == true){
            transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        }
    }

    private void OnMouseDown(){
        float xDist = Camera.main.WorldToScreenPoint(transform.position).x - screenPoint.x;
        float yDist = Camera.main.WorldToScreenPoint(transform.position).y - screenPoint.y;
        float distToObj = Mathf.Sqrt((xDist * xDist) + (yDist * yDist));
        if (distToObj > transform.localScale.magnitude * 1f) {
            obstacleMove = true;
        }
    }
    void OnMouseUp()
    {
        obstacleMove = false;
    }
}
