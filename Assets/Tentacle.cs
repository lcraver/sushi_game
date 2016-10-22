using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Tentacle : MonoBehaviour {

    GameObject player;
    public GameObject armObject;
    public List<GameObject> tentacleObjects = new List<GameObject>();

    public enum Type { walking1, walking2, blockbreak, blockpull, detach1, detach2 };
    public Type type = Type.walking1;
    
    public Tentacle InitTentacle(GameObject _player, Type _type)
    {
        type = _type;
        player = _player;

        armObject = this.transform.Find("arm").gameObject;

        foreach(Transform child in armObject.transform)
        {
            tentacleObjects.Add(child.gameObject);
        }

        return this;
    }
}
