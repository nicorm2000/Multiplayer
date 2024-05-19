using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float bulletSpeed = 10.0f;

    private int originPlayerID = -1;

    private Rigidbody rb;

    /// <summary>
    /// Initializes the Rigidbody component.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Sets the direction and speed of the bullet, and records the ID of the player who fired it.
    /// </summary>
    /// <param name="direction">The direction in which the bullet should move.</param>
    /// <param name="clientIdOrigin">The ID of the player who fired the bullet.</param>
    public void SetDirection(Vector3 direction, int clientIdOrigin)
    {
        originPlayerID = clientIdOrigin;
        rb.velocity = direction * bulletSpeed;
    }

    /// <summary>
    /// Handles collision events for the bullet. If the bullet collides with a player,
    /// it triggers the OnBulletHit action in the GameManager.
    /// </summary>
    /// <param name="collision">The collision data associated with this collision event.</param>
    private void OnCollisionEnter(Collision collision)
    {
        // Only the server handles collision logic
        if (NetworkManager.Instance.isServer)
        {
            if (collision.transform.TryGetComponent(out PlayerController pc))
            {
                // If the player hit is not the one who fired the bullet
                if (pc.clientID != originPlayerID)
                {
                    GameManager.Instance.OnBulletHit?.Invoke(pc.clientID);
                }
            }
        }
        Destroy(gameObject);
    }
}