using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public bool debug;

    public static PlayerController inst;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else if (inst != this)
            Destroy(gameObject);
    }

    public Text guiText;
    public List<bool> isShowingText = new List<bool>();

    public bool isControllable;

    public PhysicsMaterial2D groundedPhysicsMaterial;
    public PhysicsMaterial2D airPhysicsMaterial;

    public GameObject tentaclePrefab;
    public GameObject particlePrefab;
    public GameObject ink;
    public GameObject body;
    public Sprite crying;
    public Sprite notCrying;
    public List<TentacleConnection> pastTentacles = new List<TentacleConnection>();
    public List<Tentacle> tentacles = new List<Tentacle>();

    public bool isFinished = false;

    public Vector2 currentCheckpoint = new Vector2(-2.5f, -3.4f);

    public class TentacleConnection
    {
        public float rot;
        public Tentacle.Type type; 

        public TentacleConnection(float _rot, Tentacle.Type _type)
        {
            rot = _rot;
            type = _type;
        }
    }

    public Rigidbody2D rb;
    public float lastRotation;
    public float currRotation;
    public float currentAngularSpeed;
    public float maxAngularSpeed = 5f;
    public float speedMult = 2f;
    public float slowDownMult = 2f;
    public float jumpForce = 100f;

    public LayerMask groundLayers;
    public bool WasGrounded = false;
    public bool IsGrounded = false;
    public bool IsHoldingObject = false;

    public void SetCheckpoint(Vector2 _pos)
    {
        currentCheckpoint = _pos;
    }

    void Start ()
    {
        rb = this.GetComponent<Rigidbody2D>();
        body = this.transform.Find("body_sprite").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = crying;
        ink = this.transform.Find("ink").gameObject;
        ToggleText(false);
        //CreateTentacle(Tentacle.Type.grab, 20);
        //CreateTentacle(Tentacle.Type.grab, 160);
        //CreateTentacle(Tentacle.Type.grab, 70);
        //CreateTentacle(Tentacle.Type.grab, 110);
        //CreateTentacle(Tentacle.Type.grab, 135);
        DisplayText("Move with Arrow Jeys / Right Joystick \n \n Jump with Z / A Button", 5);
    }

    public void CreateExampleTentacles()
    {
        CreateTentacle(Tentacle.Type.grab, 20);
        CreateTentacle(Tentacle.Type.grab, 160);
        CreateTentacle(Tentacle.Type.grab, 70);
        CreateTentacle(Tentacle.Type.grab, 110);
        Debug.Log("created tentacle");
    }

    public void DeleteTentacles()
    {
        Destroy(this.GetComponent<FixedJoint2D>());
        foreach (Tentacle tentacle in tentacles)
        {
            Destroy(tentacle.gameObject);
            Destroy(this.GetComponent<FixedJoint2D>());
        }
        tentacles.Clear();

        this.transform.position = currentCheckpoint;
        RecreateTentacles();
    }

    public void RecreateTentacles()
    {
        List<TentacleConnection> connections = new List<TentacleConnection>();

        foreach (TentacleConnection tentacle in pastTentacles)
        {
            connections.Add(new TentacleConnection(tentacle.rot, tentacle.type));
        }

        pastTentacles.Clear();

        PlayTentaclesGetAnimation(connections);
    }

    public void CreateTentacle(Tentacle.Type _type, float _rot)
    {
        GameObject tentacleTemp = Instantiate(tentaclePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        tentacleTemp.name = tentacles.Count.ToString() + " - " + _type.ToString();
        tentacles.Add(tentacleTemp.GetComponent<Tentacle>());
        pastTentacles.Add(new TentacleConnection(_rot, _type));
        tentacles[tentacles.Count - 1].InitTentacle(this.gameObject, _type, _rot);
        tentacleTemp.transform.SetParent(this.gameObject.transform);
        tentacleTemp.transform.localScale = Vector3.one;
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
        isControllable = false;
        this.transform.DOMoveY(startPos.y + 1.5f, 0.5f);
        this.transform.DORotate(Vector3.zero, 0.5f);
        AudioManager.inst.PlaySound("armget");
        yield return new WaitForSeconds(0.5f);
        GameObject tentacleParticle = Instantiate(particlePrefab, this.transform.position, Quaternion.identity) as GameObject;
        tentacleParticle.transform.SetParent(this.transform);
        tentacleParticle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _rot + 180));
        yield return new WaitForSeconds(0.25f);
        isControllable = true;
        if (tentacles.Count == 0)
            body.GetComponent<SpriteRenderer>().sprite = notCrying;
        DisplayText("Tentacle Get! \n \n Can now attach to things with right bumper / x", 3f);
        AudioManager.inst.PlaySound("armpop");
        CreateTentacle(_type, _rot);
        rb.isKinematic = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(tentacleParticle);
    }

    public void PlayTentaclesGetAnimation(List<TentacleConnection> _tentacles)
    {
        StartCoroutine(PlayTentaclesGetAnimationLoop(_tentacles));
    }

    public IEnumerator PlayTentaclesGetAnimationLoop(List<TentacleConnection> _tentacles)
    {
        Vector2 startPos = this.transform.position;
        rb.isKinematic = true;
        isControllable = false;
        this.transform.DOMoveY(startPos.y + 1.5f, 0.5f);
        this.transform.DORotate(Vector3.zero, 0.5f);
        AudioManager.inst.PlaySound("armget");
        yield return new WaitForSeconds(0.5f);
        List<GameObject> tentacleParticles = new List<GameObject>();
        List<TentacleConnection> tentaclesCopy = new List<TentacleConnection>();
        foreach (TentacleConnection tentacle in _tentacles)
        {
            tentacleParticles.Add(Instantiate(particlePrefab, this.transform.position, Quaternion.identity) as GameObject);
            tentacleParticles[tentacleParticles.Count - 1].transform.SetParent(this.transform);
            tentacleParticles[tentacleParticles.Count - 1].transform.rotation = Quaternion.Euler(new Vector3(0, 0, tentacle.rot + 180));
            tentaclesCopy.Add(tentacle);
        }
        yield return new WaitForSeconds(0.25f);
        isControllable = true;
        if (tentacles.Count == 0)
            body.GetComponent<SpriteRenderer>().sprite = notCrying;
        AudioManager.inst.PlaySound("armpop");
        foreach (TentacleConnection tentacle in tentaclesCopy)
        {
            CreateTentacle(tentacle.type, tentacle.rot);
        }
        rb.isKinematic = false;
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject tentacleParticle in tentacleParticles)
        {
            Destroy(tentacleParticle);
        }
    }

    void FixedUpdate()
    {
        currRotation = rb.rotation;
        currentAngularSpeed = rb.angularVelocity;
        float horizontal = Input.GetAxis("Horizontal");
        if (isControllable)
        {
            if (horizontal > 0)
            {
                rb.AddTorque(-horizontal * speedMult);
                rb.AddForce(new Vector2(1, 0));
            }
            else if (horizontal < 0)
            {
                rb.AddTorque(-horizontal * speedMult);
                rb.AddForce(new Vector2(-1, 0));
            }
            else
            {
                rb.AddTorque(slowDownMult * (lastRotation - currRotation));
            }

            if (Mathf.Abs(currentAngularSpeed) > maxAngularSpeed)
                rb.AddTorque(lastRotation - currRotation);
        }

        lastRotation = currRotation;
    }

    IEnumerator GoToMenu(float _time)
    {
        yield return new WaitForSeconds(_time);
        SceneManager.LoadScene("main menu");
    }

    void Update()
    {
        if (isControllable && this.transform.position.x > 82 && !isFinished)
        {
            isFinished = true;
            isControllable = false;
            rb.isKinematic = true;

            DisplayText("Congratulations you Escaped! \n\n And collected " + tentacles.Count + "/4 of your tentacles.", 10f);
            StartCoroutine(GoToMenu(4f));
        }

        IsGrounded = isGrounded();
        if (!WasGrounded && IsGrounded)
            AudioManager.inst.PlaySound("squid", 0.5f);

        if (isControllable)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                DeleteTentacles();
            }

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                AudioManager.inst.PlaySound("jump");
                rb.AddForce(new Vector2(0, jumpForce));
                ink.GetComponent<ParticleSystem>().Play();
            }
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
        
        WasGrounded = IsGrounded;
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

            if(tentacle.type == Tentacle.Type.grab)
            {
                if (tentacle.heldObject != null)
                {
                    if (Physics2D.OverlapCircle(tentacle.heldObject.transform.position, tentacle.heldObject.GetComponent<CircleCollider2D>().radius * 2, groundLayers))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.DrawSphere(this.transform.position - new Vector3(0, 0.25f), 0.25f);

            foreach (Tentacle tentacle in tentacles)
            {
                foreach (GameObject tentaclePart in tentacle.tentacleObjects)
                {
                    Gizmos.DrawSphere(tentaclePart.transform.position, 0.25f);
                }

                if (tentacle.type == Tentacle.Type.grab)
                {
                    if (tentacle.heldObject != null)
                    {
                        Gizmos.DrawSphere(tentacle.heldObject.transform.position, tentacle.heldObject.GetComponent<CircleCollider2D>().radius * 2);
                    }
                }
            }
        }
    }

    public void DisplayText(string _text, float _time)
    {
        if (guiText != null)
            StartCoroutine(DisplayTextLoop(_text, _time));
    }

    public IEnumerator DisplayTextLoop(string _text, float _time)
    {
        guiText.text = _text;
        ToggleText(true);
        yield return new WaitForSeconds(_time);
        isShowingText.RemoveAt(0);
        if (isShowingText.Count == 0)
            ToggleText(false);
    }

    public void ToggleText(bool _toggle)
    {
        if (guiText != null)
        {
            guiText.gameObject.SetActive(_toggle);
            if (_toggle)
                isShowingText.Add(true);
        }
    }
}
