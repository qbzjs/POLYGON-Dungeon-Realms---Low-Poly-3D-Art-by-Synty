using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalleGargouillesScript : MonoBehaviour
{
    public GameObject RoxaneGameObject;
    private StateMachine RoxaneStateMachine;
    private Companion RoxaneCompanionScript;
    public GameObject WaypointRoxane;
    public GameObject DavidGameObject;
    private StateMachine DavidStateMachine;
    private Companion DavidCompanionScript;
    public GameObject WaypointDavid;
    public GameObject DirectionRegard;
    public GameObject parentTorche;
    void Awake()
    {
        RoxaneStateMachine = RoxaneGameObject.GetComponent<StateMachine>();
        RoxaneCompanionScript = RoxaneGameObject.GetComponent<Companion>();
        DavidStateMachine = DavidGameObject.GetComponent<StateMachine>();
        DavidCompanionScript = DavidGameObject.GetComponent<Companion>();
    }
    // Start is called before the first frame update
    void Start()
    {
        AllumerTorchesSalle(false);
        RoxaneStateMachine.enabled = false;
        DavidStateMachine.enabled = false;
        RoxaneCompanionScript.Move(WaypointRoxane.transform.position, 2.5f, "");
        DavidCompanionScript.Move(WaypointDavid.transform.position, 2.5f, "");
        StartCoroutine(CheckDestinationAtteinte());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AllumerTorchesSalle(true);
        }
        
    }
    private IEnumerator CheckDestinationAtteinte()
    {
        yield return new WaitForSeconds(0.1f);
        if (RoxaneCompanionScript.ReachDestination() && DavidCompanionScript.ReachDestination())
        {
            RoxaneCompanionScript.StopMoving();
            DavidCompanionScript.StopMoving();
            RoxaneGameObject.transform.LookAt(DirectionRegard.transform);
            DavidGameObject.transform.LookAt(DirectionRegard.transform);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(CheckDestinationAtteinte());
    }

    public void EnigmeResolu()
    {
        RoxaneStateMachine.enabled = true;
        DavidStateMachine.enabled = true;
    }
    public void AllumerTorchesSalle(bool active)
    {
        for (int i = 0; i < parentTorche.transform.childCount; i++)
        {
            Transform transformTorche = parentTorche.transform.GetChild(i);
            for (int y = 0; y < transformTorche.childCount; y++)
            {
                Transform particulesTorche = transformTorche.GetChild(y);
                particulesTorche.gameObject.SetActive(active);
            }
        }
    }
}
