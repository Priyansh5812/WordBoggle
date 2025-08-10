using System;
using System.Collections.Generic;
using UnityEngine;
using WordBoggle;

public static class EventManager
{   
    public static FuncEvent<string> OnGetRandomWord
    {
        get; private set;
    } = new();

    public static FuncEvent<string , bool> OnSearchWordInTree
    {
        get; private set;
    } = new();

    public static ActionEvent<LetterTile> OnSelectionStarted
    {
        get; private set;
    } = new();

    public static ActionEvent<LetterTile> OnSelect
    {
        get; private set;
    } = new();

    public static ActionEvent OnSelectionEnded
    {
        get; private set;
    } = new();

    public static FuncEvent<string , WordValidationType> OnValidateWord
    {
        get; private set;
    } = new();

    public static ActionEvent<List<LetterTile>> OnValidWordSelected
    {
        get; private set;
    } = new();

    public static ActionEvent OnExistingWordSelected
    {
        get; private set;
    } = new();

    public static ActionEvent OnInvalidWordSelected
    {
        get; private set;
    } = new();

    public static ActionEvent OnGameRestart
    {
        get; private set;
    } = new();

    public static FuncEvent<bool> IsGameOver
    {
        get; private set;
    } = new();

    public static FuncEvent<LetterTile, List<LetterTile>> GetBlockedNeighbours
    {
        get; private set;
    } = new();

    public static FuncEvent<LetterTile, LetterTile, bool> AreNeighbours
    {
        get; private set;
    } = new();
    public static FuncEvent<(int,int) , LetterTile> GetTileFromGrid
    {
        get; private set;
    } = new();

}

public class ActionEvent
{
    private event Action baseAction;

    public void Invoke() => baseAction?.Invoke();

    public void AddListener(Action action) => baseAction += action;

    public void RemoveListener(Action action) => baseAction -= action;

    public bool IsListenerExist(Action action)
    {
        Delegate[] delegates = baseAction.GetInvocationList();

        foreach (var i in delegates)
        {
            if (i.Equals(action))
            {
                return true;
            }

        }

        return false;
    }
}
public class ActionEvent<T1>
{
    private event Action<T1> baseAction;

    public void Invoke(T1 value) => baseAction?.Invoke(value);

    public void AddListener(Action<T1> action) => baseAction += action;

    public void RemoveListener(Action<T1> action) => baseAction -= action;

    public bool IsListenerExist(Action<T1> action)
    {
        Delegate[] delegates = baseAction.GetInvocationList();

        foreach (var i in delegates)
        {
            if (i.Equals(action))
            {
                return true;
            }

        }

        return false;
    }
}
public class ActionEvent<T1, T2>
{
    private event Action<T1, T2> baseAction;

    public void Invoke(T1 value_1 , T2 value_2) => baseAction?.Invoke(value_1 , value_2);
    public void AddListener(Action<T1, T2> action) => baseAction += action;

    public void RemoveListener(Action<T1, T2> action) => baseAction -= action;

    public bool IsListenerExist(Action<T1, T2> action)
    {
        Delegate[] delegates = baseAction.GetInvocationList();

        foreach (var i in delegates)
        {
            if (i.Equals(action))
            {
                return true;
            }

        }

        return false;
    }


}

public class FuncEvent<T1>
{
    private event Func<T1> baseFunc;

    public T1 Invoke() => baseFunc.Invoke();

    public void AddListener(Func<T1> action) => baseFunc += action;

    public void RemoveListener(Func<T1> action) => baseFunc -= action;

    public bool IsListenerExist(Func<T1> action)
    {
        Delegate[] delegates = baseFunc.GetInvocationList();

        foreach (var i in delegates)
        {
            if (i.Equals(action))
            {
                return true;
            }

        }

        return false;
    }
}

public class FuncEvent<T1 , T2>
{
    private event Func<T1 , T2> baseFunc;

    public T2 Invoke(T1 value) => baseFunc.Invoke(value);

    public void AddListener(Func<T1 , T2> action) => baseFunc += action;

    public void RemoveListener(Func<T1 , T2> action) => baseFunc -= action;

    public bool IsListenerExist(Func<T1 , T2> action)
    {
        Delegate[] delegates = baseFunc.GetInvocationList();

        foreach (var i in delegates)
        {
            if (i.Equals(action))
            {
                return true;
            }

        }

        return false;
    }
}

public class FuncEvent<T1, T2, T3>
{
    private event Func<T1, T2, T3> baseFunc;

    public T3 Invoke(T1 value_1, T2 value_2) => baseFunc.Invoke(value_1, value_2);

    public void AddListener(Func<T1, T2, T3> action) => baseFunc += action;

    public void RemoveListener(Func<T1, T2, T3> action) => baseFunc -= action;

    public bool IsListenerExist(Func<T1, T2, T3> action)
    {
        Delegate[] delegates = baseFunc.GetInvocationList();

        foreach (var i in delegates)
        {
            if (i.Equals(action))
            {
                return true;
            }

        }

        return false;
    }
}




