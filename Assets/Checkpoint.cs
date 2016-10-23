using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public Vector3 pos;

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("Set Checkpoint");
        if (coll.gameObject.tag == "Player")
        {
            PlayerController.inst.SetCheckpoint(this.transform.position + pos);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(this.transform.position + pos, new Vector3(0.1f, 0.1f, 0.1f));
    }
}
