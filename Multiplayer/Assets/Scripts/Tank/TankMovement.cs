using UnityEngine;
using Net;

public class TankMovement : MonoBehaviour
{
    [SerializeField, NetVariable(0)] float velocityY;
    [SerializeField, NetVariable(1)] float velocityX;
    [NetVariable(2)] Vector2 movementInput;

    PlayerController playerController;
    Rigidbody RB;

    void Awake()
    {
        RB = GetComponent<Rigidbody>();
        playerController ??= GetComponentInParent<PlayerController>();
    }

    void FixedUpdate()
    {
        if (playerController.currentPlayer)
        {
            if (Input.GetKey(KeyCode.W))
            {
                movementInput.y = 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movementInput.y = -1;
            }

            if (Input.GetKey(KeyCode.A))
            {
                movementInput.x = 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                movementInput.x = -1;
            }
        }
        else
        {
            TankMovementInputChecker();
        }
    }

    public void TankMovementInputChecker()
    {
        if (movementInput.y == 1)
        {
            RB.AddForce(transform.forward * velocityY, ForceMode.Force);
        }

        if (movementInput.y == -1)
        {
            RB.AddForce(-transform.forward * velocityY);
        }

        if (movementInput.x == 1)
        {
            transform.eulerAngles += Vector3.down * velocityX * Time.deltaTime;
        }

        if (movementInput.x == -1)
        {
            transform.eulerAngles += Vector3.up * velocityX * Time.deltaTime;
        }
    }
}