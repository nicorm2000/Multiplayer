using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float bulletSpeed = 10.0f;

    private int originPlayerID = -1;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector3 direction, int clientIdOrigin)
    {
        originPlayerID = clientIdOrigin;
        rb.velocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (NetworkManager.Instance.isServer)
        {
            if (collision.transform.TryGetComponent(out PlayerController pc))
            {
                if (pc.clientID != originPlayerID)
                {
                    GameManager.Instance.OnBulletHit?.Invoke(pc.clientID);
                }
            }
        }

        Destroy(gameObject);
    }
}