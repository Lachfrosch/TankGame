using UnityEngine;
using System.Collections;
using Unity.Netcode;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : NetworkBehaviour
{
    public bool OnlyDeactivate;

    void OnEnable()
    {
        StartCoroutine(nameof(CheckIfAlive));
    }

    IEnumerator CheckIfAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!GetComponent<ParticleSystem>().IsAlive(true))
            {
                if (OnlyDeactivate)
                {
                    this.gameObject.SetActive(false);
                }
                else
                    //IMPORTANT --> Added by Lachfrosch in order to make Effects work with Netcode!
                    if (IsServer)
                {

                    gameObject.GetComponent<NetworkObject>().Despawn();
                    GameObject.Destroy(this.gameObject);
                }
                break;
            }
        }
    }
}
