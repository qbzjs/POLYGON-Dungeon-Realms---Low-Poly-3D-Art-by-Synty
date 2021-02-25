using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    public Animator _animator;
    public GameObject lighting;
    public Transform tlighting;
    public bool isCreated;
    public GameObject g;
    public GameObject ybot;
    public bool isLighting;
    public int manaPool;

    void Start() {
        
        manaPool = ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam;
        isCreated = false;
        isLighting = false;
        _animator = GetComponent<Animator>();
        g = Instantiate(lighting, tlighting);
        g.SetActive(false);
    }

    public void InstantiateSpell() {

        Instantiate(lighting, transform);
    }

    IEnumerator ManaSubstract() {

        ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().EnleverMana((manaPool / 100) * 2 );
        Debug.Log(ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam);
        yield return new WaitForSeconds(1);

        if (isCreated == true) {
            isLighting = true;
        }
    }
   
    public void Update() { 

        if (ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam <= 0) {

            _animator.SetBool("Lighting", false);
            g.SetActive(false);
            isCreated = false;
            isLighting = false;
        }
    
        else if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.E)) {

            if(!isCreated) {
                _animator.SetBool("Lighting", true);
                g.SetActive(true);
                isCreated = true;
                isLighting = true;
            }
        }

        else if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.R)) {

            if(isCreated) {
                _animator.SetBool("Lighting", false);
                g.SetActive(false);
                isCreated = false;
                isLighting = false;
            }
        }

        if (isLighting == true) {
            StartCoroutine(ManaSubstract());
            isLighting = false;
        }
    }
}
