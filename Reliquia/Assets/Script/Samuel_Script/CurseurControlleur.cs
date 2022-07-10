using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseurControlleur : MonoBehaviour
{
    public Texture2D curseurClassique, curseurCroix, curseurMain;

    private CurseurControlleur sInstance;

    public CurseurControlleur Instance { get; private set; }

    void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;

        Cursor.SetCursor(curseurClassique, Vector2.up + Vector2.left, CursorMode.Auto);
        DontDestroyOnLoad(this);
    }
}
