using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public PlayerController[] currentPlayer;
    public PhotonView view;
    public bool canShoot = false;

    private int playersInGame;
    private bool hasGameEnded = false;
    private bool playerTurnEnded = false;
    private bool hasTurnStarted = false;
    private float maxTurnTime = 20f;
    private float currentTurnTime;
    private float delayInTurns = 5f;
    private int currentTurn = 0;

    [SerializeField] private string playerPrefabLocation;
    [SerializeField] private Transform spawnPosition1;
    [SerializeField] private Transform spawnPosition2;
    [SerializeField] private TextMeshProUGUI turnTimeText;
    [SerializeField] private TextMeshProUGUI turnNoText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI gameloadingText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentPlayer = new PlayerController[PhotonNetwork.PlayerList.Length];      //getting all the players on intitialization
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);                          //syncing players
        currentTurnTime = maxTurnTime;
        currentTurn = 0;
    }
    [PunRPC]
    void ImInGame()                                                                  
    {
        playersInGame++;
        
        if (playersInGame == PhotonNetwork.PlayerList.Length)               // when all the players are in the scene - spawn the players
        {
            SpawnPlayer();
            StartTurn();
        }
    }
    [PunRPC]
    void SpawnPlayer()                            //function to be called on all clients to instantiate playerprefabs based on spawnpositions
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPosition1.position, Quaternion.identity, 0);
            PlayerController playerScript = playerObj.GetComponent<PlayerController>();
            playerScript.view.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
        else
        {
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPosition2.position, Quaternion.identity, 0);
            PlayerController playerScript = playerObj.GetComponent<PlayerController>();
            playerScript.view.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
    }
    void StartTurn()                             //after player instantiates in game scene , start their turns
    {
        hasTurnStarted = true;
        canShoot = true;
        playerTurnEnded = false;
    }
    void GameLoop()                              //function to loop turns  
    {
        int timetoint;
        timetoint = (int)currentTurnTime;
        turnTimeText.text = "Time remaining : " + timetoint.ToString();
        turnNoText.text = "Turn No. : " + currentTurn.ToString();
        if (playerTurnEnded == false && hasGameEnded == false)
        {
            delayInTurns -= Time.deltaTime;
            if (delayInTurns <= 0)
            {
                if (hasTurnStarted)
                {
                    currentTurnTime -= Time.deltaTime;
                    if (currentTurnTime <= 0)
                    {
                        currentTurnTime = maxTurnTime;
                        currentTurn++;
                        delayInTurns = 5f;
                        playerTurnEnded = true;
                        hasTurnStarted = false;
                        canShoot = false;
                        StartTurn();
                    }
                }
            }
        }
    }
    void LoopEnd()                               //function to declare winner on UI
    {
        if (currentTurn == 3)
        {
            hasGameEnded = true;
            if (hasGameEnded)
            {
                if (currentPlayer[0].totalPoint == currentPlayer[1].totalPoint)
                {
                    winnerText.text = "Game Is a Draw";
                }
                if (currentPlayer[0].totalPoint > currentPlayer[1].totalPoint)
                {
                    winnerText.text = PhotonNetwork.PlayerList[0].NickName + " Is Winner";
                }
                else if(currentPlayer[0].totalPoint < currentPlayer[1].totalPoint)
                {
                    winnerText.text = PhotonNetwork.PlayerList[1].NickName + " Is Winner";
                }
                Invoke("GameEnd", 10f);                        //after 10 seconds return to lobby
                gameloadingText.text = "Loading Lobby....";
            }
        }
    }
    private void Update()
    {
        GameLoop();
        LoopEnd();
    }
    void GameEnd()
    {
        NetworkManager.instance.photonView.RPC("StartGame", RpcTarget.All, "Lobby");  //sending all clients to lobby
    }
}
