using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


public class SimplePlatformController : MonoBehaviour {

	[HideInInspector] public bool facingRight = true;
	[HideInInspector] public bool jump = false;
	public float moveForce = 365f;
	public float maxSpeed = 5f;
	public float jumpForce = 1000f;
	public Transform groundCheck;

	private bool grounded = false;
	private Animator anim;
	private Rigidbody2D rb2d;

	/*For Swipe Detection and controls*/

	public float maxTime=0.5f;
	public float minSwipeDist = 400f;

	float startTime;
	float endTime;

	Vector3 startPos;
	Vector3 endPos;

	float swipeDistance;
	float swipeTime;


	// Use this for initialization
	void Awake () 
	{
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () 
	{
		swipeDetect ();
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
	}


	void swipeDetect()
	{
		for(int i=0;i<Input.touchCount;i++) 
		{
			//Touch touch = Input.GetTouch (i);//if ith touch is Swipe
			Touch touch = Input.GetTouch (i);
			if (touch.phase == TouchPhase.Began) 
			{
				//Debug.Log ("Touch started");
				startTime = Time.time;
				startPos = touch.position;
			}
			else if(touch.phase == TouchPhase.Ended)
			{
				endTime = Time.time;
				endPos = touch.position;
				//Debug.Log ("Touch ended");
				swipeDistance = (endPos - startPos).magnitude;
				swipeTime = endTime - startTime;
				//Debug.Log ("d: " + swipeDistance);
				//Debug.Log ("t: " + swipeTime);
				if (swipeTime < maxTime && swipeDistance < minSwipeDist) 
				{
					swipeUp ();
				}

			}


		}

	}
	void swipeUp(){

		Vector2 distance = endPos - startPos;
		if (Mathf.Abs (distance.x) > Mathf.Abs (distance.y)) 
		{
			Debug.Log ("Horizontal Swipe");

			if (distance.x > 0) 
			{
				Debug.Log ("Right Swipe");
			}
			if (distance.x < 0) 
			{
				Debug.Log ("Left Swipe");
			}


		}
		else if(Mathf.Abs (distance.x) < Mathf.Abs (distance.y))
		{
			if(grounded)
				jump = true;
			Debug.Log ("Vertical Swipe");
			if (distance.x > 0) 
			{
				Debug.Log ("Right Swipe");
			}
			if (distance.x < 0) 
			{
				Debug.Log ("Left Swipe");
			}
		}


	}
	void FixedUpdate()
	{


		float h = CrossPlatformInputManager.GetAxis("HorizontalM");

		//Debug.Log (h);
		anim.SetFloat ("Speed", Mathf.Abs (h));


		if (h * rb2d.velocity.x < maxSpeed)
			rb2d.AddForce(Vector2.right * h * moveForce);

		if (Mathf.Abs (rb2d.velocity.x) > maxSpeed)
			rb2d.velocity = new Vector2(Mathf.Sign (rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

		if (h > 0 && !facingRight)
			Flip ();
		else if (h < 0 && facingRight)
			Flip ();

		if (jump)
		{
			anim.SetTrigger("Jump");
			rb2d.AddForce(new Vector2(0f, jumpForce));
			jump = false;
		}


	}


	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}