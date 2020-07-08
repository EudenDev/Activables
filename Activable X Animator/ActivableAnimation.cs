using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ANDRES

/// <summary>
/// Component that launches an animation trigger according to its state.
/// </summary>
public class ActivableAnimation : Activable
{
    [Tooltip("Optional reference")]
    public Animator animator;
    [Space]
    public string inactiveTrigger = "Inactive";
    public string activationTrigger = "Activation";
    public string activeTrigger = "Active";
    public string deactivationTrigger = "Deactivation";

    // :: Hashes
    private int inactiveID;
    private int activationID;
    private int activeID;
    private int deactivationID;

    protected override void Awake()
    {
        base.Awake();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        inactiveID = Animator.StringToHash(inactiveTrigger);
        activationID = Animator.StringToHash(activationTrigger);
        activeID = Animator.StringToHash(activeTrigger);
        deactivationID = Animator.StringToHash(deactivationTrigger);
    }

    private void OnEnable() => OnStateEnter += StateEnter;
    private void OnDisable() => OnStateEnter -= StateEnter;

    private void StateEnter(State state)
    {
        switch (state)
        {
            case State.Deactivation:    // - Called when entering Deactivation state
                animator.SetTrigger(deactivationID);
                break;
            case State.Inactive:        // - Called when entering the Idle state
                animator.SetTrigger(inactiveID);
                break;
            case State.Activation:      // - Called when entering Activation state
                animator.SetTrigger(activationID);
                break;
            case State.Active:          // - Called when entering Active state
                animator.SetTrigger(activeID);
                break;
            default:
                break;
        }
    }
}
