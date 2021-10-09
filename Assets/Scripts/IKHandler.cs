using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandler : MonoBehaviour {

    Animator anim;
    Vector3 lookPos;
    Vector3 IK_lookPos;
    Vector3 targetPos;
    PlayerController pl;

    public float lerpRate = 15;
    public float updateLookPosThreshold = 2;
    public float lookWeight = 1;
    public float bodyWeight = 0.9f;
    public float headWeight = 1;
    public float clampWeight = 1;

    public float rightHandWeight = 1;
    public float leftHandWeight = 1;

    public Transform rightHandTarget;
    public Transform rightElbowTarget;
    public Transform leftHandTarget;
    public Transform leftElbowTarget;

	// Use this for initialization
	void Start () {
        this.anim = GetComponent<Animator>();
        pl = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);

        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);

        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);

        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightHandWeight);
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftHandWeight);

        anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowTarget.position);
        anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowTarget.position);

        this.lookPos = pl.lookPos;
        lookPos.z = transform.position.z;

        float distanceFromPlayer = Vector3.Distance(lookPos, transform.position);

        if (distanceFromPlayer > updateLookPosThreshold)
        {
            targetPos = lookPos;
        }

        IK_lookPos = Vector3.Lerp(IK_lookPos, targetPos, Time.deltaTime * lerpRate);

        anim.SetLookAtWeight(lookWeight, bodyWeight, headWeight, headWeight, clampWeight);
        anim.SetLookAtPosition(IK_lookPos);
 
    }
}
