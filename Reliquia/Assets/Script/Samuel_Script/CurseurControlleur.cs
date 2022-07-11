using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Supervise le comportement du curseur, c'est ici que se fait
/// les changements visuel du cuseur (visable ou icone du curseur)</summary>
/// TODO Intégrer aux autres scènes
public class CurseurControlleur : MonoBehaviour
{
    public Texture2D curseurClassique, curseurMain;

    public static CurseurControlleur Instance { get; private set; }

    void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;

        Cursor.SetCursor(curseurClassique, Vector2.up + Vector2.left, CursorMode.Auto);
        DontDestroyOnLoad(this);
    }

    public void LockCurseur(bool lockValue) {
        Cursor.lockState = lockValue ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockValue;
    }
}
