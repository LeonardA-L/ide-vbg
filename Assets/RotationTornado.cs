using UnityEngine;

public class RotationTornado : MonoBehaviour
{
    public float rotSpeed = 50;
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed, Space.World);
    }
}