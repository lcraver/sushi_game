using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public static PlayerController inst;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else if (inst != this)
            Destroy(gameObject);
    }

    public GameObject tentaclePrefab;
    public List<Tentacle> tentacles = new List<Tentacle>();

    public Rigidbody2D rb;
    public float lastRotation;
    public float currRotation;
    public float currentAngularSpeed;
    public float maxAngularSpeed = 5f;
    public float speedMult = 2f;
    public float slowDownMult = 2f;
    public float jumpForce = 100f;

    public LayerMask groundLayers;
    public bool IsGrounded = false;

    void Start ()
    {
        rb = this.GetComponent<Rigidbody2D>();
        //CreateTentacle(Tentacle.Type.walking1, 45);
        //CreateTentacle(Tentacle.Type.walking1, 75);
        //CreateTentacle(Tentacle.Type.walking1, 105);
        //CreateTentacle(Tentacle.Type.walking1, 135);
    }

    public void CreateTentacle(Tentacle.Type _type, float _rot)
    {
        GameObject tentacleTemp = Instantiate(tentaclePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        tentacleTemp.name = tentacles.Count.ToString() + " - " + _type.ToString();
        tentacles.Add(tentacleTemp.GetComponent<Tentacle>());
        tentacles[tentacles.Count - 1].InitTentacle(this.gameObject, _type);
        tentacleTemp.transform.SetParent(this.gameObject.transform);
        tentacleTemp.transform.localPosition = Vector3.zero;
        tentacleTemp.transform.localRotation = Quaternion.Euler(new Vector3(0,0,_rot));
        FixedJoint2D joint = this.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = tentacles[tentacles.Count - 1].tentacleObjects[0].GetComponent<Rigidbody2D>();
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

    void Update()
    {
        IsGrounded = isGrounded();

        if(Input.GetButtonDown("Jump") && IsGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce));
        }
    }

    bool isGrounded()
    {
        bool isGrounded = false;
        isGrounded = Physics2D.OverlapCircle(this.transform.position, 1f, groundLayers);
        foreach (Tentacle tentacle in tentacles)
        {
            if (Physics2D.OverlapCircle(tentacle.armObject.transform.position, 1f, groundLayers))
                IsGrounded = true;
        }

        return isGrounded;
    }
    
    //void OnDrawGizmos()
    //{
    //    foreach (Tentacle tentacle in tentacles)
    //    {
    //        Gizmos.color = new Color(0, 1, 0, 0.25f);
    //        Gizmos.DrawSphere(tentacle.armObject.transform.position, 1f);
    //    }
    //}
}
