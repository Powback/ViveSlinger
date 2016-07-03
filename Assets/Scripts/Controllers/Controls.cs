using UnityEngine;
using System.Collections;
using VRTK;

public class Controls : MonoBehaviour
{
    private HookManager hookManager;

    // Use this for initialization
    void Start()
    {

        hookManager = GetComponent<HookManager>();
        if (GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError(
                "VRTK_ControllerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

        GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged +=
            new ControllerInteractionEventHandler(DoTriggerAxisChanged);

        GetComponent<VRTK_ControllerEvents>().ApplicationMenuPressed +=
            new ControllerInteractionEventHandler(DoApplicationMenuPressed);
        GetComponent<VRTK_ControllerEvents>().ApplicationMenuReleased +=
            new ControllerInteractionEventHandler(DoApplicationMenuReleased);

        GetComponent<VRTK_ControllerEvents>().GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
        GetComponent<VRTK_ControllerEvents>().GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        GetComponent<VRTK_ControllerEvents>().TouchpadReleased +=
            new ControllerInteractionEventHandler(DoTouchpadReleased);

        GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart +=
            new ControllerInteractionEventHandler(DoTouchpadTouchStart);
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd +=
            new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged +=
            new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
    }

    void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
    {
        //Debug.Log("Controller on index '" + index + "' " + button + " has been " + action
        // + " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
    }

    void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "pressed down", e);
        hookManager.Hook();
    }

    void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "released", e);
        hookManager.UnHook();
    }

    void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "axis changed", e);
    }

    void DoApplicationMenuPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "pressed down", e);
        GameManager.Instance.player.transform.position = Vector3.zero;
    }

    void DoApplicationMenuReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "released", e);
    }

    void DoGripPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "pressed down", e);
    }

    void DoGripReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "released", e);
    }

    void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (GameManager.Instance.controller.IsGrounded())
        {
            GameManager.Instance.controller.Jump();
        }

        if (GameManager.Instance.controller.isRoping)
        {
            hookManager.StartHookPull();
        }

    }

    void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "released", e);
        
        hookManager.StopHookPull();
        
    }

    void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "touched", e);
        float x = e.touchpadAxis.x;
        float y = e.touchpadAxis.y;

        Vector3 vel = new Vector3(x, 0f, y);
        GameManager.Instance.controller.targetVelocityRaw = vel;
    }

    void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "untouched", e);
        GameManager.Instance.controller.targetVelocityRaw = Vector3.zero;
    }

    void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "axis changed", e);
        float x = e.touchpadAxis.x;
        float y = e.touchpadAxis.y;
 
        Vector3 vel = new Vector3(x, 0f, y);
        GameManager.Instance.controller.targetVelocityRaw = vel;


    }
}