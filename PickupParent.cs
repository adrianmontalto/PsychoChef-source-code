﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PickupParent : MonoBehaviour
{
    private bool isHolding = false;
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    public GameObject model;

    //always called before any start functions and also just after a prefab is instatiated(if gameobject is inactive during start up awake is not called until it is made active)
    void Awake ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
    //called for every frame
	void FixedUpdate ()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        if(device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            //Debug.Log("holding down touch trigger");
        }
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            //Debug.Log("activated touch down");
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            //Debug.Log("activated touch up");
        }
    }

    void OnTriggerStay(Collider col)
    {
        //checks to see if you are already holding an item
        if(isHolding == false)
        {
            //Debug.Log("you have collided with" + col.name + "and activated on trigger stay");
            //checks to see if your touching an object
            if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
            {
                Debug.Log("you have collided with" + col.name + "while holding down touch");
                //rigidbody is no longer affected by physics system
                col.attachedRigidbody.isKinematic = true;
                //sets the sphere to this objects transform
                col.gameObject.transform.SetParent(this.gameObject.transform);
                model.GetComponent<MeshRenderer>().enabled = false;
                isHolding = true;
            }
        }

        //checks to see if triger is up
        if(device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("you have released touch while colliding with" + col.name);
            //stops object being a child of the controller
            col.gameObject.transform.SetParent(null);
            //sets the rigidbody to be affected by physics sytem
            col.attachedRigidbody.isKinematic = false;
            TossObject(col.attachedRigidbody);
            model.GetComponent<MeshRenderer>().enabled = true;
            isHolding = false;       
        }
    }

    void TossObject(Rigidbody rigidbody)
    {
        //convert local space vectors to world space vectors
        //
        //define origin to define conversion
        Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;

        //check to see if origin is not null
        if(origin !=  null)
        {
            rigidbody.velocity = origin.TransformVector(device.velocity);
            rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else
        {
            rigidbody.velocity = device.velocity;
            rigidbody.angularVelocity = device.angularVelocity;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //checks to see if the hand has hit the stove
        if(col.tag == "StoveKnob")
        {
            col.GetComponent<StoveKnob>().SetActive();
        }

        if(col.tag == "Finish")
        {
            Debug.Log("hit bell");
            col.GetComponent<Bell>().SetDone(true);
        }

        if(col.tag == "SceneButton")
        {
            col.GetComponent<SceneButton>().SetActive(true);
        }
    }
}
