using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MissatgeGlobal", menuName = "Scriptable Objects/MissatgeGlobal")]
public class MissatgeGlobal : ScriptableObject
{
    public event Action OnEvent;
    public void Raise() =>
        OnEvent?.Invoke();

}
