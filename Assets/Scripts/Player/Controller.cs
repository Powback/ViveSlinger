using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

//left shift to crouch
[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(CapsuleCollider))]
[AddComponentMenu("Character/Controller")]
public class Controller : MonoBehaviour {
	[Tooltip("Main camera refferenced by other scripts")]
	public GameObject mainCamera;
    [Tooltip("The collider to add force to")]
    public GameObject collider;
    [Tooltip("Climbing speed on a ladder")]
	public float climbSpeed = 2.5f;
	[Tooltip("How many units to fall to start counting fall damage")]
	public float fallThreshold = 3f;
	[Tooltip("How much to multiply the damage by")]
	public float fallDamageMultiplier = 3f;
	
	[Tooltip("Footsteps per second when walking")]
	public float walkSteps = 3.5f;
	[Tooltip("Footsteps per second when crouching")]
	public float crouchSteps = 2f;
	public GameObject footstepPrefab;
	
	[Tooltip("Your speed when on a Slippymat")]
	public float slippySpeed = 12f;
	[Tooltip("Your normal walking speed")]
	public float walkSpeed = 6f;
	[Tooltip("Your crouching speed")]
	public float crouchSpeed = 2.5f;
	[Tooltip("Your running speed")]
	public float runSpeed = 10f;
	
	[Tooltip("Higher gravity, faster fall speed")]
	public float gravity = 13f;
	[Tooltip("Jumping power")]
	public float jumpHeight = 2f;
	[Tooltip("How many times you are allowed to jump when in midair")]
	public int maxJumps = 1;
	[Tooltip("Speed acceleration when in midair (1 is max)")]
	public float airAccelerator = 0.15f;
	[Tooltip("Speed acceleration when in on ground (1 is max)")]
	public float groundAccelerator = 0.6f;
	[Tooltip("Disables the movement and jumping")]
	public bool controllerAble = true;

    public Vector3 targetVelocityRaw;
    public bool isRoping;


	float speed;
	float acceleration;
	int jumpsDone = 0;
	public float jumpedYPos;
	float landedYPos;
	bool lastGrounded;
	float nextFootstep;
	bool moving;
	bool onSlippyMat;
	public bool onLadder;
	bool jumpedOffLadder;
	float tempGravity;
    float tempAirAccelerator;


	
	void Awake ()
	{
		GetComponent<Rigidbody>().freezeRotation = true;
		GetComponent<Rigidbody>().useGravity = false;
	    tempAirAccelerator = airAccelerator;
	    //If networked, remove the */ and /*.
	    
		//if(GetComponent<PhotonView>().isMine)
	//	{
			lastGrounded = IsGrounded();
		//}
	//	else
	//	{
	//		enabled = false;
	//		GetComponent<Rigidbody>().isKinematic = true;
	//	}
		
	}
	void Update()
	{
		float footsteps = walkSteps;
		if(Input.GetKey("left shift"))
		{
			footsteps = crouchSteps;
		}
		
		float footstepRate = 1f/footsteps;
		if(Time.time > nextFootstep && moving && IsGrounded() && controllerAble)
		{
			nextFootstep = Time.time+footstepRate;
			//If were not a slippymat, then make normal footsteps
			if(!onSlippyMat)
			{
				//If networked, remove the comment
				//GetComponent<PhotonView>().RPC("Step", PhotonTargets.All);
				Step ();
			}
			else
			{
				//If on a Slippymat, then theres a 30% of making a footstep
				if(Random.value < 0.3f)
				{
					//If networked, remove the comment
					//GetComponent<PhotonView>().RPC("Step", PhotonTargets.All);
					Step ();
				}
			}
		}
		
		if(controllerAble)
		{
			if (Input.GetButtonDown("Jump"))
			{
                Jump();
			}
		}
	}
	//If networked, remove the comment
	[PunRPC]
	void Step()
	{
		if(footstepPrefab)
		{
			GameObject footstep = Instantiate(footstepPrefab, transform.position, Quaternion.identity) as GameObject;
			Destroy(footstep, 1f);
		}
	}

