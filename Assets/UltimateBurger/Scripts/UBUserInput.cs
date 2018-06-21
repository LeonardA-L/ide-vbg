using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vbg;

namespace ub
{
    public class UBUserInput : MonoBehaviour
    {
        public int controllerID = -1;


        public UBCharacterController.Request GetRequest()
        {
            UBCharacterController.Request req = new UBCharacterController.Request();

            float LeftThruster = Input.GetAxis("Modifier" + GetControllersuffix());
            float RightThruster = Input.GetAxis("Attack_ALT" + GetControllersuffix());

            req.LThruster = LeftThruster;
            req.RThruster = -RightThruster;

            return req;
        }

        public void SetController(int _controllerID)
        {
            controllerID = _controllerID;
        }
        string GetControllersuffix()
        {
            return "_P" + controllerID;
        }
    }
}