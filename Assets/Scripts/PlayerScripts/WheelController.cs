using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    //reference to all our wheels
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;


    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backleftTransform;
    //"Accelration: rate of change of veloity" "BreakForce: Measure of braking power of vehicle" 
    public float accelration = 500f;
    public float breakForce = 300f;
    public float maxTurnAngle = 30f;


    private float currentAccelration = 0f;
    private float currentBreakForce = 0f;
    private float currentTurnAngle = 0f;

    private void FixedUpdate()
    {
        //getting forward/backward accelration from the verticle axis(W AND S key)
        currentAccelration = accelration * Input.GetAxis("Vertical");

        //If we're pressing space, give currentBreakForce a value
        if (Input.GetKey(KeyCode.Space))
        {
            currentBreakForce = breakForce;
        }
        else
        {
            currentBreakForce = 0f;
        }

        //apply accelration to front wheel collider
        frontRight.motorTorque = currentAccelration;
        frontLeft.motorTorque = currentAccelration;

        //apply breakforce to wheels collider
        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;


        //Take care of the steering 
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backleftTransform);
        UpdateWheel(backRight, backRightTransform);

    }

    void UpdateWheel(WheelCollider col, Transform trans)
    {
        //Get wheel colider state
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        //set wheel transform state
        trans.position = position;
        trans.rotation = rotation;


    }
}

