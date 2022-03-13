using UnityEngine;
using UnityEngine.UI;

public class MarqeurQuete_Script : MonoBehaviour
{
    [System.NonSerialized] public Sprite icone;
    [System.NonSerialized] public Image image;

    [System.NonSerialized] public GameObject iconeCompas;

    private void Awake()
    {
        icone = gameObject.GetComponent<PhysicaltemInventaire>().thisItem.itemImage;
        image = gameObject.GetComponent<PhysicaltemInventaire>().itemImage;

       
    }

    private void Start()
    {
        Compas_Script.instance.AddMarqueurQuete(this);
    }

    public Vector2 position
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }
}
