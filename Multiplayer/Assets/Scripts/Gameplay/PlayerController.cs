using UnityEngine;
using Net;

public class PlayerController : MonoBehaviour
{
    public int health = 3;

    [SerializeField] float speed = 5f;
    [SerializeField] GameObject bulletPrefab;
    float cooldownShoot = 0.5f;

    [SerializeField] bool canShoot = true;
    CharacterController cc;

    public bool currentPlayer = false;
    public int clientID = -1;

    GameManager gm;
    NetworkManager nm;

    AudioSource audioSource;
    Animator animator;

    static int positionMessageOrder = 1;
    static int bulletsMessageOrder = 1;

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

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0.0f) * speed * Time.deltaTime;

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
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 mousePosition = hit.point;
                mousePosition.z = 0f;
                Vector3 direction = mousePosition - transform.position;
                direction.Normalize();

                GameObject bullet = Instantiate(bulletPrefab, transform.position + direction, Quaternion.identity);
                bullet.GetComponent<BulletController>().SetDirection(direction, clientID);

                NetVector3 netBullet = new NetVector3(MessagePriority.NonDisposable, (nm.ClientID, new Vec3(direction.x, direction.y, direction.z)));
                netBullet.CurrentMessageType = MessageType.BulletInstatiate;
                netBullet.MessageOrder = bulletsMessageOrder;
                nm.GetNetworkClient().SendToServer(netBullet.Serialize());
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
        NetVector3 netVector3 = new NetVector3(MessagePriority.Sorteable, (nm.ClientID, new Vec3(transform.position.x, transform.position.y, transform.position.z)));
        netVector3.MessageOrder = positionMessageOrder;
        NetworkManager.Instance.GetNetworkClient().SendToServer(netVector3.Serialize());
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
            NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
            nm.networkEntity.SendMessage(netDisconnection.Serialize());
            nm.networkEntity.RemoveClient(clientID);
        }
    }
}