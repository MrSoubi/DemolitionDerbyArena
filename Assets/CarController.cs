using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private List<Wheel> wheels = new List<Wheel>();

    [SerializeField] private CarData data;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Rigidbody rb;

    float drag;
    float returnedSince;

    private void Start()
    {
        drag = rb.drag;
    }
    private void Update()
    {
        if (Input.GetButtonDown(input.jump) && IsGrounded())
        {
            rb.AddForce(transform.up * data.jumpForce, ForceMode.Impulse);
        }

        if (!IsGrounded())
        {
            rb.drag = 0f;
        }
        else
        {
            rb.drag = drag;
        }

        if (IsReturned())
        {
            returnedSince += Time.deltaTime;
        }
        else
        {
            returnedSince = 0.0f;
        }

        if (returnedSince > 2.0f)
        {
            rb.AddForceAtPosition((Vector3.up + Vector3.right / 2) * data.jumpForce /2, transform.position, ForceMode.Impulse);
            returnedSince = 0.0f;
        }
    }

    public bool IsGrounded()
    {
        bool result = false;
        foreach (Wheel wheel in wheels)
        {
            result |= wheel.isGrounded();
        }
        return result;
    }

    public bool IsReturned()
    {
        return Vector3.Dot(transform.up, Vector3.down) > 0.9f;
    }
}
