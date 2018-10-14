using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	public Animator animator;

	void Awake()
	{
		animator = GetComponentInChildren<Animator>();		
	} 
	
	// Update is called once per frame
	void Update () {
		animator.SetFloat("Vertical", GameManager.Instance.InputController.Vertical);
		animator.SetFloat("Horizontal", GameManager.Instance.InputController.Horizontal);

		animator.SetBool("IsWalking", GameManager.Instance.InputController.IsWalking);
		animator.SetBool("IsSprinting", GameManager.Instance.InputController.IsSprinting);
		animator.SetBool("IsCrouched", GameManager.Instance.InputController.IsCrouched);	
	}
}
