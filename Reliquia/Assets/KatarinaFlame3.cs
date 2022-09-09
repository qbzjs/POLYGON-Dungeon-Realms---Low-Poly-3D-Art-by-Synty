using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.ThirdPersonSystem;
using AlexandreDialogues;

public class KatarinaFlame3 : MonoBehaviour
{

    public Transform player;

    public GameObject flamesPrefab;
    public GameObject Cutscene;

    public Vector3 targetLastPos;
    public Vector3[] flamesPositions;

    public float randomNumber;

    public bool stop = false;
    public float lifetime = 3f;
    public float speed = 3f;
    public Rigidbody rigid;
    public float speedX = 3f;
    public float speedZ = 3f;


    private Health _thirdPersonSystem;




    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _thirdPersonSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        DestroyObjectDelayed();
        // targetLastPos = player.position;

        Vector3 targetLastPos = new Vector3(player.position.x, (player.position.y + 1f), player.position.z);
        transform.LookAt(targetLastPos);

        randomNumber = Random.Range(0, 4);
        Debug.Log("Does it even instanciate");
        switch (randomNumber)
        {
            case 0:
                this.transform.position = flamesPositions[0];
                this.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case 1:
                this.transform.position = flamesPositions[1];
                this.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case 2:
                this.transform.position = flamesPositions[2];
                this.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case 3:
                this.transform.position = flamesPositions[3];
                this.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
        }
    }

    void Update()
    {


        if (transform.position != targetLastPos && stop == false)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        if (transform.position == targetLastPos && stop == false)
        {
            stop = true;
            Rigidbody fireRigidBody = this.gameObject.AddComponent<Rigidbody>();
        }

        switch (randomNumber)
        {
            case 0:
                rigid.velocity = new Vector3(speedX, 0, 0);
                break;
            case 1:
                rigid.velocity = new Vector3(0, 0, -speedZ);
                break;
            case 2:
                rigid.velocity = new Vector3(-speedX, 0, 0);
                break;
            case 3:
                rigid.velocity = new Vector3(0, 0, speedZ);
                break;
        }


    }

    void OnCollisionEnter(Collision other)
    {
        //Destroy(this.gameObject);

       // if (other.gameObject.name == "William_Player")
       // {
//
         //   Debug.Log("Player hit");
        //    SoundManager.instance.Play("Explosion");

      //  }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "William_Player")
        {

            Debug.Log("Player hit");
            SoundManager.instance.Play("Explosion");
            // _thirdPersonSystem.Die();
            // appeler thirdPersonSystem et faire le calcul des dommages
            // faire des dégats overtime ici

        }


    }



    void DestroyObjectDelayed()
    {
        Destroy(this.gameObject, lifetime);
    }
}
