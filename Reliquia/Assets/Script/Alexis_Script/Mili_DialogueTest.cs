using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mili_DialogueTest : MonoBehaviour{

    public Text dialogueTest;

    public string[] dialoguesTestMili1;
    public string[] dialoguesTestMili2;
    public string[] dialoguesTestMili3;
    public string[] dialoguesTestMiliLighting;
    public string selectedString;

    public float[] textTimeDisplay; 

    public bool textWasDisplayed1;
    public bool textWasDisplayed2;
    public bool textWasDisplayed3;
    public bool textWasDisplayedLighting;


    void OnTriggerEnter(Collider c){

        

        if(c.gameObject.name == "Dialogue Trigger 1" && textWasDisplayed1 == false){
            textWasDisplayed1 = true;
            selectedString = dialoguesTestMili1[0];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger1());
        }

        // Continue here

        if (c.gameObject.name == "Dialogue Trigger 2" && textWasDisplayed2 == false)
        {
            textWasDisplayed2 = true;
            selectedString = dialoguesTestMili2[0];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger2());
        }

        if (c.gameObject.name == "Dialogue Trigger 3" && textWasDisplayed3 == false)
        {
            textWasDisplayed3 = true;
            selectedString = dialoguesTestMili3[0];
            dialogueTest.text = selectedString;
            StartCoroutine(DialogueTrigger3());
        }

        


    }

    IEnumerator DialogueTrigger1(){
        dialogueTest.gameObject.SetActive(true);
        yield return new WaitForSeconds (textTimeDisplay[0]);
        selectedString = dialoguesTestMili1[1];
        dialogueTest.text = selectedString;
        yield return new WaitForSeconds (textTimeDisplay[0]);
        dialogueTest.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator DialogueTrigger2(){
        dialogueTest.gameObject.SetActive(true);
        yield return new WaitForSeconds (textTimeDisplay[0]);
        dialogueTest.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator DialogueTrigger3(){
        dialogueTest.gameObject.SetActive(true);
        yield return new WaitForSeconds (textTimeDisplay[2]);
        selectedString = dialoguesTestMili3[1];
        dialogueTest.text = selectedString;
        yield return new WaitForSeconds (textTimeDisplay[0]);
        selectedString = dialoguesTestMili3[2];
        dialogueTest.text = selectedString;
        yield return new WaitForSeconds (textTimeDisplay[0]);
        selectedString = dialoguesTestMili3[3];
        dialogueTest.text = selectedString;
        yield return new WaitForSeconds (textTimeDisplay[1]);
        dialogueTest.gameObject.SetActive(false);
        yield break;
    }

    // Booléen nécessaire : Si le joueur active Lighting

    IEnumerator DialogueTriggerLighting(){

        if(textWasDisplayedLighting == false){

            textWasDisplayedLighting = true;

            dialogueTest.gameObject.SetActive(true);
            selectedString = dialoguesTestMiliLighting[0];
            dialogueTest.text = selectedString;
            yield return new WaitForSeconds (textTimeDisplay[0]);
            selectedString = dialoguesTestMiliLighting[1];
            dialogueTest.text = selectedString;
            yield return new WaitForSeconds (textTimeDisplay[0]);
            selectedString = dialoguesTestMiliLighting[2];
            dialogueTest.text = selectedString;
            yield return new WaitForSeconds (textTimeDisplay[0]);
            selectedString = dialoguesTestMiliLighting[3];
            dialogueTest.text = selectedString;
            yield return new WaitForSeconds (textTimeDisplay[0]);
            dialogueTest.gameObject.SetActive(false);
            yield break;

        }
    }


}
