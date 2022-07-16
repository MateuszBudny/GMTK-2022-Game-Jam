using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LowkeyFramework.AttributeSaveSystem
{
    public class ReferenceableScriptableObjectJsonConverter : JsonConverter<ReferenceableScriptableObject>
    {
        /// <summary>
        /// Key: string GUID<br></br>
        /// Value: ReferenceableScriptableObject
        /// </summary>
        private readonly Dictionary<string, ReferenceableScriptableObject> allReferenceableScriptableObjects;

        public ReferenceableScriptableObjectJsonConverter() : base()
        {
            allReferenceableScriptableObjects = GetReferenceableScriptableObjects().ToDictionary(saveableSO => saveableSO.Guid);
        }

        public override void WriteJson(JsonWriter writer, ReferenceableScriptableObject value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Guid);
        }

        public override ReferenceableScriptableObject ReadJson(JsonReader reader, Type objectType, ReferenceableScriptableObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string guid = (string)reader.Value;
            return string.IsNullOrEmpty(guid) ? null : allReferenceableScriptableObjects[guid];
        }

        private List<ReferenceableScriptableObject> GetReferenceableScriptableObjects()
        {
            return Resources.FindObjectsOfTypeAll(typeof(ReferenceableScriptableObject))
                .Select(referenceableSO => (ReferenceableScriptableObject)referenceableSO)
                .ToList();
        }
    }
}