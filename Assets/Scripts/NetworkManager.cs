using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;                     //creating instance
    private void Awake()                                   
    {
        if(instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();                     //connecting using photon network settings
    }
    public void CreateRoom(string roomName)                        //function to create room
    {
        PhotonNetwork.CreateRoom(roomName);                       
    }
    public void JoinRoom(string roomName)                          //function to join room
    {
        PhotonNetwork.JoinRoom(roomName);                         
    }
    [PunRPC]
    public void StartGame(string level)                          // function to load level for all clients in game
    {
        PhotonNetwork.LoadLevel(level);
    }
}
