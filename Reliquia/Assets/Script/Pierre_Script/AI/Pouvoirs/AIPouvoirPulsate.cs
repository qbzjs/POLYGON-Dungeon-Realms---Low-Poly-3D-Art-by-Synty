using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPouvoirPulsate : AIPouvoir
{
    [SerializeField]
    private ParticleSystem _particulesBras;
    [SerializeField]
    private ParticleSystem _particulesPouvoir;

    private Collider collider;

    private IFighter owner;

    private void Awake()
    {
        _particulesBras.Stop();
        _particulesPouvoir.Stop();
        owner = GetComponentInParent<IFighter>();

        collider = GetComponent<BoxCollider>();
        collider.enabled = false;
    }

    public override void Use()
    {
        if (_tempsRechargeActuel == 0)
            StartCoroutine(TempsRechargeDemarrer());
        else if (_estDisponible)
        {
            _particulesBras.Play();

            StartCoroutine(TempsRechargeDemarrer());
        }
    }

    public void PulsateAnimEvent()
    {
        _particulesPouvoir.Play();
        SoundManager.instance.Play("SfxPouvoir1");
        collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.Equals(William_Script.instance.gameObject))
        {
            collider.enabled = false;
            William_Script.instance.Hurt(owner.GetStrength());
        }
    }

    public bool CanUse() { return _estDisponible; }
}
