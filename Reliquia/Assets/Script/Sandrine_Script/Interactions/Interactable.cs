using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool contour;

    
    //public keyParameter keyParam;

    public delegate void Actions();
    static public event Actions INTERACT_ACTIONS;
    static public event Actions UNDO_ACTIONS;

    public eActionState actionState;

    private Outline InteractOutline;
    private bool isOutline;


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
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteActions()
    {
        Debug.Log("This is : " + this);
        // INTERACT_ACTIONS();
        showOutline();

    }
    
    public void ExecuteUndo()
    {
        // UNDO_ACTIONS();
        hideOutline();
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

}

public enum eActionState
{
    outline,
    key,
    anim,
}

[Serializable]
public struct keyParameter
{
    public bool isKey;
    public char key;
}
