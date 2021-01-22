using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class LevelLoader : MonoBehaviour {

    public GameObject canvasToFade;
    public GameObject amulette; // Amulette remplie
    public Text loadingPoints; // "Chargement..."
    public int sceneIndex;

    public GameObject[] Canvas;

    public static LevelLoader instance;


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
        //for (int i = 0; i < LevelLoader.instance.Canvas.Length; i++) LevelLoader.instance.Canvas[i].SetActive(true);
        sceneIndex = SaveManager.instance.idScene;
    }

    void Start() {

        StartCoroutine(LoadAsynch(sceneIndex));
    }

    IEnumerator LoadAsynch(int sceneIndex) {

        /// Le code ci-dessous est présent uniquement pour l'aperçu ///

        canvasToFade.GetComponent<CanvasGroup>().DOFade(0, 1f);
        yield return new WaitUntil(() => canvasToFade.GetComponent<CanvasGroup>().alpha == 0);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        while (operation.progress < .9f) {

            for (float i=0;i<1.1f;i+=.1f) {

                if (loadingPoints.text.Length == 13)
                    loadingPoints.text = "Chargement";

                else
                    loadingPoints.text = loadingPoints.text + ".";

                amulette.GetComponent<CanvasGroup>().DOFade(i, 0.3f);
                Debug.Log(operation.progress);
                yield return new WaitForSeconds(.4f);
            }
        }

        canvasToFade.GetComponent<CanvasGroup>().DOFade(1, 1f);
        yield return new WaitUntil(() => canvasToFade.GetComponent<CanvasGroup>().alpha == 1);
        operation.allowSceneActivation = true;
        Debug.Log(operation.progress);
    }
}
