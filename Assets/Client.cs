using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;	

public class Client : MonoBehaviour {

	private Socket _clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
	private byte[] _recieveBuffer = new byte[648];
	public GameObject[] objs;
	List<List<List<Vector3>>> vectors = new List<List<List<Vector3>>> ();
	List<List<Vector3>> positions = new List<List<Vector3>>();
	private int real_read;
	private bool started = false;
	private int time_read = 0;
	public int level = 1;
	public int challenge = 0;
	bool completed = false;
	List<int[]> hands_selected = new List<int[]>();
	List<GameObject> cubos;

	private void SetupServer(String ip_address)
	{
		objs = GameObject.FindGameObjectsWithTag("bolitas");
		Vector3 t = new Vector3 (0, 0, 0);
		for (int i = 0; i < 2; i++) {
			List<Vector3> list_position_fingers = new List<Vector3>();
			List<List<Vector3>> list_vectors_hands = new List<List<Vector3>>();
			for (int j = 0; j < 5; j++) {
				List<Vector3> list_vectors_finger= new List<Vector3	>();
				t = new Vector3 (0, 0, 0);
				list_position_fingers.Add(t);
				for (int k=0; k < 4; k++) {
					//String s = String.Format ("Sphere_" + (i+1).ToString() + "_"+ (j+1).ToString() +"_"+ (k+1).ToString());
					//objs.Add (GameObject.Find (s));
					t = new Vector3 (0, 0, 0);
					list_vectors_finger.Add(t);
				}

				list_vectors_hands.Add(list_vectors_finger);
			}
			positions.Add (list_position_fingers);
			vectors.Add (list_vectors_hands);
		}
		try
		{
			//_clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.43"), 8080));
			_clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip_address), 8080));
		}
		catch(SocketException ex)
		{
			Debug.Log(ex.Message);
		}
		_clientSocket.BeginReceive(_recieveBuffer,0,_recieveBuffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),null);

	}

	void swap_fingers( int lvl ){
		
	}

	void Start(){
		VRSettings.enabled = true;
		PlayerPrefs.SetInt ("started", 0);
		cubos = GameObject.FindGameObjectsWithTag("cubo").ToList();
		Physics.gravity = new Vector3(0.0f,0.0f,20f);
		SetupServer (PlayerPrefs.GetString("Ip"));
		PlayerPrefs.SetInt("yellow", 1);
		PlayerPrefs.SetInt("red", 1);
		PlayerPrefs.SetInt("blue", 1);
		PlayerPrefs.SetInt("white", 1);
	}
	void Update(){
		int yellow = PlayerPrefs.GetInt("yellow");
		int red = PlayerPrefs.GetInt("red");
		int blue = PlayerPrefs.GetInt("blue");
		int white = PlayerPrefs.GetInt("white");
		if (GameObject.FindGameObjectsWithTag ("dedos").Length == 0 && yellow == 1 && red == 1 && blue == 1 && white == 1) {
			hands_selected.Clear ();
			challenge++;
			Debug.Log ("PASAMIS CHALLENGE");
			if (challenge == 5) {
				challenge = 1;
				level++;
				swap_fingers (level);
			}
			for (int y = 0; y < challenge; y++) {
				int idx = UnityEngine.Random.Range (0, cubos.Count);
				int hand = UnityEngine.Random.Range (0, 2);
				int finger = UnityEngine.Random.Range (0, 5);
				int[] tmp = new int[3]{hand, finger, idx};
				hands_selected.Add (tmp);
				PlayerPrefs.SetInt(cubos[idx].GetComponent<Renderer>().material.name, 0);
			}
		}
		for (int ele = 0; ele < hands_selected.Count; ele++) {
			if (PlayerPrefs.GetInt (cubos[hands_selected[ele] [2]].GetComponent<Renderer>().material.name) == 1){
				hands_selected.RemoveAt (ele);
			}
		}
		objs = GameObject.FindGameObjectsWithTag ("bolitas").ToList().Concat(GameObject.FindGameObjectsWithTag ("dedos").ToList()).ToArray();
		int i = 0;
		int j = 0;
		int k = 0;
		Color c = new Color(0.0f, 0.0f, 0.0f);

		if (started) {
			c = new Color (0.0f, 1.0f, 0.0f);
		} else {
			if (real_read == 1) {
				c = new Color (1.0f, 1.0f, 0.0f);
				if (time_read < 50) {
					time_read++;
				} else {
					PlayerPrefs.SetInt ("started", 1);
					started = true;
					c = new Color (0.0f, 1.0f, 0.0f);
				}
			}
			else{
				time_read = 0;
				c = new Color(1.0f, 0.0f, 0.0f);
			}
		}			

		Vector3 vec = new Vector3 (positions [i] [j].x, 600 - positions [i] [j].y,positions [i] [j].z);
		foreach (GameObject obj in objs) {
			obj.tag = "bolitas";
			obj.GetComponent<Renderer> ().material.color = c;
			vec = new Vector3 (vec.x+vectors[i][j][k].x,(vec.y+vectors[i][j][k].y*-1),vec.z+vectors[i][j][k].z);
			//obj.transform.position = new Vector3 (positions[i][j].x, positions[i][j].y, positions[i][j].z);
			obj.transform.position = new Vector3 (vec.x,vec.y,vec.z);
			k++;
			if (k == 4) {
				for (int t = 0; t < hands_selected.Count; t++) {
					if (hands_selected [t][0] == i && hands_selected [t][1] == j) {
						obj.gameObject.tag = "dedos";
						obj.gameObject.GetComponent<Renderer> ().material = cubos [hands_selected [t] [2]].gameObject.GetComponent<Renderer> ().material;
					}
				}
				k = 0;
				j++;
				if (j == 5) {
					j = 0;
					i++;
				}
				if (i < 2) {
					vec = new Vector3 (positions [i] [j].x, 600 - positions [i] [j].y, positions [i] [j].z);
				}
			}
		}
	}

	private void ReceiveCallback(IAsyncResult AR)
	{
		//Check how much bytes are recieved and call EndRecieve to finalize handshake
		int recieved = _clientSocket.EndReceive(AR);

		if(recieved <= 0)
			return;
		//Copy the recieved data into new buffer , to avoid null bytes
		byte[] recData = new byte[recieved];
		Buffer.BlockCopy(_recieveBuffer,0,recData,0,recieved);

		byte[] tmp_ = new List<byte> (recData).GetRange (0, 4).ToArray ();
		int package_type = BitConverter.ToInt32 (tmp_, 0);
		int[] finger_ord = new  int[10];
		for (int i = 0; i < 10; i++) {
			tmp_ = new List<byte> (recData).GetRange((i*4)+4, 4).ToArray ();
			finger_ord[i] = BitConverter.ToInt32 (tmp_, 0);
		}

		tmp_ = new List<byte> (recData).GetRange (44, 4).ToArray ();
		real_read = BitConverter.ToInt32 (tmp_, 0);
		float x, y, z;
		int dedo = 0;
		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 5; j++) {
				for (int k=0; k < 4; k++) {
					x = System.BitConverter.ToSingle (recData, ((i*4*3*4*5)+(j*4*3*4)+(k*12)+(4*dedo))+48);
					dedo = dedo + 1;
					y = System.BitConverter.ToSingle (recData, ((i*4*3*4*5)+(j*4*3*4)+(k*12)+(4*dedo))+48);
					dedo = dedo + 1;
					z = System.BitConverter.ToSingle (recData, ((i*4*3*4*5)+(j*4*3*4)+(k*12)+(4*dedo))+48);
					dedo = 0;
					vectors [i] [j] [k] = new Vector3(x, y, z);
				}
			}
		}
		int pos = 48 + (2*5*4*3*4);
		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 5; j++) {
				x = System.BitConverter.ToSingle (recData, ((i*4*3*5)+(j*4*3)+(4*dedo))+pos);
					dedo = dedo + 1;
				y = System.BitConverter.ToSingle (recData, ((i*4*3*5)+(j*4*3)+(4*dedo))+pos);
					dedo = dedo + 1;
				z = System.BitConverter.ToSingle (recData, ((i*4*3*5)+(j*4*3)+(4*dedo))+pos);
					dedo = 0;

				positions [i] [j] = new Vector3(x, y, z);
			}
		}
		//Process data here the way you want , all your bytes will be stored in recData

		//Start receiving again
		_clientSocket.BeginReceive(_recieveBuffer,0,_recieveBuffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),null);
	}

	private void SendData(byte[] data)
	{
		SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
		socketAsyncData.SetBuffer(data,0,data.Length);
		_clientSocket.SendAsync(socketAsyncData);
	}

}
