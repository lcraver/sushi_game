using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public Dictionary<Tentacle.Type, Tentacle> tentacles = new Dictionary<Tentacle.Type, Tentacle>();

    public Rigidbody2D rb;
    public float lastRotation;
    public float currRotation;
    public float currentAngularSpeed;
    public float maxAngularSpeed = 5f;
    public float speedMult = 2f;
    public float slowDownMult = 2f;

    void Start ()
    {
        rb = this.GetComponent<Rigidbody2D>();
        //Tentacle tentacle = new Tentacle(this.gameObject, Tentacle.Type.walking1);
    }

    void FixedUpdate()
    {
        currRotation = rb.rotation;
        currentAngularSpeed = rb.angularVelocity;
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0)
            rb.AddTorque(-horizontal * speedMult);
        else if (horizontal < 0)
            rb.AddTorque(-horizontal * speedMult);
        else
        {
            rb.AddTorque(slowDownMult * (lastRotation - currRotation));
        }

        if (Mathf.Abs(currentAngularSpeed) > maxAngularSpeed)
            rb.AddTorque(lastRotation - currRotation);

        lastRotation = currRotation;
    }
}
