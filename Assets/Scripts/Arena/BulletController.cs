using UnityEngine;
using System.Collections;

public class BulletController:LerpRigidbody {
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float moveSpeed;

    [HideInInspector]
    public int ownerId;

    public void Launch(int id, Collider2D ownerCol) {
        Physics2D.IgnoreCollision(ownerCol, col); // Ignore collision with the tank that fired us
        ownerId = id;
        rb.velocity = transform.up * moveSpeed;
    }

    protected override void SelfMovement() {
        // Nothing to do here, rigidbody velocity takes care of it
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(photonView.isMine) {
            if(other.CompareTag("Player")) {
                other.SendMessage("Hit", ownerId);
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }

    protected override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        base.OnPhotonSerializeView(stream, info);
    }
}
