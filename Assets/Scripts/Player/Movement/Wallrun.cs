using UnityEngine;
using System.Collections;

public class Wallrun : MonoBehaviour
{
    private GameObject player;
    private Rigidbody playerRB;
    private float velocity;
    public float wallRunSpeed;
    public int wallRun;
    private int wallJump;
    public GameObject camHolder;
    private float orgGravity;
    public float newGravity;
    public float jumpForce;
    public float jumpForceUp;
    public float forwardSpeed;
    public float sideSpeed;
    public bool isWallRunning;
    
    
	// Use this for initialization
	void Start ()
	{
	    player = transform.gameObject;
	    playerRB = player.GetComponent<Rigidbody>();
	    //camHolder = transform.GetComponent<NetworkCharacterComp>().cam.transform.parent.gameObject;
	    orgGravity = player.GetComponent<Controller>().gravity;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (wallRun == 0)
	    {
	        isWallRunning = false;
	    }
	    else
	    {
	        isWallRunning = true;
	    }

	    if (playerRB.velocity.magnitude < 1)
	    {
	        velocity = 0;
	    }
	    else
	    {
            velocity = playerRB.velocity.magnitude;
	    }
	    if (velocity > wallRunSpeed || orgGravity == orgGravity)
	    {
	        Ray rayR = new Ray(transform.position, transform.right);
	        Ray rayL = new Ray(transform.position, transform.right*-1);

	        RaycastHit hit;
	        if (Physics.Raycast(rayR, out hit, 1.5f) && hit.collider.name != "Player(Clone)" && transform.GetComponent<Controller>().IsGrounded() == false)
	        {
	            wallRun = 1;
	        }
            else if (Physics.Raycast(rayL, out hit, 1.5f) && hit.collider.name != "Player(Clone)" && transform.GetComponent<Controller>().IsGrounded() == false)
	        {

	            wallRun = -1;
	        }
	        else
	        {
	            wallRun = 0;
	        }

	    }
	    else
	    {
	        wallRun = 0;
	    }

        if (orgGravity == orgGravity)
        {
            Ray rayF = new Ray(transform.position, transform.forward);
            Ray rayB = new Ray(transform.position, transform.forward * -1);

            RaycastHit hit;
            if (Physics.Raycast(rayF, out hit, 1f) && hit.collider.name != "Player(Clone)" && transform.GetComponent<Controller>().IsGrounded() == false)
            {
                wallJump = 1;
            }
            else if (Physics.Raycast(rayB, out hit, 1f) && hit.collider.name != "Player(Clone)" && transform.GetComponent<Controller>().IsGrounded() == false)
            {

                wallJump = -1;
            }
            else
            {
                wallJump = 0;
            }

        }


        if (wallRun != 0 && Input.GetAxisRaw("Horizontal") != 0)
	    {
            playerRB.AddForce((transform.right*sideSpeed) * wallRun, ForceMode.Acceleration);
            if (Input.GetButtonDown("Jump"))
            {
                player.GetComponent<Controller>().gravity = orgGravity;
                playerRB.AddForce((transform.right * (wallRun * -1)) * jumpForce, ForceMode.Impulse);
                playerRB.AddForce(transform.up * jumpForceUp, ForceMode.Impulse);
                transform.GetComponent<Controller>().jumpedYPos = transform.position.y;
            }
	    } else if(wallJump != 0 && wallJump != -1)
	    {
	        
	    }
        

	}

    void FixedUpdate()
    {
        if (wallRun != 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                playerRB.AddForce(player.transform.forward*forwardSpeed, ForceMode.Acceleration);
                player.GetComponent<Controller>().gravity = newGravity;
            }
            //playerRB.AddForce((player.transform.right*10f) * wallRun, ForceMode.Acceleration);
            camHolder.transform.localRotation = Quaternion.Lerp(camHolder.transform.localRotation,Quaternion.Euler(0, 0, 30f * wallRun), Time.deltaTime*2f);
        }
        else
        {
            player.GetComponent<Controller>().gravity = orgGravity;
        }
    }
}
