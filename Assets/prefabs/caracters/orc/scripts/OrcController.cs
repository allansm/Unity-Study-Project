using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcController : MonoBehaviour {
	
	//public
	public float velocity;
	public Animator animator;
	public Transform target;
	public Transform transform;
	public GameObject weapon;
	public GameObject player;
	public GameObject drop;
	//private
	private int direction;
	private Vector3 currentScale; 
	private bool stun = false;
	private bool canMove = true;
	private bool isRight = false;
	private float x;
	private float recoverTime = 0;
	public bool equiped = true;
	private float equipCooldown = 0.6f;
	private bool recoveringWeapon = false;
	private bool playerIsEquiped = false;
	private bool playerInRisk = false;
	private bool canAttack = false;
	private float attackCooldown = 1f;

	// Use this for initialization
	void Start () {
		currentScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		run ();
	}
	//execute all orc actions
	/*
		start ai movement function
		if stunned recover movement after 5 seconds
		disable weapon if stunned and not equiped
		start all animations
	*/
	private void run(){
		move ();
		recoverFromStunHead ();
		turnWeapon ();
		attackCooldown -= Time.deltaTime;
		//animation on bottom
		animation();
	}
	//actions

	//execute enemy ai movement
	/*
		if can move set direction and velocity of axis x
		change position of sprite to right if x > 0 and inverse if x < 0
		get direction to move based on player
		execute movement
		else stop
	*/
	private void move(){
		if (canMove) {
			x = direction * velocity * Time.deltaTime;
			flip ();
			direction = (target.position.x > transform.position.x) ? 1 : -1;
			if (drop.GetComponent<DropController> ().isPlaced()) {
				direction = (drop.GetComponent<DropController>().dropLocation.transform.position.x > transform.position.x) ? 1 : -1;
			}
			transform.Translate (x, 0, 0);
		} else {
			x = 0;
		}
	}

	private void attack(){
		if (attackCooldown <= 0) {
			canAttack = true;
			attackCooldown = 1f;
			player.GetComponent<CharacterController> ().receiveAttack ();
		}
	}
	//stun ai ,remove weapon ,stop movement from player
	public void stunHead(){
		if (this.equiped && !recoveringWeapon) {
			stun = true;
			canMove = false;
			equiped = false;
			//disableWeapon ();
			drop.GetComponent<DropController> ().drop ((isRight)?2:-2);
		}
	}
	//recover ai movement after 5 seconds stunned
	private void recoverFromStunHead(){
		if (stun) {
			recoverTime += Time.deltaTime;
		}
		if (recoverTime >= 5) {
			stun = false;
			canMove = true;
		}
	}
	//recover weapon lost after stun and reset recovertime
	/*
		if not equiped and not stunned and colide player equiped and kickline
		enable recoverweapon animation 
		enable next head stun

	*/
	private void recoverWeapon(){
		if (playerIsEquiped && playerInRisk) {
			recoveringWeapon = true;
			recoverTime = 0;
		}
	}
	//disable weapon when stunned enable when collide player unarmed after 7 miliseconds and down player after operation
	private void turnWeapon(){
		if (equiped && recoveringWeapon) {
			if (equipCooldown <= 0) {
				enableWeapon ();
				recoveringWeapon = false;
				playerIsEquiped = false;
				playerInRisk = false;
				equipCooldown = 0.6f;

				downPlayer();
			}
			equipCooldown -= Time.deltaTime;
		} 
		if(!equiped && stun){
			disableWeapon ();
		}
		if (equiped && !recoveringWeapon) {
			enableWeapon ();
		}
	}
	//extra functions

	//change x axis position of enemy
	private void flip(){
		if (x > 0 && !isRight) {
			currentScale.x *= -1;
			transform.localScale = currentScale;
			isRight = true;
		}
		if (x < 0 && isRight) {
			currentScale.x *= -1;
			transform.localScale = currentScale;
			isRight = false;
		}
	}
	//enable sprite of weapon
	private void enableWeapon(){
		weapon.GetComponentInChildren<Renderer> ().enabled = true;
	}
	//disable sprite of weapon
	private void disableWeapon(){
		weapon.GetComponentInChildren<Renderer>().enabled = false;
	}
	//execute down function in player
	private void downPlayer(){
		int dir = (target.position.x > transform.position.x) ? 1 : -1;
		player.GetComponent<CharacterController> ().down (dir);
	}
	public void receiveAttack(){
		animator.SetTrigger ("receiveAttack");
	}
	//animationExecution
	/*
		if x axis > 0 enable movement animation
		when stun enable stunHead animation
		when not equiped and recovering weapon execute recoverWeapon animation and equip
	*/
	public void animation(){
		moveAnimation ();
		stunHeadAnimation ();
		recoverWeaponAnimation ();
		attackAnimation ();
	}
	//animations
	private void moveAnimation(){
		if (x != 0) {
			animator.SetBool ("move", true);
		} else {
			animator.SetBool ("move", false);
		}
	}
	private void stunHeadAnimation(){
		if (stun) {
			animator.SetBool ("stunHead", true);
		} else {
			animator.SetBool ("stunHead", false);
		}
	}
	private void recoverWeaponAnimation(){
		if (recoveringWeapon && !equiped && !canMove) {
			animator.SetTrigger ("recoverWeapon");
			equiped = true;
		} 
	}
	private void attackAnimation(){
		if (canAttack) {
			animator.SetTrigger ("attack");
			canAttack = false;
		}
	}
	void OnCollisionEnter2D(Collision2D collision){
		
	}
	//on contact player
	/*
		when contact player and unarmed and not stunned recover weapon
		when enemy colide player stop movement
	*/
	void OnCollisionStay2D(Collision2D collision){
		
		if (collision.gameObject.name == "player") {
			canMove = false;
			playerIsEquiped = collision.gameObject.GetComponent<CharacterController> ().getEquiped ();
			playerInRisk = collision.gameObject.GetComponent<CharacterController> ().inRisk ();
			if (!equiped && !stun) {
				recoverWeapon ();
			}
			if (equiped && collision.gameObject.GetComponent<CharacterController>().inGround()) {
				attack ();
			}
		}
		if (collision.gameObject.name == "dropweapon") {
			collision.gameObject.GetComponent<DropController> ().hideDrop ();
			recoverTime = 0;
			equiped = true;
		}
	}
	//on exit contact player
	/*
		if player exit orc head and not stunned recover movement
	*/
	void OnCollisionExit2D(Collision2D collision){
		if (collision.gameObject.name == "player" && !stun) {
			canMove = true;
		}
	}
}
