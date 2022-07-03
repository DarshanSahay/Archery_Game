using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public int playerId;
    public Player photonPlayer;
    public PhotonView view;
    private bool canRotate = false;

    [SerializeField] private Animator anim;
    [SerializeField] private CharacterController player;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform playerSpine;
    [SerializeField] private Transform playerRotation;
    [SerializeField] private Transform camRotation;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject followCamera;
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject playerCrossHair;
    [SerializeField] private Camera cam;
    [SerializeField] private Joystick horizontalJoystick;
    [SerializeField] private Button shootButton;
    public int totalPoint;


    private float speed;
    private float launchForce;
    private float mouseSensitivity;
    private Vector3 direction;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        speed = 4;
        launchForce = 30f;
        mouseSensitivity = 50f;
        playerRotation.rotation = transform.rotation;
        camRotation.rotation = followCamera.transform.rotation;
    }
    void Update()
    {
        LocalPlayer();
    }
    void LocalPlayer()
    {
        if (view.IsMine)
        {
            if (GameManager.instance.canShoot == true)
            {
                cam.gameObject.SetActive(true);
                followCamera.gameObject.SetActive(true);
                horizontalJoystick.gameObject.SetActive(true);
                shootButton.gameObject.SetActive(true);
                eventSystem.gameObject.SetActive(true);
                HandleSidewaysMovement();
                HandleRotation();
            }
        }
    }
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        if (canRotate)
        {
            Mathf.Clamp(mouseX, -15, 15);
            transform.Rotate(0, mouseX  * Time.deltaTime, 0);
        }
    }
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        playerId = player.ActorNumber;
        GameManager.instance.currentPlayer[playerId - 1] = this;
    }
    void HandleSidewaysMovement()
    {
        float vertical = horizontalJoystick.Horizontal;
        direction = new Vector3(0f, 0f, vertical).normalized;
        player.Move(new Vector3(0, 0, direction.z * Time.deltaTime) * speed);

        if(direction != Vector3.zero)
        {
            anim.SetBool("canMove", true);
        }
        else
        {
            anim.SetBool("canMove", false);
        }
        anim.SetFloat("MoveSideways", vertical);
    }
    public void EnableShoot()
    {
        anim.SetBool("canShoot", true);
        anim.SetBool("hasShoot", false);
        aimCamera.gameObject.SetActive(true);
        playerCrossHair.gameObject.SetActive(true);
        canRotate = true;
    }
    public void HoldShootButton()
    {
        anim.SetBool("hasShoot", true);
        anim.SetBool("canShoot", false);
        aimCamera.gameObject.SetActive(false);
        playerCrossHair.gameObject.SetActive(false);
    }
    public void InstantiateArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        arrow.GetComponentInChildren<Rigidbody>().velocity = arrow.transform.up * launchForce;
        arrow.transform.rotation = Quaternion.LookRotation(arrow.GetComponent<Rigidbody>().velocity);
        GameManager.instance.canShoot = false;
        canRotate = false;
        transform.rotation = Quaternion.Lerp(transform.rotation, playerRotation.rotation,Time.time * 0.05f);
        followCamera.transform.rotation = Quaternion.Lerp(followCamera.transform.rotation, camRotation.rotation, Time.time * 0.05f);
        followCamera.gameObject.SetActive(true);
        Destroy(arrow, 5f);
    }

    public void AddPoints(int point,Player currentPlayer)
    {
        totalPoint = (int)currentPlayer.CustomProperties["PlayerScore"];
        totalPoint += point;
        ExitGames.Client.Photon.Hashtable updatedScore = new ExitGames.Client.Photon.Hashtable();
        updatedScore["PlayerScore"] = totalPoint;
        currentPlayer.SetCustomProperties(updatedScore);
    }
}
