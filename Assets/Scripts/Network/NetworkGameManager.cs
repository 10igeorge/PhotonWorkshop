using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Photon;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkGameManager:PunBehaviour {
    public static Rect gameBounds;
    public static NetworkGameManager I;
    // All players
    public List<Color> playerColors;
    private List<Color> availableColors;

    // Just the current player
    public int playerColorIndex;
    public GameObject tankPrefab;
    private TankController currentTank;

    public void Awake() {
        // Started from the wrong scene? Jump back to menu
        if(!PhotonNetwork.connected) {
            SceneManager.LoadScene("Menu");
            return;
        }

        I = this;
        // Calculate camera/game bounds
        Camera cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;
        gameBounds = new Rect(-width, -height, width * 2f, height * 2f);
    }

    public void Start() {
        // Store available colors
        availableColors = new List<Color>(playerColors);

        if(PhotonNetwork.isMasterClient) {
            // Join the game using the next available color
            JoinGame(NextColor());
        }
        // If we're not the master, wait for the master to recognize us and assign us a color
    }

    public void Update() {
        // Use escape to quit
        if(Input.GetKeyDown(KeyCode.Escape)) {
            // When we leave, all our objects will be automatically destroyed
            PhotonNetwork.LeaveRoom();
        }
    }

    // ******************** Player Join/Leave ********************

    private void ReleaseColor(PhotonPlayer p) {
        int colorIndex = (int)p.customProperties["color"];
        playerColors.Add(playerColors[colorIndex]);
    }

    private int NextColor() {
        Color c = availableColors[0];
        availableColors.RemoveAt(0);
        return playerColors.IndexOf(c);
    }

    [PunRPC]
    public void JoinGame(int colorIndex) {
        // Set player color and store in properties
        playerColorIndex = colorIndex;
        Hashtable playerProperties = new Hashtable {
            { "color", colorIndex }
        };
        PhotonNetwork.player.SetCustomProperties(playerProperties);
        // Spawn Immediately
        StartCoroutine(Spawn(0f));
    }

    // ******************** Player Tank State & RPCs ********************

    public void TankWasDestroyed() {
        StartCoroutine(Spawn(1f)); // Respawn after 1 second
    }

    private IEnumerator Spawn(float delay) {
        yield return new WaitForSeconds(delay);
        // Instantiate player's tank
        GameObject tankGO = PhotonNetwork.Instantiate(tankPrefab.name, Random.insideUnitCircle * 10f, Quaternion.identity, 0);
        currentTank = tankGO.GetComponent<TankController>();
        // Set color on the tank (and on all versions of it on other players' screens)
        currentTank.photonView.RPC("SetColor", PhotonTargets.AllBuffered, playerColorIndex);
    }

    // ******************** Network State ********************

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
        if(PhotonNetwork.isMasterClient) {
            // Assign the player a color and tell them to join
            photonView.RPC("JoinGame", newPlayer, NextColor());
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        if(PhotonNetwork.isMasterClient) {
            // Release that player's color
            ReleaseColor(otherPlayer);
        }
    }

    public override void OnLeftRoom() {
        // Left room? Back to main menu
        SceneManager.LoadScene("Menu");
    }

    public override void OnDisconnectedFromPhoton() {
        // Disconnected? Back to main menu
        SceneManager.LoadScene("Menu");
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause) {
        // Failed to connect? Back to main menu
        SceneManager.LoadScene("Menu");
    }

    public void OnDestroy() {
        I = null;
    }
}
