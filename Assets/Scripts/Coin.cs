using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    // Use this for initialization
    int value;
    float timeLeft;
    bool b = true;
    
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (!b)
        {

            timeLeft -= Time.deltaTime*1000;
            if (timeLeft < 0)
            {
                Destroy(gameObject);
            }
        }
        
	}

    public void setValues(int[] data)
    {
        this.timeLeft = data[0];
        this.value = data[1];
        b = false;

    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Tank")
        {
            col.gameObject.SendMessage("coinAdded" ,value);
            Destroy(gameObject);
        }
    }
}
