using UnityEngine;
using System.Collections;

public class TankController:LerpRigidbody {
    public GameObject explosionPrefab;
    public GameObject bulletPrefab;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float bulletFireDelay;

    // Firing
    private float bulletFireTimer = 0f;

    [PunRPC] public void SetColor(int playerIndex) {
        SpriteRenderer sp = GetComponentInChildren<SpriteRenderer>();
        sp.color = NetworkGameManager.I.playerColors[playerIndex];
    }

    public override void Update() {
        // Movement
        base.Update();
        // Other Input
        if(photonView.isMine) {
            // Firing timer
            if(bulletFireTimer > 0f) {
                bulletFireTimer -= Time.deltaTime;
            }
            // Input
            GetInput();
        }
    }

    private void GetInput() {
        if(Input.GetKey(KeyCode.Space) && bulletFireTimer <= 0f) {
            bulletFireTimer = bulletFireDelay;
            Shoot();
        }
        if(Input.GetKeyDown(KeyCode.K)) {
            Die(NetworkGameManager.I.playerIndex);
        }
    }

    protected override void SelfMovement() {
        Vector2 newPos = (Vector2)transform.position + (Vector2)transform.up * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        Quaternion newRot = transform.rotation * Quaternion.Euler(0f, 0f, rotSpeed * -Input.GetAxisRaw("Horizontal") * Time.deltaTime);
        if(NetworkGameManager.gameBounds.Contains(newPos)) {
            Move(newPos, newRot);
        }
    }

    void Shoot() {
        // Shoot a bullet
        GameObject bulletGo = PhotonNetwork.Instantiate(bulletPrefab.name, transform.position + transform.up * 0.5f, transform.rotation, 0);
        BulletController bc = bulletGo.GetComponent<BulletController>();
        Physics2D.IgnoreCollision(bc.col, col);
        bc.photonView.RPC("Launch", PhotonTargets.All, NetworkGameManager.I.playerIndex);
    }

    void Hit(int sourcePlayerIndex) {
        Die(sourcePlayerIndex);
    }

    void Die(int sourcePlayerIndex) {
        // RPC that we were destroyed
        PhotonNetwork.RPC(photonView, "TankDestroyed", PhotonTargets.All, false, sourcePlayerIndex);
    }

    // ******************** RPC Calls ********************

    [PunRPC] void TankDestroyed(int sourcePlayerIndex) {
        if(photonView.isMine) {
            // We died! Destroy ourselves and tell the game manager
            PhotonNetwork.Destroy(gameObject);
            NetworkGameManager.I.TankWasDestroyed();
            // Tell everyone else that the person who killed us scored a point
            PlayerUI.I.PlayerScored(sourcePlayerIndex);
        }
        // Either we, or someone else died. Either way, spawn an explosion at its position
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

    protected override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        base.OnPhotonSerializeView(stream, info);
    }
}
