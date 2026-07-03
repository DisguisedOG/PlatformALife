using Godot;
using System;
using System.Collections.Generic;

public partial class MultiplayerManager : Node
{
    public static MultiplayerManager Instance { get; private set; }
    
    private const int PORT = 7777;
    private const int MAX_CLIENTS = 2;

    public List<long> ConnectedPlayers = new List<long>();
    
    public string PendingJoinIp { get; private set; } = "";

    public override void _Process(double delta)
    {
        if (!string.IsNullOrEmpty(PendingJoinIp) && GetTree().CurrentScene != null && GetTree().CurrentScene.Name == "World")
        {
            var peer = new ENetMultiplayerPeer();
            var error = peer.CreateClient(PendingJoinIp, PORT);
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to join server: " + error);
            }
            else
            {
                Multiplayer.MultiplayerPeer = peer;
                GD.Print("Joining server at " + PendingJoinIp);
            }
            PendingJoinIp = "";
        }
    }

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
            return;
        }
        
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
        
        GD.Print("MultiplayerManager Initialized.");
    }

    public void HostGame()
    {
        var peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(PORT, MAX_CLIENTS);
        if (error != Error.Ok)
        {
            GD.PrintErr("Failed to start server: " + error);
            return;
        }
        
        Multiplayer.MultiplayerPeer = peer;
        ConnectedPlayers.Add(1);
        GD.Print("Server started on port " + PORT);
    }

    public void JoinGame(string ipAddress)
    {
        PendingJoinIp = ipAddress;
        GameManager.Instance.LoadWorld("MultiplayerTest", 0, 0);
    }

    private void OnPeerConnected(long id)
    {
        GD.Print("Player connected! ID: " + id);
        ConnectedPlayers.Add(id);
        
        if (Multiplayer.IsServer())
        {
            var playersNode = GetTree().Root.GetNodeOrNull("World/Players");
            if (playersNode != null)
            {
                var playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/PixelMan.tscn");
                var player = playerScene.Instantiate<PixelMan>();
                player.Name = id.ToString();
                
                var worldGen = GetTree().Root.GetNodeOrNull<WorldGenerator>("World/WorldGenerator");
                if (worldGen != null)
                {
                    player.GlobalPosition = worldGen.GetSpawnPosition(0);
                }
                
                playersNode.AddChild(player, true);
            }
        }
    }

    private void OnPeerDisconnected(long id)
    {
        GD.Print("Player disconnected! ID: " + id);
        ConnectedPlayers.Remove(id);
        
        var playerNode = GetTree().Root.GetNodeOrNull($"World/Players/{id}");
        if (playerNode != null) playerNode.QueueFree();
    }

    private void OnConnectedToServer()
    {
        GD.Print("Successfully connected to server!");
        ConnectedPlayers.Add(Multiplayer.GetUniqueId());
    }

    private void OnConnectionFailed()
    {
        GD.PrintErr("Connection failed!");
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnServerDisconnected()
    {
        GD.PrintErr("Server disconnected!");
        ConnectedPlayers.Clear();
        Multiplayer.MultiplayerPeer = null;
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }
}
