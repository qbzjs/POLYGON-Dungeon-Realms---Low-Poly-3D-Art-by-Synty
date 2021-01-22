using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<Type, BaseState> _availableStates;

    public BaseState CurrentState { get; private set; }
    public event Action<BaseState> OnStateChanged;

    public void SetStates(Dictionary<Type, BaseState> states)
    {
        _availableStates = states;
    }

    private void Update()
    {
        if (CurrentState == null)
        {
            if (_availableStates == null)
            {
                return;
            }
            CurrentState = _availableStates.Values.First();
            
        }
        // Active l'état en cours en appelant la fonction Tick.
        Type nextState = CurrentState?.Tick();
        if (nextState != null && nextState != CurrentState?.GetType())
        {
            SwitchToNewState(nextState);
        }
    }
    /// <summary>
    /// Invoque le nouvel état
    /// </summary>
    /// <param name="nextState">nouvel état</param>
    private void SwitchToNewState(Type nextState)
    {
        CurrentState = _availableStates[nextState];
        OnStateChanged?.Invoke(CurrentState);

    }
}
