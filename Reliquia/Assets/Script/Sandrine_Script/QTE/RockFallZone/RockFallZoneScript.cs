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

    private GameObject player;
    private Animator animPlayer;
    private bool runAnim;
    private float timerAnim;
    private float deltaTime = 2f;

    private void Start()
    {
        qteEvent = qte.GetComponentInChildren<QTEEvent>();
        qteManager = qte.GetComponentInChildren<QTEManager>();
    }

    private void Update()
    {
        timerAnim += Time.deltaTime;

        if (runAnim && timerAnim > deltaTime && animPlayer != null)
        {
            runAnim = false;
            animPlayer.SetBool("Esquive", false);
            animPlayer.SetBool("QTEEboulement", false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (animPlayer == null)
            {
                player = other.gameObject;
                animPlayer = player.GetComponent<Animator>();
                animPlayer.SetBool("QTEEboulement", true);
                qteManager.startEvent(qteEvent);
            }
            
            
            
        }
    }

    public void OnQTESuccess()
    {
        if (player != null && animPlayer != null)
        {
            animPlayer.SetBool("Esquive", true);
            timerAnim = -2f;
            runAnim = true;
        }
    }

    public void OnQTEFailed()
    {
        if (player != null && animPlayer != null)
        {
            animPlayer.SetBool("Dead", true);
            timerAnim = -2f;
            runAnim = true;
            StartCoroutine(ReLoad());
        }
    }

    private IEnumerator ReLoad()
    {
        float countWait = 6f;
        yield return new WaitForSeconds(countWait);
        animPlayer.SetBool("Dead", false);
        animPlayer.SetBool("QTEEboulement", false);
        ReLoadCheckpoint();
    }

    private void ReLoadCheckpoint()
    {
        string nom = GameManager.instance.nomSauvegarde;
        if (nom != null && SaveManager.instance != null)
        {
            bool retrunToCheckPoint = true;
            InventaireSauvegarde.instance.ResetSessionInventaire();
            SaveManager.instance.LoadInGame(nom, retrunToCheckPoint);
        }

    }

}
