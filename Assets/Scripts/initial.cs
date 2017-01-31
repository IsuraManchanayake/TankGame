using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System;
using System.Threading;
using UnityEngine.UI;

public class initial : MonoBehaviour {
	//Constants AI implementation 
	private ArrayList bonus = new ArrayList ();
	private bool isForward = true;
	
	//Variables
	public static ArrayList bricks = new ArrayList();
	public static ArrayList stones = new ArrayList();
	public static ArrayList water = new ArrayList();

	public static ArrayList messages = new ArrayList();

	private string ourself;
	private int[] loc = new int[3];
	private ArrayList otherLoc;


	//Constants
	public static int[] directions=new int[] { 0, 270, 180, 90 };

	public static int[][] shellDirections = new int[][]{ new int[] {0,1}, new int[] {1,0}, new int[] {0,-1}, new int[] {-1,0}};

	public static float[] positions=new float[] { -4.5f, -3.5f, -2.5f, -1.5f, -0.5f, 0.5f, 1.5f, 2.5f, 3.5f, 4.5f };
	public static string[] tankNameMap = new string[] {"P0", "P1", "P2", "P3", "P4"};
	public static ArrayList tanks = new ArrayList ();
	public static GameObject[] labels = new GameObject[5];
	public static GameObject[] headers = new GameObject[5];
	public static GameObject status = null;
	public static float[] damages = new float[]{2.0f, 1.5f, 1.0f, 0.5f, 0f};



	//Thread requirements
	private static string previousMsg="";
	private static TcpListener listener = null;
	private static Thread joining;




	// Use this for initialization
	void Start () {

		for (int i = 0; i < 5; i++) {
			labels[i] = GameObject.Find("Canvas/" + tankNameMap[i] + "/Health");
			headers [i] = GameObject.Find ("Canvas/" + tankNameMap [i]);
			headers[i].SetActive (false);
		}
		status = GameObject.Find ("Canvas/STATUS");
		status.SetActive (false);

		tanks.Add (GameObject.Find("tank"));
		tanks.Add (GameObject.Find("tank4"));
		tanks.Add (GameObject.Find("tank5"));
		tanks.Add (GameObject.Find("tank3"));
		tanks.Add (GameObject.Find("tank2"));


		Thread listeningThread = new Thread(startListening);

		joining = new Thread(SendJoin);

		listeningThread.Start();
		joining.Start ();
	}

