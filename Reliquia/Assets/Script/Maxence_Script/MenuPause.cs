using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuPause : MonoBehaviour
{
    public static MenuPause instance;

    public GameObject parentPause;

    GameManager gameManager;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        Debug.Log(SceneManager.GetActiveScene().buildIndex);

        if (SceneManager.GetActiveScene().buildIndex >= 2)
        {
            parentPause.SetActive(true);

            StartCoroutine(cachePrefab());
        }

        StartCoroutine(cachePrefab());
    }

    IEnumerator cachePrefab()
    {
        yield return new WaitForSeconds(0.1f);
        parentPause.SetActive(false);
    }
}
