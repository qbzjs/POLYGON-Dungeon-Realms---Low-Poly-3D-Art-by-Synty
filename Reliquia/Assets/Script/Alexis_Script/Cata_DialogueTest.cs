using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cata_DialogueTest : MonoBehaviour{

    public Text dialogueTest;

    public string[] dialoguesTestCata1;
    public string[] dialoguesTestCata2;
    public string[] dialoguesTestCata3;
    public string selectedString;

    public float[] textTimeDisplay; 

    public bool textWasDisplayed1;
    public bool textWasDisplayed2;
    public bool textWasDisplayed3;


    void OnTriggerEnter(Collider c){

        

        if(c.gameObject.name == "Dialogue Trigger 1" && textWasDisplayed1 == false){
            textWasDisplayed1 = true;
            selectedString = dialoguesTestCata1[0];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger1());
        }

        // Continue here

        if (c.gameObject.name == "Dialogue Trigger 2" && textWasDisplayed2 == false)
        {
            textWasDisplayed2 = true;
            selectedString = dialoguesTestCata2[0];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger2());
        }

        if (c.gameObject.name == "Dialogue Trigger 3" && textWasDisplayed3 == false)
        {
            textWasDisplayed3 = true;
            selectedString = dialoguesTestCata3[0];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger3());
        }

        


    }


    IEnumerator DialogueTrigger1(){
        dialogueTest.gameObject.SetActive(true);
        yield return new WaitForSeconds (textTimeDisplay[0]);
        dialogueTest.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator DialogueTrigger2(){
        dialogueTest.gameObject.SetActive(true);
        yield return new WaitForSeconds (textTimeDisplay[0]);
        selectedString = dialoguesTestCata2[1];
        dialogueTest.text = selectedString;
        yield return new WaitForSeconds (textTimeDisplay[0]);
        dialogueTest.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator DialogueTrigger3(){
        dialogueTest.gameObject.SetActive(true);
        yield return new WaitForSeconds (textTimeDisplay[0]);
        selectedString = dialoguesTestCata3[1];
        dialogueTest.text = selectedString;
        yield return new WaitForSeconds (textTimeDisplay[1]);
        selectedString = dialoguesTestCata3[2];
        dialogueTest.text = selectedString;
        yield return new WaitForSeconds (textTimeDisplay[0]);
        dialogueTest.gameObject.SetActive(false);
        yield break;
    }


}
