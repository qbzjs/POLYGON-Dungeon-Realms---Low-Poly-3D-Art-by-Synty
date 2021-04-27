using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Interactable : MonoBehaviour
{
    public bool contour;


    //public keyParameter keyParam;
    [Header("objet et axe de rotation")]
    public GameObject axisObject;
    public Axis axis;

    public delegate void Actions();
    static public event Actions INTERACT_ACTIONS;
    static public event Actions UNDO_ACTIONS;

    //public eActionState actionState;

    private Outline InteractOutline;
    private bool isOutline;

    private bool rotate;
    private bool rotateDirection;

    private Quaternion delta;
    private Quaternion deltaInit;


    // Start is called before the first frame update
    void Start()
    {
        isOutline = contour;
        InteractOutline = GetComponent<Outline>();
        if (isOutline)
        {
            INTERACT_ACTIONS += showOutline;
            UNDO_ACTIONS += hideOutline;
        }
        if (axisObject == null)
        {
            return;
        }
        deltaInit = axisObject.transform.rotation;
        if (axis == Axis.X)
        {
            delta = Quaternion.Euler(axisObject.transform.rotation.x - 90, axisObject.transform.rotation.y, axisObject.transform.rotation.z + 178.094f);
        }
        if (axis == Axis.Y)
        {
            delta = Quaternion.Euler(axisObject.transform.rotation.x, axisObject.transform.rotation.y - 180, axisObject.transform.rotation.z);
        }

    }

   

    // Update is called once per frame
    void Update()
    {
        if (rotate && rotateDirection)
        {
            axisObject.transform.rotation = Quaternion.Lerp(axisObject.transform.rotation, delta, 5f * Time.deltaTime); //
        }
        if (rotate && !rotateDirection)
        {
            axisObject.transform.rotation = Quaternion.Lerp(axisObject.transform.rotation, deltaInit, 5f * Time.deltaTime);
        }
        
    }

    public void ExecuteActions()
    {
        // INTERACT_ACTIONS();
        if (!InteractOutline.enabled)
        {
            showOutline();
            return;
        }
        if (InteractOutline.enabled)
        {
            PlayAnim(true);
            return;
        } 
        

    }
    
    public void ExecuteUndo()
    {
        // UNDO_ACTIONS();
        hideOutline();
        PlayAnim(false);
    }

    public void showOutline()
    {
        InteractOutline.OutlineMode = Outline.Mode.OutlineVisible;
        InteractOutline.enabled = true;
    }

    private void hideOutline()
    {
        InteractOutline.enabled = false;
    }

    private void PlayAnim(bool open)
    {
        hideOutline();
        if (axisObject != null)
        {
            rotate = true;
            rotateDirection = open;
        }
    }

}

public enum eActionState
{
    outline,
    key,
    anim,
}

