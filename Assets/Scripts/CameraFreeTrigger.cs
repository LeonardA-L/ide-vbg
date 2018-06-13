using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CameraFreeTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(UnityEngine.Collider _collider)
        {
            if (_collider.gameObject.tag != GameManager.Constants.TAG_PLAYER)
                return;

            CameraManager.Instance.SetFreeSettings();
        }
    }
}