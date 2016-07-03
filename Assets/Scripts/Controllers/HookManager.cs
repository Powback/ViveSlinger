using UnityEngine;
using System.Collections;

public class HookManager : MonoBehaviour
{
    public int id;
    public SteamVR_TrackedController controller;
    public GameObject hook;
    public bool hooked;
    public GameObject currentHook;
    private bool pulling;
    private Controller playerController;

    public float springInAir;
    public float springDamperInAir;
    public float springOnGround;
    public float springDamperOnGround;
    public GameObject line;
    public GameObject hookAttatcher;
    GameObject currentLine;

    void Start()
    {
        controller = transform.GetComponent<SteamVR_TrackedController>();
        id = (int) controller.GetComponent<SteamVR_TrackedController>().controllerIndex;
        JointHandler.Instance.CreateJoint((int) id);
        playerController = GameManager.Instance.player.GetComponent<Controller>().GetComponent<Controller>();
    }

    void Update()
    {
        if (playerController.IsGrounded())
        {
            JointHandler.Instance.SetSpringLimit(id, springOnGround);
            JointHandler.Instance.SetSpringDamper(id, springOnGround);
        } else
        {
            JointHandler.Instance.SetSpringLimit(id, springInAir);
            JointHandler.Instance.SetSpringDamper(id, springDamperOnGround);
        }
    }

    public void Hook()
    {
        if (hooked)
            return;

        hooked = true;
        Ray raycast = new Ray(hookAttatcher.transform.position, hookAttatcher.transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit, GameManager.Instance.maxHook);
        if (bHit)
        {
            CreateHook(hit);
        }
        StartCoroutine(HookUpdate());
    }

    public void UnHook()
    {
        currentHook = null;
        JointHandler.Instance.RemoveLimit(id);
        Debug.Log(id);
        hooked = false;
        GameManager.Instance.player.GetComponent<Controller>().isRoping = false;
        StopCoroutine(HookUpdate());
    }

    public void CreateHook(RaycastHit hit)
    {
        
        currentHook = Instantiate(hook, hit.point, Quaternion.identity) as GameObject;
        JointHandler.Instance.ConnectBody(id, currentHook.GetComponent<Rigidbody>());
        float distance = Vector3.Distance(hookAttatcher.transform.position, hit.point);
        JointHandler.Instance.SetLimit(id, distance, "a", 0);
        JointHandler.Instance.SetSpringLimit(id, 100f);
        JointHandler.Instance.SetSpringDamper(id, 100f);
        //JointHandler.Instance.UpdateAnchor(id, transform.position);
        Vector3 conAnchor = currentHook.transform.InverseTransformPoint(currentHook.transform.position);
        //JointHandler.Instance.SetConnectedAnchor(id, conAnchor);
        GameManager.Instance.player.GetComponent<Rigidbody>().useGravity = true;
        GameManager.Instance.player.GetComponent<Controller>().isRoping = true;
    }


    public void StartHookPull()
    {
        pulling = true;
        StartCoroutine(HookPull());

    }
    public void StopHookPull()
    {
        pulling = false;
        StopCoroutine(HookPull());
        //Debug.Log("stop pulling");
       
    }

    private IEnumerator HookPull()
    {
        while (pulling)
        {
            JointHandler.Instance.SetLimit(id, 60, "p", 1);
            yield return null;
        }
        yield return true;
    }

    private IEnumerator HookUpdate()
    {
        while (hooked)
        {
            JointHandler.Instance.SetAnchorPos(id, hookAttatcher.transform.localPosition);
            if (currentLine != null)
            {
                
                Vector3 between = currentHook.transform.position - hookAttatcher.transform.position;
                float distance = between.magnitude;
                Vector3 scale = new Vector3(0.005f,0.005f, distance);
                currentLine.transform.localScale = scale;
                currentLine.transform.position = hookAttatcher.transform.position + new Vector3(between.x / 2, between.y / 2, between.z / 2);
                currentLine.transform.LookAt(currentHook.transform.position);
            } else
            {
                currentLine = (GameObject) Instantiate(line, Vector3.zero, Quaternion.identity);
            }

            yield return null;
        }
        yield return true;
    }


}
