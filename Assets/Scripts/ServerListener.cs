using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;



public class ServerListener : MonoBehaviour
{

    public bool isconnection;

    Quaternion initializingRotation;
    //Initializing details
    private System.Collections.Generic.List<Vector3> walles = new System.Collections.Generic.List<Vector3>();
    private System.Collections.Generic.List<Vector3> stones = new System.Collections.Generic.List<Vector3>();
    private System.Collections.Generic.List<Vector3> waters = new System.Collections.Generic.List<Vector3>();
    private bool boardRestrict = true;

    //Temporary objects
    System.Collections.Generic.List<CoinInner> coinsToDraw = new System.Collections.Generic.List<CoinInner>();
    System.Collections.Generic.List<HealthInner> healthToDraw = new System.Collections.Generic.List<HealthInner>();

    //Prefabs that need for the scenario
    public GameObject wall;
    public GameObject stone;
    public GameObject water;
    public GameObject health;
    public GameObject coin;

    void Start()
    {
        initializingRotation = transform.rotation;
        var thread = new System.Threading.Thread(listen);
        thread.Start();
    }
    
    void Update()
    {
        if (!boardRestrict)
        {
            foreach (Vector3 vec in walles)
            {
                Instantiate(wall, vec, initializingRotation);
            }
            foreach (Vector3 vec in stones)
            {
                Instantiate(stone, vec, initializingRotation);
            }
            foreach (Vector3 vec in waters)
            {
                Instantiate(water, vec, initializingRotation);
            }
            boardRestrict = true;
        }
        while (coinsToDraw.Count > 0)
        {
            CoinInner c = coinsToDraw[0];
            coinsToDraw.RemoveAt(0);
            GameObject game = Instantiate(coin, c.getPosition(), initializingRotation) as GameObject;
            //Send data to coin. So that it can work independently later
            game.SendMessage("setValues", new int[] { c.getTimeLeft(), c.getCoinValue() });
            UnityEngine.Debug.logger.Log("Coin   " + c.getX() + "," + c.getY() + " " + c.getCoinValue() + "  time" + c.getTimeLeft());
        }
        while (healthToDraw.Count > 0)
        {
            HealthInner c = healthToDraw[0];
            healthToDraw.RemoveAt(0);
            GameObject game = Instantiate(health, c.getPosition(), initializingRotation) as GameObject;
            //Send data to coin. So that it can work independently later
            game.SendMessage("setValues", c.getTimeLeft());
            UnityEngine.Debug.logger.Log("Health   " + c.getX() + "," + c.getY()  + "  time" + c.getTimeLeft());
        }
    }
   
