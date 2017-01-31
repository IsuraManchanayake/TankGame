using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private static readonly int UP = 0;
    private static readonly int RIGHT = 1;
    private static readonly int DOWN = 2;
    private static readonly int LEFT = 3;
    private int rotation;
    bool b = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (b)
        {
            Vector3 position = transform.position;
            if (rotation == UP)
            {
                position.y += Time.deltaTime * 3;
            }
            else if (rotation == DOWN)
            {
                position.y -= Time.deltaTime * 3;
            }
            else if (rotation == RIGHT)
            {
                position.x += Time.deltaTime * 3;
            }
            else
            {
                position.x -= Time.deltaTime * 3;
            }
            transform.position = position;
            if (position.x > 9.5 || position.x < -0.5 || position.y > 0.5 || position.y < -9.5)
            {
                Destroy(gameObject);
            }
        }
	}
    void setPosition(int rotation)
    {
        Quaternion initRotation = transform.rotation;
        this.rotation = rotation;
        b = true;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Stone" || col.gameObject.tag == "Tank" || col.gameObject.tag == "Brick")
        {
            Destroy(gameObject);
        }
           
    }
}
