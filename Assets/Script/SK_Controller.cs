using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_Controller : MonoBehaviour
{
    private Animator _animator;
    private Transform skate_tr;
    private Rigidbody rigid_skate;

    int IdleHash = Animator.StringToHash("Locomotion.Idle");
    //int OlliesHash = Animator.StringToHash("Ollies_system");
    
    public float MaxSpeed = 0.06f;
    public float speed_m = 0.1f;
    public float jumpspeed = 100;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;
    private float junk_speed = 0.0f;

    public WheelCollider[] wheelcollider = new WheelCollider[4];
    public Transform[] tireMeshes = new Transform[4];

    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        skate_tr = GetComponent<Transform>();
        rigid_skate = GetComponent<Rigidbody>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        
        var TurningSpeed = Input.GetAxis("Horizontal");
        var Speed = Input.GetAxis("Vertical");
        var Ollies_X = Input.GetAxis("Jump");

        _animator.SetFloat("Speed", Speed);
        _animator.SetFloat("TurningSpeed", TurningSpeed);

        Vector3 movedir = (Vector3.forward * Speed) + (Vector3.right * TurningSpeed);

        junk_speed = MaxSpeed;

        ////////// When player pressed/unpressed "S" ///////////////////
        if (Speed == -1 || stateInfo.nameHash == IdleHash)
        {
            skate_tr.Translate(0, 0, 0);
            Speed = -1;
            Debug.Log("Speed= " + Speed);
        }
        else if (Speed > -1)
        {
            skate_tr.Translate(0, 0, junk_speed);
        }

        ////////// Ollies System /////////////////
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid_skate.AddForce(Vector3.up * jumpspeed);
            _animator.SetFloat("Ollies_X", Ollies_X);
            _animator.SetTrigger("Jump_Ollies");
            junk_speed = speed_m;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _animator.SetTrigger("Ollies_trigger");
            junk_speed = MaxSpeed;
        }


        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed_m;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpspeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        transform.Rotate(0, Input.GetAxis("Horizontal"), 0);


        /////////// Calculate Velocity ////////////
        
        

    }
    
    void UpdateMeshesPosition()
    {
        for(int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 pos;
            wheelcollider[i].GetWorldPose(out pos, out quat);

            tireMeshes[i].position = pos;
            tireMeshes[i].rotation = quat;
        }
    }

}
