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
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject followCamera;
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private GameObject playerCrossHair;
    [SerializeField] private Camera cam;
    [SerializeField] private Joystick horizontalJoystick;
    [SerializeField] private Button shootButton;
    
    public int totalPoint = 0;
    private float speed;
    private float launchForce;
    private Vector3 direction;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        speed = 4;
        launchForce = 30f;
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
                HandleSidewaysMovement();
                HandleRotation();
            }
        }
    }
    void HandleRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        float mouseX = Input.GetAxis("Mouse X");
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
        if (canRotate)
        {
            transform.Rotate(0, Input.GetAxis("Horizontal") * 100 * Time.deltaTime, 0);
        }
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
    }
    public void HoldShootButton()
    {
        canRotate = true;
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
        followCamera.gameObject.SetActive(true);
        Destroy(arrow, 5f);
    }

    [PunRPC]
    public void AddPoints(int point)
    {
        totalPoint += point;
    }
}
