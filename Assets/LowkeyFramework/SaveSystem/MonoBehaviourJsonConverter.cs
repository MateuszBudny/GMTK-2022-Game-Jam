using Newtonsoft.Json;
using System;
using UnityEngine;

public class MonoBehaviourJsonConverter : JsonConverter<MonoBehaviour>
{
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, MonoBehaviour value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override MonoBehaviour ReadJson(JsonReader reader, Type objectType, MonoBehaviour existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        Debug.Log("is existing value null: " + (existingValue == null) + ", has existing value: " + hasExistingValue);
        if(existingValue != null)
        {
            Debug.Log("existing value: " + existingValue.name);
        }
        return null;
    }
}
