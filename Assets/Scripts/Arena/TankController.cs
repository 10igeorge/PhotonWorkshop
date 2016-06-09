using UnityEngine;
using System.Collections;

public class TankController:LerpRigidbody {
    public GameObject explosionPrefab;
    public GameObject bulletPrefab;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;

    // Set by Spawn in NetworkPlayer
    [HideInInspector] public NetworkPlayer networkPlayer;

    public override void Update() {
        // Movement
        base.Update();
        // Other Input
        if(photonView.isMine) {
            GetInput();
        }
    }

    private void GetInput() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Shoot();
        }
        if(Input.GetKeyDown(KeyCode.K)) {
            Die();
        }
    }

    protected override void SelfMovement() {
        Vector2 newPos = (Vector2)transform.position + (Vector2)transform.up * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        Quaternion newRot = transform.rotation * Quaternion.Euler(0f, 0f, rotSpeed * -Input.GetAxisRaw("Horizontal") * Time.deltaTime);
        Move(newPos, newRot);
    }

    void Shoot() {
        // Shoot a bullet
        GameObject bulletGo = PhotonNetwork.Instantiate(bulletPrefab.name, transform.position + transform.up * 0.5f, transform.rotation, 0);
        BulletController bc = bulletGo.GetComponent<BulletController>();
        bc.Launch(photonView.ownerId, col);
    }

    void Hit(int sourcePlayerId) {
        Die();
    }

    void Die() {
        // RPC that we were destroyed
        PhotonNetwork.RPC(photonView, "TankDestroyed", PhotonTargets.All, false, photonView.ownerId);
    }

    // ******************** RPC Calls ********************

    [PunRPC] void TankDestroyed(int playerID) {
        Debug.Log("TankDestroyed - " + playerID);
        if(photonView.isMine) {
            // We died! Destroy ourselves and tell our network player
            PhotonNetwork.Destroy(gameObject);
            networkPlayer.TankWasDestroyed();
        }
        // Either we, or someone else died. Either way, spawn an explosion at its position
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

    protected override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        base.OnPhotonSerializeView(stream, info);
    }
}
