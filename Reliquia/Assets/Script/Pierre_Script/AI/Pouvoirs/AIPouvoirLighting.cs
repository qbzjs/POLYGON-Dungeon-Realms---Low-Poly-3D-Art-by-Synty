using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPouvoirLighting : AIPouvoir
{
    [SerializeField]
    private GameObject _lightingOrbe;
    [SerializeField]
    private GameObject _lightingPrefabProjectile;

    private IFighter owner;
    [System.NonSerialized]
    public Vector3 TargetPos;

    private void Awake()
    {
        _lightingOrbe.SetActive(false);
        owner = GetComponent<IFighter>();
    }

    public override void Use()
    {
        _lightingOrbe.SetActive(true);
        if(_tempsRechargeActuel == 0)
            StartCoroutine(TempsRechargeDemarrer());

        Actif = true;
        if (_estDisponible && TargetPos != null)
        {
            GameObject projectile = Instantiate(_lightingPrefabProjectile, _lightingOrbe.transform.position, Quaternion.LookRotation(TargetPos, Vector3.up));
            projectile.GetComponent<LightingProjectileBehaviour>().owner = owner;
            StartCoroutine(TempsRechargeDemarrer());
        }
    }

    public void Stop()
    {
        _lightingOrbe.SetActive(false);
        Actif = false;
    }
}
