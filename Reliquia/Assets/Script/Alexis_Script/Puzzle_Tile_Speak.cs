using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_Tile_Speak : MonoBehaviour{

        string puzzleTileSpeak = "Puzzle_Tile_Speak";
        public bool solved = false;
    

        void OnTriggerEnter(Collider other){

            if (other.gameObject.name == puzzleTileSpeak){
                solved = true;
            }

        }

        void OnTriggerExit(Collider other){
        if (other.gameObject.name == puzzleTileSpeak){
            solved = false;
        }
    }
}
