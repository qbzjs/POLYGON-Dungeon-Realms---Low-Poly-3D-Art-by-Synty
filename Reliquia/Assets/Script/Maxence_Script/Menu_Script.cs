using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class Menu_Script : MonoBehaviour
{
    private Image fondTransition;
    [SerializeField] private GameObject popUpQuitter;

    private VideoPlayer IntroCinematique;

    // Start is called before the first frame update
    void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();

        fondTransition = GameObject.Find("Canvas/FondNoir").GetComponent<Image>();
        
        if(scene.name == "Menu_00")
        {
            IntroCinematique = GameObject.Find("Canvas/CinematiqueIntro").GetComponent<VideoPlayer>();

            StartCoroutine(JouerCinematiqueIntro());
        }
    }

    IEnumerator JouerCinematiqueIntro()
    {
        IntroCinematique.Play();
        yield return new WaitForSeconds(7);

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            IntroCinematique.targetCameraAlpha -= 0.1f;

            if (IntroCinematique.targetCameraAlpha <= 0)
            {
                fondTransition.DOFade(0, 0.5f).SetDelay(0.5f);
            }
        }
    }

    public void menuPrincipale()
    {
        fondTransition.DOFade(1, 0.5f).OnComplete(() => SceneManager.LoadScene("Menu_01")); //Lance la scène Menu_01 lorsque le fondu au noir est fini
    }
    public void quitterJeu()
    {
        Instantiate(popUpQuitter, new Vector2(1520, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
    }
}
