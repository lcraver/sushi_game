using UnityEngine;
using System.Collections;

public class ArmPartCollision : MonoBehaviour {

    public GameObject tentacleMaster;

    void Awake()
    {
        tentacleMaster = this.transform.parent.parent.gameObject;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player" && !tentacleMaster.GetComponent<CollectableTentacle>().collected)
        {
            PlayerController.inst.PlayTentacleGetAnimation(tentacleMaster.GetComponent<CollectableTentacle>().type, tentacleMaster.GetComponent<CollectableTentacle>().rot);
            Destroy(tentacleMaster);
        }
    }
}
