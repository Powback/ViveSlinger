using UnityEngine;
using System.Collections;

public class PlayerVelocity : MonoBehaviour {
    int velocity;
    private float realVelocity;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    realVelocity = transform.GetComponent<Rigidbody>().velocity.magnitude;
	    if (realVelocity < 1)
	    {
	        velocity = 0;
	    }
	    else
	    {
	        realVelocity *= 3.6f;
	        velocity = Mathf.RoundToInt(realVelocity);
	    }
	}
    void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(100f,10f,100f,40f),velocity.ToString());
    }
}
