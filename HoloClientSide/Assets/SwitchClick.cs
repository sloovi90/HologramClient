using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SwitchClick : EventTrigger {

	// Use this for initialization
	void Start () {
		
	}
    public override void OnPointerClick(PointerEventData data)
    {
        GameObject.Find("GameController").GetComponent<InputController>().TogglePresentationMode();
    }
    // Update is called once per frame
    void Update () {
		
	}
}
