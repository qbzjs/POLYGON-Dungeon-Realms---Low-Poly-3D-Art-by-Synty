using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIPouvoir : MonoBehaviour
{
    [SerializeField]
    protected float TempsRecharge = 5;
    protected bool _estDisponible = true;
    protected float _tempsRechargeActuel;
    public bool Actif = false;

    public abstract void Use();

    protected IEnumerator TempsRechargeDemarrer()
    {
        _estDisponible = false;
        _tempsRechargeActuel = 0;
        while (_tempsRechargeActuel <= TempsRecharge)
        {
            _tempsRechargeActuel += Time.deltaTime;
            yield return null;
        }
        _estDisponible = true;
    }
}
