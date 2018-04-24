using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class Credit : MonoBehaviour {
    private string creditStringID = null;
    private int creditID = -1;

#if UNITY_EDITOR
    void Awake()
    {
        //Debug.Log("Editor causes this Awake");
        GenerateID();
    }
#endif
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        //Debug.Log(creditStringID);
        GenerateID();
#endif
    }

    private void GenerateID()
    {
        if(creditStringID == null || creditStringID == "")
        {
            Credit[] credits = FindObjectsOfType(typeof(Credit)) as Credit[];

            int max = -1;

            foreach(Credit credit in credits)
            {
                if(credit == this)
                {
                    continue;
                }

                max = Mathf.Max(max, credit.creditID);
            }

            max++;

            Scene scene = SceneManager.GetActiveScene();
            creditStringID = scene.name + "_" + max;
            creditID = max;
        }
    }
}
