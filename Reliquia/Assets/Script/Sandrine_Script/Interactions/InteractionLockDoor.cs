using AlexandreDialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class InteractionLockDoor : MonoBehaviour
{
    public GameObject axisObject;
    public Axis axis;
    [Header("Pour l'axe Y uniquement")]
    public rotationDirection direction;

    public ItemInventaire item;

    private bool rotate;
    private bool rotateDirection;

    private Quaternion delta;
    private Quaternion deltaInit;

    private Interactable interactable;

    private William_Script wilScript;
    private GameManager gameManager;
    private bool isLockedDoor;

    // Interaction Dialogue
    private InGameDialogueManager inGameDialogueManager;
    private GameObject dialogue;
    private InGameDialogue inGameDialogueClef;
    private InGameDialogue inGameDialogueSesame;

    private void OnEnable()
    {
        
        William_Script.INTERACT_ACTIONS += OpenDoor;
    }

    private void OnDisable()
    {
        William_Script.INTERACT_ACTIONS -= OpenDoor;
    }

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        wilScript = GameObject.FindGameObjectWithTag("Player").GetComponent<William_Script>();
        gameManager = FindObjectOfType<GameManager>();
        inGameDialogueManager = FindObjectOfType<InGameDialogueManager>();
        dialogue = GameObject.FindGameObjectWithTag("DialogueDoorLock");
        inGameDialogueClef = dialogue.GetComponent<DialogueAttached>().inGameDialogue;
        dialogue.SetActive(false);
        dialogue = GameObject.FindGameObjectWithTag("DialogueSesame");
        inGameDialogueSesame = dialogue.GetComponent<DialogueAttached>().inGameDialogue;
        dialogue.SetActive(false);


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

    private void OpenDoor()
    {
        bool hasKey = InventaireManager.instance.playerInventory.sacochesInventory.Contains(item);
        //bool hasKey = wilScript.HasKey(item);
        if (!hasKey)
        {
            isLockedDoor = true;
        }
        else
        {
            //bool useKey = wilScript.UseKey(item);
            //isLockedDoor = !useKey;
            InventaireManager.instance.playerInventory.sacochesInventory.Remove(item);
            isLockedDoor = false;
        }

        if (isLockedDoor)
        {
            //gameManager.AfficherMessageInteraction("Missing Key");
            gameManager.FermerMessageInteraction();
            inGameDialogueManager.StartDialogue(inGameDialogueClef);
            return;
        }
        inGameDialogueManager.StartDialogue(inGameDialogueSesame);
        if (interactable.InteractOutline.enabled && !interactable.itemActive) // ouvrir, jouer l'anim (E)
        {
            interactable.itemActive = true;
            PlayAnim(true);
            return;

        }
        if (interactable.InteractOutline.enabled && interactable.itemActive) // ouvrir, jouer l'anim (E)
        {
            interactable.itemActive = false;
            PlayAnim(false);


        }
    }

    private void PlayAnim(bool open)
    {
        //hideOutline();
        if (axisObject != null)
        {
            rotate = true;
            rotateDirection = open;
            StartCoroutine(RotateAnim());
        }
    }

    private IEnumerator RotateAnim()
    {
        if (rotate && rotateDirection && axisObject.transform.rotation != delta)
        {
            int i = 50;
            do
            {
                i--;
                axisObject.transform.rotation = Quaternion.Lerp(axisObject.transform.rotation, delta, 5f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            } while (i > 0);



        }
        if (rotate && !rotateDirection && axisObject.transform.rotation != deltaInit)
        {
            int i = 50;
            do
            {
                i--;
                axisObject.transform.rotation = Quaternion.Lerp(axisObject.transform.rotation, deltaInit, 5f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            } while (i > 0);

        }
    }

}
