using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerUI:Photon.MonoBehaviour {
    public static PlayerUI I;
    public Text scoreText;

    private Dictionary<int, int> playerScores = new Dictionary<int, int>();

    public void PlayerScored(int sourcePlayerIndex) {
        int newScore = playerScores[sourcePlayerIndex] + 1;
        photonView.RPC("UpdatePlayerScore", PhotonTargets.All, sourcePlayerIndex, newScore);
    }

    [PunRPC]
    public void UpdatePlayerScore(int playerIndex, int newScore) {
        playerScores[playerIndex] = newScore;
        UpdateText();
    }

    [PunRPC]
    public void PlayerLeft(int playerIndex) {
        playerScores.Remove(playerIndex);
        UpdateText();
    }

    public void SendAllScoresToNewPlayer(PhotonPlayer newPlayer) {
        foreach(KeyValuePair<int, int> playerScore in playerScores) {
            photonView.RPC("UpdatePlayerScore", newPlayer, playerScore.Key, playerScore.Value);
        }
    }

    private void UpdateText() {
        // Update UI
        string newScoreText = "";
        foreach(KeyValuePair<int, int> playerScore in playerScores) {
            Color playerColor = NetworkGameManager.I.playerColors[playerScore.Key];
            newScoreText += string.Format("<color=#{2}ff>P{0}</color> - {1}\n", playerScore.Key, playerScore.Value, ColorToHex(playerColor));
        }
        scoreText.text = newScoreText;
    }

    private string ColorToHex(Color c) {
        Color32 c32 = c;
        string hex = c32.r.ToString("X2") + c32.g.ToString("X2") + c32.b.ToString("X2");
	    return hex;
    }

    public void Awake() {
        I = this;
    }

    public void OnDestroy() {
        I = null;
    }
}
