using System;
using UnityEngine;

[Serializable]
public abstract class SerializableWithValidation : ISerializationCallbackReceiver
{
    protected virtual void OnValidate() { }
    void ISerializationCallbackReceiver.OnBeforeSerialize() => OnValidate();
    void ISerializationCallbackReceiver.OnAfterDeserialize() { }
}
