using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnClick : MonoBehaviour {

	public void LoadByIndex(int scene){
		InputField mainInputField = (InputField)GameObject.Find("InputField").GetComponent<InputField> ();
		Debug.Log (mainInputField.text);
		PlayerPrefs.SetString("Ip",mainInputField.text);
		SceneManager.LoadScene (scene);
	}
}
