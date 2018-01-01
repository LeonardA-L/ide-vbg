using UnityEngine;

public class Collider : MonoBehaviour {
	void Start () {
        GetComponent<MeshRenderer>().enabled = GameManager.Debug.SHOW_COLLIDERS_INGAME;
	}
}
