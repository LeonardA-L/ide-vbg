using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRatio : StateMachineBehaviour {

    public string floatName = "Ratio";

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.speed = 0.0f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float ratio = animator.GetFloat(floatName);
        animator.ForceStateNormalizedTime(ratio);
	}
}
