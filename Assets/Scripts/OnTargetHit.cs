using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnTargetHit : MonoBehaviour
{
    [SerializeField] private int onHitPoint;
    public ScoreManager score;

    private void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)           //checking if anything collides with the target
    {
        if(collision != null)
        {
            collision.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            PlayerController.instance.AddPoints(onHitPoint, PhotonNetwork.LocalPlayer);
            score.view.RPC("UpdatePlayerScore",RpcTarget.All);
        }
    }
}
