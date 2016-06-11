using UnityEngine;
using System.Collections;
using Photon;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class JoinGameMenu:PunBehaviour {
    public Button joinGameButton;
    public Image statusPanel;
    public Text statusText;

    public void Start() {
#if UNITY_EDITOR
        PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
#endif
        // Status
        SetStatus("Connecting...", Color.yellow);
        // Start with join game button disabled
        joinGameButton.interactable = false;
        // Connect
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    // ******************** Network State ********************

    public override void OnJoinedLobby() {
        // Status
        SetStatus("Connected to Lobby", Color.green);
        // Enable join button
        joinGameButton.interactable = true;
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        SetStatus("Join room failed, creating new room", Color.red);
        // Couldn't join a room, create one for us instead
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom() {
        SetStatus("Joined room successfully", Color.green);
        // Change scene
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg) {
        SetStatus("Creating room failed, try again", Color.red);
        // Creating room failed, re-enable join button
        joinGameButton.interactable = true;
    }

    // ******************** UI Interactions ********************

    public void ClickJoin() {
        // Prevent double click
        joinGameButton.interactable = false;
        // Status
        SetStatus("Joining random room", Color.yellow);
        // Try joining a room
        PhotonNetwork.JoinRandomRoom();
    }

    private void SetStatus(string status, Color color) {
        statusText.text = status;
        statusPanel.color = color;
    }
}
