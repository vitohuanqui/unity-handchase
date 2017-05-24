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
		Debug.Log ("emntro");
		if ((col.gameObject.tag == "dedos" && col.gameObject.GetComponent<Renderer> ().material.color == this.GetComponent<Renderer>().material.color) || (started != 1))
		{
			colliders++;
			if (started == 1){
				PlayerPrefs.SetInt(this.GetComponent<Renderer>().material.name, 1);
				col.gameObject.tag = "bolitas";
			}
			//if (colliders == 1)
			//	position = new Vector3(position.x+col.gameObject.transform.position.x,
			//		position.y+col.gameObject.transform.position.y,
			//		position.z+col.gameObject.transform.position.z);
		}
		if (col.gameObject.tag == "base_floot") {
			transform.position = new Vector3 (transform.position.x, transform.position.y, -100);
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
			colliders = 0;
		}
	}

	void Update(){
		if (colliders > 1) {
			transform.position = new Vector3 (transform.position.x, 250, -100);
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
			colliders = 0;
		}
	}
}
