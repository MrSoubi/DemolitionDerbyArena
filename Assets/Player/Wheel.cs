using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wheel : MonoBehaviour
{
    [SerializeField] private Rigidbody carRigidBody;
    [SerializeField] private Transform carTransform;
    [SerializeField] private CarData carData;
    [SerializeField] private PlayerInput input;
    [SerializeField] private WheelData data;
    [SerializeField] private GameObject mesh;

    float steerInput = 0.0f;

    // Update is called once per frame
    void FixedUpdate(){
        steerInput = 0.0f;
        if (data.isSteerable)
        {
            steerInput = Input.GetAxis(input.steer);
        }
        
        transform.rotation = carTransform.rotation;
        transform.Rotate(transform.up, steerInput * 30.0f);

        if (data.isSteerable)
        {
            mesh.transform.rotation = transform.rotation;
        }

        LayerMask mask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, data.maxSuspensionDistance + data.wheelRadius, mask))
        {
            Suspension(hit);
            Steering(hit);
            if (data.isMotorised)
            {
                Acceleration(hit);
            }
        }
    }

    public void Suspension(RaycastHit tireRay){
        Vector3 springDir = transform.up;
        Vector3 tireWorldVel = carRigidBody.GetPointVelocity(transform.position);
        float offset = data.suspensionRestDir - tireRay.distance;
        float vel = Vector3.Dot(springDir, tireWorldVel);
        float force = (offset * data.springStrength) - (vel * data.springDamper);
        carRigidBody.AddForceAtPosition(springDir * force, transform.position);

        mesh.transform.position = transform.position + new Vector3(0, -Mathf.Min(tireRay.distance, data.maxSuspensionDistance) + data.wheelRadius, 0);
    }

    public void Steering(RaycastHit tireRay){
        Vector3 steeringDir = transform.right;
        Vector3 tireWorldVel = carRigidBody.GetPointVelocity(transform.position);
        float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);
        float desiredVelChange = -steeringVel * data.tireGripFactor;
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
        carRigidBody.AddForceAtPosition(steeringDir * data.tireMass * desiredAccel, transform.position);
    }

    public void Acceleration(RaycastHit tireRay){
        float accelInput = Input.GetAxis(input.throttle) - Input.GetAxis(input.reverse);

        Vector3 accelDir = transform.forward;
        if (Mathf.Abs(accelInput) > 0.0f){
            float carSpeed = Vector3.Dot(carTransform.forward, carRigidBody.velocity);
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carData.maxSpeed);
            float availableTorque = carData.powerCurve.Evaluate(normalizedSpeed) * accelInput;
            carRigidBody.AddForceAtPosition(accelDir * availableTorque * carData.enginePower, transform.position);
        }
    }

    public bool isGrounded()
    {
        LayerMask mask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, data.maxSuspensionDistance + data.wheelRadius, mask);
    }
}
