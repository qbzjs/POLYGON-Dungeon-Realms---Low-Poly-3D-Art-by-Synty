using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PopUp_Script : MonoBehaviour
{

    [SerializeField] private InputField changerNomSauvegarde;
    [SerializeField] private Text textPopUp;

    bool fHasSpace;

    // Start is called before the first frame update
    void Awake()
    {
        transform.localPosition = new Vector3(1520, 0, 0);
        transform.DOLocalMoveX(0f, 0.5f).SetUpdate(true).SetEase(Ease.OutBack);
    }

    public void quitterJeu()
    {
        Application.Quit();
    }

    public void fermerPopUp()
    {
        transform.DOMoveX(-1630f, 0.5f).SetUpdate(true).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        if(SceneManager.GetActiveScene().name != "Menu_00") GameManager.instance.popUpActif = false;
    }

    public void ecraserSauvegarde()
    {
        SaveManager.instance.Save(GameManager.instance.SlotSaveSelect.GetComponent<SavedGame>());
        fermerPopUp();
    }

    public void retourMenuPrincipal()
    {
        transform.parent.transform.parent.GetComponent<HUD_Script>().imageTransition.DOFade(1, 1.5f).SetUpdate(true).OnComplete(() => SceneManager.LoadScene("Menu_01"));
    }

    public void nouveauNomSauvegarde()
    {
        fHasSpace = changerNomSauvegarde.transform.GetChild(2).GetComponent<Text>().text.Contains(" ");

        if (changerNomSauvegarde.transform.GetChild(2).GetComponent<Text>().text == "" && EventSystem.current.currentSelectedGameObject.name == "BoutonNon")
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }
        else if (fHasSpace == true) StartCoroutine(popUpText());
        else if (changerNomSauvegarde.transform.GetChild(2).GetComponent<Text>().text == "") StartCoroutine(popUpText());
        else
        {
            GameManager.instance.SlotSaveSelect.GetComponent<SavedGame>().nameSave = changerNomSauvegarde.transform.GetChild(2).GetComponent<Text>().text;
            PlayerPrefs.SetString(GameManager.instance.SlotSaveSelect.GetComponent<SavedGame>().MyIndex.ToString(), GameManager.instance.SlotSaveSelect.GetComponent<SavedGame>().nameSave);
            SaveManager.instance.NewSave(GameManager.instance.SlotSaveSelect.GetComponent<SavedGame>());
            Destroy(gameObject);
        }
    }

    IEnumerator popUpText()
    {
        textPopUp.color = Color.red;
        if (fHasSpace) textPopUp.text = "Veuillez ne pas mettre d'espace";
        else textPopUp.text = "Veuillez remplir le champ de texte";

        yield return new WaitForSeconds(2);

        textPopUp.color = Color.white;
        textPopUp.text = "Veuillez entrer un nom de sauvegarde";
    }
}
