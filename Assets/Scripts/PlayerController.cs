using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // public variables
    public float MaxSpeed = 20;
    public float Acceleration = 64;
    public float JumpSpeed = 8;
    public float JumpDuration = 150;

    // Input variables
    private float horizantal;
    private float vertical;
    private float jumpInput;

    // Internal Variables
    private bool onTheGround;
    private float jmpDuration;
    private bool jumpKeyDown = false;
    private bool canVariableJump = false;
    private float movement_Anim;

    Rigidbody rigid;
    Animator anim;
    LayerMask layerMask;
    Transform modelTrans;

    public Transform shoulderTrans;
    public Transform rightShoulder;
    public Vector3 lookPos;
    public float lookPosThres;
    GameObject rsp;

	// Use this for initialization
	void Start ()
    {
        rigid = GetComponent<Rigidbody>();
        SetupAnimator();

        layerMask = ~(1 << 8);

        rsp = new GameObject();
        rsp.name = transform.root.name + "Right shoulder IK helper";
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        InputHandler();
        UpdateRigidbodyValues();
        MovementHandler();
        HandleRotation();
        HandleAimingPos();
        HandleAnimations();
        HandleShoulder();
	}

    private void InputHandler()
    {
        horizantal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        jumpInput = Input.GetAxis("Fire2");
    }

    private void MovementHandler()
    {
        onTheGround = isOnGround();

        if (horizantal < -0.1f)
        {
            if(rigid.velocity.x > -this.MaxSpeed)
            {
                rigid.AddForce(new Vector3(-this.Acceleration, 0, 0));
            }
            else
            {
                rigid.velocity = new Vector3(-this.MaxSpeed, rigid.velocity.y, 0);
            }
        }
        else if(horizantal > 0.1f)
        {
            if(rigid.velocity.x < this.MaxSpeed)
            {
                rigid.AddForce(new Vector3(this.Acceleration, 0, 0));
            }
            else
            {
                rigid.velocity = new Vector3(this.MaxSpeed, rigid.velocity.y, 0);
            }
        }

        if(jumpInput > 0.1f)
        {
            if(!jumpKeyDown)
            {
                jumpKeyDown = true;

                if(onTheGround)
                {
                    rigid.velocity = new Vector3(rigid.velocity.y, this.JumpSpeed, 0);
                    jmpDuration = 0.0f;
                }
            }
            else if(canVariableJump)
            {
                jmpDuration += Time.deltaTime;

                if(jmpDuration < this.JumpDuration / 1000)
                {
                    rigid.velocity = new Vector3(rigid.velocity.x, this.JumpSpeed, 0);
                }
            }
        }
        else
        {
            jumpKeyDown = false;
        }
    }

    private void UpdateRigidbodyValues()
    {
        if(onTheGround)
        {
            rigid.drag = 4;
        }
        else
        {
            rigid.drag = 0;
        }
    }

    private void HandleRotation()
    {
        Vector3 directionToLook = lookPos - transform.position;
        directionToLook.y = 0;
        Debug.Log("Dir to look: " + directionToLook);
        if ((directionToLook.x > 0 && directionToLook.x < lookPosThres) || (directionToLook.x < 0 && directionToLook.x > lookPosThres * -1))
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15);
    }

    private void HandleAimingPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 lookP = hit.point;
            lookP.z = transform.position.z;
            lookPos = lookP;
        }
    }

    private void HandleAnimations()
    {
        anim.SetBool("OnAir", !onTheGround);
        float animValue = horizantal;

        if(lookPos.x < transform.position.x)
        {
            animValue = -animValue;
        }

        anim.SetFloat("Movement", animValue, 0.1f, Time.deltaTime);
    }

    private void HandleShoulder()
    {
        if ((lookPos.x > 0 && lookPos.x < lookPosThres && lookPos.y < lookPosThres) || (lookPos.x < 0 && lookPos.x > lookPosThres * -1 && lookPos.y < lookPosThres))
            return;

        shoulderTrans.LookAt(lookPos);
        Vector3 rightShoulderPos = rightShoulder.TransformPoint(Vector3.zero);
        rsp.transform.position = rightShoulderPos;
        rsp.transform.parent = transform;

        shoulderTrans.position = rsp.transform.position;
    }  

    private void SetupAnimator()
    {
        anim = GetComponent<Animator>();

        foreach(var childAnimator in GetComponentsInChildren<Animator>())
        {
            if(childAnimator != anim)
            {
                anim.avatar = childAnimator.avatar;
                modelTrans = childAnimator.transform;
                Destroy(childAnimator);
                break;
            }
        }
    }

    private bool isOnGround()
    {
        bool retVal = false;
        float lengthToSearch = 1.5f;
        Vector3 lineStart = transform.position + Vector3.up;
        Vector3 vectorToSearch = -Vector3.up;
        RaycastHit hit;

        if(Physics.Raycast(lineStart, vectorToSearch, out hit, lengthToSearch, layerMask))
        {
            retVal = true;
        }

        return retVal;
    }
}
