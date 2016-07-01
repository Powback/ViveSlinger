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

    void Start()
    {
        controller = transform.GetComponent<SteamVR_TrackedController>();
        id = (int) controller.GetComponent<SteamVR_TrackedController>().controllerIndex;
        JointHandler.Instance.CreateJoint((int) id);

    }

    public void Hook()
    {
        if (hooked)
            return;

        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit, GameManager.Instance.maxHook);
        if (bHit)
        {
            CreateHook(hit);
        }
    }

    public void UnHook()
    {
        currentHook = null;
        JointHandler.Instance.RemoveLimit(id);
        Debug.Log(id);
        hooked = false;
        GameManager.Instance.player.GetComponent<Controller>().isRoping = false;
    }

    public void CreateHook(RaycastHit hit)
    {
        
        currentHook = Instantiate(hook, hit.point, Quaternion.identity) as GameObject;
        JointHandler.Instance.ConnectBody(id, currentHook.GetComponent<Rigidbody>());
        float distance = Vector3.Distance(transform.position, hit.point);
        JointHandler.Instance.SetLimit(id, distance, "a");
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
        Debug.Log("stop pulling");
       
    }

    private IEnumerator HookPull()
    {
        while (pulling)
        {
            Debug.Log("pulling");
            JointHandler.Instance.SetLimit(id, 99, "p");
            yield return true;
        }
        yield return true;
    }
}
