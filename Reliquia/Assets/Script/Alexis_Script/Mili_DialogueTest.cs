using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mili_DialogueTest : MonoBehaviour{

    public Text dialogueTest;

    public string[] dialoguesTestMili;
    public string selectedString;

    public float[] textTimeDisplay; 


    void OnTriggerEnter(Collider c){

        StartCoroutine(DialogueTrigger());

        if(c.gameObject.name == "Dialogue Trigger 1"){
            selectedString = dialoguesTestMili[0];
            dialogueTest.text = selectedString;
        }

        // Continue here

        if (c.gameObject.name == "Dialogue Trigger 2")
        {
            selectedString = dialoguesTestMili[1];
            dialogueTest.text = selectedString;
        }

        

    }

    IEnumerator DialogueTrigger(){
        dialogueTest.gameObject.SetActive(true);
        yield return new WaitForSeconds (textTimeDisplay[0]);
        dialogueTest.gameObject.SetActive(false);
        yield break;
    }


}
