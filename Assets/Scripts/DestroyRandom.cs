using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyRandom : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    public float probability = 0.0f;

	// Use this for initialization
	void Start () {
        float rand = Random.Range(0.0f, 1.0f);
        if(rand < probability)
        {
            Destroy(gameObject);
        }
	}
}
