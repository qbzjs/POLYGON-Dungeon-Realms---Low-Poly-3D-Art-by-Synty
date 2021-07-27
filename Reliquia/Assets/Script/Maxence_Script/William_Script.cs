using Boo.Lang;
using clavier;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class William_Script : MonoBehaviour
{

    GameManager gameManager;
    RaccourciClavier_Script raccourciClavier;
    Compas_Script compas_Script;
    PhysicaltemInventaire physicaltemInventaire;

    public PlayerInventory playerInventory;
    public Inventaire_Script inventaire;

    private CharacterController characterController;

    [SerializeField] private GameObject ParentMenu;

    public static William_Script instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        raccourciClavier = FindObjectOfType<RaccourciClavier_Script>();
        compas_Script = FindObjectOfType<Compas_Script>();
        gameManager = FindObjectOfType<GameManager>();
    }

    ItemInventaire item;
    Interactable interactableObject;
    Interactable interactableItem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            physicaltemInventaire = other.gameObject.GetComponent<PhysicaltemInventaire>();
            item = physicaltemInventaire.thisItem;
            gameManager.AfficherMessageInteraction("");
        }
        if (other.CompareTag("Interactable")) //&& interactableItem == null
        {

            interactableObject = other.gameObject.GetComponent<Interactable>();
            interactableObject.ApplyOutline(true);

        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (gameManager != null) // work arround : error on play (sandrine)
        {
            gameManager.FermerMessageInteraction();
        }
        if (other.CompareTag("Interactable"))
        {
            interactableObject = null;
            other.gameObject.GetComponent<Interactable>().ApplyOutline(false);
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(raccourciClavier.toucheClavier["Action"]) && GameManager.instance.MessageInteraction != null && GameManager.instance.MessageInteraction.activeSelf == true)
        {
            if (physicaltemInventaire != null && item != null)
            {
                physicaltemInventaire.AddItem(item);
                //gameManager.FermerMessageInteraction();
                interactableItem = null;
            }
            gameManager.FermerMessageInteraction();
        }
        if (Input.GetKeyDown(raccourciClavier.toucheClavier["Action"]) && interactableObject != null)
        {
            bool needKey = interactableObject.IsLocked();
            bool isLockedDoor = false;
            if (needKey)
            {
                ItemInventaire key = interactableObject.GetKey();

                if (key != null)
                {

                    bool hasKey = playerInventory.GetItemByTypeFromSacoche(key);
                    bool useKey = playerInventory.UseItem(key);

                    if (!hasKey)
                    {
                        isLockedDoor = true;
                        gameManager.AfficherMessageInteraction("Missing Key");
                    }
                } else
                {
                    isLockedDoor = true;
                    gameManager.AfficherMessageInteraction("Missing Key");
                }
                
            }

            bool isOnlyOnceInteract = false;

            if (!isLockedDoor)
            {
                isOnlyOnceInteract = interactableObject.ExecuteActions();
            }
             

            if (isOnlyOnceInteract)
            {
                interactableObject.ApplyOutline(false);
                interactableObject = null;                
            }
            gameManager.FermerMessageInteraction();
        }
        RaycastHit hit;
        //Vector3 rayDirection = (Vector3.forward - Vector3.up) - (transform.position + Vector3.up);
        //Vector3 rayDirection = Vector3.forward - Vector3.right;

        var angle = transform.rotation; //* startingAngle
        var rayDirection = angle * Vector3.forward;

        rayDirection.Normalize();
        Debug.Log("RayDirection : " + rayDirection);
        float rayDistance = 2f;

        if (Physics.Raycast(transform.position, rayDirection, out hit, rayDistance))
        {
            
            //Debug.DrawRay(transform.position, rayDirection, Color.red);
            var target = hit.transform;
            if (target != null && target.CompareTag("ItemInteractable"))
            {
            //Debug.DrawRay(transform.position, rayDirection, Color.cyan);
            // Set Inventaire
            physicaltemInventaire = target.gameObject.GetComponent<PhysicaltemInventaire>();
            item = physicaltemInventaire.thisItem;
            gameManager.AfficherMessageInteraction("");
            // Add Contour Blanc
            interactableItem = target.gameObject.GetComponent<Interactable>();
            interactableItem.ApplyOutline(true);
            }
            else
            {
                gameManager.FermerMessageInteraction();
            }
        }else if (interactableItem != null)
        {
            interactableItem.ApplyOutline(false);
            gameManager.FermerMessageInteraction();
        }

        }
    

}
