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
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        //for (int i = 0; i < LevelLoader.instance.Canvas.Length; i++) LevelLoader.instance.Canvas[i].SetActive(true);
        //sceneIndex = SaveManager.instance.idScene;
    }
    private void Start()
    {
        canvasToFade.SetActive(false);
    }

    //void Start() {

    //    StartCoroutine(LoadAsynch(sceneIndex));
    //}
    public void LoadScene(int idScene)
    {
        Debug.Log("Started");

        StartCoroutine(LoadAsynch(idScene));
    }

    IEnumerator LoadAsynch(int sceneIndex) {

        /// Le code ci-dessous est présent uniquement pour l'aperçu ///

        canvasToFade.SetActive(true);
        canvasToFade.GetComponent<CanvasGroup>().DOFade(1, 1f);
        yield return new WaitUntil(() => canvasToFade.GetComponent<CanvasGroup>().alpha == 1);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {

            for (float i = 0; i < 1.1f; i += .1f)
            {

                if (loadingPoints.text.Length == 13)
                    loadingPoints.text = "Chargement";

                else
                    loadingPoints.text = loadingPoints.text + ".";

                amulette.GetComponent<CanvasGroup>().DOFade(i, 0.3f);
                Debug.Log(operation.progress);
                yield return new WaitForSeconds(.4f);
            }
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        canvasToFade.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        yield return new WaitUntil(() => canvasToFade.GetComponent<CanvasGroup>().alpha == 0);
        canvasToFade.SetActive(false);
        Debug.Log(operation.progress);
    }
}
