using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compas_Script : MonoBehaviour
{
    public GameObject iconePrefab;
    [SerializeField] List<MarqeurQuete_Script> marqueurQuete = new List<MarqeurQuete_Script>();

    public RawImage compasImage;
    public Sprite marqueurImage;
    private Transform playerCamera;

    public float maxDistance = 50f;

    float CompasUnite;

    public static Compas_Script instance;

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
    }

    private void Start()
    {
        playerCamera = GameObject.FindWithTag("MainCamera").transform;
        CompasUnite = compasImage.rectTransform.rect.width / 360f;
    }

    void Update()
    {
        compasImage.uvRect = new Rect(playerCamera.localEulerAngles.y / 360f, 0f, 1f, 1f);

        foreach(MarqeurQuete_Script marqueur in marqueurQuete)
        {
            marqueur.image.rectTransform.anchoredPosition = GetPosOnCompas(marqueur);
            
            float dist = Vector2.Distance(new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z), marqueur.position);
            float scale = 0f;

            if (dist < maxDistance) scale = 1f - (dist / maxDistance);

            marqueur.image.rectTransform.localScale = Vector3.one * scale;
        }
    }

    public void AddMarqueurQuete (MarqeurQuete_Script marqueur)
    {
        GameObject newMarqueur = Instantiate(iconePrefab, compasImage.transform);
        marqueur.iconeCompas = newMarqueur;

        marqueur.image = newMarqueur.GetComponent<Image>();
        marqueur.image.sprite = marqueurImage;

        marqueurQuete.Add(marqueur);
    }

    public void RemoveMarqueurQuete (MarqeurQuete_Script marqueur)
    {
        Destroy(marqueur.iconeCompas);
        marqueurQuete.Remove(marqueur);
    }

    Vector2 GetPosOnCompas (MarqeurQuete_Script marqueur)
    {
        Vector2 playerPos = new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z);
        Vector2 playerFwd = new Vector2(playerCamera.transform.forward.x, playerCamera.transform.forward.z);

        float angle = Vector2.SignedAngle(marqueur.position - playerPos, playerFwd);

        return new Vector2(CompasUnite * angle, 0f);
    }
}
