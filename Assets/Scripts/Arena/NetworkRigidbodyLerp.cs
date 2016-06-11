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
        SelfMovement();
    }

    protected virtual void SelfMovement() {
        // Override this and call Move()
    }

    protected void Move(Vector2 newPos, Quaternion newRot) {
        rb.MovePosition(newPos);
        rb.MoveRotation(newRot.eulerAngles.z);
    }

    protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }
}
