using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpController : MonoBehaviour
{

    public static CharacterController cc;
    public TpController Instance;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        Instance = this;
        TpCamera.UseOrCreateMainCamera();
    }

    void Update()
    {
        if (Camera.main == null)
            return;
        GetLocomotionInput();
        HandleActionInput();
        TpMotor.Instance.UpdateMotor();
    }

    void GetLocomotionInput()
    {
        var deadZone = 0.1f;

		TpMotor.Instance.VerticalVelocity = TpMotor.Instance.MoveVector.y;
        TpMotor.Instance.MoveVector = Vector3.zero;

        if (GameManager.Instance.InputController.Vertical > deadZone || GameManager.Instance.InputController.Vertical < -deadZone)
            TpMotor.Instance.MoveVector += new Vector3(0, 0, GameManager.Instance.InputController.Vertical);

        if (GameManager.Instance.InputController.Horizontal > deadZone || GameManager.Instance.InputController.Horizontal < -deadZone)
            TpMotor.Instance.MoveVector += new Vector3(GameManager.Instance.InputController.Horizontal, 0, 0);
    }

    void HandleActionInput()
    {
		
        if (GameManager.Instance.InputController.Jump)
        {
            Jump();
        }
    }

    void Jump()
    {
        TpMotor.Instance.Jump();
    }
}