	// Update is called once per frame
	void Update ()
	{
		//controllers manual override
		bool w = Input.GetKeyUp ("w");
		bool d = Input.GetKeyUp ("d");
		bool s = Input.GetKeyUp ("s");
		bool a = Input.GetKeyUp ("a");
		bool l = Input.GetKeyUp ("l");

		if (a) {
			sendToServer ("LEFT#");
		} else if (d) {
			sendToServer ("RIGHT#");
		}else if (w) {
			sendToServer ("UP#");
		} else if (s) {
			sendToServer ("DOWN#");
		}else if(l){
			sendToServer ("SHOOT#");
		}


		try{
			if (initial.messages.Count != 0) {
				String msgReceived = initial.messages [0].ToString ();
				initial.messages.RemoveAt (0);

				msgReceived = msgReceived.Substring (0, msgReceived.Length - 1);

				string commandType = msgReceived.Substring(0, 2);

				msgReceived = msgReceived.Substring (2);

				if (commandType == "I:") {//Game initialization
					joining.Abort();
					//P<num>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y>
					msgReceived = msgReceived.Substring (3);
					string[] category = msgReceived.Split (':');

					//Bricks 
					foreach (string coordinate in category[0].Trim().Split(';')) {
						string[] xy = coordinate.Trim ().Split (',');
						int x = int.Parse (xy [0].Trim ());
						int y = int.Parse (xy [1].Trim ());

						//place bricks
						GameObject tem = Instantiate (GameObject.Find ("brick"));
						tem.transform.position = new Vector3 (initial.positions[x], initial.positions[9-y], 0f);
						bricks.Add(tem);

					}

					//Stone  
					foreach (string coordinate in category[1].Trim().Split(';')) {
						string[] xy = coordinate.Trim ().Split (',');
						int x = int.Parse (xy [0].Trim ());
						int y = int.Parse (xy [1].Trim ());

						//place Stone
						GameObject tem = Instantiate (GameObject.Find ("stone"));
						tem.transform.position = new Vector3 (initial.positions[x], initial.positions[9-y], 0f);
						stones.Add(tem);
					}

					//water  
					foreach (string coordinate in category[2].Trim().Split(';')) {
						string[] xy = coordinate.Trim ().Split (',');
						int x = int.Parse (xy [0].Trim ());
						int y = int.Parse (xy [1].Trim ());

						//place water
						GameObject tem = Instantiate (GameObject.Find ("water"));
						tem.transform.position = new Vector3 (initial.positions[x], initial.positions[9-y], 0f);
						water.Add(tem);
					}
				
				} else if (commandType == "S:") { //Join acceptance
					//P0;0,0;0:P1;0,9;0:P2;9,0;0:P3;9,9;0:P4;5,5;0
					string[] part = msgReceived.Split(':');

					string[] parts = part[part.Length-1].Split(';');
						
					ourself = "P1"; //parts [0].Trim();

					Debug.LogWarning(ourself);

					string[] xy = parts [1].Split (',');

					int x = int.Parse (xy [0].Trim ());

					int y = int.Parse (xy [1].Trim ());

					int direct = int.Parse (parts [2].Trim ());

					for (int i=0; i<5; i++){
						if(tankNameMap[i]==ourself){
							((GameObject) tanks[i]).transform.position = new Vector3 (initial.positions[x], initial.positions[9-y], 0f);
							((GameObject) tanks[i]).transform.rotation = Quaternion.Euler(0, 0, initial.directions[direct]); 
						}
					}

					//update location
					loc[0]=x;
					loc[1]=y;
					loc[2]=direct;


				} else if (commandType == "G:") {
					otherLoc = new ArrayList();
					string[] category = msgReceived.Split (':');

					foreach (string playerDetails in category) {
						string[] parts = playerDetails.Split (';');



						for (int i=0; i<5; i++){

							if (parts[0] == tankNameMap[i]) {


								//geting the location
								string[] position = parts [1].Split (',');
								int x = int.Parse (position [0].Trim ());
								int y = int.Parse (position [1].Trim ());

								//getting the direction
								int direct = int.Parse (parts [2].Trim ());

								//shot
								int shot = int.Parse(parts[3].Trim());

								//health
								int health = int.Parse(parts[4].Trim());

								//coins
								int coins = int.Parse(parts[5].Trim());


								//shoot
								if (shot==1){
									GameObject tem = Instantiate (GameObject.Find ("bullet"));

									tem.transform.rotation = Quaternion.Euler(0, 0, initial.directions[direct]); 

									tem.transform.position = new Vector3 (initial.positions[x+ shellDirections[direct][0]], initial.positions[9-y+shellDirections[direct][1]], 0f);

									tem.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(shellDirections[direct][0]*3, shellDirections[direct][1]*3);

								}

								//update scores
								setScore(i, health, coins);
	

								//set the positions in actual game object
								((GameObject) tanks[i]).transform.position = new Vector3 (initial.positions[x], initial.positions[9-y], 0f);
								((GameObject) tanks[i]).transform.rotation = Quaternion.Euler(0, 0, initial.directions[direct]); 

								if(health==0){
									((GameObject) tanks[i]).SetActive(false);
								}

								//update the location of ourtank
								if (tankNameMap[i]==ourself){
									loc[0]=x;
									loc[1]=y;
									loc[2]=direct;
									Debug.LogWarning(x.ToString()+y.ToString()+direct.ToString());
								}else{
									if (health!=0){
										otherLoc.Add(new int[]{x, y, direct});
									}
								}
							}
						}

						string [] temp = parts[0].Split(',');
						if(temp.Length==3){

							foreach (string brickDamage in parts){
								
								string [] tem = brickDamage.Split(',');

								int x = int.Parse(tem[0]);
								int y = int.Parse(tem[1]);
								int damage = int.Parse(tem[2]);

								foreach(GameObject g in bricks){
									Transform trans = g.GetComponent<Transform>();

									if (trans.position.x==positions[x] && trans.position.y==positions[9-y]){
										trans.localScale = new Vector3(2f, damages[damage], 0f);
										if (damage==4){
											bricks.Remove(g);
											Destroy(g);
										}
									}
								}

							}
						}
					}
					//AI script
					//Main issue was we cannot keep track the health, coin bonuses due to inconsistency
					//coner protected approach
					//put your AI script here



				}else if (commandType == "C:") {
					//0,2:60831:1569
					string[] parts = msgReceived.Split(':');

					string[] xy = parts[0].Split(',');
					int x = int.Parse(xy[0].Trim());
					int y = int.Parse(xy[1].Trim());

					float time = float.Parse(parts[1].Trim());

					//int value= int.Parse(parts[2].Trim());

					GameObject tem = Instantiate (GameObject.Find ("coin"));
					Destroy(tem, 80);
					tem.transform.position = new Vector3 (initial.positions[x], initial.positions[9-y], 0f);

					if (y==loc[1]){
						bonus.Add(tem);
					}



				}else if (commandType == "L:") {
					//0,2:60831:1569
					string[] parts = msgReceived.Split(':');

					string[] xy = parts[0].Split(',');
					int x = int.Parse(xy[0].Trim());
					int y = int.Parse(xy[1].Trim());

					float time = float.Parse(parts[1].Trim());

					GameObject tem = Instantiate (GameObject.Find ("medic"));
					Destroy(tem, 80);
					tem.transform.position = new Vector3 (initial.positions[x], initial.positions[9-y], 0f);

					if (y==loc[1]){
						bonus.Add(tem);
					}


				}else if(commandType == "PI"){
					setStatus();
				}
			}


		}catch(Exception){

		}



/*
 * S:P1: 1,1:0#
 * I:P<num>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y># 	P0:bricks:stone:water#
 * G:P1;< player location  x>,< player location  y>;<Direction>;< whether shot>;<health>;< coins>;< points>: …. P5;< player location  x>,< player location  y>;<Direction>;< whether shot>;<health>;< coins>;< points>: < x>,<y>,<damage-level>;< x>,<y>,<damage-level>;< x>,<y>,<damage-level>;< x>,<y>,<damage-level>…..< x>,<y>,<damage-level>#
 * C:<x>,<y>:<LT>:<Val>#
 * L:<x>,<y>:<LT>#
 * GAME_FINISHED# 
 * 
 */


	}


