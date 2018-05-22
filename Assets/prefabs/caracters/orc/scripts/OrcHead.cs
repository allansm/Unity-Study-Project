using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcHead : MonoBehaviour {
	//public
	public OrcController orc;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	//actions
	private void stun(){
		orc.stunHead ();
	}
	//colissions
	void OnCollissionEnter(Collider2D collision){
		stun ();
	}
	void OnTriggerEnter2D(Collider2D collission){
		stun ();
	}
}
