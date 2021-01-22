using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class PrefabLoader : MonoBehaviour {

    public GameObject prefab;

    public void PrefabToLoad(GameObject canvasToFade) {

        StartCoroutine(FadeOut(canvasToFade));
    }

    IEnumerator FadeOut(GameObject canvasToFade) {
        
        canvasToFade.GetComponent<CanvasGroup>().DOFade(0, 0.9f);
        yield return new WaitUntil(() => canvasToFade.GetComponent<CanvasGroup>().alpha == 0);
        Instantiate(prefab);
    }
}
