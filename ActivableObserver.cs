using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// ANDRES

/// <summary>
/// A component that looks on a <see cref="Activator"/> list and 
/// activates a list of <see cref="Activable"/>. It can also send a Unity Event.
/// </summary>
public class ActivableObserver : Activable
{
    public enum Condition { AllActivated, OneActivated }
    public enum Disposal { AlwaysUpdate, StopChecking, StopThenDestroy }

    //:: Input
    [Header("Input")]
    [Tooltip("Activables to watch.")]
    public List<Activable> observed;
    public Condition activationCondition;

    // :: Output
    [Header("Output")]
    [Tooltip("State to be set to all activables.")]
    public Activable.State sendState = Activable.State.Activation;
    [Tooltip("Activables to modify.")]
    public List<Activable> activables;
    // :: Other events
    [Tooltip("Other free events to be executed when this is Activated")]
    public UnityEvent OnExecute;
    // ::
    [Tooltip("Behaviour of this Observer becomes Active. \n" +
        "Always Update: Keeps checking the activators condition.\n" +
        "Stop Checking: Become unreactive to any change.\n" +
        "StopThenDestroy: Destroys this game object.\n")]
    public Disposal afterCondition = Disposal.AlwaysUpdate;

    //[ConditionalField("afterCondition", Disposal.KeepChecking)]
    [Tooltip("Only with AlwaysUpdate. The state to send when the condition becomes false.")]
    public Activable.State resetState = Activable.State.Deactivation;

    [HideInInspector]
    private bool destroyRequest;

    /// <summary>
    /// Launches output directly without checking the Inputs
    /// </summary>
    public void Execute()
    {
        IsActivated = true;
        OnExecute?.Invoke();
        ChangeStateForAll(sendState);
    }

    private void OnEnable() => OnStateEnter += StateEnter;
    private void OnDisable() => OnStateEnter -= StateEnter;

    private void StateEnter(State state)
    {
        switch (state)
        {
            case State.Deactivation:
            case State.Activation:
                NextState();
                break;
            default:
                break;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!IsActivated)
        {
            if (GetCondition())
            {
                Execute();
            }
        }
        else
        {
            switch (afterCondition)
            {
                case Disposal.AlwaysUpdate:
                    if (!GetCondition())
                    {
                        ChangeStateForAll(resetState);
                        IsActivated = false;
                    }
                    break;
                case Disposal.StopChecking:
                    // - nothing...
                    break;
                case Disposal.StopThenDestroy:
                    destroyRequest = true;
                    break;
                default:
                    break;
            }
        }
        // Destroy next frame just in case
        if (destroyRequest)
        {
            Destroy(gameObject);
        }
    }

    bool GetCondition()
    {
        switch (activationCondition)
        {
            case Condition.AllActivated:
                return AreAllTrue();
            case Condition.OneActivated:
                return IsOneTrue();
            default:
                return false;
        }
    }

    #region Conditions
    bool IsOneTrue()
    {
        for (int i = 0; i < observed.Count; i++)
        {
            if (observed[i] == null) { continue; }
            if (observed[i].IsActivated)
            {
                return true;
            }
        }
        return false;
    }

    bool AreAllTrue()
    {
        for (int i = 0; i < observed.Count; i++)
        {
            if (observed[i] == null) { continue; }
            if (!observed[i].IsActivated)
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    void ChangeStateForAll(Activable.State state)
    {
        for (int i = 0; i < activables.Count; i++)
        {
            activables[i].CurrentState = state;
        }
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < observed.Count; i++)
        {
            if (observed[i] == null) { continue; }
            Gizmos.color = observed[i].IsActivated ? Color.green : Color.red;
            Vector3 targetPos = observed[i].transform.position;
            Gizmos.DrawLine(transform.position, targetPos);
            Gizmos.DrawSphere(targetPos, 0.2f);
        }
        // Draw a line from this to activables
        for (int i = 0; i < activables.Count; i++)
        {
            if (activables[i] == null) { continue; }
            //Gizmos.color = !IsActivated ? Color.grey : Color.green;
            Gizmos.color = !GetCondition() ? Color.black : GetStateColor(sendState);
            Vector3 targetPos = activables[i].transform.position;
            Gizmos.DrawLine(transform.position, targetPos);
            Gizmos.DrawSphere(targetPos, 0.2f);
        }
    }

}
