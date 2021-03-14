using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate : MonoBehaviour
{
    public Animator _animator;
    public GameObject pulsate;
    public GameObject g;
    public GameObject ybot;
    public GameObject capsule;
    public Transform tpulsate;
    public bool isCreated;
    public bool isPulsate;
    public bool isStunt;
    public int manaPool;

    void Start() {
        
        manaPool = ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam;
        isCreated = false;
        isPulsate = false;
        _animator = GetComponent<Animator>();
        g = Instantiate(pulsate, tpulsate);
        g.SetActive(false);
    }

    public void InstantiateSpell() {

        Instantiate(pulsate, transform);
    }

    IEnumerator ManaSubstract() {

        ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().EnleverMana((manaPool / 100) * 25);
        Debug.Log(ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam);

        for (int spellDuration = 7; spellDuration > 0; spellDuration--)
        {
            yield return new WaitForSeconds(1);
            Debug.Log(spellDuration);
        }

        _animator.SetBool("Attaque", false);
        g.SetActive(false);
        isCreated = false;

        if (isCreated == true) {
            isPulsate = true;
        }
    }

    IEnumerator EnnemyAnimation() {

        isStunt = true;

        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Attaque", false);

        for (int stunt = 10; stunt > 0; stunt--) {
            Debug.Log("Ennemy stunt for " + stunt + " sec");
            yield return new WaitForSeconds(1);
        }

        capsule.GetComponent<Capsule>().isMoving = false;
        isStunt = false;
    }

    public void Update() { 

        if (ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam <= 0) {

            _animator.SetBool("Attaque", false);
            g.SetActive(false);
            isCreated = false;
            isPulsate = false;
        }
    
        else if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.P)) {

            if(!isCreated && capsule.GetComponent<Capsule>().isNear == true) {
                _animator.SetBool("Attaque", true);
                StartCoroutine(EnnemyAnimation());
                g.SetActive(true);
                isCreated = true;
                isPulsate = true;
            }
        }
 
        if (isPulsate == true) {
            StartCoroutine(ManaSubstract());
            isPulsate = false;
        }

        if (isStunt == true) {
            capsule.GetComponent<Capsule>().isMoving = true;
        }
    }
}
