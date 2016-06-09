using UnityEngine;
using System.Collections;

public class TankController : Photon.MonoBehaviour {
    public GameObject explosionPrefab;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotSpeed;
    [SerializeField]
    private float snapDistance;
    [SerializeField]
    private float snapAngle;

    private Vector3 playerPosActual;
    private Quaternion playerRotActual;

    private Rigidbody2D rb;
    [HideInInspector] // Set by Spawn in NetworkPlayer
    public NetworkPlayer networkPlayer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

	
	// Update is called once per frame
	void Update () {
        if (!photonView.isMine) {
            //transform.position = Vector3.Lerp(transform.position, this.playerPosActual, Time.deltaTime * 5);
            //transform.rotation = Quaternion.Lerp(transform.rotation, this.playerRotActual, Time.deltaTime * 5);
            Move((Vector2)playerPosActual, playerRotActual.eulerAngles.z);
        }
        else { GetInput(); }
	}

    void GetInput() {

        Vector2 newPos = (Vector2)transform.position + (Vector2)transform.up * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        float newRot = transform.rotation.eulerAngles.z + (rotSpeed * -Input.GetAxisRaw("Horizontal") * Time.deltaTime);

        Move(newPos, newRot);

        if(Input.GetKeyDown(KeyCode.K)) {
            Die();
        }
    }

    void Move(Vector2 newPos, float newRot) {
        float lagDistance = Vector2.Distance(newPos, transform.position);
        float lagRotation = Mathf.Abs(((newRot + 360) % 360) - transform.rotation.eulerAngles.z);

        if (photonView.isMine) {
            rb.MovePosition(newPos);
            rb.MoveRotation(newRot);
        }
        else {
            transform.position = lagDistance > snapDistance ? 
                (Vector3)newPos :
                Vector3.Lerp(transform.position, this.playerPosActual, Time.deltaTime * 5);

            transform.rotation = lagRotation > snapAngle ? 
                Quaternion.Euler(0, 0, newRot) : 
                Quaternion.Lerp(transform.rotation, this.playerRotActual, Time.deltaTime * 5);
        }
    }

    void Die() {
        // RPC that we were destroyed
        PhotonNetwork.RPC(photonView, "TankDestroyed", PhotonTargets.All, false, photonView.ownerId, transform.position);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else {
            // Network player, receive data
            this.playerPosActual = (Vector3)stream.ReceiveNext();
            this.playerRotActual = (Quaternion)stream.ReceiveNext();
        }
    }

    // ******************** RPC Calls ********************

    [PunRPC]
    void TankDestroyed(int playerID, Vector3 pos) {
        Debug.Log("TankDestroyed - " + playerID);
        if(photonView.isMine) {
            // We died! Destroy ourselves and tell our network player
            PhotonNetwork.Destroy(gameObject);
            networkPlayer.TankWasDestroyed();
        }
        // Either we, or someone else died. Either way, spawn an explosion at its position
        Instantiate(explosionPrefab, pos, Quaternion.identity);
    }
}
