using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRoll : MonoBehaviour
{
    [SerializeField] private WheelCollider WheelL;
    [SerializeField] private WheelCollider WheelR; 
    private float antiRoll = 5000.0f;
    private Rigidbody carRigidbody;

    public void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WheelL.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius)
                      / WheelL.suspensionDistance;

        bool groundedR = WheelR.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius)
                      / WheelR.suspensionDistance;

        var antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedL)
            carRigidbody.AddForceAtPosition(WheelL.transform.up * -antiRollForce, WheelL.transform.position);
        if (groundedR)
            carRigidbody.AddForceAtPosition(WheelR.transform.up * antiRollForce, WheelR.transform.position);
    }
}
