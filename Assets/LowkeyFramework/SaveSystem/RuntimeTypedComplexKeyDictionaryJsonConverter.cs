using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LowkeyFramework.AttributeSaveSystem
{
    public class RuntimeTypedComplexKeyDictionaryJsonConverter : JsonConverter
    {
        private Type keysType;
        private Type valuesType;

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keysType = objectType.GetGenericArguments()[0];
                if (!IsPrimitive(keysType))
                {
                    this.keysType = keysType;
                    valuesType = objectType.GetGenericArguments()[1];

                    return true;
                }
            }

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            InvokeMethodFromTypedComplexKeyDictionaryJsonConverter(nameof(WriteJson), writer, value, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return InvokeMethodFromTypedComplexKeyDictionaryJsonConverter(nameof(ReadJson), reader, objectType, existingValue, serializer);
        }

        private object InvokeMethodFromTypedComplexKeyDictionaryJsonConverter(string methodName, params object[] methodArgs)
        {
            Type genericType = typeof(ComplexKeyDictionaryJsonConverter<,>).MakeGenericType(new Type[] { keysType, valuesType });
            object genericDict = Activator.CreateInstance(genericType);
            return genericType.GetMethod(methodName).Invoke(genericDict, methodArgs);
        }

        private bool IsPrimitive(Type type) =>
            type.IsPrimitive ||
            new Type[] {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
    }
}