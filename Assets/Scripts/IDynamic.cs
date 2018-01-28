using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public interface IDynamic
    {
        void RegisterGameEffect(GameEffect ge);
        void UnRegisterGameEffect(GameEffect ge);
    }
}