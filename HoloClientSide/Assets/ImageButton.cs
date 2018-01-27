using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageButton : MonoBehaviour {
    public void ImageRequestOnClick()
    {
        InputController input = GameObject.Find("GameController").GetComponent<InputController>();
        input.RequestModel(GetComponentInChildren<Text>().text);
    }
    
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
