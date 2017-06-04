using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTriggered : MonoBehaviour {
	GameObject grabbedObject;
	float grabbedObjectSize; 
	int colliders;
	Vector3 position=new Vector3(0,0,0);
	Vector3 position_new_cube;
	bool colition_flag;
	float value = Mathf.PI / 4;
	List<GameObject> parents_cube = new List<GameObject>();
	void Start()
	{
	}

	void OnTriggerEnter(Collider other){
		//GameObject objs = GameObject.Find ("Cube");
		if (other.gameObject.tag == "floor"){
			colition_flag = true;
	//		position_new_cube = new Vector3(transform.position.x, other.gameObject.transform.position.y + 25, transform.position.z);
		}
		if (other.gameObject.tag == "dedos") {
			colliders++;
			other.gameObject.tag = "bolitas";
			position = other.gameObject.transform.position;
		}
	}

	void TryGrabObject(GameObject grab){
		if (grab == null)
			return;
		grab = grabbedObject;
		grabbedObjectSize = grab.GetComponent<Renderer> ().bounds.size.magnitude;
	}

	void DropObject()
	{
		if (grabbedObject == null)
			return;
		grabbedObject = null;
	}

	void OnCollisionEnter(Collision col){
		int started = PlayerPrefs.GetInt ("started");
		//Debug.Log ("entro");
		if ((col.gameObject.tag == "dedos" && col.gameObject.GetComponent<Renderer> ().material.color == this.GetComponent<Renderer>().material.color) || (started != 1))
		{
			colliders++;
			if (started == 1){
				Camera.main.GetComponent<AudioSource> ().Play ();
				PlayerPrefs.SetInt(this.GetComponent<Renderer>().material.name, 1);
				col.gameObject.tag = "bolitas";
			}
			//if (colliders == 1)
			//	position = new Vector3(position.x+col.gameObject.transform.position.x,
			//		position.y+col.gameObject.transform.position.y,
			//		position.z+col.gameObject.transform.position.z);
		}
		if (col.gameObject.tag == "base_floot") {
			calculate_new_position();
			colliders = 0;
		}
	}
	void calculate_new_position()
	{
		float angulo = Random.Range(-value, value);
		float cosa = Mathf.Cos (angulo);
		float sina = Mathf.Sin (angulo);
		Vector2 new_vector = new Vector2 (Camera.main.transform.forward.x*cosa - Camera.main.transform.forward.y*sina,
										  Camera.main.transform.forward.x*sina + Camera.main.transform.forward.y*cosa);
		transform.position = new Vector3 (400*(new_vector.x), 650+400*(new_vector.y), -400);
		transform.eulerAngles = Vector3.zero;
		this.GetComponent<Rigidbody> ().velocity = new Vector3(0,0,Random.Range(0,100));
		this.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
	}
	void Update(){
		if (colliders > 1) {
			calculate_new_position();
			colliders = 0;
		}
	}
}
