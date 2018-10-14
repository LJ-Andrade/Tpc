using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpMotor : MonoBehaviour
{

    public static TpMotor Instance;

    public float ForwardSpeed = 5f;
    public float BackwardSpeed = 5f;
    public float StrafingSpeed = 5f;
    public float JumpSpeed = 6f;
    public float SlideSpeed = 10f;
    public float Gravity = 21f;
    public float TerminalVelocity = 20f;
    public float SlideThreshold = 0.6f;
    public float MaxControllableSlideMagnitude = 0.4f;
    
    private Vector3 slideDirection;
    public Vector3 MoveVector { get; set; }
    public float VerticalVelocity { get; set; }


    void Awake()
    {
        Instance = this;
    }

    public void UpdateMotor()
    {
        SnapAlignCharacterWithCamera();
        ProcessMotion();
    }

    void ProcessMotion()
    {
		MoveVector = transform.TransformDirection(MoveVector);

		if(MoveVector.magnitude > 1)
			MoveVector = Vector3.Normalize(MoveVector);

        ApplySlide();

		MoveVector *= MoveSpeed();
		    MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);
		ApplyGravity();
		TpController.cc.Move(MoveVector * Time.deltaTime);
    }

    public void Jump()
    {
        if(TpController.cc.isGrounded)
            VerticalVelocity = JumpSpeed;
    }

    void ApplyGravity()
    {
        
        // Aseguramos que no estamos exediendo velocidad terminal y si es así aplicamos gravedad.
        if (MoveVector.y > -TerminalVelocity)
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);
        
        if (TpController.cc.isGrounded && MoveVector.y < -1)
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
    }

    void ApplySlide()
    {
        if(!TpController.cc.isGrounded)
            return;

        slideDirection = Vector3.zero;
        RaycastHit hitInfo;

        if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo))
        {
            if(hitInfo.normal.y < SlideThreshold)
                slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
        }

        if(slideDirection.magnitude < MaxControllableSlideMagnitude)
        {
            MoveVector += slideDirection;
        }
        else
        {
            MoveVector = slideDirection;
        }
    }

	void SnapAlignCharacterWithCamera()
	{
		if(MoveVector.x != 0 || MoveVector.z != 0)
		{
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}

    float MoveSpeed()
    {
        var moveSpeed = 5f;

        if(slideDirection.magnitude > 0)
        {
            moveSpeed = SlideSpeed;
        }

        return moveSpeed;
    }
}