    public void Jump()
    {
        if (IsGrounded())
        {
            jumpsDone = 0;
            jumpedYPos = transform.position.y;

            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, CalculateJumpVerticalSpeed(), GetComponent<Rigidbody>().velocity.z);
        }
        else
        {
            if (jumpsDone < maxJumps - 1)
            {
                jumpsDone++;
                jumpedYPos = transform.position.y;

                GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, CalculateJumpVerticalSpeed(), GetComponent<Rigidbody>().velocity.z);
            }
        }
    }
	//Pretty usefull if you want your players speed to be to scale with their size.
	public void Resize(float scale)
	{
		scale = Mathf.Clamp(scale, 0.15f, 3f);
		
		jumpHeight = scale * 2f;
		walkSpeed = scale * 6f;
		crouchSpeed = scale * 3f;
		runSpeed = scale * 10f;
		
		Vector3 newSize = new Vector3();
		newSize.x = scale;
		newSize.y = scale;
		newSize.z = scale;
		
		transform.position += Vector3.up*scale/2f;
		transform.localScale = newSize;
	}
	void FixedUpdate ()
	{
		//CapsuleCollider capsule = GetComponent<CapsuleCollider>();
		if(Input.GetButton("Crouch"))
		{
			if(onSlippyMat)
			{
				speed = slippySpeed;
			}
			else
			{
				speed = crouchSpeed;
			}
			//capsule.height = Mathf.Lerp(capsule.height, 0.75f, Time.deltaTime*10f);
		}
		else if (Input.GetButton("Run"))
		{
			if(onSlippyMat)
			{
				speed = slippySpeed;
			}
			else
			{
				speed = runSpeed;
			}
			//capsule.height = Mathf.Lerp(capsule.height, 2f, Time.deltaTime*10f);
		}
		else
		{
			if(onSlippyMat)
			{
				speed = slippySpeed;
			}
			else
			{
				speed = walkSpeed;
			}
			//capsule.height = Mathf.Lerp(capsule.height, 2f, Time.deltaTime*10f);
		}
		
		if(lastGrounded != IsGrounded())
		{
			lastGrounded = IsGrounded();
			//This part is where you landed.
			if(lastGrounded == true)
			{
				landedYPos = transform.position.y;
				if(jumpedYPos > landedYPos)
				{
					float distanceFell = jumpedYPos - landedYPos;
					if(distanceFell > fallThreshold)
					{
						distanceFell -= fallThreshold;
						float damageToDeal = Mathf.Round(distanceFell*fallDamageMultiplier);
						Debug.Log("Dealt "+damageToDeal+" damage");
						//Use your own script that contains your player health
						//GetComponent<YourScriptWithHealth>().health -= damageToDeal;
					}
				}
			}
			//This part is where you jumped or fell.
			else
			{
				jumpedYPos = transform.position.y;
				jumpsDone = 0;
			}
		}
		moving = Input.GetButton("Horizontal") || Input.GetButton("Vertical");
		
		if (IsGrounded())
		{
			if(controllerAble == true)
			{
				if(onSlippyMat)
				{
					acceleration = 0.1f;
				}
				else
				{
					acceleration = groundAccelerator;
				}
			}
		}
		else
		{
			if(controllerAble == true)
			{
				acceleration = airAccelerator;
			}
		}
        Vector3 targetVelocity = new Vector3();
	    if (!Util.AlmostEquals(Input.GetAxis("Horizontal"), 0.0f, 0.1f) ||
	        !Util.AlmostEquals(Input.GetAxis("Vertical"), 0.0f, 0.1f))
	    {
	        targetVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
	        targetVelocityRaw = targetVelocity;

	    }
	    else
	    {
	        targetVelocity = targetVelocityRaw;
	    }
		
		targetVelocity = collider.transform.TransformDirection(targetVelocity.normalized) * speed;
		
		if(controllerAble == false)
		{
			targetVelocity = Vector3.zero;
		}
		else
		{
			//Limit the velocity if its higher than usual, this happens when walking diagonally.
			if(targetVelocity.sqrMagnitude > (speed * speed))
			{
				targetVelocity = targetVelocity.normalized * speed;
			}
		}
		Vector3 velocityChange = (targetVelocity - GetComponent<Rigidbody>().velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -acceleration, acceleration);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -acceleration, acceleration);
		velocityChange.y = 0;
		
		GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
		GetComponent<Rigidbody>().AddForce(new Vector3 (0, -gravity * GetComponent<Rigidbody>().mass, 0));
	}
	public bool IsGrounded()
	{
        
		//All done for you perfectly, you can resize your player and you will still be able to use it.
		float castRadius = transform.localScale.x/2-0.1f;
		float castDistance = transform.localScale.x+0.1f;
		
		//Casts 5 raycasts from different parts of the player, for accuracy.
		Vector3 leftCast = new Vector3(collider.transform.position.x-castRadius, collider.transform.position.y, collider.GetComponent<Collider>().transform.position.z);
		Vector3 rightCast = new Vector3(collider.transform.position.x+castRadius, collider.transform.position.y, collider.transform.position.z);
		Vector3 frontCast = new Vector3(collider.transform.position.x, collider.transform.position.y, collider.transform.position.z+castRadius);
		Vector3 backCast = new Vector3(collider.transform.position.x, collider.transform.position.y, collider.transform.position.z-castRadius);
		Vector3 centerCast = collider.transform.position;
		
		return Physics.Raycast(leftCast, -collider.transform.up, castDistance) || Physics.Raycast(rightCast, -collider.transform.up, castDistance) || 
			Physics.Raycast(frontCast, -collider.transform.up, castDistance) || Physics.Raycast(backCast, -collider.transform.up, castDistance) || 
				Physics.Raycast(centerCast, -collider.transform.up, castDistance);
	}
	float CalculateJumpVerticalSpeed ()
	{
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
	void OnTriggerEnter(Collider what)
	{
		//Extra feature, slippery mats, once walked on, youre much faster.
		//Make an object named Slippymat, and set IsTrigger on, done.
		if(what.name == "Slippymat")
		{
			onSlippyMat = true;
		}
	}
	void OnCollisionEnter() {
        if(isRoping == false)
    		airAccelerator = 0f;
	}
	void OnCollisionExit() {
	    if (isRoping == false)
	    {
	        airAccelerator = tempAirAccelerator;
	        
	    }
	}


}