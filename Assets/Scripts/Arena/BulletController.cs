using UnityEngine;
using System.Collections;

public class BulletController:Photon.MonoBehaviour {
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float moveSpeed;

    [HideInInspector] public int ownerPlayerIndex = -1;

    public Collider2D col;
    private Rigidbody2D rb;

    public void Awake() {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update() {
        // Outside the screen? Destoy ourselves
        if(!NetworkGameManager.gameBounds.Contains(transform.position)) {
            Destroy(gameObject);
        }
    }

    public void Launch(int ownerIndex) {
        ownerPlayerIndex = ownerIndex;
        rb.velocity = transform.up * moveSpeed;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            other.SendMessage("Hit", ownerPlayerIndex);
        }
        Destroy(gameObject);
    }
}
