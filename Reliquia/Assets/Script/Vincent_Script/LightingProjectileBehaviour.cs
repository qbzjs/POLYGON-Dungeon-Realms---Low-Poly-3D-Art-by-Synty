using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingProjectileBehaviour : MonoBehaviour
{
    public GameObject fxImpact;
    public float force = 1000f;
    public float duration = 3f;

    public IFighter owner;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * force);
        Destroy(gameObject, duration);

        if (gameObject.GetComponent<William_Script>() != null) owner = GetComponent<William_Script>();
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

            Enemy _enemy = other.gameObject.GetComponent<Enemy>();
            _enemy.Hurt(owner.GetStrength());
        }
    }
}
