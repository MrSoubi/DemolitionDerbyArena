using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private Rigidbody carRigidBody;
    [SerializeField] private Transform carTransform;
    [SerializeField] private CarData carData;
    [SerializeField] private WheelData data;

    float accelInput;
    float steerInput;


    // Update is called once per frame
    void FixedUpdate(){
        accelInput = (Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0.0f);
        steerInput = (Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0.0f);

        transform.localEulerAngles = new Vector3(0, 30.0f * steerInput, 0);

        LayerMask mask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, mask))
        {
            Suspension(hit);
            if (data.isSteerable){
                Steering(hit);
            }
            if (data.isMotorised){
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
        Vector3 accelDir = transform.forward;
        Debug.Log(transform.position);
        if (Mathf.Abs(accelInput) > 0.0f){
            float carSpeed = Vector3.Dot(carTransform.forward, carRigidBody.velocity);
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carData.maxSpeed);
            float availableTorque = carData.powerCurve.Evaluate(normalizedSpeed) * accelInput;
            carRigidBody.AddForceAtPosition(accelDir * availableTorque * carData.enginePower, transform.position);
        }
    }
}
