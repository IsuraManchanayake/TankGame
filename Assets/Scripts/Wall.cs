using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    // Use this for initialization
    public GameObject next;

    int count = 0;

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter2D(Collision2D col)
    {
        
        Debug.logger.Log("colided"+ col.gameObject.tag);
        if (col.gameObject.tag == "Bullet")
        {
            if(count == 12)
            {
                Destroy(gameObject);
                count = 0;
            }

            count++;

            /*Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Destroy(gameObject);
            if (next != null)
            {
                Instantiate(next, position, rotation);
                Debug.logger.Log("colided opa");
            }*/
        }
    }
}
