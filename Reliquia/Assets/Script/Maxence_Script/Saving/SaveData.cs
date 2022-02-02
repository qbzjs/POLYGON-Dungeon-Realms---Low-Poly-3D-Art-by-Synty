using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{

    public DataSave MyDataSave { get; set; }

    public PlayerData MyPlayerData { get; set; }

    public SceneData MySceneData { get; set; }

    public DateTime MyDateTime { get; set; }

    public SaveData()
    {
        MyDateTime = DateTime.Now;
    }
}

[Serializable]
public class DataSave
{
    public string MySaveName { get; set; }

    public DataSave(string saveName)
    {
        MySaveName = saveName;
    }
}

[Serializable]
public class PlayerData
{
    public int MyLife { get; set; }

    public int MyMaxLife { get; set; }

    public int MyMana { get; set; }

    public int MyMaxMana { get; set; }

    public float MyX { get; set; }

    public float MyY { get; set; }

    public float MyZ { get; set; }

    public PlayerData(int life, int maxLife, int mana, int maxMana, Vector3 position)
    {
        MyLife = life;
        MyMaxLife = maxLife;

        MyMana = mana;
        MyMaxMana = maxMana;

        MyX = position.x;
        MyY = position.y;
        MyZ = position.z;
    }
}

[Serializable]
public class SceneData
{
    public int IdScene { get; set; }
    public string NameScene { get; set; }
	public int Avancement { get; set;}

    public SceneData(int sceneId, string sceneName, int avancement)
    {
        IdScene = sceneId;
        NameScene = sceneName;
		Avancement = avancement;
    }
}
