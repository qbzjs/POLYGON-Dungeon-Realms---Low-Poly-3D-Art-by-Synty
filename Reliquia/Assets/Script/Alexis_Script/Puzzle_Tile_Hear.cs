using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_Tile_Hear : MonoBehaviour{

        string puzzleTileHear = "Puzzle_Tile_Hear";
        public bool solved = false;
    

        void OnTriggerEnter(Collider other){

            if (other.gameObject.name == puzzleTileHear){
                solved = true;
            }

        }

        void OnTriggerExit(Collider other){
        if (other.gameObject.name == puzzleTileHear){
            solved = false;
        }
    }
}
