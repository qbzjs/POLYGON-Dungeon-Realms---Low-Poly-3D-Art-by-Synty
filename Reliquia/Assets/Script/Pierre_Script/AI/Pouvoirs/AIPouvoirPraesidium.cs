using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPouvoirPraesidium : AIPouvoir
{
    [SerializeField]
    private float DureePouvoir = 5;
    [SerializeField]
    private GameObject _particulesPouvoir;

    private void Awake()
    {
        _particulesPouvoir.SetActive(false);
    }

    public override void Use()
    {
        if (_estDisponible)
        {
            _particulesPouvoir.SetActive(true);
            StartCoroutine(VerifierBouclierActif());
            SoundManager.instance.Play("SfxPouvoir2");
        }
    }

    /// <summary>
    /// Désaciver les particules quand le pouvoir n'est plus actif.
    /// </summary>
    /// <returns></returns>
    private IEnumerator VerifierBouclierActif()
    {
        float dureeBouclier = 0;
        Actif = true;
        while (Actif)
        {
            dureeBouclier += Time.deltaTime;
            if (dureeBouclier >= DureePouvoir)
            {
                Actif = false;
            }
            yield return null;
        }
        _particulesPouvoir.SetActive(false);
        StartCoroutine(TempsRechargeDemarrer());
    }
}
