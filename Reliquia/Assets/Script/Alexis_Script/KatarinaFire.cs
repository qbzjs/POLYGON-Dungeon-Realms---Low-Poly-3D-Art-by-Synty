using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.ThirdPersonSystem;

public class KatarinaFire : MonoBehaviour
{

    public Transform player;

    public GameObject flamesPrefab;

    public Vector3 targetLastPos;

    public bool stop = false;
    public float lifetime = 3f;
    public float speed = 3f;
    public float speedPhase2 = 30f;

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

        if (other.gameObject.CompareTag("Inflammable"))
        {
            //Transform inflammablePos = c;
            Debug.Log("Touching");
            Instantiate(flamesPrefab, new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z), Quaternion.identity);
            Destroy(other.gameObject, lifetime);
        }

        if (other.gameObject.name == "William_Player")
        {

            Debug.Log("Player hit");

        }
    }


    void DestroyObjectDelayed()
    {
        Destroy(this.gameObject, lifetime);
    }
}
