using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public PhotonView view;
    private int player1Score;
    private int player2Score;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        UpdatePlayerScore();
    }
    [PunRPC]
    public void UpdatePlayerScore()
    {
        player1Score = GameManager.instance.currentPlayer[0].totalPoint;
        player2Score = GameManager.instance.currentPlayer[1].totalPoint;
        player1ScoreText.text = PhotonNetwork.PlayerList[0].NickName + " Score : " + player1Score.ToString();
        player2ScoreText.text = PhotonNetwork.PlayerList[1].NickName + " Score : " + player2Score.ToString();
    }
}
