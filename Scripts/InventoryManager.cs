using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryManager : Node
{
    public int Gold { get; private set; } = 0;
    public Dictionary<string, int> Items { get; private set; } = new Dictionary<string, int>();

    public override void _Ready()
    {
        // If we loaded from a save file, populate inventory
        if (GameManager.Instance != null && GameManager.Instance.LoadedSaveData != null)
        {
            Gold = GameManager.Instance.LoadedSaveData.Gold;
            
            if (GameManager.Instance.LoadedSaveData.Items != null)
            {
                foreach (var item in GameManager.Instance.LoadedSaveData.Items)
                {
                    Items[item.Key] = item.Value;
                }
            }
        }
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        UpdateHUD();
    }

    public void AddItem(string itemName, int amount = 1)
    {
        if (Items.ContainsKey(itemName))
        {
            Items[itemName] += amount;
        }
        else
        {
            Items.Add(itemName, amount);
        }
        UpdateHUD();
    }

    public bool HasItem(string itemName, int amount = 1)
    {
        if (Items.ContainsKey(itemName))
        {
            return Items[itemName] >= amount;
        }
        return false;
    }

    public void RemoveItem(string itemName, int amount = 1)
    {
        if (HasItem(itemName, amount))
        {
            Items[itemName] -= amount;
            if (Items[itemName] <= 0)
            {
                Items.Remove(itemName);
            }
            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        var hud = GetTree().Root.GetNodeOrNull<HUD>("World/HUD");
        if (hud != null)
        {
            hud.RefreshInventory(this);
        }
    }
}
