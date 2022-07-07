using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ItemEffect : MonoBehaviour
{
    public void Heal()
    {
        Debug.Log("heal");
    }

    public void IncreaseMana()
    {
        Debug.Log("IncreaseMana");
    }
}
