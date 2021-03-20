using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_Tile_See : MonoBehaviour{

        string puzzleTileSee = "Puzzle_Tile_See";
        public bool solved = false;
    

        void OnTriggerEnter(Collider other){

            if (other.gameObject.name == puzzleTileSee){
                solved = true;
            }

        }

        void OnTriggerExit(Collider other){
        if (other.gameObject.name == puzzleTileSee){
            solved = false;
        }
    }
}
