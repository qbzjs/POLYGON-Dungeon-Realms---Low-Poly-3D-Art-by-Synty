using DiasGames.ThirdPersonSystem;
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

    public eInteractableType type;

    public enum eInteractableType
    {
        Door,
        Chest,
        Brasier,
        DoorLock, 
        Bandage,
        Clef

    }


    [HideInInspector]
    

    

    // private

    public Outline InteractOutline;

    public bool itemActive; // si l'objet a déjà interagit

    private Interactable[] childrenInteractable;


    // Start is called before the first frame update
    void Start()
    {
        
        //isOutline = contour;
        if (goContour != null)
        {
            InteractOutline = goContour.GetComponent<Outline>();
        }
        else if (GetComponent<Outline>() != null)
        {
            InteractOutline = GetComponent<Outline>();
        }

    }

    public void ApplyOutline(bool show)
    {
        if (InteractOutline == null)
        {
            return;
        }
        // INTERACT_ACTIONS();
        if (show) // !InteractOutline.enabled // afficher le contour (OnTriggerExit)
        {
            ShowOutline();
            ActiveInteraction(true);

        } else
        {
            hideOutline();
            ActiveInteraction(false);
        }
    }

    public void ActiveInteraction(bool On)
    {
        switch (type)
        {
            case eInteractableType.Door:
                InteractionDoor iDoor = GetComponent<InteractionDoor>();
                iDoor.enabled = On;
                break;
            case eInteractableType.Chest:
                InteractionChest iChest = GetComponent<InteractionChest>();
                iChest.enabled = On;
                break;
            case eInteractableType.Brasier:
                InteractionBrasier iBrasier = GetComponent<InteractionBrasier>();
                iBrasier.enabled = On;
                break;
            case eInteractableType.DoorLock:
                InteractionLockDoor iDoorLock = GetComponent<InteractionLockDoor>();
                iDoorLock.enabled = On;
                break;
            case eInteractableType.Bandage:
                break;
            case eInteractableType.Clef:
                break;
            default:
                break;
        }
    }

    // Active les collider uniquement pour les items à l'interieur d'objets interactables
    public bool InOnlyOnce()
    {
        childrenInteractable = goContour.GetComponentsInChildren<Interactable>();// Object interactable 1 seule fois
        // Debug.Log("childrenInteractable : " + childrenInteractable);
        if (childrenInteractable.Length > 0)
        {
            for (int i = 0; i < childrenInteractable.Length; i++)
            {
                // Debug.Log("childre is : " + childrenInteractable[i].gameObject.name);
                if (childrenInteractable[i].gameObject.CompareTag("ItemInteractable"))
                {
                    // Debug.Log("childre is with itemInteractable: " + childrenInteractable[i].gameObject.name);
                    childrenInteractable[i].gameObject.GetComponent<Collider>().enabled = true;
                }
            }

            goContour.GetComponent<Collider>().enabled = false;
            hideOutline();
            itemActive = false;
            return true;
        }
        return false;
    }

    public void ShowOutline()
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
        if (InteractOutline == null )
        {
            return;
        }
        InteractOutline.enabled = false;
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

