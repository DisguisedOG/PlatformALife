using Godot;
using System;

public partial class ShopUI : CanvasLayer
{
    private PixelMan currentPlayer;
    private InventoryManager currentInv;

    public override void _Ready()
    {
        GetNode<Button>("Panel/VBoxContainer/BuyFishingRodBtn").Pressed += () => OnBuyPressed("Fishing Rod", 50);
        GetNode<Button>("Panel/VBoxContainer/BuyPotionBtn").Pressed += () => OnBuyPressed("Health Potion", 10);
        GetNode<Button>("Panel/VBoxContainer/BuyLandDeedBtn").Pressed += () => OnBuyPressed("Land Deed", 200);
        
        GetNode<Button>("Panel/VBoxContainer/SellWoodBtn").Pressed += () => OnSellPressed("Wood", 2);
        GetNode<Button>("Panel/VBoxContainer/SellStoneBtn").Pressed += () => OnSellPressed("Stone", 4);
        GetNode<Button>("Panel/VBoxContainer/SellClayBtn").Pressed += () => OnSellPressed("Clay", 3);
        GetNode<Button>("Panel/VBoxContainer/SellFishBtn").Pressed += () => OnSellPressed("Fish", 15);
        
        GetNode<Button>("Panel/VBoxContainer/CloseBtn").Pressed += CloseShop;

        this.Visible = false;
    }

    public void OpenShop(PixelMan player)
    {
        currentPlayer = player;
        currentInv = player.GetNodeOrNull<InventoryManager>("InventoryManager");
        this.Visible = true;
    }

    public void CloseShop()
    {
        this.Visible = false;
        GetTree().Paused = false;
    }

    private void OnBuyPressed(string itemName, int cost)
    {
        if (currentInv != null && currentInv.Gold >= cost)
        {
            currentInv.AddGold(-cost);
            currentInv.AddItem(itemName, 1);
            GD.Print($"Bought {itemName} for {cost} Gold.");
        }
        else
        {
            GD.Print($"Not enough Gold for {itemName}!");
        }
    }

    private void OnSellPressed(string itemName, int value)
    {
        if (currentInv != null && currentInv.HasItem(itemName, 1))
        {
            currentInv.RemoveItem(itemName, 1);
            currentInv.AddGold(value);
            GD.Print($"Sold {itemName} for {value} Gold.");
        }
        else
        {
            GD.Print($"You don't have any {itemName} to sell!");
        }
    }
}
