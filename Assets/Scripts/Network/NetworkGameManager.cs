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
    public int playerIndex;
    public GameObject tankPrefab;
    private TankController currentTank;

    public void Awake() {
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

        // Join the game using the next available color
        JoinGame(NextPlayerIndex());
    }

    public void Update() {
        // Use escape to quit
        if(Input.GetKeyDown(KeyCode.Escape)) {
            // Clear our score
            PlayerUI.I.PlayerLeft(playerIndex);
            // Release our player color
            ReleasePlayerIndex(playerIndex);
            // Leave the game
            SceneManager.LoadScene("Menu");
        }
    }

    // ******************** Player Join/Leave ********************

    private void ReleasePlayerIndex(int releasedPlayerIndex) {
        playerColors.Add(playerColors[releasedPlayerIndex]);
    }

    private int NextPlayerIndex() {
        Color c = availableColors[0];
        availableColors.RemoveAt(0);
        return playerColors.IndexOf(c);
    }

    public void JoinGame(int newPlayerIndex) {
        // Set player color and store in properties
        playerIndex = newPlayerIndex;
        // Init our score
        PlayerUI.I.UpdatePlayerScore(newPlayerIndex, 0);
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
        GameObject tankGO = (GameObject)Instantiate(tankPrefab, Random.insideUnitCircle * 10f, Quaternion.identity);
        currentTank = tankGO.GetComponent<TankController>();
        currentTank.SetColor(playerIndex);
    }

    public void OnDestroy() {
        I = null;
    }
}
