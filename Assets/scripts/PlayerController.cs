using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

    public bool Debug;

    public static PlayerController inst;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else if (inst != this)
            Destroy(gameObject);
    }

    public PhysicsMaterial2D groundedPhysicsMaterial;
    public PhysicsMaterial2D airPhysicsMaterial;

    public GameObject tentaclePrefab;
    public GameObject particlePrefab;
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

    public void PlayTentacleGetAnimation(Tentacle.Type _type, float _rot)
    {
        StartCoroutine(PlayTentacleGetAnimationLoop(_type, _rot));
    }

    public IEnumerator PlayTentacleGetAnimationLoop(Tentacle.Type _type, float _rot)
    {
        Vector2 startPos = this.transform.position;
        rb.isKinematic = true;
        this.transform.DOMoveY(startPos.y + 1.5f, 0.5f);
        this.transform.DORotate(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);
        GameObject tentacleParticle = Instantiate(particlePrefab, this.transform.position, Quaternion.identity) as GameObject;
        tentacleParticle.transform.SetParent(this.transform);
        tentacleParticle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _rot + 180));
        yield return new WaitForSeconds(0.25f);
        CreateTentacle(_type, _rot);
        rb.isKinematic = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(tentacleParticle);
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
        if(Input.GetButtonDown("Jump") && IsGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce));
        }

        if (IsGrounded)
        {
            foreach (Tentacle tentacle in tentacles)
            {
                foreach (GameObject tentaclePart in tentacle.tentacleObjects)
                {
                    tentaclePart.GetComponent<Collider2D>().sharedMaterial = groundedPhysicsMaterial;
                }
            }
        }
        else
        {
            foreach (Tentacle tentacle in tentacles)
            {
                foreach (GameObject tentaclePart in tentacle.tentacleObjects)
                {
                    tentaclePart.GetComponent<Collider2D>().sharedMaterial = airPhysicsMaterial;
                }
            }
        }

        IsGrounded = isGrounded();
    }

    bool isGrounded()
    {
        if(Physics2D.OverlapCircle(this.transform.position - new Vector3(0, 0.25f), 0.25f, groundLayers))
        {
            return true;
        }

        foreach (Tentacle tentacle in tentacles)
        {
            foreach (GameObject tentaclePart in tentacle.tentacleObjects)
            {
                if (Physics2D.OverlapCircle(tentaclePart.transform.position, 0.25f, groundLayers))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if (Debug)
        {
            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.DrawSphere(this.transform.position - new Vector3(0, 0.25f), 0.25f);

            foreach (Tentacle tentacle in tentacles)
            {
                foreach (GameObject tentaclePart in tentacle.tentacleObjects)
                {
                    Gizmos.DrawSphere(tentaclePart.transform.position, 0.25f);
                }
            }
        }
    }
}
