using System;
using UnityEngine;
// ANDRES

/// <summary>
/// Adds a 4 steps state machine. This also uses <see cref="IActivable"/>
/// </summary>
public class Activable : MonoBehaviour, IActivable
{
    public enum State { Inactive, Activation, Active, Deactivation, }
    // :: State Machine
    [SerializeField]
    [Tooltip("Initial state of this component when starting the scene.\n On runtime, this is the current state.")]
    private State _state;

    // TODO: ++ Is Active public Event?

    /// <summary>
    /// The State of this <see cref="Activable"/>, modify it to launch
    /// state change events.
    /// </summary>
    public State CurrentState
    {
        get {
            return _state;
        }
        set {
            _state = value;
            OnStateEnter?.Invoke(_state);
        }
    }
    /// <summary>
    /// Subscribe to this to receive calls the frame the state has changed.
    /// </summary>
    public Action<State> OnStateEnter;
    /// <summary>
    /// Subscribe to this to receive calls every frame with the current state.
    /// </summary>
    public Action<State> OnUpdate;


    public bool IsActivated
    {
        get {
            return _state == State.Active || _state == State.Deactivation;
            //return _state != State.Inactive;
        }
        set {
            if (_state == State.Active || _state == State.Inactive)
            {
                CurrentState = value ? State.Activation : State.Deactivation;
            }
        }
    }

    /// <summary>
    /// Moves the current state to the next one
    /// </summary>
    public void NextState()
    {
        CurrentState = (State)(((int)CurrentState + 1) % 4);
    }

    #region IActivable Methods

    public virtual void Activate() => CurrentState = State.Activation;

    public virtual void Deactivate() => CurrentState = State.Deactivation;

    public virtual void ActivateInmediate() => CurrentState = State.Active;

    public virtual void DeactivateInmediate() => CurrentState = State.Inactive;

    #endregion

    // :: UNITY methods

    // - 
    protected virtual void Awake() { CurrentState = _state; }

    // Cannot get update if component/object is deactivated.
    protected virtual void Update()
    {
        OnUpdate?.Invoke(_state);
    }
#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Color gizmoColor = GetStateColor(_state);
        gizmoColor.a = 0.6f;
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 1f);
    }

    protected Color GetStateColor(State state)
    {
        switch (state)
        {
            case State.Activation:
                return Color.HSVToRGB(0.09f, 0.84f, 1.0f);           
            case State.Active:
                return Color.green;            
            case State.Deactivation:
                return Color.red;            
            case State.Inactive:
                return Color.gray;
            default:
                return Color.black;            
        }
    }
#endif
}
