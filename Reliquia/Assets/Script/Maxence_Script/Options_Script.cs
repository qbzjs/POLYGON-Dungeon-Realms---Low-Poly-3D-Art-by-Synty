using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;
using clavier;

public class Options_Script : MonoBehaviour
{

    /// <summary>
    /// Gameobjet généraux
    /// </summary>
    [SerializeField] Image[] ImageBoutonOptions;

    [SerializeField] private Transform ContenuOptions;

    [SerializeField] private Text TitreMenuActif;

    /// <summary>
    /// Gameobjet menu réglages
    /// </summary>
    [SerializeField] private Text ValeurMusique;

    [SerializeField] private Slider VolumeMusiques;

    [SerializeField] private Text ValeurAffichage;

    /// <summary>
    /// Gameobjet menu paramètres généraux
    /// </summary>
    [SerializeField] private Text ValeurSouris;
    [SerializeField] private Text ValeurResolution;
    [SerializeField] private Text ValeurQualiteEffets;

    private bool InversionSourisActive;
    private bool SousTitresActive;
    private bool FullEcran;

    private string[] TextesQualitesEffets = new string[] { "FAIBLE", "NORMALE", "ÉLEVÉE", "ULTRA" };
    private string[] TextesResolution = new string[] { "FAIBLE", "NORMALE", "ÉLEVÉE" };

    private int indexEffets;
    private int indexResolution;

    [SerializeField] private Button boutonRetour;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Menu_01") 
        { 
            valeurAuLancement(); 
        }

        GameManager.instance.raccourciClavier = gameObject.GetComponent<RaccourciClavier_Script>();
    }

    public void valeurAuLancement()
    {
        boutonRetour.onClick.AddListener(MenuManager_Script.instance.afficherMenuPrincipal);

        //Set l'état des sous-titres au lancement du jeu
        SousTitresActive = Convert.ToBoolean(PlayerPrefs.GetInt("SousTitreEtat", 1));

        //Set l'état d'inversion de la souris au lancement du jeu
        InversionSourisActive = Convert.ToBoolean(PlayerPrefs.GetInt("InversionSourisEtat", 0));
        ValeurSouris.text = !InversionSourisActive ? "NON" : "OUI";
        
        //Set le volume de la musique au lancement du jeu
        VolumeMusiques.value = PlayerPrefs.GetFloat("EtatVolumeMusique", 1f);

        ValeurMusique.text = (VolumeMusiques.value * 100).ToString("N0");

        //Set l'affichage de l'écran au lancement du jeu
        FullEcran = Convert.ToBoolean(PlayerPrefs.GetInt("EcranEtat", 1));
        Screen.SetResolution(1920, 1080, FullEcran);
        ValeurAffichage.text = FullEcran == true ? "FULL" : "FENÊTRÉ";

        //Set la qualité des effets au lancement du jeu
        indexEffets = PlayerPrefs.GetInt("EtatQualiteEffets", 3);
        QualitySettings.SetQualityLevel(indexEffets);
        ValeurQualiteEffets.text = TextesQualitesEffets[indexEffets];

        //Set la résolution au lancement du jeu
        indexResolution = PlayerPrefs.GetInt("EtatResolution", 0);
        ValeurResolution.text = TextesResolution[indexResolution];

        AfficherReglages();
    }

    public void AfficherReglages()
    {
        TitreMenuActif.text = "Réglages";
        ContenuOptions.DOLocalMoveX(0, 0.35f).SetEase(Ease.OutQuint);
        for(int i = 0; i < 3; i++)
        {
            ImageBoutonOptions[i].DOColor(new Color32(115, 115, 155, 255), 0.2f);
        }

        ImageBoutonOptions[0].DOColor(new Color32(247, 247, 247, 255), 0.2f);
    }

    public void AfficherParamGeneraux()
    {
        TitreMenuActif.text = "Paramètres Généraux";
        ContenuOptions.DOLocalMoveX(-2000, 0.35f).SetEase(Ease.OutQuint);
        for (int i = 0; i < 3; i++)
        {
            ImageBoutonOptions[i].DOColor(new Color32(115, 115, 155, 255), 0.2f);
        }

        ImageBoutonOptions[1].DOColor(new Color32(247, 247, 247, 255), 0.2f);
    }

    public void AfficherControles()
    {
        TitreMenuActif.text = "Contrôles";
        ContenuOptions.DOLocalMoveX(-4000, 0.35f).SetEase(Ease.OutQuint);
        for (int i = 0; i < 3; i++)
        {
            ImageBoutonOptions[i].DOColor(new Color32(115, 115, 155, 255), 0.2f);
        }

        ImageBoutonOptions[2].DOColor(new Color32(247, 247, 247, 255), 0.2f);
    }

    public void ChangerVolumeMusique()
    {
        ValeurMusique.text = (VolumeMusiques.value * 100).ToString("N0");
        PlayerPrefs.SetFloat("EtatVolumeMusique", VolumeMusiques.value);
    }

    public void ChangerValeurAffichages()
    {
        //Avoir le nom de la valeur actuelle de l'affichage
        string index = ValeurAffichage.text;

        switch (index) //Change l'affichage en fonction de la valeur actuelle
        {
            case "FULL":
                FullEcran = false;
                Screen.SetResolution(1920, 1080, FullEcran);
                ValeurAffichage.text = "FENÊTRÉ";
                PlayerPrefs.SetInt("EcranEtat", Convert.ToInt32(FullEcran));
                break;

            case "FENÊTRÉ":
                FullEcran = true;
                Screen.SetResolution(1920, 1080, FullEcran);
                ValeurAffichage.text = "FULL";
                PlayerPrefs.SetInt("EcranEtat", Convert.ToInt32(FullEcran));
                break;
        }
    }

    public void ChangerValeurSouris()
    {
        InversionSourisActive = !InversionSourisActive;
        ValeurSouris.text = !InversionSourisActive ? "NON" : "OUI";
        PlayerPrefs.SetInt("InversionSourisEtat", Convert.ToInt32(InversionSourisActive));
    }

    public void ChangerResolution()
    {
        //Avoir le nom de la valeur actuelle de la résolution 
        string index = ValeurResolution.text;

        switch (index) //Change la résolution en fonction de la valeur actuelle
        {
            case "FAIBLE":
                indexResolution = 1;
                break;

            case "NORMALE":
                indexResolution = 2;
                break;

            case "ÉLEVÉE":
                indexResolution = 0;
                break;
        }
        QualiteResolution();
    }

    public void QualiteResolution()
    {
        ValeurResolution.text = TextesResolution[indexResolution];
        PlayerPrefs.SetInt("EtatResolution", indexResolution);
    }

    public void ChangerQualiteEffets()
    {
        //Avoir le nom de la valeur actuelle de la qualité 
        string index = ValeurQualiteEffets.text;

        switch (index) //Change la qualité en fonction de la valeur actuelle
        {
            case "FAIBLE":
                indexEffets = 1;
                break;

            case "NORMALE":
                indexEffets = 2;
                break;

            case "ÉLEVÉE":
                indexEffets = 3;
                break;

            case "ULTRA":
                indexEffets = 0;
                break;
        }
        QualiteEffets();
    }

    public void QualiteEffets()
    {
        QualitySettings.SetQualityLevel(indexEffets);
        ValeurQualiteEffets.text = TextesQualitesEffets[indexEffets];
        PlayerPrefs.SetInt("EtatQualiteEffets", indexEffets);
    }
}
