using clavier;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Script : MonoBehaviour
{
    public GameObject MenuPause;
    public Transform MenuInventaire;

    public GameObject MessageInteraction;
    public Text TexteMessageInteraction;

    public Transform ParentBarresVieMana;
    public GameObject ParentCompas;

    public Transform ParentSlotSave;
    public Transform ParentSlotLoad;
    public Transform ParentBoutonMenu;
    public Transform ParentOptions;

    //[SerializeField] private Button boutonMenuSave;
    //[SerializeField] private Button boutonMenuLoad;
    [SerializeField] private Button boutonMenuOptions;
    [SerializeField] private Button boutonRetourOption;

    public GameObject[] SloatsLoadSave;
    public GameObject prefabMenuOptions;

    [SerializeField] private GameObject prefab;

    public Image imageTransition;
    public GameObject fondPause;

    [SerializeField] private CanvasGroup GrimoireInventaire;
    private Vector3 grimmoireItemPouvoirPos = new Vector3(477f, 0f, 0f);

    private Vector3 grimmoireMapPos = new Vector3(-478f, -76f, 0f);
    [SerializeField] private Transform grimmoireScale;

    [SerializeField] private CanvasGroup sacocheInventaire;
    [SerializeField] private CanvasGroup zoneItemsGrimmoir;

    [SerializeField] private Transform boutonMap;
    [SerializeField] private Transform boutonPouvoirs;
    [SerializeField] private Transform boutonItem;

    int activeMenu;

    public Texture2D cerclePosPlayer;
    public RawImage cerclePosPlayerImage;
    Color[] colors = new Color[1]; 

    public Texture2D parentGrimoireMask;
    public RawImage parentGrimoireMaskImage;

    [SerializeField] private Image pefabPartiGrimoir;

    [SerializeField] private CanvasGroup parentMap;

    public static HUD_Script instance;

    [Header("SFX")]
    [NonSerialized] public AudioSource audioSource;
    public AudioClip SFX_OpenInventory;
    public AudioClip SFX_CloseInventory;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            audioSource = GetComponent<AudioSource>();
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //for(int i = 0; i < LevelLoader.instance.Canvas.Length; i++) LevelLoader.instance.Canvas[i].SetActive(false);

        colors[0] = new Color(1, 1, 1, 0);

        prefab = Instantiate(prefabMenuOptions, GameObject.FindGameObjectWithTag("Options").transform);
        boutonRetourOption =  prefab.transform.GetChild(1).GetComponent<Button>();
        imageTransition.DOFade(0, 1.5f);
        fondPause.SetActive(false);

        ManageGame();

        boutonMenuOptions.onClick.AddListener(GameManager.instance.menuOptions);
        boutonRetourOption.onClick.AddListener(GameManager.instance.menuOptions);

        itemMenu(1);

        SaveManager.instance.LoadMap(GameManager.instance.nomSauvegarde);

        cerclePosPlayer = cerclePosPlayerImage.texture as Texture2D;

        colors = cerclePosPlayer.GetPixels();
    }

    void Start()
    {
        Debug.Log("start");
        SaveManager.instance.saveSlots.Clear();

        foreach (GameObject slots in SloatsLoadSave)
        {
            SaveManager.instance.saveSlots.Add(slots);
        }

        StartCoroutine(SaveManager.instance.affichageSaveLoad());

        //setInfoWilliam();

        //SetPixelMapMask();
    }

    public void setInfoWilliam()
    {
        RessourcesVitalesWilliam_Scrip.instance.manaWilliam = InformationsPlayer.instance.williamMana;
        RessourcesVitalesWilliam_Scrip.instance.vieWilliam = InformationsPlayer.instance.williamVie;
        RessourcesVitalesWilliam_Scrip.instance.maxMana = InformationsPlayer.instance.maxWilliamMana;
        RessourcesVitalesWilliam_Scrip.instance.maxVie = InformationsPlayer.instance.maxWilliamVie;

        RessourcesVitalesWilliam_Scrip.instance.SetVie(InformationsPlayer.instance.williamVie);
        RessourcesVitalesWilliam_Scrip.instance.SetMana(InformationsPlayer.instance.williamMana);

        William_Script.instance.transform.position = InformationsPlayer.instance.williamPosition;
    }

    public void ManageGame()
    {
        GameManager.instance.MenuPause = MenuPause;
        GameManager.instance.MenuInventaire = MenuInventaire;

        GameManager.instance.MessageInteraction = MessageInteraction;
        GameManager.instance.TexteMessageInteraction = TexteMessageInteraction;

        GameManager.instance.ParentBarresVieMana = ParentBarresVieMana;
        GameManager.instance.ParentCompas = ParentCompas;

        GameManager.instance.ParentSlotSave = ParentSlotSave;
        GameManager.instance.ParentSlotLoad = ParentSlotLoad;
        GameManager.instance.ParentBoutonMenu = ParentBoutonMenu;
        GameManager.instance.ParentOptions = ParentOptions;

        GameManager.instance.GrimoireInventaire = GrimoireInventaire;

        GameManager.instance.voirMenu = false;
        GameManager.instance.menuSlots = false;
        GameManager.instance.menuPauseOuvert = false;
        GameManager.instance.menuInventaireOuvert = false;
        GrimoireInventaire.alpha = Convert.ToInt32(GameManager.instance.voirMenu);
        GameManager.instance.ParentBoutonMenu.DOMoveX(-780f, 0.01f);
        GameManager.instance.FermerMessageInteraction();

        if(GameManager.instance.menuInventaireOuvert = false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ContinuerJeu()
    {
        GameManager.instance.menuPause();
    }

    public void QuitterPartie()
    {
        GameManager.instance.retourMenu();
    }

    public void LoadCheckpoint()
    {
        string nom = GameManager.instance.nomSauvegarde;
        SaveManager.instance.LoadInGame(nom);
    }

    public void mapMenu(int menuIndex)
    {
        if(activeMenu == 1)
        {
            boutonMap.SetSiblingIndex(2);
            boutonMap.GetComponent<Text>().fontSize = 70;
            boutonItem.GetComponent<Text>().fontSize = 45;
            sacocheInventaire.DOFade(0, 0.25f);
            zoneItemsGrimmoir.DOFade(0, 0.25f).OnComplete(() =>
            {
                grimmoireScale.DOScale(0.9f, 0.25f);
                grimmoireScale.DOLocalMove(grimmoireMapPos, 0.5f).OnComplete(() => parentMap.DOFade(1, 0.25f));
            });
            activeMenu = menuIndex;
        }
        else if (activeMenu == menuIndex) return;
        else
        {
            activeMenu = menuIndex;
            boutonMap.SetSiblingIndex(2);
            boutonMap.GetComponent<Text>().fontSize = 70;
            boutonItem.GetComponent<Text>().fontSize = 45;
        }
    }

    public void itemMenu(int menuIndex)
    {
        if (activeMenu == menuIndex) return;

        if (activeMenu == 2)
        {
            audioSource.PlayOneShot(SFX_OpenInventory);
            boutonItem.SetSiblingIndex(2);
            boutonItem.GetComponent<Text>().fontSize = 70;
            boutonMap.GetComponent<Text>().fontSize = 45;
            grimmoireScale.DOScale(1f, 0.25f);
            parentMap.DOFade(0, 0.25f);
            grimmoireScale.DOLocalMove(grimmoireItemPouvoirPos, 0.25f).OnComplete(() =>
            {
                sacocheInventaire.DOFade(1, 0.25f);
                zoneItemsGrimmoir.DOFade(1, 0.25f);
            });
            activeMenu = menuIndex;
        }
        else 
        {
            activeMenu = menuIndex;
            boutonItem.SetSiblingIndex(2);
            parentMap.DOFade(0, 0.25f);
            boutonItem.GetComponent<Text>().fontSize = 70;
            boutonMap.GetComponent<Text>().fontSize = 45;
        }
    }

    void SetPixelMapMask()
    {
        for(int i = 0; i < cerclePosPlayerImage.rectTransform.rect.height; i++)
        {
            for (int j = 0; j < cerclePosPlayerImage.rectTransform.rect.height; j++)
            {
                parentGrimoireMask.SetPixels(i, j, (int)cerclePosPlayerImage.rectTransform.rect.width, (int)cerclePosPlayerImage.rectTransform.rect.height, colors);
            }
        }
        parentGrimoireMask.Apply();
    }
}
