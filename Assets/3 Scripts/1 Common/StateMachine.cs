using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : class
{
    private T ownerEntity;
    private IState<T> currentState;
    public IState<T> CurrentState => currentState;
    private IState<T> previousState;
    private IState<T> globalState;

    public void Setup(T owner, IState<T> entryState)
    {
        ownerEntity = owner;
        currentState = null;
        previousState = null;
        globalState = null;

        ChangeState(entryState);
    }

    public void Execute()
    {
        globalState?.Execute(ownerEntity);
        currentState?.Execute(ownerEntity);
    }

    public void ChangeState(IState<T> newState)
    {
        if (newState == null) return;

        if (currentState != null)
        {
            previousState = currentState;
            currentState?.Exit(ownerEntity);
        }

        currentState = newState;
        currentState.Enter(ownerEntity);
    }

    public void SetGlobalState(IState<T> newState)
    {
        if (newState == null) return;
        if (globalState != null)
            globalState?.Exit(ownerEntity);

        globalState = newState;
        globalState.Enter(ownerEntity);
    }

    public void RevertToPreviousState()
    {
        ChangeState(previousState);
    }
}
