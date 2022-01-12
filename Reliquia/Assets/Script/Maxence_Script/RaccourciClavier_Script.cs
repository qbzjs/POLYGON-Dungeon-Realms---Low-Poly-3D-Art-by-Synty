using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace clavier
{
	public class RaccourciClavier_Script : MonoBehaviour
    {

        public Dictionary<string, KeyCode> toucheClavier = new Dictionary<string, KeyCode>();

        public Text avancer, gauche, reculer, droite, action, saut, pouvoir1, pouvoir2, pouvoir3, pouvoir4, courir, accroupir, attaque, garde, menuPause, menuInventaire;
        [SerializeField] private Text[] texteAssignationTouche;

        int Alpha;

        private GameObject toucheSelectionne;

        private Color32 normal = new Color32(115, 115, 115, 255);
        private Color32 selection = new Color32(247, 247, 247, 255);

        bool toucheExistante;
        string nomToucheExistante;

        private void Awake()
        {
            //PlayerPrefs.DeleteAll();
            AssignationTouche();
        }

        public void AssignationTouche()
        {
            toucheClavier.Add("Avancer", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Avancer", "Z")));
            toucheClavier.Add("Gauche", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Gauche", "Q")));
            toucheClavier.Add("Reculer", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Reculer", "S")));
            toucheClavier.Add("Droite", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Droite", "D")));

            toucheClavier.Add("Action", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Action", "E")));
            toucheClavier.Add("Saut", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Saut", "Space")));

            toucheClavier.Add("PouvoirSpecial", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("PouvoirSpecial", "Tab")));
            toucheClavier.Add("Pouvoir1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pouvoir1", "Alpha1")));
            toucheClavier.Add("Pouvoir2", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pouvoir2", "Alpha2")));
            toucheClavier.Add("Pouvoir3", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pouvoir3", "Alpha3")));
            toucheClavier.Add("Pouvoir4", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pouvoir4", "Alpha4")));

            toucheClavier.Add("Courir", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Courir", "LeftShift")));
            toucheClavier.Add("Accroupir", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Accroupir", "LeftControl")));
            toucheClavier.Add("Attaque", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Attaque", "Mouse0")));
            toucheClavier.Add("Garde", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Garde", "Mouse1")));
            toucheClavier.Add("MenuPause", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MenuPause", "Escape")));
            toucheClavier.Add("MenuInventaire", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MenuInventaire", "I")));
            
            avancer.text = toucheClavier["Avancer"].ToString();
            gauche.text = toucheClavier["Gauche"].ToString();
            reculer.text = toucheClavier["Reculer"].ToString();
            droite.text = toucheClavier["Droite"].ToString();

            action.text = toucheClavier["Action"].ToString();
            saut.text = toucheClavier["Saut"].ToString();

            pouvoir1.text = toucheClavier["Pouvoir1"].ToString();
            pouvoir2.text = toucheClavier["Pouvoir2"].ToString();
            pouvoir3.text = toucheClavier["Pouvoir3"].ToString();
            pouvoir4.text = toucheClavier["Pouvoir4"].ToString();

            courir.text = toucheClavier["Courir"].ToString();
            accroupir.text = toucheClavier["Accroupir"].ToString();
            attaque.text = toucheClavier["Attaque"].ToString();
            garde.text = toucheClavier["Garde"].ToString();

            menuPause.text = toucheClavier["MenuPause"].ToString();
            menuInventaire.text = toucheClavier["MenuInventaire"].ToString();

            for (int i = 0; i < 16; i++)
            {
                switch (texteAssignationTouche[i].text)
                {
                    case "Alpha0":
                        texteAssignationTouche[i].text = "0";
                        break;

                    case "Alpha1":
                        texteAssignationTouche[i].text = "1";
                        break;

                    case "Alpha2":
                        texteAssignationTouche[i].text = "2";
                        break;

                    case "Alpha3":
                        texteAssignationTouche[i].text = "3";
                        break;

                    case "Alpha4":
                        texteAssignationTouche[i].text = "4";
                        break;

                    case "Alpha5":
                        texteAssignationTouche[i].text = "5";
                        break;

                    case "Alpha6":
                        texteAssignationTouche[i].text = "6";
                        break;

                    case "Alpha7":
                        texteAssignationTouche[i].text = "7";
                        break;

                    case "Alpha8":
                        texteAssignationTouche[i].text = "8";
                        break;

                    case "Alpha9":
                        texteAssignationTouche[i].text = "9";
                        break;

                    case "Mouse0":
                        texteAssignationTouche[i].text = "CLIC-G";
                        break;

                    case "Mouse1":
                        texteAssignationTouche[i].text = "CLIC-D";
                        break;

                    case "LeftShift":
                        texteAssignationTouche[i].text = "MAJ-G";
                        break;

                    case "RightShift":
                        texteAssignationTouche[i].text = "MAJ-D";
                        break;

                    case "LeftControl":
                        texteAssignationTouche[i].text = "CTRL-L";
                        break;

                    case "RightControl":
                        texteAssignationTouche[i].text = "CTRL-D";
                        break;

                    case "Escape":
                        texteAssignationTouche[i].text = "ÉCHAP";
                        break;
                }
            }
        }

        void OnGUI()
        {
            if (toucheSelectionne != null)
            {
                Event e = Event.current;
                if (e.isKey || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                {

                    toucheExistante = toucheClavier.ContainsValue(e.keyCode);

                    if (toucheExistante == true)
                    {
                        nomToucheExistante = toucheClavier.FindKeyByValue(e.keyCode);
                        toucheClavier[nomToucheExistante] = toucheClavier[toucheSelectionne.name];
                        GameObject.Find(nomToucheExistante).GetComponent<Text>().text = toucheClavier[toucheSelectionne.name].ToString();
                        GameObject.Find(nomToucheExistante).GetComponent<Text>().color = normal;
                        ChangementTextToucheExistante();
                    }


                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        e.keyCode = KeyCode.Mouse0;
                        toucheExistante = toucheClavier.ContainsValue(e.keyCode);
                        if (toucheExistante)
                        {
                            nomToucheExistante = toucheClavier.FindKeyByValue(e.keyCode);
                            toucheClavier[nomToucheExistante] = toucheClavier[toucheSelectionne.name];
                            GameObject.Find(nomToucheExistante).GetComponent<Text>().text = toucheClavier[toucheSelectionne.name].ToString();
                            GameObject.Find(nomToucheExistante).GetComponent<Text>().color = normal;
                            ChangementTextToucheExistante();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        e.keyCode = KeyCode.Mouse1;
                        toucheExistante = toucheClavier.ContainsValue(e.keyCode);
                        if (toucheExistante)
                        {
                            nomToucheExistante = toucheClavier.FindKeyByValue(e.keyCode);
                            toucheClavier[nomToucheExistante] = toucheClavier[toucheSelectionne.name];
                            GameObject.Find(nomToucheExistante).GetComponent<Text>().text = toucheClavier[toucheSelectionne.name].ToString();
                            GameObject.Find(nomToucheExistante).GetComponent<Text>().color = normal;
                            ChangementTextToucheExistante();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        e.keyCode = KeyCode.LeftShift;
                        toucheExistante = toucheClavier.ContainsValue(e.keyCode);
                        if (toucheExistante)
                        {
                            nomToucheExistante = toucheClavier.FindKeyByValue(e.keyCode);
                            toucheClavier[nomToucheExistante] = toucheClavier[toucheSelectionne.name];
                            GameObject.Find(nomToucheExistante).GetComponent<Text>().text = toucheClavier[toucheSelectionne.name].ToString();
                            GameObject.Find(nomToucheExistante).GetComponent<Text>().color = normal;
                            ChangementTextToucheExistante();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.RightShift))
                    {
                        e.keyCode = KeyCode.RightShift;
                        toucheExistante = toucheClavier.ContainsValue(e.keyCode);
                        if (toucheExistante)
                        {
                            nomToucheExistante = toucheClavier.FindKeyByValue(e.keyCode);
                            toucheClavier[nomToucheExistante] = toucheClavier[toucheSelectionne.name];
                            GameObject.Find(nomToucheExistante).GetComponent<Text>().text = toucheClavier[toucheSelectionne.name].ToString();
                            GameObject.Find(nomToucheExistante).GetComponent<Text>().color = normal;
                            ChangementTextToucheExistante();
                        }
                    }

                    toucheClavier[toucheSelectionne.name] = e.keyCode;
                    toucheSelectionne.GetComponent<Text>().text = e.keyCode.ToString();
                    toucheSelectionne.GetComponent<Text>().color = normal;

                    switch (e.keyCode)
                    {
                        case KeyCode.Alpha0:
                            toucheSelectionne.GetComponent<Text>().text = "0";
                            break;

                        case KeyCode.Alpha1:
                            toucheSelectionne.GetComponent<Text>().text = "1";
                            break;

                        case KeyCode.Alpha2:
                            toucheSelectionne.GetComponent<Text>().text = "2";
                            break;

                        case KeyCode.Alpha3:
                            toucheSelectionne.GetComponent<Text>().text = "3";
                            break;

                        case KeyCode.Alpha4:
                            toucheSelectionne.GetComponent<Text>().text = "4";
                            break;

                        case KeyCode.Alpha5:
                            toucheSelectionne.GetComponent<Text>().text = "5";
                            break;

                        case KeyCode.Alpha6:
                            toucheSelectionne.GetComponent<Text>().text = "6";
                            break;

                        case KeyCode.Alpha7:
                            toucheSelectionne.GetComponent<Text>().text = "7";
                            break;

                        case KeyCode.Alpha8:
                            toucheSelectionne.GetComponent<Text>().text = "8";
                            break;

                        case KeyCode.Alpha9:
                            toucheSelectionne.GetComponent<Text>().text = "9";
                            break;

                        case KeyCode.Mouse0:
                            toucheSelectionne.GetComponent<Text>().text = "CLIC-G";
                            break;

                        case KeyCode.Mouse1:
                            toucheSelectionne.GetComponent<Text>().text = "CLIC-D";
                            break;

                        case KeyCode.LeftShift:
                            toucheSelectionne.GetComponent<Text>().text = "MAJ-G";
                            break;

                        case KeyCode.RightShift:
                            toucheSelectionne.GetComponent<Text>().text = "MAJ-D";
                            break;

                        case KeyCode.LeftControl:
                            toucheSelectionne.GetComponent<Text>().text = "CTRL-L";
                            break;

                        case KeyCode.RightControl:
                            toucheSelectionne.GetComponent<Text>().text = "CTRL-D";
                            break;

                        case KeyCode.Escape:
                            toucheSelectionne.GetComponent<Text>().text = "ÉCHAP";
                            break;
                    }

                    SauvegardeToucheClavier();
                    toucheSelectionne = null;
                }
            }
        }

        public void ChangerTouche(GameObject clique)
        {
            if (toucheSelectionne != null)
            {
                toucheSelectionne.GetComponent<Text>().color = normal;
            }

            toucheSelectionne = clique;
            toucheSelectionne.GetComponent<Text>().color = selection;
        }

        public void SauvegardeToucheClavier()
        {
            foreach (var touche in toucheClavier)
            {
                PlayerPrefs.SetString(touche.Key, touche.Value.ToString());
            }

            PlayerPrefs.Save();
        }

        public void ChangementTextToucheExistante()
        {
            switch (GameObject.Find(nomToucheExistante).GetComponent<Text>().text)
            {
                case "Alpha0":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "0";
                    break;

                case "Alpha1":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "1";
                    break;

                case "Alpha2":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "2";
                    break;

                case "Alpha3":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "3";
                    break;

                case "Alpha4":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "4";
                    break;

                case "Alpha5":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "5";
                    break;

                case "Alpha6":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "6";
                    break;

                case "Alpha7":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "7";
                    break;

                case "Alpha8":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "8";
                    break;

                case "Alpha9":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "9";
                    break;

                case "Mouse0":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "CLIC-G";
                    break;

                case "Mouse1":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "CLIC-D";
                    break;

                case "LeftShift":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "MAJ-G";
                    break;

                case "RightShift":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "MAJ-D";
                    break;

                case "LeftControl":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "CTRL-L";
                    break;

                case "RightControl":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "CTRL-D";
                    break;

                case "Escape":
                    GameObject.Find(nomToucheExistante).GetComponent<Text>().text = "ÉCHAP";
                    break;

            }
        }
    }
}
