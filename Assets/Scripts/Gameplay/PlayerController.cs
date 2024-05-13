using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int health = 3;

    [SerializeField] float speed = 5f;
    [SerializeField] float cooldownShoot = 0.2f;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] bool canShoot = true;
    CharacterController cc;

    public bool currentPlayer = false;
    public int clientID = -1;

    private GameManager gm;
    private NetworkManager nm;

    private void Awake()
    {
        cc = transform.GetComponent<CharacterController>();
    }

    private void Start()
    {
        gm = GameManager.Instance;
        nm = NetworkManager.Instance;
    }

    private void Update()
    {
        if (!nm.isServer && currentPlayer)
        {
            Movement();
            Shoot();
        }
    }

    public void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = speed * Time.deltaTime * new Vector3(horizontalInput, verticalInput, 0.0f);
        cc.Move(movement);
        SendPosition();
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 mousePosition = hit.point;
                mousePosition.z = 0f; // Z value is the same for both players due to 3D
                Vector3 direction = mousePosition - transform.position;
                direction.Normalize();

                GameObject bullet = Instantiate(bulletPrefab, transform.position + direction, Quaternion.identity);
                bullet.GetComponent<BulletController>().SetDirection(direction, clientID);

                NetVector3 netBullet = new NetVector3((nm.actualClientId, direction));
                netBullet.SetMessageType(MessageType.BulletInstatiate);
                nm.SendToServer(netBullet.Serialize());

                canShoot = false;
                Invoke(nameof(SetCanShoot), cooldownShoot);
            }
        }
    }

    private void SendPosition()
    {
        NetVector3 netVector3 = new NetVector3((nm.actualClientId, transform.position));
        NetworkManager.Instance.SendToServer(netVector3.Serialize());
    }

    private void SetCanShoot()
    {
        canShoot = true;
    }

    public void ServerShoot(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + direction, Quaternion.identity);
        bullet.GetComponent<BulletController>().SetDirection(direction, clientID);
    }

    public void OnReciveDamage() // Server is the only one using this method
    {
        health--;

        if (health <= 0)
        {
            NetIDMessage netDisconnection = new (clientID);
            nm.Broadcast(netDisconnection.Serialize());
            nm.RemoveClient(clientID);
        }
    }
}