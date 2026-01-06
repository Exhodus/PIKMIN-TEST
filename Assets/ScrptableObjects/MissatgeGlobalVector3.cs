using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MissatgeGlobalVector3", menuName = "Scriptable Objects/MissatgeGlobalVector3")]
public class MissatgeGlobalVector3 : ScriptableObject
{
    public event Action<Vector3> OnEvent;
    public void Raise(Vector3 vector) =>
        OnEvent?.Invoke(vector);
}
