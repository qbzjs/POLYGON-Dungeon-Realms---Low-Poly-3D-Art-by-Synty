using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData_Script
{
    public float[] position;

    public PlayerData_Script (William_Script william)
    {
        position = new float[3];
        position[0] = william.transform.position.x;
        position[1] = william.transform.position.y;
        position[2] = william.transform.position.z;
    }
}
