using UnityEngine;
using System.Collections;

public class BulletController:Photon.MonoBehaviour {
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private float moveSpeed;

    [HideInInspector]
    public int ownerPlayerIndex = -1;

    public Collider2D col;
    private Rigidbody2D rb;

    public void Awake() {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Start() {
        if(ownerPlayerIndex == -1) {
            // If we don't have an owner ID, that means we were spawned mid-flight (a player joined while we were flying)
            // Since we never launched our position is probably random and meaningless, so just move us off the field entirely
            transform.position = Vector3.right * 100f;
        }
    }

    public void Update() {
        if(photonView.isMine) {
            // Outside the screen? Destoy ourselves
            if(!NetworkGameManager.gameBounds.Contains(transform.position)) {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    [PunRPC]
    public void Launch(int ownerIndex) {
        ownerPlayerIndex = ownerIndex;
        rb.velocity = transform.up * moveSpeed;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(photonView.isMine) {
            if(other.CompareTag("Player")) {
                other.SendMessage("Hit", ownerPlayerIndex);
                PhotonNetwork.Destroy(gameObject);
            } else {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
