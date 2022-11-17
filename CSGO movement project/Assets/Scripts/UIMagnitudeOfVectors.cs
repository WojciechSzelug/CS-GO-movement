using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMagnitudeOfVectors : MonoBehaviour
{
    public Text velocityMagnitude;
    [Space()]
    public Rigidbody rb;

    private void Update()
    {
        velocityMagnitude.text = rb.velocity.magnitude.ToString();
    }

}
