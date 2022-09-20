using System.Collections;
using System.Collections.Generic;
using DiasGames.ThirdPersonSystem;
using UnityEngine;

public class KatarinaScript : MonoBehaviour
{

    public GameObject fireBall;
    public GameObject fireBall2;
    public GameObject ColumnFlame;
    public GameObject KatarinaPraesidium;
    public Transform player;
    public GameObject Light;
    public GameObject Cutscene1;
    public GameObject Cutscene2;

    public bool active = false;
    public float distanceActiv = 5f;

    public float cooldownTimer;
    public float cooldown = 3f;
    public float FlameCooldown = 6f;

    public GameObject TP1;
    public GameObject TP2;
    public GameObject TP3;
    public GameObject TP4;
    public GameObject TPMiddle;

    public GameObject Cylinder1;
    public GameObject Cylinder2;
    public GameObject Cylinder3;
    public GameObject Cylinder4;

    public bool FirstTP = true;
    public bool Cutscene = false;
    public bool Phase1 = true;
    public bool Phase2 = false;
    public bool Phase3 = false;

    private bool C1Destroyed = false;
    private bool C2Destroyed = false;
    private bool C3Destroyed = false;
    private bool C4Destroyed = false;

    public float KatarinaLife = 5f;


    void Start()
    {
        player = GameObject.Find("William_Player").GetComponent<Transform>();
    }

    void Update()
    {

        float dist = Vector3.Distance(player.position, transform.position);


        if (dist < distanceActiv)
        {
            active = true;  

            if(KatarinaLife < 3f && Phase1 == true)
            {
                Light.SetActive(false);
            }

            if(KatarinaLife == 0f && Phase1 == true)
            {
                Phase1 = false;
                Phase2 = true;
                
            }

            if(FirstTP == true && Phase2 == true)
            {
                TeleportAway();
                FirstTP = false;
                
            }

            if(KatarinaLife == 0f && Phase2 == true)
            {
                Light.SetActive(false);
            }

            if(C1Destroyed && C2Destroyed  && C3Destroyed && C4Destroyed == null)
            {
                TeleportBackToMiddle();
                // Méthode secondaire pour T2 --> T3, ne marche pas :(
            }
        }

        if (active == true)
        {

            transform.LookAt(player);

            if (cooldownTimer >= 0)
            {
                cooldownTimer -= 1 * Time.deltaTime;
            }

            if (cooldownTimer <= 0 && Cutscene == false && Phase1 == true)
            {
                Instantiate(fireBall, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1f, this.gameObject.transform.position.z), Quaternion.identity);
                SoundManager.instance.Play("Cast");
                cooldownTimer = cooldown;
            }

            if (cooldownTimer <= 0 && Cutscene == false && Phase2 == true)
            {
                Instantiate(fireBall2, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1f, this.gameObject.transform.position.z), Quaternion.identity);
                SoundManager.instance.Play("Cast");
                cooldownTimer = cooldown;
            }

            if (cooldownTimer <= 0 && Cutscene == false && Phase3 == true)
            {
                Instantiate(ColumnFlame, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1f, this.gameObject.transform.position.z), Quaternion.identity);
                SoundManager.instance.Play("Cast");
                cooldownTimer = FlameCooldown;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pouvoir"))
        {
            KatarinaLife = KatarinaLife - 1f;


            if (KatarinaLife == 0f && Phase1 == true)
            {
               // Cutscene = true;
               // Cutscene1.SetActive(true);
                Debug.Log("Phase2");
                KatarinaLife = 3f;
                Phase1 = false;
                Phase2 = true;
                Cutscene1.SetActive(true);
            }
                if (Phase2 == true)
            {
                
                TeleportAway();
                SoundManager.instance.Play("tp_away");
            }

            if (KatarinaLife == 0f && Phase2 == true)
            {
                Debug.Log("Phase3");
                KatarinaLife = 4f;
                Phase2 = false;
                Phase3 = true;
            }
            if (Phase3 == true)
            {

                TeleportBackToMiddle();
                SoundManager.instance.Play("tp_away");
                KatarinaPraesidium.SetActive(true);
                Instantiate(ColumnFlame, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1f, this.gameObject.transform.position.z), Quaternion.identity);
                SoundManager.instance.Play("Cast");
            }

           if (KatarinaLife == 3f && Phase3 == true && KatarinaPraesidium == false);
            {
                KatarinaPraesidium.SetActive(true);
            }

            if (KatarinaLife == 2f && Phase3 == true && KatarinaPraesidium == false) ;
            {
                KatarinaPraesidium.SetActive(true);
            }

            if (KatarinaLife == 1f && Phase3 == true && KatarinaPraesidium == false) ;
            {
                KatarinaPraesidium.SetActive(true);
            }
        }
    }

    public void TeleportAway()
    {
        int TPAwayCount = Random.Range(0, 5);
        if(TPAwayCount == 1)
        {
            this.transform.position = TP1.transform.position;
            Debug.Log("Pick 1");
            SoundManager.instance.Play("tp_away");
        }

        if (TPAwayCount == 2)
        {
            this.transform.position = TP2.transform.position;
            Debug.Log("Pick 2");
            SoundManager.instance.Play("tp_away");
        }

        if (TPAwayCount == 3)
        {
            this.transform.position = TP3.transform.position;
            Debug.Log("Pick 3");
            SoundManager.instance.Play("tp_away");
        }

        if (TPAwayCount == 4)
        {
            this.transform.position = TP4.transform.position;
            Debug.Log("Pick 4");
            SoundManager.instance.Play("tp_away");
        }
    }

    public void TeleportBackToMiddle()
    {
        this.transform.position = TPMiddle.transform.position;
        SoundManager.instance.Play("tp_away");
    }

    private void OnDestroy()
    {
        if(Cylinder1.gameObject == null)
        {
            Debug.Log("Detruit");
            C1Destroyed = true;
}

        if (Cylinder2.gameObject == null)
        {
            Debug.Log("Detruit");
            C2Destroyed = true;
        }

        if (Cylinder3.gameObject == null)
        {
            Debug.Log("Detruit");
            C3Destroyed = true;
        }

        if (Cylinder4.gameObject == null)
        {
            Debug.Log("Detruit");
            C4Destroyed = true;
        }
    }
}


