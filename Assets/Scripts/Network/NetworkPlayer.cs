using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NetworkPlayer:Photon.MonoBehaviour {
    public GameObject tankPrefab;
    private TankController currentTank;

    public void Awake() {
        // If we started on the wrong scene, go back to the menu
        if(!PhotonNetwork.connected) {
            SceneManager.LoadScene("Menu");
            return;
        }
        // Instantiate player's tank
        GameObject tankGO = PhotonNetwork.Instantiate(tankPrefab.name, transform.position, Quaternion.identity, 0);
        currentTank = tankGO.GetComponent<TankController>();
    }

    public void Update() {
        // Use escape to quit
        if(Input.GetKeyDown(KeyCode.Escape)) {
            // When we leave, all our objects will be automatically destroyed
            PhotonNetwork.LeaveRoom();
        }
    }

    public void OnLeftRoom() {
        // Left room? Back to main menu
        SceneManager.LoadScene("Menu");
    }

    public void OnDisconnectedFromPhoton() {
        // Disconnected? Back to main menu
        SceneManager.LoadScene("Menu");
    }

    public void OnFailedToConnectToPhoton() {
        // Failed to connect? Back to main menu
        SceneManager.LoadScene("Menu");
    }
}
