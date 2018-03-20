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
        protected static SoundManager instance;
        public static SoundManager Instance
        {
            get
            {
                return instance;
            }
        }

        public bool wwiseIsActivated = false;

        public bool NamespaceExists(string className)
        {
            Type type = Type.GetType(className);
            return type != null;
        }

        private void Start()
        {
            instance = this;
            wwiseIsActivated = NamespaceExists("AkSoundEngine");
            Debug.Log("Wwise is activated: " + wwiseIsActivated);

            //AkSoundEngine.IsInitialized();
        }

        public static void PostEvent(string _eventName, GameObject _gameObject)
        {
            AkSoundEngine.PostEvent(_eventName, _gameObject);
        }

    }
}