    void listen()
    {
        try
        {
            // create listener with ip and port
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7000);
            listener.Start();

            // continous listening 
            while (true)
            {
                // We will store the textual representation of the bytes we receive. 
                string value;

                // Accept the sender and take message as a stream. 
                using (var networkStream = listener.AcceptTcpClient().GetStream())
                {
                    // Create a memory stream to copy the message. 

                    var bytes = new System.Collections.Generic.List<byte>();

                    int asw = 0;
                    while (asw != -1)
                    {
                        asw = networkStream.ReadByte();
                        bytes.Add((Byte)asw);
                    }

                    // Convert bytes to text. 
                    value = Encoding.UTF8.GetString(bytes.ToArray());

                }
                String[] datas = value.Split(':');
                UnityEngine.Debug.logger.Log(datas[0]+"   "+value);
                // wall data
                if (datas[0].Equals("I"))
                {

                    String walls = datas[2];
                    walles = getVecotors(walls);
                    String stone = datas[3];
                    stones = getVecotors(stone);
                    String water = datas[4].Trim();
                    //At the end String there are unwanter  characters. one is # and other is a unicode
                    water = water.Substring(0, water.Length - 2);
                    waters = getVecotors(water);
                    boardRestrict = false;


                }
                // coin data
                else if (datas[0].ToUpper().Equals("C"))
                {
                    String[] cod = datas[1].Split(',');
                    Vector3 coinPosition = new Vector3(Int32.Parse(cod[0]), -Int32.Parse(cod[1]));
                    int coinCountTime = Int32.Parse(datas[2]);
                    datas[3] = datas[3].Trim();
                    datas[3] = datas[3].Substring(0, datas[3].Length - 2);
                    int coinValue = Int32.Parse(datas[3]);
                    CoinInner o = new CoinInner(coinPosition, coinValue, coinCountTime);
                    coinsToDraw.Add(o);
                    //UnityEngine.Debug.logger.Log("Coin   " + coinPosition.x+","+coinPosition.y+" "+coinValue+"  time"+coinCountTime);

                }
                // health data
                else if (datas[0].ToUpper().Equals("L"))
                {
                    String[] cod = datas[1].Split(',');
                    Vector3 healthPosition = new Vector3(Int32.Parse(cod[0]), -Int32.Parse(cod[1]));
                    datas[2] = datas[2].Trim();
                    datas[2] = datas[2].Substring(0, datas[2].Length - 2);
                    int healthCountTime = Int32.Parse(datas[2]);
                    HealthInner he = new HealthInner(healthPosition, healthCountTime);
                    healthToDraw.Add(he);
                    //UnityEngine.Debug.logger.Log("Health   " + healthPosition.x + "," + healthPosition.y + " " + healthCountTime);
                }
                // Call an external function (void) given. 

                //else if (datas[0].ToUpper().Equals("P"))
                //{
                    //Debug.logger.Log(datas.ToString());
                    // Tank Status Update [Pn;x,y;d;shot;health;coins;points]
                    /*var state = d.Split(';');
                    var currentTank = tanks[int.Parse(state[0].Substring(1))];
                    currentTank.x = int.Parse(state[1].Split(',')[0]);
                    currentTank.y = int.Parse(state[1].Split(',')[1]);
                    currentTank.direction = (Direction)int.Parse(state[2]);
                    currentTank.isShot = int.Parse(state[3]) == 1;
                    currentTank.health = int.Parse(state[4]);
                    currentTank.coins = int.Parse(state[5]);
                    currentTank.points = int.Parse(state[6]);*/
                //}

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occured");
        }
    }
    /*
    A set off cordinates (2,3:2,4:....) will be passed to this method as a string.
    This method will decode them and create list of vectors fromt those cordinates..
    */
    System.Collections.Generic.List<Vector3> getVecotors(String data)
    {
        String[] cod = data.Split(';');
        System.Collections.Generic.List<Vector3> vectors = new System.Collections.Generic.List<Vector3>();

        foreach (String e in cod)
        {
            String[] d = e.Split(',');
            //Y is made minus because our grid system in unity is creates as such.
            Vector3 v = new Vector3(Int32.Parse(d[0]), -Int32.Parse(d[1]), 0);
            vectors.Add(v);
        }
        return vectors;

    }
    class CoinInner
    {
        Vector3 position;
        int coinValue;
        int timeLeft;
        public float getX()
        {
            return position.x;
        }
        public float getY()
        {
            return position.y;
        }
        public int getCoinValue()
        {
            return coinValue;
        }
        public int getTimeLeft()
        {
            return timeLeft;
        }
        public Vector3 getPosition()
        {
            return position;
        }
        public CoinInner(Vector3 pos, int coin, int time)
        {
            this.position = pos;
            this.coinValue = coin;
            this.timeLeft = time;
        }
    }
    class HealthInner
    {
        Vector3 position;
        int timeLeft;
        public float getX()
        {
            return position.x;
        }
        public float getY()
        {
            return position.y;
        }
        public Vector3 getPosition()
        {
            return position;
        }
        public int getTimeLeft()
        {
            return timeLeft;
        }
        public HealthInner(Vector3 pos, int time)
        {
            this.position = pos;
            this.timeLeft = time;
        }
    }


}

 