using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Specific Tag")]
    public class SpecificTag : GameEffectActivate
    {
        [Tooltip("Only Activator with one of these tags will be able to activate this GameEffect")]
        public List<string> allowedTags;
        [Tooltip("Invert the list to make an exclude filter")]
        public bool revertList = false;

        void Start()
        {
        }

        public override bool IsActive(IDynamic idy)
        {
            if (idy == null)
            {
                return false;
            }

            VBGCharacterController cc = idy as VBGCharacterController;
            Dynamic dy = idy as Dynamic;

            GameObject go = cc != null ? cc.gameObject : dy.gameObject;

            bool ret = false;

            if(revertList)
            {
                ret = true;
                foreach (string t in allowedTags)
                {
                    ret &= go.tag != t;
                }
            } else
            {
                foreach (string t in allowedTags)
                {
                    ret |= go.tag == t;
                }
            }

            return ret;
        }
    }
}