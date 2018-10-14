using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpCamera : MonoBehaviour
{

    public static TpCamera Instance;
    public Transform TargetLookAt;

	public float Distance = 5f;
	public float DistanceMin = 3f;
	public float DistanceMax = 10f;
	public float DistanceSmooth = 0.05f;
	public float MouseSensitivity_X = 5f;
	public float MouseSensitivity_Y = 5f;
	public float MouseWheelSensitivity = 5f;
	public float MinLimit_Y = -40f;
	public float MaxLimit_Y = -80f;
	public float Smooth_X = 0.05f;
	public float Smooth_Y = 0.1f;
	public float OcclusionDistanceStep = 0.5f;
	public int MaxOcclusionChecks = 10;
	
	float mouseX = 0f;
	float mouseY = 0f;
	float velocityX = 0f;
	float velocityY = 0f;
	float velocityZ = 0f;
	float velDistance = 0f;
	float startDistance = 0f;
	Vector3 position = Vector3.zero;
	Vector3 desiredPosition = Vector3.zero;
	float desiredDistance = 0f;
    void Awake()
    {
        Instance = this;
    }

	void Start()
	{
		Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
		startDistance = Distance;
		ResetCam();
	}

    void LateUpdate()
    {
		if(TargetLookAt == null)
			return;

		HandlePlayerInput();

		// Check if camera is occluded
		var count = 0;
		do
		{
			CalculateDesiredPosition();
			count++;
		}
		while(CheckIfOccluded(count));
		
		UpdatePosition();
    }

	void HandlePlayerInput()
	{
		var deadZone = 0.01f;

		// if(Input.GetMouseButton(1))
		// {
		// 	mouseX += Input.GetAxis("Mouse X") * MouseSensitivity_X;
		// 	mouseY -= Input.GetAxis("Mouse Y") * MouseSensitivity_Y;
		// }

		mouseX += Input.GetAxis("Mouse X") * MouseSensitivity_X;
		mouseY -= Input.GetAxis("Mouse Y") * MouseSensitivity_Y;

		mouseY = Helper.ClampAngle(mouseY, MinLimit_Y, MaxLimit_Y);

		if(Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
		{
			desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, DistanceMin, DistanceMax);
		}
	}

	void CalculateDesiredPosition()
	{
		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, DistanceSmooth);
		desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
	}

	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
		return TargetLookAt.position + rotation * direction;
	}

	bool CheckIfOccluded(int count)
	{
		var isOccluded = false;
		var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);

		if(nearestDistance != -1)
		{
			if(count < MaxOcclusionChecks)
			{
				isOccluded = true;
				Distance -= OcclusionDistanceStep;
				if(Distance < 0.25f)
					Distance = 0.25f;
			}
			else 
			{
				Distance = nearestDistance - Camera.main.nearClipPlane;
			}
			desiredDistance = Distance;

		}
		return isOccluded;
	}

	float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		var nearestDistance = -1f;

		RaycastHit hitInfo;
		Camera cam = GetComponent<Camera>();
		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);
		Debug.DrawLine(from, to + transform.forward * -cam.nearClipPlane, Color.red);
		Debug.DrawLine(from, clipPlanePoints.UpperLeft);
		Debug.DrawLine(from, clipPlanePoints.LowerLeft);
		Debug.DrawLine(from, clipPlanePoints.UpperRight);
		Debug.DrawLine(from, clipPlanePoints.LowerRight);

		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

		if(Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
			nearestDistance = hitInfo.distance;

		if(Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		if(Physics.Linecast(from, to + transform.forward * -cam.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		return nearestDistance;
	}

	void UpdatePosition()
	{
		var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velocityX, Smooth_X);
		var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velocityY, Smooth_Y);
		var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velocityZ, Smooth_X);

		position = new Vector3(posX, posY, posZ);
		transform.position = position;
		transform.LookAt(TargetLookAt);
	}

	
	public void ResetCam()
    {
		mouseX = 0;
		mouseY = 10;
		Distance = startDistance;
		desiredDistance = Distance;
    }

    public static void UseOrCreateMainCamera()
    {
		GameObject tempCamera;
		GameObject targetLookAt;
		TpCamera myCamera;

		if(Camera.main != null)
		{
			tempCamera = Camera.main.gameObject;
		}
		else
		{
			tempCamera = new GameObject("MainCamera");
			tempCamera.AddComponent(typeof(Camera));
			tempCamera.tag = "MainCamera";
		}

		tempCamera.AddComponent<TpCamera>();
		myCamera = tempCamera.GetComponent<TpCamera>() as TpCamera;

		targetLookAt = GameObject.Find("targetLookAt") as GameObject;

		if(targetLookAt == null)
		{
			targetLookAt = new GameObject("targetLookAt");
			targetLookAt.transform.position = Vector3.zero;
		}

		myCamera.TargetLookAt = targetLookAt.transform;
    }

}
