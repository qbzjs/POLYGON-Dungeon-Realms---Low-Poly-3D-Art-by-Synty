using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Interactable : MonoBehaviour
{
    [Header("Object pour contour blanc")]
    //public bool contour;
    public GameObject goContour;



    //public keyParameter keyParam;

    public GameObject axisObject;
    public Axis axis;
    [Header("Pour l'axe Y uniquement")]
    public rotationDirection direction;

    [HideInInspector]
    

    public delegate void Actions();
    static public event Actions INTERACT_ACTIONS;
    static public event Actions UNDO_ACTIONS;

    //public eActionState actionState;

    private Outline InteractOutline;
    private bool isOutline;

    private bool rotate;
    private bool rotateDirection;

    private bool itemActive; // si l'objet a déjà interagit


    private Quaternion delta;
    private Quaternion deltaInit;


    // Start is called before the first frame update
    void Start()
    {
        //isOutline = contour;
        if (goContour != null)
        {
            isOutline = true;
            InteractOutline = goContour.GetComponent<Outline>();
        }
        else if (GetComponent<Outline>() != null)
        {
            InteractOutline = GetComponent<Outline>();
        }
            

        // InteractOutline = GetComponent<Outline>();
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
            delta = Quaternion.Euler(axisObject.transform.rotation.x - 90, axisObject.transform.rotation.y, axisObject.transform.rotation.z + 178.094f); //  + 178.094f corrige rotation en z
        }
        if (axis == Axis.Y && direction == rotationDirection.Forward)
        {

            delta = axisObject.transform.rotation * Quaternion.Euler(0, 90, 0);
        }
        if (axis == Axis.Y && direction == rotationDirection.Backward)
        {
            delta = axisObject.transform.rotation * Quaternion.Euler(0, -90, 0);
        }

    }

   

    // Update is called once per frame
    void Update()
    {
        if (rotate && rotateDirection && axisObject.transform.rotation != delta)
        {

            axisObject.transform.rotation = Quaternion.Lerp(axisObject.transform.rotation, delta, 5f * Time.deltaTime);

        }
        if (rotate && !rotateDirection && axisObject.transform.rotation != deltaInit)
        {

            axisObject.transform.rotation = Quaternion.Lerp(axisObject.transform.rotation, deltaInit, 5f * Time.deltaTime);

        }
        
    }

    public void applyOutline(bool show)
    {
        if (InteractOutline == null)
        {
            return;
        }
        // INTERACT_ACTIONS();
        if (show) // !InteractOutline.enabled // afficher le contour (OnTriggerExit)
        {
            showOutline();

        } else
        {
            hideOutline();

        }
    }

    public void ExecuteActions()
    {

        if (InteractOutline.enabled && !itemActive) // ouvrir, jouer l'anim (E)
        {
            itemActive = true;
            PlayAnim(true);
            return;
        }
        if (InteractOutline.enabled && itemActive) // ouvrir, jouer l'anim (E)
        {
            itemActive = false;
            PlayAnim(false);
            return;
        }


    }
    
    public void ExecuteUndo() 
    {
        // UNDO_ACTIONS(); OnTriggerExit()
        hideOutline();
        // PlayAnim(false); 
    }

    public void showOutline()
    {
        if (InteractOutline == null)
        {
            return;
        }
        InteractOutline.OutlineMode = Outline.Mode.OutlineVisible;
        InteractOutline.enabled = true;
    }

    private void hideOutline()
    {
        if (InteractOutline == null)
        {
            return;
        }
        InteractOutline.enabled = false;
    }

    private void PlayAnim(bool open)
    {
        //hideOutline();
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

public enum rotationDirection
{
    Forward,
    Backward
}

