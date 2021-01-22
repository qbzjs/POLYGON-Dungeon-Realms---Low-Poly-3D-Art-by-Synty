using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public Item item;

    [SerializeField] private Image imageItem;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(item.description);

        GetComponent<MeshFilter>().mesh = item.forme;
        GetComponent<MeshRenderer>().material = item.material;

        imageItem.sprite = item.imageInventaireCompas;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
