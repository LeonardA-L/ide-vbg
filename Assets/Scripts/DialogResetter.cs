using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class DialogResetter : MonoBehaviour
    {
        public List<GameObject> dialogs = new List<GameObject>();

        public void Reset()
        {
            foreach (GameObject go in dialogs)
            {
                go.SetActive(false);
            }

            SwitchManager.Instance.SetSwitch("Dialog_Zeus_1", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_2", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_3", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_4", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_5", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_6", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_8", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_10", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_11", false);
            SwitchManager.Instance.SetSwitch("Dialog_Zeus_12", false);
        }

    }
}