using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    public Texture2D crosshairImage;
    private bool showXhair;
    private float maxDist;
    // Use this for initialization
    void Start()
    {
       // maxDist = transform.parent.parent.GetComponent<RopeSwingV2>().maxAttatchDistance;
    }
    void OnGUI()
    {
        if (showXhair)
        {
            float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
            float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
            GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
        }
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position,transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDist))
        {
            showXhair = true;
        }
        else
        {
            showXhair = false;
        }
    }
}
