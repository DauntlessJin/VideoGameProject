using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_Controller : MonoBehaviour
{
    private Animator _animator;
    private Transform skate_tr;
    private Rigidbody rigid_skate;

    int IdleHash = Animator.StringToHash("Locomotion.Idle");
    int PushHash = Animator.StringToHash("Locomotion.Push");
    int RunHash = Animator.StringToHash("Locomotion.FC_Cycle");
    //int OlliesHash = Animator.StringToHash("Ollies_system");

    public float MaxSpeed = 0.06f;
    public float speed_m = 0.1f;
    public float jumpspeed = 100;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;
    private float junk_speed = 0.0f;

    public float TimeLeft_Push = 10.0f;
    public float TimeLeft_Stop = 5.0f;

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

        //var TurningSpeed = Input.GetAxis("Horizontal");
        var Speed = Input.GetAxis("Vertical");
        var Ollies_X = Input.GetAxis("Jump");
        _animator.SetFloat("Speed", Speed);
        junk_speed = MaxSpeed;

        //------------- Turning Animation -------------//
        var Turning_Speed = Input.GetAxis("Horizontal");
        _animator.SetFloat("Turning_Speed", Turning_Speed);
        Vector3 movedir = (Vector3.forward * Speed) + (Vector3.right * Turning_Speed);


        //------------- Start Run & delay 20 second -------------//
        if (Input.GetKeyDown(KeyCode.W))
        {
            Start_Run();
            TimeLeft_Push = 10.0f;
        }
        
        TimeLeft_Push -= Time.deltaTime;
        Debug.Log("Timer = " + TimeLeft_Push);

        if (TimeLeft_Push < 0)
        {
                _animator.SetTrigger("Push");
                TimeLeft_Push = 10.0f;
        }

        //------------- Stop Animation -------------//
        if (Speed == -1 || stateInfo.nameHash == IdleHash)
        {
            skate_tr.Translate(0, 0, 0);
            TimeLeft_Stop -= Time.deltaTime;
            if (TimeLeft_Stop < 0)
            {
                Debug.Log("Start Stop Animation");
                _animator.SetTrigger("Stop");
                TimeLeft_Stop = 5.0f;
            }
        }
        else if (Speed > -1)
        {
            skate_tr.Translate(0, 0, junk_speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            skate_tr.Translate(0, 0, 0);
            TimeLeft_Stop -= Time.deltaTime;
            if (TimeLeft_Stop < 0)
            {
                Debug.Log("Start Stop Animation");
                _animator.SetTrigger("Stop");
                TimeLeft_Stop = 5.0f;
            }
        }

        //if (Input.GetKey(KeyCode.S) || stateInfo.nameHash == IdleHash)
        //{
        //    skate_tr.Translate(0, 0, 0);
        //    TimeLeft_Stop -= Time.deltaTime;
        //    if (TimeLeft_Stop < 0)
        //    {
        //        _animator.SetTrigger("Stop");
        //        TimeLeft_Stop = 5.0f;
        //    }
        //}


        //------------- Ollies System -------------//
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
    
    void Start_Run()
    {
        _animator.SetTrigger("Start_Run");
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
