using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate : MonoBehaviour
{
    public Animator _animator;
    public GameObject pulsate;
    public Transform tpulsate;
    public bool isCreated;
    public GameObject g;
    public GameObject ybot;
    public bool isPulsate;
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

        ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().EnleverMana((manaPool / 100) * 30 );
        Debug.Log(ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam);

        while (true)
        {
            
        }
        yield return new WaitForSeconds(20);
        _animator.SetBool("Lighting", false);
        g.SetActive(false);
        isCreated = false;

        if (isCreated == true) {
            isPulsate = true;
        }
    }
   
    public void Update() { 

        if (ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam <= 0) {

            _animator.SetBool("Lighting", false);
            g.SetActive(false);
            isCreated = false;
            isPulsate = false;
        }
    
        else if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.T)) {

            if(!isCreated) {
                _animator.SetBool("Lighting", true);
                g.SetActive(true);
                isCreated = true;
                isPulsate = true;
            }
        }
 
        if (isPulsate == true) {
            StartCoroutine(ManaSubstract());
            isPulsate = false;
        }
    }
}
