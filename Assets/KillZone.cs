using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("Hit KillZone");
        if (coll.gameObject.tag == "Player")
        {
            PlayerController.inst.DeleteTentacles();
            AudioManager.inst.PlaySound("grill");
        }
    }
}
