using UnityEngine;
using System.Collections;

public class VRRigidbody : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject collider;

	
	// Update is called once per frame
	void Update ()
	{
        Quaternion rot = MainCamera.transform.localRotation;
	    rot.x = 0;
	    rot.z = 0;
        Vector3 scale = new Vector3(1,MainCamera.transform.localPosition.y,1);
	    Vector3 pos = MainCamera.transform.localPosition;
	    float yPos = transform.localPosition.y;
	    pos.y = 1f;
       // Collider.transform.localScale = scale;
	    collider.transform.localPosition = pos;
	    collider.transform.localRotation = rot;
	}
}
