using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

namespace vbg
{
    public class MenuButtonBehaviour : MonoBehaviour, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            SoundManager.Instance.PostEvent("Play_UI_Move", gameObject);
        }
    }
}