using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//controll all actions of character
public class CharacterController : MonoBehaviour {
	//public
	public float velocity;
	public Transform transform;
	public Animator animator;
	public Transform camera;
	public GameObject weapon;
	public Rigidbody2D rigidbody;
	//private 
	private Vector2 currentposition;
	private Vector3 currentscale;
	private Vector3 cameraposition;
	private Vector3 weaponposition;
	private float x = 0;
	private float y = 0;
	private bool isRight = true;
	private bool equiped = false;
	private bool isGrounded = true;
	private bool kickEnable = false;
	private bool isDown = false;
	private float downCooldown = 10f;
	private bool canMove = true;
	private int dir;
	private bool dd = false;
	private bool inAttack = false;
	private float attackCooldown = 1f;
	private GameObject target;
	private bool attackExecuted = false;
	private bool inAttackRange = false;
	void Start () {
		currentscale = transform.localScale;
		currentscale.x *= -1;
		transform.localScale = currentscale;
	}

	void Update () {
		run ();
	}

	//execute all character functions
	/*
		update location of camera
		when can move set direction of sprite and execute movement based on input direction
		when keypressed equals space and player is grounded execute jump
		enable and disable weapon if equiped
		push character to direction of enemy skill
		execute all animations
	*/
	private void run(){
		//animation on top
		animation ();
		//cameraUpdate ();
		attackCooldown-= Time.deltaTime;
		move ();
		jump ();
		turnWeapon ();
		doDown ();
		attack ();
	}
	//actions

	//execute character movement if can move
	/*
		if can move get direction of input 
		flip to direct of move
		execute movement
	*/
	private void move(){
		if (canMove) {
			x = Input.GetAxis ("Horizontal") * Time.deltaTime * velocity;
			flip ();
			transform.Translate (x, 0, 0);
		}
	}
	//execute character jump
	/*
		if keypressed equals space and is grounded
		set jump velocity on rigidbody to execute jump 
		else set rigidbody force to 0
	*/
	private void jump(){
		if (Input.GetKey (KeyCode.Space) && isGrounded) {
			y = 7f;
			Vector3 vel = rigidbody.velocity;
			vel.y = y;
			rigidbody.velocity = vel;
		} else {
			y = 0;
			rigidbody.AddForce (new Vector2 (0, y));
		}
	}
	//enable or disable weapon sprite
	/*
		if equiped enable weapon sprite else disable
	*/
	private void turnWeapon(){
		if (equiped) {
			enableWeapon ();
		}
		if (!equiped) {
			disableWeapon ();
		}
	}
	//enable kick
	/*
		kick enemy when colide kick area
	*/
	private void kick(){
		kickEnable = true;
	}
	//push character to far distance when necessary
	/*
		when doDown activate execute push slowly
		else stop
	*/
	private void doDown(){
		if (--downCooldown > 0 && dd) {
			
			transform.Translate (dir, 0.2f, 0);
		} else {
			dd = false;
			downCooldown = 10;
			canMove = true;
		}
	}
	//execute from enemy to push character
	/*
		stop player movement
		set private direction to equal a external directional value
		execute down animation
		enable do down function
	*/
	public void down(int dir){
		if (!isDown && !dd && canMove && equiped) {
			canMove = false;
			isDown = true;
			equiped = false;
			this.dir = dir;
			dd = true;
		}
	}
	public bool inRisk(){
		return !isDown && !dd && canMove && equiped;
	}
	public void receiveAttack(){
		animator.SetTrigger ("receiveAttack");
	}
	public bool inGround(){
		return this.isGrounded;
	}
	private void attack(){
		if (equiped && Input.GetKey (KeyCode.J) && isGrounded && !inAttack && attackCooldown <= 0) {
			attackExecuted = false;
			inAttack = true;
			attackCooldown = 1f;
		}
		//desabilitado por ultimo :D
		/*
			target exist when colide enemy
			target won't exist when exit colide enemy
			attack is executed when target exist and animation of attack has executed
			enemy receive attack when target exist and attack is executed

			--problems--

			--solved--
			enemy receive attack when player is out of range
		*/
		if (attackExecuted) {
			target.GetComponentInParent<OrcController> ().receiveAttack ();
			attackExecuted = false;
		}
	}
	//extraFunctions

	//flip sprite image
	/*
		if move position < 0 and current position flip sprite to left
		if move position < 0 and current position flip sprite to right
	*/
	private void flip(){
		if (x < 0 && isRight) {
			currentscale.x *=  -1;
			transform.localScale = currentscale;
			isRight = false;

		} 
		if(x > 0 && !isRight){
			currentscale.x *= -1;
			transform.localScale = currentscale;
			isRight = true;
		}
	}
	//update camera position
	/*
		set camera position equal to character position
	*/
	private void cameraUpdate(){
		cameraposition = camera.position;
		cameraposition.x = transform.position.x;
		//cameraposition.y = transform.position.y+2.5f;
		camera.position = cameraposition;
	}
	//enable weapon sprite
	private void enableWeapon(){
		weapon.GetComponentInChildren<Renderer> ().enabled = true;
	}
	//disable weapon sprite
	private void disableWeapon(){
		weapon.GetComponentInChildren<Renderer>().enabled = false;
	}
	public bool getEquiped(){
		return this.equiped;
	}
	//animationExecution
	private void animation(){
		moveAnimation ();
		jumpAnimation ();
		weaponIdle ();
		kickAnimation ();
		downAnimation ();
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
	private void jumpAnimation(){
		if (!isGrounded && !dd && !isDown) {
			animator.SetBool ("jump", true);
		} else {
			animator.SetBool ("jump",false);
		}
	}
	private void weaponIdle(){
		if (equiped) {
			animator.SetBool ("equiped", true);
		} else {
			animator.SetBool ("equiped", false);
		}
	}
	private void kickAnimation(){
		if (kickEnable) {
			animator.SetBool ("kick", true);
			kickEnable = false;
		} else {
			animator.SetBool ("kick", false);
		}
	}
	private void downAnimation(){
		if (downCooldown == 10 && dd && !canMove) {
			animator.SetTrigger ("down");
		}
	}
	private void attackAnimation(){
		if (inAttack) {
			animator.SetTrigger ("attack");
			inAttack = false;
			attackExecuted = inAttackRange;
		}
	}
	//collisions
	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.name == "Cube") {
			isGrounded = true;
			kickEnable = true;
		}
		if (collision.gameObject.name == "dropweapon") {
			collision.gameObject.GetComponent<DropController> ().hideDrop ();
			equiped = true;
		}
		if (collision.gameObject.name == "orc") {
			target = collision.gameObject;
			inAttackRange = true;
		}
		print (collision.gameObject.name);
	}
	void OnCollisionExit2D(Collision2D collision){
		if (collision.gameObject.name == "Cube") {
			isGrounded = false;
		}
		if (collision.gameObject.name == "orc") {
			inAttackRange = false;
			inAttack = false;
		}
	}
	void OnTriggerStay2D(Collider2D collision){
		if (collision.gameObject.tag == "orcHead") {
			kick ();
		}
		if (collision.gameObject.tag == "recoverWeaponRange" && isDown) {
			//canMove = false;
			isDown = false;
		}
		               
	}
	void OnTriggerExit2D(Collider2D collision){
		if (collision.gameObject.tag == "recoverWeaponRange") {
			//canMove = true;
		}
	}
}
