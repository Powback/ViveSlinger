
using UnityEngine;

// Very simple smooth mouselook modifier for the MainCamera in Unity
// by Francis R. Griffiths-Keam - www.runningdimensions.com

[AddComponentMenu("Camera/Simple Smooth Mouse Look Drone ")]
public class Mouselook_drone : MonoBehaviour
{
	Vector2 _mouseAbsolute;
	Vector2 _smoothMouse;
	Vector2 _mouseAbsoluteTemp;
	
	
	
	public Vector2 clampInDegrees = new Vector2(360, 180);
	public bool lockCursor;
	public Vector2 sensitivity = new Vector2(2, 2);
	public Vector2 smoothing = new Vector2(3, 3);
	public Vector2 targetDirection;
	public Vector2 targetCharacterDirection;
	public Quaternion yRotation;
	public Quaternion yRotationTemp;
	public bool isFreeLook = false;	
	bool isFreeLookReseting = false;
	bool canMove = true;
	bool xComplete = false;
	bool yComplete = false;
	// Assign this if there's a parent object controlling motion, such as a Character Controller.
	// Yaw rotation will affect this object instead of the camera if set.
	public GameObject characterBody;
	
	void Start()
	{
		// Set target direction to the camera's initial orientation.
		targetDirection = transform.localRotation.eulerAngles;
		
		// Set target direction for the character body to its inital state.
		if (characterBody) targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
	}

	void FixedUpdate()
	{
		if(Input.GetButton("Fire1")) {
			transform.GetComponent<Camera>().fieldOfView = Mathf.Lerp(transform.GetComponent<Camera>().fieldOfView, 20f, Time.deltaTime * 4f);
//			int sens = (int) Mathf.RoundToInt(transform.GetComponent<Camera>().fieldOfView / 45);
			//sensitivity = Vector2(sens, sens); 
		}
		if(Input.GetButton("Fire2")) {
			transform.GetComponent<Camera>().fieldOfView = Mathf.Lerp(transform.GetComponent<Camera>().fieldOfView, 90f, Time.deltaTime * 4f);
		}
		// Ensure the cursor is always locked when set
		if (Input.GetButtonDown("Escape")) {
			if (lockCursor == true) {
				lockCursor = false;
			} else {
				lockCursor = true;
			}
		}
		if (Input.GetButton ("Freelook")) {
			isFreeLook = true;
			
		}
		if(Input.GetButtonUp("Freelook")) {
			_mouseAbsoluteTemp.x = Mathf.Clamp(_mouseAbsoluteTemp.x, -100f, 100f);
			isFreeLookReseting = true;
			canMove = false;
		}
		
		if(isFreeLookReseting == true) {
			
			_mouseAbsoluteTemp.x = Mathf.Lerp(_mouseAbsoluteTemp.x, 0f, Time.deltaTime * 10f);
			_mouseAbsoluteTemp.y = Mathf.Lerp(_mouseAbsoluteTemp.y, _mouseAbsolute.y, Time.deltaTime * 10f);
			float tempYminus = _mouseAbsolute.y - 2f;
			float tempYplus = _mouseAbsolute.y + 2f;
			//Debug.Log(_mouseAbsoluteTemp.x + " || " + _mouseAbsoluteTemp.y + " | " + _mouseAbsolute.y);
			
			if(_mouseAbsoluteTemp.x <3f && _mouseAbsoluteTemp.x >= -3f ) {
				//Debug.Log("We hit 0 on x");
				xComplete = true;
			}
			if(_mouseAbsoluteTemp.y <= tempYplus && _mouseAbsoluteTemp.y >= tempYminus) {
				//Debug.Log("We matched Y");
				yComplete = true;
			}
			if(xComplete == true && yComplete == true) {
				isFreeLookReseting = false;
				isFreeLook = false;
				canMove = true;
				yComplete = false;
				xComplete = false;
			}
		}
//		Screen.lockCursor = lockCursor;
		
		// Allow the script to clamp based on a desired target value.
		var targetOrientation = Quaternion.Euler(targetDirection);
		var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);
		
		// Get raw mouse input for a cleaner reading on more sensitive mice.
		var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
		
		// Scale input against the sensitivity setting and multiply that against the smoothing value.
		mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
		
		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
		_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
		
		// Find the absolute mouse movement value from point zero.
		if(canMove == true) {
			if (isFreeLook == true) {
				_mouseAbsoluteTemp += _smoothMouse;
			} else {
				_mouseAbsolute += _smoothMouse;
				_mouseAbsoluteTemp = _mouseAbsolute;
			}
		}
		// Clamp and apply the local x value first, so as not to be affected by world transforms.
		if (clampInDegrees.x < 360) {
			_mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
			_mouseAbsoluteTemp.x = Mathf.Clamp(_mouseAbsoluteTemp.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
			
		}
		if(isFreeLook == false) {
			_mouseAbsolute.y = Mathf.Lerp(_mouseAbsolute.y, Mathf.Clamp(_mouseAbsolute.y, -90, 90), 10f);
			var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
			transform.localRotation = xRotation;
			
		} else {
			
			_mouseAbsoluteTemp.y = Mathf.Lerp(_mouseAbsoluteTemp.y, Mathf.Clamp(_mouseAbsoluteTemp.y, -90, 90), 10f);
			var xRotation = Quaternion.AngleAxis(-_mouseAbsoluteTemp.y, targetOrientation * Vector3.right);
			transform.localRotation = xRotation;
		}
		
		// Then clamp and apply the global y value.
		if(isFreeLook == false) {
			if (clampInDegrees.y < 360)
				_mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
		} else {
			if (clampInDegrees.y < 360)
				_mouseAbsoluteTemp.y = Mathf.Clamp(_mouseAbsoluteTemp.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
		}
		
		transform.localRotation *= targetOrientation;
		
		// If there's a character body that acts as a parent to the camera
		if (characterBody)
		{
			if(isFreeLook == true) {
				_mouseAbsoluteTemp.x = Mathf.Lerp(_mouseAbsoluteTemp.x, Mathf.Clamp(_mouseAbsoluteTemp.x, -106f, 106f), 10f);
				yRotationTemp = Quaternion.AngleAxis(_mouseAbsoluteTemp.x, transform.InverseTransformDirection(Vector3.up));
				transform.localRotation *= yRotationTemp;
				//Debug.Log(yRotationTemp);
				
			} else {
				_mouseAbsoluteTemp.x = 0f;
				yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, characterBody.transform.up);
				if(Input.GetAxisRaw("Mouse X") > 0) {
					characterBody.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 0.08f);
				}

				if(Input.GetAxisRaw("Mouse X") < 0) {
					characterBody.GetComponent<Rigidbody>().AddRelativeTorque((Vector3.up * 0.08f) * -1f);
				}
				//characterBody.rigidbody.AddRelativeTorque(characterBody.transform.up * 0.1f);
				//Debug.Log(targetCharacterOrientation);
				//characterBody.transform.localRotation = yRotation;
				//characterBody.transform.localRotation *= targetCharacterOrientation;
				
			}
			
		}
		else
		{
			yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
			transform.localRotation *= yRotation;
		}
	}
}
