using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    // Use this for initialization
    float timeLeft;
    bool b = true;
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!b)
        
        {
            timeLeft -= Time.deltaTime * 1000;
            if (timeLeft < 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void setValues(int timeLeft)
    {
        this.timeLeft = timeLeft;
        b = false;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Tank")
        {
            col.gameObject.SendMessage("healthGained");
            Destroy(gameObject);
        }
    }
}
