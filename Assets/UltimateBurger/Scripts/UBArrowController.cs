using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UBArrowController : MonoBehaviour {
    public string ennemy = "SushiPlayer";
    public float lerp = 0.02f;
    private Transform ennemyGo;
	// Use this for initialization
	void Start () {
        ennemyGo = GameObject.Find(ennemy).transform;
        Debug.Log(ennemyGo);

    }
	
	// Update is called once per frame
	void Update () {
        Vector3 diff = ennemyGo.position - transform.position;
        diff.Normalize();
        transform.forward = Vector3.Slerp(transform.forward, diff, lerp);

	}
}
