using Net;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField, NetVariable(0)] float velocityY;
    [SerializeField, NetVariable(1)] float velocityX;

    PlayerController playerController;
    Rigidbody RB;

    void Awake()
    {
        RB = GetComponent<Rigidbody>();
        playerController = GetComponentInParent<PlayerController>();
    }

    void FixedUpdate()
    {
        if (playerController.currentPlayer)
        {
            Vector3 movement = Vector3.zero;
            float rotation = 0;

            if (Input.GetKey(KeyCode.W))
            {
                movement += transform.forward * velocityY;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movement -= transform.forward * velocityY;
            }

            if (Input.GetKey(KeyCode.A))
            {
                rotation -= velocityX;
            }

            if (Input.GetKey(KeyCode.D))
            {
                rotation += velocityX;
            }

            // Send movement to server for validation
            SendMovementToServer(movement, rotation);
        }
    }

    private void SendMovementToServer(Vector3 movement, float rotation)
    {
        //if (NetworkManager.Instance.isServer)
        //{
        //    // Server can apply movement directly
        //    RB.AddForce(movement, ForceMode.Force);
        //    transform.eulerAngles += Vector3.up * rotation * Time.deltaTime;
        //}
        //else
        //{
        //    // Client sends movement request to server
        //    NetVector3 netMovement = new(MessagePriority.Default, new Vec3(playerController.clientID, movement));
        //    netMovement.CurrentMessageType = MessageType.Position;
        //    NetworkManager.Instance.networkEntity.SendMessage(netMovement.Serialize());
        //}
    }
}