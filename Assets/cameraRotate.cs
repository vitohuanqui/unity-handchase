using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRotate : MonoBehaviour {


	public float horizontalSpeed = 2.0F;
	public float verticalSpeed = 2.0F;
	// Use this for initialization
	void Start () {
	}

         void Update() {
             float h = horizontalSpeed * Input.GetAxis("Mouse X");
             float v = verticalSpeed * Input.GetAxis("Mouse Y");
             transform.Rotate(v, h, 0);
         }
}
