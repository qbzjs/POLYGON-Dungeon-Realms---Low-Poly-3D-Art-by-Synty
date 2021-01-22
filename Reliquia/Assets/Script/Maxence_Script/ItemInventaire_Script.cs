using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventaireItem
{
    string NomItem { get; }

    Sprite Image { get; }

    void OnPickup();

    void OnDrop();
}

public class InventaireEventArgs : EventArgs
{
    public InventaireEventArgs(IInventaireItem item)
    {
        Item = item;
    }

    public IInventaireItem Item;
}
