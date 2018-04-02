using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace vbg
{

    public class SoundEvent
    {
        public static readonly string HEPHAISTOS_SIMPLE_ATK = "Event_HEPHA_ATK";
    }

    public class SoundManager : MonoBehaviour
    {
        public bool disable = false;

        protected static SoundManager instance;
        public static SoundManager Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            instance = this;
        }

        public void PostEvent(string _eventName, GameObject _gameObject)
        {
            if (!AkSoundEngine.IsInitialized())
                return;

            if(disable)
                return;

            AkSoundEngine.PostEvent(_eventName, _gameObject);
        }

    }
}