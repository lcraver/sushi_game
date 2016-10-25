using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bg : MonoBehaviour {

    int current = 0;
    public List<Sprite> sprites = new List<Sprite>();

	// Use this for initialization
	void Start () {
        InvokeRepeating("animate", 0, 0.28f);
	}
	
	// Update is called once per frame
	void animate() {
        current++;
        if (current > sprites.Count)
            current = 0;
        this.GetComponent<SpriteRenderer>().sprite = sprites[current];
	}
}
