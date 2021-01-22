using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class InventaireSauvegarde : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;

    public static InventaireSauvegarde instance;
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

    public void LoadInventory()
    {
        playerInventory.sacochesInventory.Clear();
        playerInventory.consommablesInventory.Clear();
        playerInventory.objetsQuetesInventory.Clear();
        playerInventory.puzzlesInventory.Clear();

        LoadSacoche();
        LoadConsommable();
        LoadObjetsQuetes();
        LoadPuzzles();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) //Temporaire
        {
            SaveInventory(); 
        }
    }

    public void SaveInventory()
    {
        SaveSacoche();
        SaveConsommables();
        SaveObjetsQuetes();
        SavePuzzles();
    }

    #region resetInventaires
    public void ResetSacoche()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Sacoche" + string.Format("/{0}.sac", i)))
        {
            File.Delete(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Sacoche" + string.Format("/{0}.sac", i));
            i++;
        }
    }

    public void ResetConsommable()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Consommables" + string.Format("/{0}.cons", i)))
        {
            File.Delete(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Consommables" + string.Format("/{0}.cons", i));
            i++;
        }
    }

    public void ResetObjetsQuetes()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/ObjetsQuetes" + string.Format("/{0}.odq", i)))
        {
            File.Delete(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/ObjetsQuetes" + string.Format("/{0}.odq", i));
            i++;
        }
    }

    public void ResetPuzzles()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Puzzles" + string.Format("/{0}.puz", i)))
        {
            File.Delete(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Puzzles" + string.Format("/{0}.puz", i));
            i++;
        }
    }
    #endregion

    #region saveInventaires

    public void SaveSacoche()
    {
        ResetSacoche();
        for (int i = 0; i < playerInventory.sacochesInventory.Count; i++)
        {
            FileStream file = File.Create(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Sacoche" + string.Format("/{0}.sac", i));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(playerInventory.sacochesInventory[i]);
            binary.Serialize(file, json);
            file.Close();
        }
    }
    public void SaveConsommables()
    {
        ResetConsommable();
        for (int i = 0; i < playerInventory.consommablesInventory.Count; i++)
        {
            FileStream file = File.Create(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Consommables" + string.Format("/{0}.cons", i));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(playerInventory.consommablesInventory[i]);
            binary.Serialize(file, json);
            file.Close();
        }
    }
    public void SaveObjetsQuetes()
    {
        ResetObjetsQuetes();
        for (int i = 0; i < playerInventory.objetsQuetesInventory.Count; i++)
        {
            FileStream file = File.Create(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/ObjetsQuetes" + string.Format("/{0}.odq", i));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(playerInventory.objetsQuetesInventory[i]);
            binary.Serialize(file, json);
            file.Close();
        }
    }
    public void SavePuzzles()
    {
        ResetPuzzles();
        for (int i = 0; i < playerInventory.puzzlesInventory.Count; i++)
        {
            FileStream file = File.Create(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Puzzles" + string.Format("/{0}.puz", i));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(playerInventory.puzzlesInventory[i]);
            binary.Serialize(file, json);
            file.Close();
        }
    }

    #endregion

    #region LoadInventaires
    public void LoadSacoche()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Sacoche" + string.Format("/{0}.sac", i)))
        {
            var temp = ScriptableObject.CreateInstance<ItemInventaire>();

            FileStream file = File.Open(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Sacoche" + string.Format("/{0}.sac", i), FileMode.Open);
            BinaryFormatter binary = new BinaryFormatter();

            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), temp);
            file.Close();

            foreach (var item in Inventaire_Script.instance.items)
            {
                if (item.itemNom == temp.itemNom)
                {
                    item.itemNom = temp.itemNom;
                    item.itemDescription = temp.itemDescription;
                    /*item.itemImage = temp.itemImage;
                    item.itemMesh = temp.itemMesh;
                    item.itemMaterial = temp.itemMaterial;*/
                    item.numberHeld = temp.numberHeld;
                    item.usable = temp.usable;
                    item.unique = temp.unique;
                    item.typeItem = temp.typeItem;
                    item.thisEvent = temp.thisEvent;

                    playerInventory.sacochesInventory.Add(item);
                    i++;
                }
            }
        }
    }
    public void LoadConsommable()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Consommables" + string.Format("/{0}.cons", i)))
        {
            var temp = ScriptableObject.CreateInstance<ItemInventaire>();
            FileStream file = File.Open(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Consommables" + string.Format("/{0}.cons", i), FileMode.Open);
            BinaryFormatter binary = new BinaryFormatter();
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), temp);
            file.Close();

            foreach (var item in Inventaire_Script.instance.items)
            {
                if (item.itemNom == temp.itemNom)
                {
                    item.itemNom = temp.itemNom;
                    item.itemDescription = temp.itemDescription;
                    /*item.itemImage = temp.itemImage;
                    item.itemMesh = temp.itemMesh;
                    item.itemMaterial = temp.itemMaterial;*/
                    item.numberHeld = temp.numberHeld;
                    item.usable = temp.usable;
                    item.unique = temp.unique;
                    item.typeItem = temp.typeItem;
                    item.thisEvent = temp.thisEvent;

                    playerInventory.consommablesInventory.Add(item);
                    i++;
                }
            }
        }
    }
    public void LoadObjetsQuetes()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/ObjetsQuetes" + string.Format("/{0}.odq", i)))
        {
            var temp = ScriptableObject.CreateInstance<ItemInventaire>();
            FileStream file = File.Open(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/ObjetsQuetes" + string.Format("/{0}.odq", i), FileMode.Open);
            BinaryFormatter binary = new BinaryFormatter();

            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), temp);
            file.Close();

            foreach (var item in Inventaire_Script.instance.items)
            {
                if (item.itemNom == temp.itemNom)
                {
                    item.itemNom = temp.itemNom;
                    item.itemDescription = temp.itemDescription;
                    /*item.itemImage = temp.itemImage;
                    item.itemMesh = temp.itemMesh;
                    item.itemMaterial = temp.itemMaterial;*/
                    item.numberHeld = temp.numberHeld;
                    item.usable = temp.usable;
                    item.unique = temp.unique;
                    item.typeItem = temp.typeItem;
                    item.thisEvent = temp.thisEvent;

                    playerInventory.objetsQuetesInventory.Add(item);
                    i++;
                }
            }
        }
    }
    public void LoadPuzzles()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Puzzles" + string.Format("/{0}.puz", i)))
        {
            var temp = ScriptableObject.CreateInstance<ItemInventaire>();
            FileStream file = File.Open(Application.persistentDataPath + "/" + GameManager.instance.nomSauvegarde + "/Inventaire/Puzzles" + string.Format("/{0}.puz", i), FileMode.Open);
            BinaryFormatter binary = new BinaryFormatter();
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), temp);
            file.Close();

            foreach (var item in Inventaire_Script.instance.items)
            {
                if (item.itemNom == temp.itemNom)
                {
                    item.itemNom = temp.itemNom;
                    item.itemDescription = temp.itemDescription;
                    /*item.itemImage = temp.itemImage;
                    item.itemMesh = temp.itemMesh;
                    item.itemMaterial = temp.itemMaterial;*/
                    item.numberHeld = temp.numberHeld;
                    item.usable = temp.usable;
                    item.unique = temp.unique;
                    item.typeItem = temp.typeItem;
                    item.thisEvent = temp.thisEvent;

                    playerInventory.puzzlesInventory.Add(item);
                    i++;
                }
            }
        }
    }
    #endregion
}
