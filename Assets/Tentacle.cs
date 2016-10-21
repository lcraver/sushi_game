using UnityEngine;
using System.Collections;

public class Tentacle : MonoBehaviour {

    GameObject player;
    public GameObject tentacleObject;

    public GameObject walkingPrefab;
    public GameObject blockbreakPrefab;
    public GameObject blockpullPrefab;
    public GameObject detachPrefab;

    public enum Type { walking1, walking2, blockbreak, blockpull, detach1, detach2 };
    public Type type = Type.walking1;
    
    public Tentacle(GameObject _player, Type _type)
    {
        type = _type;
        player = _player;

        switch(_type)
        {
            case Type.walking1:
                tentacleObject = Instantiate(walkingPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                break;
            case Type.walking2:
                tentacleObject = Instantiate(walkingPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                break;
            case Type.blockbreak:
                tentacleObject = Instantiate(blockbreakPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                break;
            case Type.blockpull:
                tentacleObject = Instantiate(blockpullPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                break;
            case Type.detach1:
                tentacleObject = Instantiate(detachPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                break;
            case Type.detach2:
                tentacleObject = Instantiate(detachPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                break;
        }

        player.GetComponent<PlayerController>().tentacles.Add(_type, this);
    }
}
