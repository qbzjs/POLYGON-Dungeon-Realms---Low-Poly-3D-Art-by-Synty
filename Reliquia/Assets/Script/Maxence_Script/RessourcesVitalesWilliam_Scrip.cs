using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class RessourcesVitalesWilliam_Scrip : MonoBehaviour
{
    public int manaWilliam;
    public int vieWilliam;
    
    public int valeurMana;
    public int valeurVie;
    
    public float pourcentageMana;
    public float pourcentageVie;
    
    public int maxVie = 120;
    public int minVie = 0;
    
    public int maxMana = 120;
    public int minMana = 0;

    [SerializeField] private Text texteMana;
    [SerializeField] private Text texteVie;
    
    [SerializeField] private Image barreMana;
    [SerializeField] private Image barreVie;

    public static RessourcesVitalesWilliam_Scrip instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "test" || SceneManager.GetActiveScene().name == "sarah_Scene_Pouvoirs")
        {
            vieWilliam = maxVie;
            manaWilliam = maxMana;

            valeurMana = manaWilliam;
            valeurVie = vieWilliam;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.N)) EnleverVie(10);
        if (Input.GetKeyUp(KeyCode.B)) EnleverMana(10);

        if (Input.GetKeyUp(KeyCode.V)) RajouterVie(10);
        if (Input.GetKeyUp(KeyCode.C)) RajouterMana(10);
    }

    public void EnleverVie(int valeurEnMoins)
    {
        if (valeurVie > minVie)
        {
            vieWilliam -= valeurEnMoins;
            SetVie(vieWilliam);
        }
    }

    public void EnleverMana(int valeurEnMoins)
    {
        if (valeurMana > minMana)
        {
            manaWilliam -= valeurEnMoins;
            SetMana(manaWilliam);
        }
    }

    public void RajouterVie(int valeurEnPlus)
    {
        if (valeurVie < maxVie)
        {
            vieWilliam += valeurEnPlus;
            SetVie(vieWilliam);
        }
    }

    public void RajouterMana(int valeurEnPlus)
    {
        if (valeurMana < maxMana)
        {
            manaWilliam += valeurEnPlus;
            SetMana(manaWilliam);
        }
    }

    public void SetVie(int Vie)
    {
        if(Vie != valeurVie)
        {
            if (maxVie - minVie == 0)
            {
                valeurVie = 0;
                pourcentageVie = 0;
            }
            else
            {
                valeurVie = Vie;

                pourcentageVie = (float)valeurVie / (float)(maxVie - minVie);
            }

            texteVie.text = string.Format("{0} %", Mathf.RoundToInt(pourcentageVie * 100));
            barreVie.DOFillAmount(pourcentageVie, 0.5f);
        }
    }

    public float pourcentageVieActuel
    {
        get { return pourcentageVie; }
    }

    public int valeurVieActuel
    {
        get { return valeurVie; }
    }

    public void SetMana(int Mana)
    {
        if (Mana != valeurMana)
        {
            if (maxMana - minMana == 0)
            {
                valeurMana = 0;
                pourcentageMana = 0;
            }
            else
            {
                valeurMana = Mana;

                pourcentageMana = (float)valeurMana / (float)(maxMana - minMana);
            }

            texteMana.text = string.Format("{0} %", Mathf.RoundToInt(pourcentageMana * 100));
            barreMana.DOFillAmount(pourcentageMana, 0.5f);
        }
    }

    public float pourcentageManaActuel
    {
        get { return pourcentageMana; }
    }

    public int valeurManaActuel
    {
        get { return valeurMana; }
    }
}
