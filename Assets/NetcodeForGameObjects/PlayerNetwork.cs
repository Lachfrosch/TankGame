using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        Vector3 moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            moveDir.z = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.z = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = +1f;
        }

        TestServerRpc(moveDir, Time.deltaTime);
    }

    [ServerRpc]
    private void TestServerRpc(Vector3 moveDir, float time)
    {
        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * time;
    }
}
