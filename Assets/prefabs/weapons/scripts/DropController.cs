using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour {
	//public
	public Animator animator;
	public Transform dropLocation;
	public Transform locationToDrop;
	//private

	private Vector3 defaultLocation;
	private bool placed = false;
	// Use this for initialization
	void Start () {
		defaultLocation = dropLocation.position;
	}
	
	// Update is called once per frame
	void Update () {
		run ();
	}
	private void run(){
		animations ();
	}
	//actions
	public void drop(int distance){
		dropLocation.position = new Vector3 (locationToDrop.position.x + distance, locationToDrop.position.y+-2);
		//dropLocation.position = locationToDrop.position;
		placed = true;
	}
	public void hideDrop(){
		dropLocation.position = new Vector3(defaultLocation.x,defaultLocation.y);
		placed = false;
	}
	public bool isPlaced(){
		return this.placed;
	}
	//animations
	private void animations(){
		place ();
	}
	private void place(){
		if (placed) {
			animator.SetBool ("placed", true);
		} else {
			animator.SetBool ("placed", false);
		}
	}

}
