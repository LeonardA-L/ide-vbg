using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SoundPoster : MonoBehaviour
    {

        public void PostAudioEvent(string _eventName)
        {
            if (_eventName != null && _eventName != "")
                SoundManager.Instance.PostEvent(_eventName, this.gameObject);
        }
    }
}