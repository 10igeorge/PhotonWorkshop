using UnityEngine;
using System.Collections;

public class TankController : Photon.MonoBehaviour {

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotSpeed;

    private Vector3 playerPosActual;
    private Quaternion playerRotActual;

    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

	
	// Update is called once per frame
	void Update () {
        //if (!photonView.isMine) {
        //    transform.position = Vector3.Lerp(transform.position, this.playerPosActual, Time.deltaTime * 5);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, this.playerRotActual, Time.deltaTime * 5);
        //}
        //else { GetInput(); }
        GetInput();
	}

    void GetInput() {
        rb.MovePosition((Vector2)transform.position + (Vector2)transform.up * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical"));
        
        rb.MoveRotation(transform.rotation.eulerAngles.z + (rotSpeed * -Input.GetAxisRaw("Horizontal") * Time.deltaTime));
        
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
}
