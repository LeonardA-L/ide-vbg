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
            //bool attackPressed = Input.GetButton("Attack" + GetControllersuffix()) || axisAttackPressed;

            bool brakeL = Input.GetButton("UB_Brake_L" + GetControllersuffix());
            bool brakeR = Input.GetButton("UB_Brake_R" + GetControllersuffix());

            req.LThruster = LeftThruster;
            req.RThruster = -RightThruster;

            req.LBrake = brakeL;
            req.RBrake = brakeR;

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