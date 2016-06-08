using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NetworkPlayer:Photon.MonoBehaviour {
    public GameObject tankPrefab;
    private GameObject currentTank;

    public void Awake() {
        // If we started on the wrong scene, go back to the menu
        if(!PhotonNetwork.connected) {
            SceneManager.LoadScene("Menu");
            return;
        }
        // Instantiate player's tank
        currentTank = PhotonNetwork.Instantiate(tankPrefab.name, transform.position, Quaternion.identity, 0);
    }
}
