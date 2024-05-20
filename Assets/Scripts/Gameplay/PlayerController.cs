using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject bulletPrefab;

    private bool canShoot = true;
    private readonly float cooldownShoot = 0.5f;

    private CharacterController cc;

    public int health = 3;
    public bool currentPlayer = false;
    public int clientID = -1;

    private GameManager gm;
    private NetworkManager nm;

    private AudioSource audioSource;
    private Animator animator;

    private static int positionMessageOrder = 1;
    private static int bulletsMessageOrder = 1;

    /// <summary>
    /// Initializes references to components on the GameObject.
    /// </summary>
    private void Awake()
    {
        cc = transform.GetComponent<CharacterController>();
        audioSource = gameObject.GetComponent<AudioSource>();
        animator = gameObject.GetComponent<Animator>();
    }

    /// <summary>
    /// Sets up references to GameManager and NetworkManager instances.
    /// </summary>
    private void Start()
    {
        gm = GameManager.Instance;
        nm = NetworkManager.Instance;
    }

    /// <summary>
    /// Handles player movement and shooting if the player is the current player and not the server.
    /// </summary>
    private void Update()
    {
        if (!nm.isServer && currentPlayer)
        {
            Movement();
            Shoot();
        }
    }

    /// <summary>
    /// Handles player movement and shooting if the player is the current player and not the server.
    /// </summary>
    public void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = speed * Time.deltaTime * new Vector3(horizontalInput, verticalInput, 0.0f);

        cc.Move(movement);

        SendPosition();
    }

    /// <summary>
    /// Handles player shooting based on mouse input.
    /// </summary>
    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 mousePosition = hit.point;
                mousePosition.z = 0f; // Ensuring the bullet stays on the same Z plane
                Vector3 direction = mousePosition - transform.position;
                direction.Normalize();

                GameObject bullet = Instantiate(bulletPrefab, transform.position + direction, Quaternion.identity);
                bullet.GetComponent<BulletController>().SetDirection(direction, clientID);

                NetVector3 netBullet = new (MessagePriority.NonDisposable, (nm.actualClientId, direction))
                {
                    CurrentMessageType = MessageType.BulletInstatiate,
                    MessageOrder = bulletsMessageOrder
                };
                nm.SendToServer(netBullet.Serialize());
                bulletsMessageOrder++;

                animator.SetTrigger("Shoot");
                audioSource.Play();

                canShoot = false;
                Invoke(nameof(SetCanShoot), cooldownShoot);
            }
        }
    }

    /// <summary>
    /// Sends the player's position to the server.
    /// </summary>
    private void SendPosition()
    {
        NetVector3 netVector3 = new (MessagePriority.Sortable, (nm.actualClientId, transform.position))
        {
            MessageOrder = positionMessageOrder
        };
        NetworkManager.Instance.SendToServer(netVector3.Serialize());
        positionMessageOrder++;
    }

    /// <summary>
    /// Resets the canShoot flag to allow shooting again after a cooldown.
    /// </summary>
    private void SetCanShoot()
    {
        canShoot = true;
    }

    /// <summary>
    /// Instantiates a bullet and sets its direction, used by the server.
    /// </summary>
    /// <param name="direction">The direction to shoot the bullet.</param>
    public void ServerShoot(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + direction, Quaternion.identity);
        bullet.GetComponent<BulletController>().SetDirection(direction, clientID);
    }

    /// <summary>
    /// Reduces player health and handles player death.
    /// </summary>
    public void OnReceiveDamage() // Server is the only one using this method
    {
        health--;

        if (health <= 0)
        {
            NetIDMessage netDisconnection = new (MessagePriority.Default, clientID);
            nm.Broadcast(netDisconnection.Serialize());
            nm.RemoveClient(clientID);
        }
    }
}