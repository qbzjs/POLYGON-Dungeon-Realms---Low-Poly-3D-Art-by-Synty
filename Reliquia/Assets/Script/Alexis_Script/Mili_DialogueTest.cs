using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mili_DialogueTest : MonoBehaviour{

    public Text dialogueTest;

    public string[] dialoguesTestMili;
    public string selectedString;

    public float[] textTimeDisplay; 

    public bool textWasDisplayed1;
    public bool textWasDisplayed2;
    public bool textWasDisplayed3;


    void OnTriggerEnter(Collider c){

        

        if(c.gameObject.name == "Dialogue Trigger 1" && textWasDisplayed1 == false){
            textWasDisplayed1 = true;
            selectedString = dialoguesTestMili[0];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger());
        }

        // Continue here

        if (c.gameObject.name == "Dialogue Trigger 2" && textWasDisplayed2 == false)
        {
            textWasDisplayed2 = true;
            selectedString = dialoguesTestMili[1];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger());
        }

        if (c.gameObject.name == "Dialogue Trigger 3" && textWasDisplayed3 == false)
        {
            textWasDisplayed3 = true;
            selectedString = dialoguesTestMili[2];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger());
        }

        


    }

    IEnumerator DialogueTrigger(){
        dialogueTest.gameObject.SetActive(true);
        yield return new WaitForSeconds (textTimeDisplay[0]);
        dialogueTest.gameObject.SetActive(false);
        yield break;
    }


}
