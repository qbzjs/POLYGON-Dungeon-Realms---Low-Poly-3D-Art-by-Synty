using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFallZoneScript : MonoBehaviour
{

    public GameObject qte;
    [HideInInspector]
    public QTEEvent qteEvent;
    [HideInInspector]
    public QTEManager qteManager;

    private void Start()
    {
        qteEvent = qte.GetComponentInChildren<QTEEvent>();
        qteManager = qte.GetComponentInChildren<QTEManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            qteManager.startEvent(qteEvent);
        }
    }

}
