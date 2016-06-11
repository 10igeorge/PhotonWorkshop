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
        // Status
        SetStatus("Connecting...", Color.yellow);
        // Start with join game button enabled
        joinGameButton.interactable = true;
    }

    // ******************** UI Interactions ********************

    public void ClickJoin() {
        // Prevent double click
        joinGameButton.interactable = false;
        // Status
        SetStatus("Joining game", Color.yellow);
        // Join the game
        SceneManager.LoadScene("Game");
    }

    private void SetStatus(string status, Color color) {
        statusText.text = status;
        statusPanel.color = color;
    }
}
