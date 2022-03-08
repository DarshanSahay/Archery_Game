using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnTargetHit : MonoBehaviour
{
    [SerializeField] private int onHitPoint;
    private void OnCollisionEnter(Collision collision)           //checking if anything collides with the target
    {
        if(collision != null)
        {
            collision.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            PlayerController.instance.view.RPC("AddPoints",RpcTarget.All, onHitPoint);        // to sync points to all clients
        }
    }
}