	GameObject getBonus (){
		int notNullIndex = -1;
		for (int index = 0; index < bonus.Count; index++) {
			if (bonus[index] != null) {
				notNullIndex = index;
			}
		}

		if (notNullIndex == -1) {
			bonus.RemoveRange (0, bonus.Count);
			return null;
		} else {
			bonus.RemoveRange (0, notNullIndex);
			return (GameObject) bonus [0];
		}
	}

	void setScore(int player, int health, int coins){
		headers[player].SetActive(true);
		labels[player].GetComponent<Text>().text = "Health: " + health.ToString() +
			"\nCoins: " + coins.ToString();
	}

	void setStatus(){
		status.SetActive (true);
		status.GetComponent<Text>().text = "Game Over!";
	}





	public static void SendJoin(){
		while (true) {
			sendToServer ("JOIN#");
			Thread.Sleep (500);
		}
	}


	public static void startListening(){
		string tem = "";
		while(true){
			try{	
				if(listener==null){
					listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7000);
					listener.Start();
				}

				//wait for msg from the server
				using(var ns = listener.AcceptTcpClient().GetStream()){
					//server msg to String
					byte [] data = new byte[1024];

					int numBytesRead;
					numBytesRead = ns.Read(data, 0, data.Length);

					var memStream = new MemoryStream();
					memStream.Write(data, 0, numBytesRead);

					tem = Encoding.UTF8.GetString(memStream.ToArray());
					//got the string

					if (previousMsg!=tem){
						previousMsg=tem;
						messages.Add(tem);
					}

				}
			}catch (Exception){
			}
		}
	}

	public static void sendToServer(string msg){
		try{
			using (var sender = new TcpClient("127.0.0.1", 6000)){
				//convert msg to byte array
				var d = Encoding.UTF8.GetBytes(msg);

				//send the array
				sender.GetStream().Write(d, 0, d.Length);
			}
		}catch(Exception){
		}
	}
		

}
