using clavier;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{

    public RaccourciClavier_Script raccourciClavier;

    public GameObject MenuPause;
    public Transform MenuInventaire;
    public bool voirMenu;

    public bool menuPauseOuvert;
    public bool menuInventaireOuvert;
    public bool menuOptionOuvert;
    public bool menuSlots;

    public GameObject MessageInteraction;
    public Text TexteMessageInteraction;

    public Transform ParentBarresVieMana;
    public GameObject ParentCompas;

    public Transform ParentSlotSave;
    public Transform ParentSlotLoad;
    public Transform ParentBoutonMenu;
    public Transform ParentOptions;

    public static GameManager instance;

    public GameObject popUpNomSauvegarde;
    public GameObject popUpEcraserSauvegarde;
    public GameObject popUpRetourMenuPrincipal;

    public GameObject popUp;
    public GameObject SlotSaveSelect;

    public CanvasGroup GrimoireInventaire;

    public bool popUpActif;

    public int chapitreEnCours;
    public string nomSauvegarde;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void choixNomSauvegarde()
    {
        popUp = Instantiate(popUpNomSauvegarde, GameObject.FindGameObjectWithTag("HUD").transform);
        popUpActif = true;
    }

    public void ecraserSauvegarde()
    {
        popUp = Instantiate(popUpEcraserSauvegarde, GameObject.FindGameObjectWithTag("HUD").transform);
        popUpActif = true;
    }

    public void retourMenu()
    {
        popUp = Instantiate(popUpRetourMenuPrincipal, GameObject.FindGameObjectWithTag("HUD").transform.GetChild(3));
        popUpActif = true;
    }

    public void menuOptions()
    {
        menuSlots = !menuSlots;
        menuOptionOuvert = !menuOptionOuvert;
        if (menuSlots == true) ParentBoutonMenu.DOMoveX(-780f, 0.25f).OnComplete(() => ParentOptions.gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
        else ParentOptions.gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => ParentBoutonMenu.DOMoveX(0f, 0.25f));
    }

    public void menuSauvegarde()
    {
        menuSlots = !menuSlots;
        ParentSlotSave.DOLocalMoveX((menuSlots == true ? 0f : 2000f), 0.25f);
        ParentBoutonMenu.DOMoveX((menuSlots == true ? -780f : 0f), 0.25f);
    }

    public void menuCharger()
    {
        menuSlots = !menuSlots;
        ParentSlotLoad.DOLocalMoveX((menuSlots == true ? 0f : 2000f), 0.25f);
        ParentBoutonMenu.DOMoveX((menuSlots == true ? -780f : 0f), 0.25f);
    }

    public void menuPause()
    {
        voirMenu = !voirMenu;
        //if(voirMenu == true) MenuPause.SetActive(voirMenu);
        HUD_Script.instance.fondPause.SetActive(voirMenu);
        DeplacerUIMenu();
        menuPauseOuvert = !menuPauseOuvert;
        //ParentBoutonPause.localPosition = new Vector3(53, -170, 0);
        ParentSlotLoad.localPosition = new Vector3(2000f, 0, 0);
        ParentSlotSave.localPosition = new Vector3(2000f, 0, 0);
        ParentBoutonMenu.DOMoveX((voirMenu == true ? 0f : -780f), 0.25f);

        //MenuPause.SetActive(voirMenu);
    }


    public void menuInventaire()
    {
        voirMenu = !voirMenu;
        GrimoireInventaire.alpha = Convert.ToInt32(voirMenu);
        DeplacerUIMenu();
        menuInventaireOuvert = !menuInventaireOuvert;
        MenuInventaire.localPosition = new Vector3((voirMenu == true ? 0 : -2000f), 0, 0);
    }

    public void DeplacerUIMenu()
    {
        ParentBarresVieMana.DOMoveX((voirMenu == true ? - 632f : 0f), 0.25f);
        ParentCompas.SetActive(!voirMenu);
    }

    public void AfficherMessageInteraction(string text)
    {
        MessageInteraction.SetActive(true);
        TexteMessageInteraction.text = "Appuyer sur " + raccourciClavier.toucheClavier["Action"].ToString() + " pour intéragir";
    }

    public void FermerMessageInteraction()
    {
        if (MessageInteraction != null)
        {
            MessageInteraction.SetActive(false);
        }
        
    }
}
