using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneContainerController : MonoBehaviour {
    private bool magnetized = false;
    public Transform target;
    public float YOffest = 0;
    public void Magnetize ()
    {
        magnetized = true;
   
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (magnetized)
        {
            transform.position = target.position;
            transform.position += new Vector3(0, YOffest, 0);
        }
	}
}
