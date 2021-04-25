using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Praesidium_Gabriel : MonoBehaviour
{
    public Animator _animator;
    public GameObject praesidium;
    public Transform tpraesidium;
    public bool isCreated;
    public GameObject g;
    public GameObject ybot;
    public bool isPraesidium;
    public int manaPool;

    void Start() {
        
        manaPool = ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam;
        isCreated = false;
        isPraesidium = false;
        _animator = GetComponent<Animator>();
        g = Instantiate(praesidium, tpraesidium);
        g.SetActive(false);
    }

    public void InstantiateSpell() {

        Instantiate(praesidium, transform);
    }

    IEnumerator ManaSubstract() {

        ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().EnleverMana((manaPool / 100) * 30 );
        Debug.Log(ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam);

        for (int spellDuration = 20; spellDuration > 0; spellDuration--)
        {
            yield return new WaitForSeconds(1);
            Debug.Log(spellDuration);
        } 
        
        g.SetActive(false);
        isCreated = false;

        if (isCreated == true) {
            isPraesidium = true;
        }
    }

    IEnumerator PraesidiumAnimation() {

        _animator.SetBool("Lighting", true);
        yield return new WaitForSeconds(1);
        _animator.SetBool("Lighting", false);
    }
   
    public void Update() { 

        if (ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam <= 0) {
            g.SetActive(false);
            isCreated = false;
            isPraesidium = false;
        }
    
        else if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.T)) {

            if(!isCreated) {
                StartCoroutine(PraesidiumAnimation());
                g.SetActive(true);
                isCreated = true;
                isPraesidium = true;
            }
        }

        if (isPraesidium == true) {
            StartCoroutine(ManaSubstract());
            isPraesidium = false;
        }
    }
}
