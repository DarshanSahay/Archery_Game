using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager instance;
    public PhotonView view;
    [SerializeField] private TextMeshProUGUI player1ScoreText;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        
    }
    [PunRPC]
    public void UpdatePlayerScore()
    {
        player1ScoreText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player1ScoreText.text += player.NickName + " : " + player.CustomProperties["PlayerScore"] + "\n";
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        player1ScoreText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player1ScoreText.text += player.NickName + " : " + player.CustomProperties["PlayerScore"] + "\n";
        }
    }

}
