using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private bool isBreakingUp;
    private bool isBreakingDown;

    private float currentBreakForce;

    private float currentSteerAngle;
    
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private float maxAcceleration;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    public void Awake()
    {
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteerling();
        UpdateWheels();
    }

    private void GetInput()
    {
        verticalInput = Input.GetAxis(VERTICAL);
        horizontalInput = Input.GetAxis(HORIZONTAL);
        isBreakingUp = Input.GetKey(KeyCode.UpArrow);
        isBreakingDown = Input.GetKey(KeyCode.DownArrow);
    }
    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * maxAcceleration * 500 * Time.deltaTime;
        frontRightWheelCollider.motorTorque = verticalInput * maxAcceleration * 500 * Time.deltaTime;
        currentBreakForce = 0f;
        if (!isBreakingDown && !isBreakingUp)
        {
            currentBreakForce = breakForce;
        }
        //currentBreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }
    private void ApplyBreaking()
    {
        frontLeftWheelCollider.brakeTorque = currentBreakForce;
        frontRightWheelCollider.brakeTorque = currentBreakForce;
        rearLeftWheelCollider.brakeTorque = currentBreakForce;
        rearRightWheelCollider.brakeTorque = currentBreakForce;
    }

    private void HandleSteerling()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
