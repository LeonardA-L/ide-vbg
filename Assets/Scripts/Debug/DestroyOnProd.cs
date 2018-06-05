using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnProd : MonoBehaviour {
	void Start () {
        if (!Debug.isDebugBuild)
        {
            Destroy(gameObject);
        }
    }
}
