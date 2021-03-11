using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Singleton
    public static PlayerController Player;
    #endregion

    public float speed = 6.0F;
    public float runMultiplier = 0.5f;
    public float jumpHeight = 5f;
    public float gravity = 1.0F;
    public int maxCoyote = 5;

    [Header("Player Model")]
    public Transform playerModel;

    CharacterController character;
    Transform cam;
    Vector3 moveDirection = Vector3.zero;
    bool groundedPlayer;
    int coyoteTimer;
    bool hasJumped = false;

    private void Awake()
    {
        Player = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        character = this.GetComponent<CharacterController>();
        cam = TreasureTrackerCamera.mainCamera.cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        groundedPlayer = character.isGrounded;
        if (groundedPlayer)
        {
            coyoteTimer = 0;
            hasJumped = false;

            if(moveDirection.y < 0)
            {
                moveDirection.y = 0f;

            }
        }

        
        Vector3 move = Quaternion.Euler(0, cam.eulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, 1.0f);        //normalise diagonal movement
        
        move *= 1 + Input.GetAxis("Run") * (runMultiplier - 1); //apply dynamic run speed
        
        playerModel.LookAt(new Vector3(playerModel.position.x + move.x, playerModel.position.y, playerModel.position.z + move.z));
        character.Move(move * Time.deltaTime * speed);

        if (Input.GetButtonDown("Jump") && coyoteTimer <= maxCoyote && !hasJumped)
        {
            moveDirection.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            hasJumped = true;
        }

        moveDirection.y += gravity * Time.deltaTime;
        character.Move(moveDirection * Time.deltaTime);
        coyoteTimer += 1;

        //if (character.isGrounded)
        //{
        //    moveDirection = Quaternion.Euler(0, cam.eulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //    //playerAnimator.SetFloat("Dir X", Input.GetAxis("Horizontal"));
        //    //playerAnimator.SetFloat("Dir Y", Input.GetAxis("Vertical"));
        //    //if (moveDirection != Vector3.zero)
        //    //{
        //    //    playerAnimator.SetBool("walking", true);
        //    //    //print("walking");
        //    //}
        //    //else
        //    //{
        //    //    playerAnimator.SetBool("walking", false);
        //    //}
        //    //print((int)moveDirection.normalized.x + ", " + (int)moveDirection.normalized.z);   
        //    moveDirection = Vector3.ClampMagnitude(moveDirection, 1.0f);
        //    if (Input.GetButton("Run"))
        //    {
        //        moveDirection *= runSpeed;
        //    }
        //    moveDirection = transform.TransformDirection(moveDirection);

        //    moveDirection *= speed;
        //}
        //moveDirection.y -= gravity * Time.deltaTime;
        //character.Move(moveDirection * Time.deltaTime);
        //playerModel.LookAt(new Vector3(playerModel.position.x + moveDirection.x, playerModel.position.y, playerModel.position.z + moveDirection.z));
    }
}
