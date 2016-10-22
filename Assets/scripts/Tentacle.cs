using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Tentacle : MonoBehaviour {

    GameObject player;
    public GameObject armObject;
    public List<GameObject> tentacleObjects = new List<GameObject>();

    public enum Type { normal, grab, swing, fight };
    public Type type = Type.normal;

    public bool isGrabing;
    public GameObject heldObject;
    public Dictionary<GameObject, HingeJoint2D> heldJoints = new Dictionary<GameObject, HingeJoint2D>();
    public LayerMask grabLayers;

    public Tentacle InitTentacle(GameObject _player, Type _type)
    {
        type = _type;
        player = _player;

        armObject = this.transform.Find("arm").gameObject;

        foreach(Transform child in armObject.transform)
        {
            tentacleObjects.Add(child.gameObject);
            heldJoints.Add(child.gameObject, null);
        }

        return this;
    }

    void Update()
    {
        isGrabing = Input.GetButton("Grab");

        switch(type)
        {
            case Type.grab:
                Grab();
                break;
        }
    }
    void Grab()
    {
        foreach (GameObject tentacleObject in tentacleObjects) {
            if (Physics2D.OverlapCircle(tentacleObject.transform.position, 0.25f, grabLayers) && isGrabing && heldObject == null && !PlayerController.inst.IsHoldingObject)
            {
                if (tentacleObject != null)
                {
                    Collider2D coll = Physics2D.OverlapCircle(tentacleObject.transform.position, 0.05f, grabLayers);
                    if (coll != null)
                    {
                        Debug.Log(tentacleObject.name + " grabbed");
                        PlayerController.inst.IsHoldingObject = true;
                        heldObject = coll.gameObject;
                        heldJoints[tentacleObject] = tentacleObject.AddComponent<HingeJoint2D>();
                        heldJoints[tentacleObject].connectedBody = heldObject.GetComponent<Rigidbody2D>();
                    }
                }
            }

            if (!isGrabing && heldJoints[tentacleObject] != null)
            {
                PlayerController.inst.IsHoldingObject = false;
                heldObject = null;
                Destroy(heldJoints[tentacleObject]);
                heldJoints[tentacleObject] = null;
            }
        }
    }
}
