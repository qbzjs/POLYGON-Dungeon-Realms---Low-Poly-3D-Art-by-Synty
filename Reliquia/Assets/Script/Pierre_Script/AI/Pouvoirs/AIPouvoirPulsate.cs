using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPouvoirPulsate : AIPouvoir
{
    [SerializeField]
    private ParticleSystem _particulesBras;
    [SerializeField]
    private ParticleSystem _particulesPouvoir;

    private IFighter owner;

    private void Awake()
    {
        _particulesBras.Stop();
        _particulesPouvoir.Stop();
        owner = GetComponent<IFighter>();
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

    }

    public bool CanUse() { return _estDisponible; }
}
