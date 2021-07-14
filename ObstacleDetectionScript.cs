using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetectionScript : MonoBehaviour
{
    public GameObject explosion;
    List<GameObject> explosions = new List<GameObject>();
    public void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "rocket") {
            col.gameObject.GetComponent<Rocket>().setCrash(true);
            explosions.Add(Instantiate(explosion, col.gameObject.GetComponent<Rocket>().transform.position, col.gameObject.GetComponent<Rocket>().transform.rotation));
            col.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
