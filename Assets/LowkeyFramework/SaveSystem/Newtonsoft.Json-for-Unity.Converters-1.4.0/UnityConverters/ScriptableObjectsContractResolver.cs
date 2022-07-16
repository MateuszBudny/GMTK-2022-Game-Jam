using Newtonsoft.Json.Serialization;
using System;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters
{
    // class added for SaveSystem
    public class ScriptableObjectsContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract jsonObjectContract = base.CreateObjectContract(objectType);

            if(typeof(ScriptableObject).IsAssignableFrom(objectType))
            {
                jsonObjectContract.DefaultCreator = () =>
                {
                    return ScriptableObject.CreateInstance(objectType);
                };
            }

            return jsonObjectContract;
        }
    }
}