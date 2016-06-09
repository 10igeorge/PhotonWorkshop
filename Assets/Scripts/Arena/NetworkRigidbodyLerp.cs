using UnityEngine;
using System.Collections;

public class LerpRigidbody:Photon.MonoBehaviour {
    // Network snapping
    protected float snapDistance = 1.5f;
    protected float snapAngle = 30f;
    protected float lerpSpeed = 20f;

    protected Collider2D col;
    protected Rigidbody2D rb;
    private Vector3 playerPosActual;
    private Quaternion playerRotActual;

    public virtual void Awake() {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Update() {
        if(photonView.isMine) {
            SelfMovement();
        } else {
            Move((Vector2)playerPosActual, playerRotActual);
        }
    }

    protected virtual void SelfMovement() {
        // Override this and call Move()
    }

    protected void Move(Vector2 newPos, Quaternion newRot) {
        float lagDistance = Vector2.Distance(newPos, transform.position);
        float lagRotation = Quaternion.Angle(newRot, transform.rotation);

        if(photonView.isMine) {
            rb.MovePosition(newPos);
            rb.MoveRotation(newRot.eulerAngles.z);
        } else {
            rb.MovePosition(lagDistance > snapDistance ? (Vector3)newPos : Vector3.Lerp(transform.position, newPos, Time.deltaTime * lerpSpeed));
            rb.MoveRotation(lagRotation > snapAngle ? newRot.eulerAngles.z : Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * lerpSpeed).eulerAngles.z);
        }
    }

    protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.isWriting) {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } else {
            // Network player, receive data
            playerPosActual = (Vector3)stream.ReceiveNext();
            playerRotActual = (Quaternion)stream.ReceiveNext();
        }
    }
}
