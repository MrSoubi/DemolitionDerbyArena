using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private void Update()
    {
        Debug.Log(GetComponent<Rigidbody>().velocity.magnitude);
    }
}
