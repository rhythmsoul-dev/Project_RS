using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBasedAI<T> : Entity where T : IConvertible
{
    /// <summary>
    /// 현재 상태
    /// </summary>
    protected T CurState
    {
        get
        {
            return curState;
        }
        set
        {
            TransitionTo(value, false);
        }
    }

    /// <summary>
    /// 이전 상태
    /// </summary>
    protected T PrevState => prevState;

    /// <summary>
    /// FSM이 중단되었는지 여부
    /// </summary>
    public bool IsInterrupted { get; set; }

    protected abstract T InvalidState { get; }
    protected abstract int StateEnumCount { get; }

    private static readonly EqualityComparer<T> comparer = EqualityComparer<T>.Default;
    private readonly Dictionary<T, StateElem> states = new Dictionary<T, StateElem>();

    private T curState;
    private T prevState;

    /// <summary>
    /// 상태 조건별 이벤트 함수
    /// </summary>
    protected class StateElem
    {
        public Action Entered;
        public Func<IEnumerator> Doing;
        public Action Exited;
    }

    /// <summary>
    /// 상태 변경 내부 메서드
    /// </summary>
    /// <param name="nextState">다음 상태</param>
    /// <param name="force">강제 변경 여부 (현재 상태를 즉시 중단 후 변경)</param>
    protected void TransitionTo(T nextState, bool force = false)
    {
        if (!force && IsAIEnded())
        {
            return;
        }

        IsInterrupted = (!IsTerminalState(curState) || force);
        if (!force && IsTerminalState(curState))
        {
            return;
        }

        prevState = curState;
        curState = nextState;
        if (!comparer.Equals(prevState, InvalidState))
        {
            StateElem stateElem = states.Get(prevState, null);
            if (stateElem != null && stateElem.Exited != null)
            {
                stateElem.Exited();
            }
        }

        if (!comparer.Equals(curState, InvalidState))
        {
            StateElem stateElem = states.Get(curState, null);
            if (stateElem != null && stateElem.Entered != null)
            {
                stateElem.Entered();
            }
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        curState = InvalidState;
        prevState = InvalidState;
        DefineStates();
    }

    protected override IEnumerator OnStart()
    {
        yield return StartCoroutine(base.OnStart());

        // 현재 상태에 정의되어 있는 Doing 이벤트 함수(코루탄)를 반복 실행
        while (!IsAIEnded())
        {
            IsInterrupted = false;
            yield return StartCoroutine(OnBeforeDoingState());
            StateElem state = states.Get(curState, null);
            if (state != null)
            {
                if (state.Doing == null)
                {
                    while (!IsInterrupted)
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return StartCoroutine(state.Doing());
                }
            }
            yield return StartCoroutine(OnAfterDoingState());
            yield return null;
        }

        yield break;
    }

    protected abstract void DefineStates();

    protected virtual IEnumerator OnBeforeDoingState()
    {
        yield break;
    }

    protected virtual IEnumerator OnAfterDoingState()
    {
        yield break;
    }

    protected abstract bool IsAIEnded();

    protected abstract bool IsTerminalState(T state);

    protected void AddState(T state, StateElem stateElem)
    {
        states.Add(state, stateElem);
    }
}
