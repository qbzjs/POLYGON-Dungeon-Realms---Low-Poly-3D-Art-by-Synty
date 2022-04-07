using UnityEngine;
using UnityEngine.UI;
using DiasGames.ThirdPersonSystem;


/// <summary>
/// Attache des callback pour mettre à jour l'affichage de ressources
/// aux signaux des script Health et Mana. 
/// </summary>
public class RessourceManager : MonoBehaviour
{
    [Space]
    [Header("Gestionaire de Mana/Vie")] 
    [SerializeField] public Image barreVie;
    [SerializeField] public Text texteVie;
    [SerializeField] public Image barreMana;
    [SerializeField] public Text texteMana;
    private Health mJoueurScriptVie;
    private Mana mJoueurScriptMana; 

    void Start() {
        UpdateBarreMana();
        UpdateBarreVie();
    }

    void OnEnable() {
        // Debug.Log("RessourceManager: Enable");
        mJoueurScriptVie = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        mJoueurScriptMana = GameObject.FindGameObjectWithTag("Player").GetComponent<Mana>();
        mJoueurScriptMana.OnManaChanged += UpdateBarreMana;
        mJoueurScriptVie.OnHealthChanged += UpdateBarreVie;
    }

    void OnDisable() {
        // Debug.Log("RessourceManager: Disable");
        mJoueurScriptMana.OnManaChanged -= UpdateBarreMana;
        mJoueurScriptVie.OnHealthChanged -= UpdateBarreVie;
    }

    void UpdateBarreVie() { 
        // Debug.Log("RessourceManager: life value: " + mJoueurScriptVie.HealthValue + ", max :" + mJoueurScriptVie.MaximumHealth);
        float value = mJoueurScriptVie.HealthValue / mJoueurScriptVie.MaximumHealth;
        barreVie.fillAmount = value;
        texteVie.text = (value * 100.0f) + "%"; 
    }

    void UpdateBarreMana() { 
        // Debug.Log("RessourceManager: mana value: " + mJoueurScriptMana.ManaValue + ", max :" + mJoueurScriptMana.MaximumMana);
        float value = mJoueurScriptMana.ManaValue / mJoueurScriptMana.MaximumMana;
        barreMana.fillAmount = value; 
        texteMana.text = (value * 100.0f) + "%"; 
    }
}
