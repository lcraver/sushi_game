using UnityEngine;
using System.Collections;

public class Camera_Follow : MonoBehaviour
{
    public static Camera_Follow inst;

    void Awake()
    {
        if (!inst)
        {
            inst = this;
        }
        else
            Destroy(gameObject);
    }

    public Transform target;
    public float distance = 20f;
    public float catchUp = 5f;
    private float yOffset = 1.5f;

    public Vector2 xClamp = new Vector2(-10, -0.75f);
    public Vector2 yClamp = new Vector2(0, 62f);

    public Camera camera;

    void Start()
    {
        camera = this.GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 camPos = this.transform.position;

            camPos.y = Mathf.Lerp(camPos.y, target.position.y + yOffset, Time.fixedDeltaTime * catchUp);
            camPos.y = Mathf.Clamp(camPos.y, xClamp.x, xClamp.y);

            camPos.x = Mathf.Lerp(camPos.x, target.position.x, Time.fixedDeltaTime * catchUp);
            camPos.x = Mathf.Clamp(camPos.x, yClamp.x, yClamp.y);

            this.transform.position = camPos;
        }
    }

    public void SetYOffset(float _yOffset)
    {
        yOffset = _yOffset;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }
}
