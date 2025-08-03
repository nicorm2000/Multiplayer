using UnityEngine;
using Net;

public class TowerTurns : MonoBehaviour
{
    [NetVariable(0, NETAUTHORITY.CLIENT)] public float targetYRotation = 0;
    [SerializeField] float duration;
    [SerializeField] Transform initialPositionShooting;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] PlayerController playerController;
    public bool isRunning = false;
    NetObj netObj = new(-1, -1);

    public void Shoot()
    {
        IPrefabService prefabService = ServiceProvider.GetService<IPrefabService>();
        int prefabID = prefabService.GetIdByPrefab(bulletPrefab);
        int ownerID = playerController.clientID;

        InstancePayload instancePayload = new InstancePayload(NetObjFactory.NetObjectsCount, ownerID, prefabID, initialPositionShooting.position.x, initialPositionShooting.position.y, initialPositionShooting.position.z,
                                     initialPositionShooting.rotation.x, initialPositionShooting.rotation.y, initialPositionShooting.rotation.z, initialPositionShooting.rotation.w,
                                     bulletPrefab.transform.localScale.x, bulletPrefab.transform.localScale.y, bulletPrefab.transform.localScale.z,
                                     -1);
        NetObjFactory.NetInstance(prefabID, initialPositionShooting.position.x, initialPositionShooting.position.y, initialPositionShooting.position.z,
                                     initialPositionShooting.rotation.x, initialPositionShooting.rotation.y, initialPositionShooting.rotation.z, initialPositionShooting.rotation.w,
                                     bulletPrefab.transform.localScale.x, bulletPrefab.transform.localScale.y, bulletPrefab.transform.localScale.z,
                                     -1, ownerID);

        GameObject prefab = prefabService.GetPrefabById(instancePayload.objectId);
        INetObj parentObj = NetObjFactory.GetINetObject(instancePayload.parentInstanceID);

        GameObject instance = MonoBehaviour.Instantiate(prefab, new Vector3(instancePayload.positionX, instancePayload.positionY, instancePayload.positionZ),
                                                               new Quaternion(instancePayload.rotationX, instancePayload.rotationY, instancePayload.rotationZ, instancePayload.rotationW));

        if (parentObj != null)
        {
            instance.transform.SetParent(((GameObject)(parentObj as object)).transform);
        }

        instance.transform.localScale = new Vector3(instancePayload.scaleX, instancePayload.scaleY, instancePayload.scaleZ);


        if (instance.TryGetComponent(out INetObj obj))
        {
            obj.GetNetObj().SetValues(instancePayload.instanceId, instancePayload.ownerId);

            NetObjFactory.AddINetObject(obj.GetID(), obj);
        }
        Debug.Log("Shoot bullet");
    }

    private void Update()
    {
        if (!isRunning && playerController != null && playerController.currentPlayer)
        {
            targetYRotation = playerController.cameraOrbit.transform.eulerAngles.y;
        }

        if (Mathf.Abs(transform.eulerAngles.y - targetYRotation) > 1f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, targetYRotation, 0),
                Time.deltaTime * 10f
            );
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
        }
    }
}