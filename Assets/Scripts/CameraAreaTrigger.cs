using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CameraAreaTrigger : MonoBehaviour
    {

        private void OnTriggerEnter(UnityEngine.Collider _collider)
        {
            Debug.Log("Entered");
            Transform cameraSettings = transform.Find("Camera");
            Debug.Log(cameraSettings.rotation);

            CameraManager.Instance.SetSceneSettings(cameraSettings);
        }
    }
}