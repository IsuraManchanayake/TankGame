using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour {

    public GameObject bullet;

    // Use this for initialization
    private static readonly int UP = 0;
    private static readonly int RIGHT = 1;
    private static readonly int DOWN = 2;
    private static readonly int LEFT = 3;

    float speed = 5;
    Vector3 position;
    //preposition stores the position before a movement occurs. if a collision occurs. tank will get this cordinate
    Vector3 prePosition;
    bool b = false;
    float t = 10;
    Quaternion initRotation;
    //distance that tank goes for 0,1 seconds
    float addY = 0;
    float addX = 0;
    bool colided = false;
    int rotation = UP;
    int coinCollected;

    void Start () {
        initRotation = transform.rotation;
        position = transform.position;
        prePosition = position;
	}

    // Update is called once per frame
    void Update()
    {
        //b is used to restrict continues movemen. inputs will be taken one for one sec
        if (b)
        {
            if (t == 10)
            {
                prePosition.x = position.x;
                prePosition.y = position.y;
            }
            //t gets 10 when a key is pressed. 10 should go to 0 in one sec
            t -= Time.deltaTime*10;
            //if a collision occurs b will be false.
            if (!colided)
            {
                position.x = transform.position.x + addX*Time.deltaTime;
                position.y = transform.position.y + addY*Time.deltaTime;
                transform.position = position;
            }
            if (t <= 0)
            {
                b = false;
                t = 10;
            }
        }
        else
        {
            //Subtractions some time get answers like 2.9999 instead of 3. This is to solve that issue
            /*position.x = Mathf.Round(transform.position.x);
            position.y = Mathf.Round(transform.position.y);
            transform.position = position;*/
            if (Input.GetKey(KeyCode.RightArrow))
            {
                //if not directed on the direction. first need to direct it to the direction
                if (rotation==RIGHT && Mathf.Round(transform.position.x) < 9)
                {
                    addX = 1f;
                    addY = 0f;
                    b = true;
                    colided = false;
                }
                else
                {
                    //rotate in direction
                    transform.rotation = initRotation * Quaternion.Euler(0, 0, -90);
                    rotation = RIGHT;
                    addX = 0f;
                    addY = 0f;
                    b = true;
                    colided = false;
                }
            }else if (Input.GetKey(KeyCode.LeftArrow))
            {
                
                if (rotation==LEFT && Mathf.Round(transform.position.x )>= 1)
                {
                    addX = -1f;
                    addY = 0f;
                    b = true;
                    colided = false;
                }
                else
                {
                    transform.rotation = initRotation * Quaternion.Euler(0, 0, 90);
                    rotation = LEFT;
                    addX = 0f;
                    addY = 0f;
                    b = true;
                    colided = false;
                }
            }else if (Input.GetKey(KeyCode.DownArrow))
            {
                
                if (rotation==DOWN &&  Mathf.Round(transform.position.y) > -9)
                {
                    addY = -1f;
                    addX = 0f;
                    b = true;
                    colided = false;
                }
                else
                {
                    transform.rotation = initRotation * Quaternion.Euler(0, 0, 180);
                    rotation = DOWN;
                    addX = 0f;
                    addY = 0f;
                    b = true;
                    colided = false;
                }
            }else if (Input.GetKey(KeyCode.UpArrow))
            {
                
                if (rotation==UP && Mathf.Round(transform.position.y) <= -1)
                {
                    addX = 0f;
                    addY = 1f;
                    b = true;
                    colided = false;
                }
                else
                {
                    transform.rotation = initRotation * Quaternion.Euler(0, 0, 0);
                    rotation = UP;
                    addX = 0f;
                    addY = 0f;
                    b = true;
                    colided = false;
                }
            }else if (Input.GetKey(KeyCode.LeftControl))
            {
                Debug.logger.Log("Firing");
                Vector3 position = transform.position;
                Quaternion initialRotation = Quaternion.Euler(0, 0, 0); ;
                Debug.logger.Log("Firddd3333ing");
                if (rotation == UP)
                {
                    initialRotation = initRotation * Quaternion.Euler(0, 0, 90);
                    position.y += 1f;
                }
                else if (rotation == DOWN)
                {
                    initialRotation = initRotation * Quaternion.Euler(0, 0, -90);
                    position.y += -1f;
                }
                else if (rotation == LEFT)
                {
                    initialRotation = initRotation * Quaternion.Euler(0, 0, 180);
                    position.x -= 1f;
                }
                else
                {
                    position.x += 1f;
                }
                GameObject game = Instantiate(bullet, position, initialRotation) as GameObject;
                //game.SendMessage("setPosition",rotation);

                Debug.logger.Log("Fired");
            }
            

        }
        
    }
    public void OnCollisionEnter2D(Collision2D col)
     {

        if (col.gameObject.tag == "Brick" || col.gameObject.tag == "Stone" || col.gameObject.tag == "Water")
        {

            Debug.logger.Log("colided" + col.gameObject.tag);
            //Debug.logger.Log("Hit hit hit hit hit hit hit hit hit hit hit hit hit hit hit");
            //colided = true;
            transform.position = prePosition;
        }
    }

    public void coinAdded(int coinValue)
    {
        coinCollected += coinValue;
        Debug.logger.Log("Coin gained" + coinValue);
    }
    public void healthGained()
    {
        Debug.logger.Log("Health restores");
    }
}
