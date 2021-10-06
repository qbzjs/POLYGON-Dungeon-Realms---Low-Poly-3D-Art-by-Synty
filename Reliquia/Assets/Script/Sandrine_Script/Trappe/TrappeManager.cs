using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappeManager : MonoBehaviour
{
    private Animator[] pivots;
    public Camera cameraTrappe;

    // Start is called before the first frame update
    void Start()
    {
        //int count = GetComponentsInChildren<Animator>().Length;
        //pivots = new Animator[count];
        pivots = GetComponentsInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cameraTrappe.enabled = true;
            for (int i = 0; i < pivots.Length; i++)
            {
                pivots[i].SetBool("Activated", true);
            }
        }
    }
}
