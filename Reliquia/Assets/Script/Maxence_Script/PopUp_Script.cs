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

	RectTransform arrierePlan;
    bool fHasSpace;

    // Start is called before the first frame update
    void Awake()
    {
        transform.localPosition = new Vector3(1520, 0, 0);
        transform.DOLocalMoveX(0f, 0.5f).SetEase(Ease.OutBack);

		arrierePlan = transform.GetChild(0).GetComponent<RectTransform>();
    }

	void Update ()
	{
		if(Input.GetMouseButtonDown(0) && !SourisSurLePopup())
			fermerPopUp();
	}

	bool SourisSurLePopup ()
	{
		float minX = arrierePlan.transform.position.x - arrierePlan.sizeDelta.x / 2;
		float minY = arrierePlan.transform.position.y - arrierePlan.sizeDelta.y / 2;
		float maxX = arrierePlan.transform.position.x + arrierePlan.sizeDelta.x / 2;
		float maxY = arrierePlan.transform.position.y + arrierePlan.sizeDelta.y / 2;

		Vector2 mousePos = (Vector2)Input.mousePosition - new Vector2(Screen.width / 2, Screen.height / 2);
		return mousePos.x >= minX && mousePos.x <= maxX && mousePos.y >= minY && mousePos.y <= maxY;
	}

	public void quitterJeu()
    {
        Application.Quit();
    }

    public void fermerPopUp()
    {
        transform.DOMoveX(-1630f, 0.5f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        if(SceneManager.GetActiveScene().name != "Menu_00") GameManager.instance.popUpActif = false;
    }

    public void ecraserSauvegarde()
    {
        SaveManager.instance.Save(GameManager.instance.SlotSaveSelect.GetComponent<SavedGame>());
        fermerPopUp();
    }

    public void retourMenuPrincipal()
    {
        transform.parent.transform.parent.GetComponent<HUD_Script>().imageTransition.DOFade(1, 1.5f).OnComplete(() => SceneManager.LoadScene("Menu_01"));
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
