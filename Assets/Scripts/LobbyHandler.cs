using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject lobbyScreen;

    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;

    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private Button startGameButton;

    void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }
    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }
    void SetScreen(GameObject screen)
    {
        mainScreen.gameObject.SetActive(false);            // deactivate all screens
        lobbyScreen.gameObject.SetActive(false);

        screen.gameObject.SetActive(true);            // enable the requested screen
    }
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);              //Creating room with room name
    }
    public void OnJoinRoomButton(TMP_InputField roomNameInput)              //joining the room with name
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)          //updating player names
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }
    public override void OnJoinedRoom()                                     //updating players on room joined
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
        ExitGames.Client.Photon.Hashtable playerProperty = new ExitGames.Client.Photon.Hashtable();
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
        playerProperty["PlayerScore"] = 0;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)            // display all the players currently in the lobby
        {
            playerListText.text += player.NickName + "\n";
        }
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 2)   // only the host can start the game when the there are two players
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)        //updating lobby after a player leaves the room
    {
        UpdateLobbyUI();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }
    public void OnLeaveLobbyButton()                              
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
    public void OnStartGameButton()                              //loading the level for all clients
    {
        NetworkManager.instance.photonView.RPC("StartGame", RpcTarget.All, "Playground");
    }
}

