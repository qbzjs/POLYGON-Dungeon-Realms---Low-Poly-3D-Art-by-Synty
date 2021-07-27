using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public Image fondTransition;
    public int idScene;

    public List<GameObject> saveSlots = new List<GameObject>();
    SavedGame savedGame;

    public static SaveManager instance;

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

    // Start is called before the first frame update
    void Start()
    {
        savedGame = FindObjectOfType<SavedGame>();
    }

    public IEnumerator affichageSaveLoad()
    {
        yield return new WaitForSeconds(1);
        GestionSlots();
    }

    public void ShowSavedFile(SavedGame savedGame)
    {
        if(File.Exists(Application.persistentDataPath + "/" + savedGame.MySaveName + "/" + savedGame.MySaveName + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.MySaveName + "/" + savedGame.MySaveName + ".dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);

            file.Close();
            savedGame.ShowInfo(data);
        }
    }

    public void GestionSlots()
    {
        foreach (GameObject saveGame in saveSlots)
        {
            ShowSavedFile(saveGame.GetComponent<SavedGame>());
        }
    }

    public void Save(SavedGame savedGame)
    {
        try
        {
            if (savedGame.transform.GetChild(0).GetComponent<Text>().text == "Nouvelle partie")
            {
                GameManager.instance.choixNomSauvegarde();
            }
            else if (File.Exists(Application.persistentDataPath + "/" + savedGame.MySaveName + "/" + savedGame.MySaveName + ".dat") && GameManager.instance.popUpActif == false) GameManager.instance.ecraserSauvegarde();
            else
            {
                BinaryFormatter bf = new BinaryFormatter();

                FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.MySaveName + "/" + savedGame.MySaveName + ".dat", FileMode.Create);

                SaveData data = new SaveData();

                //SaveName(data);

                SavePlayer(data);
                SaveScene(data);

                GameManager.instance.nomSauvegarde = savedGame.MySaveName;

                SaveMap(GameManager.instance.nomSauvegarde);

                bf.Serialize(file, data);

                file.Close();

                GestionSlots();
            }
        }
        catch (Exception)
        {

        }
    }

    private void SavePlayer(SaveData data)
    {
        data.MyPlayerData = new PlayerData(RessourcesVitalesWilliam_Scrip.instance.vieWilliam, 
            RessourcesVitalesWilliam_Scrip.instance.maxVie, 
            RessourcesVitalesWilliam_Scrip.instance.manaWilliam,
            RessourcesVitalesWilliam_Scrip.instance.maxMana,
            William_Script.instance.transform.position);
    }

    private void SaveScene(SaveData data)
    {
        data.MySceneData = new SceneData(SceneManager.GetActiveScene().buildIndex, SceneManager.GetActiveScene().name);
    }

    private void SaveName(SaveData data)
    {
        data.MyDataSave = new DataSave(GameManager.instance.popUp.GetComponentInChildren<InputField>().GetComponentInChildren<Text>().text);
    }

    public void NewSave(SavedGame savedGame)
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

           Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGame.MySaveName);

           FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.MySaveName + "/" + savedGame.MySaveName + ".dat", FileMode.Create);

            Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGame.MySaveName + "/Cartes");
            Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGame.MySaveName + "/Inventaire");
            Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGame.MySaveName + "/Inventaire/Sacoche");
            Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGame.MySaveName + "/Inventaire/Consommables");
            Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGame.MySaveName + "/Inventaire/ObjetsQuetes");
            Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGame.MySaveName + "/Inventaire/Puzzles");

            SaveData data = new SaveData();

            NewSavePlayerData(data);

            bf.Serialize(file, data);

            file.Close();

            GameManager.instance.nomSauvegarde = savedGame.MySaveName;

            GestionSlots();

            LoadPlayer(data);

            idScene = data.MySceneData.IdScene;

            if (data.MySceneData.IdScene != SceneManager.GetActiveScene().buildIndex) fondTransition.DOFade(1, 1.5f).OnComplete(()=>LoadScene(data));

            GameManager.instance.menuPause();
        }
        catch (Exception)
        {

        }
    }

    private void NewSavePlayerData(SaveData data)
    {
        data.MySceneData = new SceneData(3, "maxence_SceneTestPersonnage");
        data.MyPlayerData = new PlayerData(100, 100, 100, 100, new Vector3(0,0,0));
    }

    public void SaveMap(string nomSave)
    {
        HUD_Script.instance.parentGrimoireMask = (Texture2D)HUD_Script.instance.parentGrimoireMaskImage.texture;

        var bytes = HUD_Script.instance.parentGrimoireMask.EncodeToPNG();
        var dirPath = Application.persistentDataPath + "/" + nomSave + "/Cartes";

        File.WriteAllBytes(dirPath + "/Map.png", bytes);
    }

    public void LoadMap(string nomSave)
    {
        var dirPath = Application.persistentDataPath + "/" + nomSave + "/Cartes";

        if (!File.Exists(dirPath + "/Map.png"))
        {
            HUD_Script.instance.parentGrimoireMask = HUD_Script.instance.parentGrimoireMaskImage.texture as Texture2D;

            var bytes = HUD_Script.instance.parentGrimoireMask.EncodeToPNG();

            File.WriteAllBytes(dirPath + "/Map.png", bytes);
        }
        else
        {

            byte[] byteArray = File.ReadAllBytes(dirPath + "/Map.png");

            Texture2D sampleTexture = new Texture2D(2, 2);

            bool isLoaded = sampleTexture.LoadImage(byteArray);
            if (isLoaded)
            {
                HUD_Script.instance.parentGrimoireMaskImage.texture = sampleTexture;

                HUD_Script.instance.parentGrimoireMask = (Texture2D)HUD_Script.instance.parentGrimoireMaskImage.texture;
            }
        }
    }

    public void LoadInGame(string nomSave, bool backPoint = false)
    {
        if (fondTransition == null)
        {
            fondTransition = GameObject.FindGameObjectWithTag("FondNoir").GetComponent<Image>();
        }
        try
        {
            UnityEngine.Debug.Log(Application.persistentDataPath + "/" + nomSave + "/" + nomSave + ".dat");
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/" + nomSave + "/" + nomSave + ".dat", FileMode.Open);

            SaveData data = (SaveData)bf.Deserialize(file);

            file.Close();

            LoadPlayer(data);
            LoadMap(nomSave);

            // TODO : fondTransition.DOFade() ne fonctionne pas, je l'enlève pour l'instant
            // fondTransition.DOFade(1, 1.5f).OnComplete(() => LoadScene(data)); => NOK
            // fondTransition.DOFade(1, 0.5f).OnComplete(() => SceneManager.LoadScene("Alexis_LD_Catacombes")); => OK
            //Lance la scène data lorsque le fondu au noir est fini
            if (data.MySceneData.IdScene != SceneManager.GetActiveScene().buildIndex) fondTransition.DOFade(1, 0.5f).OnComplete(() => LoadScene(data));
            else if (data.MySceneData.IdScene == SceneManager.GetActiveScene().buildIndex) HUD_Script.instance.setInfoWilliam();


            if (!backPoint)
            {
                GameManager.instance.menuPause();
            }
            
        }
        catch (Exception)
        {

        }
    }

    public void Load(SavedGame savedGame)
    {
        try
        {
            if (savedGame.transform.GetChild(0).GetComponent<Text>().text == "Nouvelle partie") GameManager.instance.choixNomSauvegarde();
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                UnityEngine.Debug.Log(Application.persistentDataPath + "/" + savedGame.MySaveName + "/" + savedGame.MySaveName + ".dat");
                FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.MySaveName + "/" + savedGame.MySaveName + ".dat", FileMode.Open);

                SaveData data = (SaveData)bf.Deserialize(file);

                file.Close();

                GameManager.instance.nomSauvegarde = savedGame.MySaveName;

                LoadPlayer(data);
                
                idScene = data.MySceneData.IdScene;

                if (data.MySceneData.IdScene != SceneManager.GetActiveScene().buildIndex) fondTransition.DOFade(1, 1.5f).OnComplete(() => LoadScene(data));

                GameManager.instance.menuPause();
            }
        }
        catch (Exception)
        {

        }
    }

    private void LoadPlayer(SaveData data)
    {
        InformationsPlayer.instance.williamVie = data.MyPlayerData.MyLife;
        InformationsPlayer.instance.maxWilliamVie = data.MyPlayerData.MyMaxLife;
        //RessourcesVitalesWilliam_Scrip.instance.SetVie(RessourcesVitalesWilliam_Scrip.instance.vieWilliam);

        InformationsPlayer.instance.williamMana = data.MyPlayerData.MyMana;
        InformationsPlayer.instance.maxWilliamMana = data.MyPlayerData.MyMaxMana;
        //RessourcesVitalesWilliam_Scrip.instance.SetMana(RessourcesVitalesWilliam_Scrip.instance.manaWilliam);

        InformationsPlayer.instance.williamPosition = new Vector3(data.MyPlayerData.MyX, data.MyPlayerData.MyY, data.MyPlayerData.MyZ);
    }
    
    private void LoadScene(SaveData data)
    {
        savedGame.idSceneActuelle = data.MySceneData.IdScene;
        savedGame.nomSceneActuelle = data.MySceneData.NameScene;

        SceneManager.LoadSceneAsync(2);
    }
}
