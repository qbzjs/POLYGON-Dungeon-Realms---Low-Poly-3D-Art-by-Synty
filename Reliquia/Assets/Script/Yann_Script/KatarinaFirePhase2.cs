using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.ThirdPersonSystem;
using AlexandreDialogues;

public class KatarinaFirePhase2 : MonoBehaviour
{

    public Transform player;

    public GameObject flamesPrefab;
    public GameObject Cutscene;

    public Vector3 targetLastPos;

    public bool stop = false;
    public float lifetime = 3f;
    public float speed = 100f;


    private Health _thirdPersonSystem;




    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        DestroyObjectDelayed();
        // targetLastPos = player.position;

        Vector3 targetLastPos = new Vector3(player.position.x, (player.position.y + 1f), player.position.z);
        transform.LookAt(targetLastPos);
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
    }

    void OnCollisionEnter(Collision other)
    {
        Destroy(this.gameObject);

        if (other.gameObject.name == "William_Player")
        {

            Debug.Log("Player hit");
            SoundManager.instance.Play("Explosion");

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Inflammable")
        {
            //Transform inflammablePos = c;
            SoundManager.instance.Play("Explosion");
            Destroy(this.gameObject);
        }

        if (other.gameObject.name == "Cylinder")
        {
            Debug.Log("Cylinder hit");
            SoundManager.instance.Play("Explosion");
            Destroy(this.gameObject);


        }
    }



    void DestroyObjectDelayed()
    {
        Destroy(this.gameObject, lifetime);
    }
}
