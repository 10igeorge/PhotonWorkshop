using UnityEngine;
using System.Collections;

public class BulletController:Photon.MonoBehaviour {
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private float moveSpeed;

    [HideInInspector]
    public int ownerId;

    private Collider2D col;
    private Rigidbody2D rb;
    private float lifeTime;

    public void Awake() {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update() {
        if(photonView.isMine) {
            lifeTime += Time.deltaTime;
            if(lifeTime > 2f) {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    [PunRPC]
    public void Launch(int id) {
        ownerId = id;
        rb.velocity = transform.up * moveSpeed;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(photonView.isMine) {
            if(other.CompareTag("Player")) {
                if(other.GetComponent<PhotonView>().ownerId != ownerId) {
                    other.SendMessage("Hit", ownerId);
                    PhotonNetwork.Destroy(gameObject);
                }
            } else {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
