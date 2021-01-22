using UnityEngine;
using UnityEngine.UI;

public class MarqeurQuete_Script : MonoBehaviour
{
    public Sprite icone;
    public Image image;

    public GameObject iconeCompas;

    private void Awake()
    {
        icone = gameObject.GetComponent<PhysicaltemInventaire>().thisItem.itemImage;
        image = gameObject.GetComponent<PhysicaltemInventaire>().itemImage;

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
