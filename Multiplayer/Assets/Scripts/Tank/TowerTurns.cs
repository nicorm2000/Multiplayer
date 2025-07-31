using System.Collections;
using UnityEngine;
using Net;

[NetTRS(NetTRS.SYNC.DEFAULT, NETAUTHORITY.CLIENT)]
public class TowerTurns : MonoBehaviour, INetObj
{
    [SerializeField] float duration;
    [SerializeField] Transform initialPositionShooting;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] PlayerController playerController;
    public bool isRunning = false;
    NetObj netObj = new(-1, -1);

    void Shoot()
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

    public IEnumerator TurnTower(Transform cam)
    {
        isRunning = true;
        float timer = 0;

        Quaternion initialRotation = transform.rotation;
        Quaternion newRotation = cam.transform.rotation;
        newRotation.x = 0;
        newRotation.z = 0;

        while (timer <= duration)
        {
            float interpolationValue = timer / duration;

            transform.rotation = Quaternion.Lerp(initialRotation, newRotation, interpolationValue);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.rotation = newRotation;
#if SERVER
        Shoot();
#endif
        isRunning = false;
    }

    public int GetID()
    {
        return netObj.ID;
    }

    public int GetOwnerID()
    {
        return netObj.OwnerId;
    }

    public NetObj GetNetObj()
    {
        return netObj;
    }

    public TRS GetTRS()
    {
        return transform.TranslateTRS();
    }

    public void SetTRS(TRS trs, NetTRS.SYNC sync)
    {
        transform?.FromTRS(trs, sync);
    }
}