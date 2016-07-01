using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JointHandler : SingletonComponent<JointHandler> {

    public Dictionary<int, ConfigurableJoint> configurableJoints = new Dictionary<int, ConfigurableJoint>();
    public Dictionary<int, float> distance = new Dictionary<int, float>();
    // Use this for initialization
    void Start () {

	}


    public void CreateJoint(int id)
    {
        ConfigurableJoint hookJoint = transform.gameObject.AddComponent<ConfigurableJoint>();
        configurableJoints.Add(id, hookJoint);
        hookJoint.anchor = new Vector3(0, 0, 0);
        hookJoint.axis = new Vector3(1, 1, 1);
        hookJoint.autoConfigureConnectedAnchor = false;
        hookJoint.connectedAnchor = new Vector3(0, 0.3f, 0);
        hookJoint.secondaryAxis = new Vector3(1, 1, 1);
        hookJoint.xMotion = ConfigurableJointMotion.Limited;
        hookJoint.yMotion = ConfigurableJointMotion.Limited;
        hookJoint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit hookJointLimit = hookJoint.linearLimit;
        SoftJointLimitSpring hookJointLimitSpring = hookJoint.linearLimitSpring;
        hookJointLimitSpring.spring = 0;
        hookJointLimitSpring.damper = 0;
        hookJointLimit.limit = 9999999999999999f;
        hookJointLimit.bounciness = 0f;
        hookJointLimit.contactDistance = 0f;

        hookJoint.linearLimit = hookJointLimit;
        hookJoint.linearLimitSpring = hookJointLimitSpring;

    }

    public bool JointExists(int id)
    {
        if (configurableJoints[id] != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool BodyExists(int id)
    {
        if (configurableJoints[id].connectedBody != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ConnectBody(int id, Rigidbody body)
    {
        if (!JointExists(id))
            return;

        if (body != null)
        {
            configurableJoints[id].connectedBody = body;
        }
    }

    public void RemoveLimit(int id)
    {
        if (!JointExists(id))
            return;

        SoftJointLimit linearLimit = configurableJoints[id].linearLimit;
        linearLimit.limit = 99999999999999999999f;
        configurableJoints[id].linearLimit = linearLimit;
    }

    public void SetLimit(int id, float limit, string mode)
    {
        if (!JointExists(id))
            return;

        SoftJointLimit linearLimit = configurableJoints[id].linearLimit;
        if (mode == "a")
            linearLimit.limit = limit;

        if (mode == "p")
        {
            float curLimit = linearLimit.limit;
            float newLimit = (curLimit/100)*limit;
            linearLimit.limit = newLimit;
        }
        configurableJoints[id].linearLimit = linearLimit;
    }
    public void SetSpringLimit(int id, float limit)
    {
        if (!JointExists(id))
            return;

        SoftJointLimitSpring linearLimit = configurableJoints[id].linearLimitSpring;
        linearLimit.spring = limit;
        configurableJoints[id].linearLimitSpring = linearLimit;
    }
    public void SetSpringDamper(int id, float limit)
    {
        if (!JointExists(id))
            return;

        SoftJointLimitSpring linearLimit = configurableJoints[id].linearLimitSpring;
        linearLimit.damper = limit;
        configurableJoints[id].linearLimitSpring = linearLimit;
    }
    public void UpdateAnchor(int id, Vector3 anchor)
    {
        if(!JointExists(id)) 
            return;

        if (configurableJoints[id].connectedBody != null)
        {
            configurableJoints[id].anchor = anchor;
        }
    }

    public void SetConnectedAnchor(int id, Vector3 pos)
    {
        if (!JointExists(id))
            return;

        if (configurableJoints[id].connectedBody != null)
        {
            configurableJoints[id].connectedAnchor = pos;
        }
    }
}
