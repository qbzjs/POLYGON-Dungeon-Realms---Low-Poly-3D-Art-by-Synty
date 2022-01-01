using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SavedGame : MonoBehaviour
{
    [SerializeField] private Text dateTime;
    [SerializeField] private Text chapitreEnCours;
    [SerializeField] private Text pourcentageAvancement;
    [SerializeField] private Text NomSauvegarde;

    [SerializeField] private GameObject texteDate;
    [SerializeField] private GameObject texteChapitre;
    [SerializeField] private GameObject textePourcentageAvancement;
    [SerializeField] private GameObject texteNomSauvegarde;

    [SerializeField] private int index;
    public string nameSave;
    public int idSceneActuelle;
    public string nomSceneActuelle;

    public int MyIndex
    {
        get
        {
            return index;
        }
    }

    public string MySaveName
    {
        get
        {
            return nameSave;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        texteDate.SetActive(false);
        texteChapitre.SetActive(false);
        textePourcentageAvancement.SetActive(false);

        //PlayerPrefs.DeleteAll();
        nameSave = PlayerPrefs.GetString(MyIndex.ToString());
    }

    public void ShowInfo(SaveData saveData)
    {
        texteDate.SetActive(true);
        texteChapitre.SetActive(true);
        textePourcentageAvancement.SetActive(true);

        dateTime.text = saveData.MyDateTime.ToString("dd/MM/yyyy") + " à " + saveData.MyDateTime.ToString("H:mm");
        nomSceneActuelle = saveData.MySceneData.NameScene;
        chapitreEnCours.text = nomSceneActuelle;
        NomSauvegarde.text = nameSave;
		pourcentageAvancement.text = saveData.MySceneData.Avancement + "%";
    }
}
