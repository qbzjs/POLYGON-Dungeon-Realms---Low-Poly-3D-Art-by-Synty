using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingProjectileBehaviour : MonoBehaviour
{
    public GameObject fxImpact;
    public float force = 1000f;
    public float duration = 3f;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * force);
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out InteractionDestructible interactionDestructible))
        {
            interactionDestructible.DetruireObjetLighting();
            Destroy(gameObject);
            Instantiate(fxImpact, transform.position, transform.rotation);
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            Instantiate(fxImpact, transform.position, transform.rotation);
        }
    }
}
