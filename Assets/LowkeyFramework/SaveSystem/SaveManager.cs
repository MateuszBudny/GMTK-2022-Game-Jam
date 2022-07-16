using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LowkeyFramework.AttributeSaveSystem
{
    public class SaveManager : SingleBehaviour<SaveManager>
    {
        // TODO:
        // is Newtonsoft using serialized fields only? Or is it ignoring Unity serialization? - it looks like it is ignoring serialization, but if field is set as [NonSerialized], then it is not added in json
        // properties also should have possibility to set them as [SaveField]
        // add virtual function to Saveables abstract to add some way of extending this system, if someone would like to save Transform or other Unity component. Or add other way to do that, but for now I think virtual function would be the best solution. EDIT: Custom JsonConverters for Unity (there is such a github project) should do just fine.
        // clearing saves from Unity window
        // displaying contents of save files in Inspector
        // editing contents of save files in Inspector

        [SerializeField]
        private string currentSaveFile = "save.json";
        public string CurrentSaveFile { get => currentSaveFile; set => currentSaveFile = value; }

        [Tooltip("If true, then new save is based on a previous save, so save is updated by new or changed values, but values that were present in a previous save and are not present in a new save, are not deleted from save. If false, then new save is based on an empty save, so all values not present in a new save are deleted.")]
        [SerializeField]
        private bool appendSaves = true;
        /// <summary>
        /// If true, then new save is based on a previous save, so save is updated by new or changed values, but values that were present in a previous save and are not present in a new save, are not deleted from save. If false, then new save is based on an empty save, so all values not present in a new save will be deleted.
        /// </summary>
        public bool AppendSaves { get => appendSaves; set => appendSaves = value; }

        /// <summary>
        /// Value is used for encrypting save file with xor method, if key is changed all previous saves generated before changing key will be inacessible
        /// </summary>
        public string key;

        [SerializeField]
        private bool log = true;

        public bool Log { get => log; set => log = value; }

        /// <summary>
        /// An event invoked before saving is started.
        /// </summary>
        public event Action OnBeforeSave;
        /// <summary>
        /// An event happening after saving is finished. Bool variable defines if saving is successful or not.
        /// </summary>
        public event Action<bool> OnAfterSave;
        /// <summary>
        /// An event happening before loading is started.
        /// </summary>
        public event Action OnBeforeLoad;
        /// <summary>
        /// An event happening after loading is finished. Bool variable defines if loading is successful or not.
        /// </summary>
        public event Action<bool> OnAfterLoad;

        /// <summary>
        /// Save SaveFields to JSON file.
        /// </summary>
        public void Save(string saveFileName = "", bool encode = false)
        {
            OnBeforeSave?.Invoke();
            string jsonSaveFileName = GetJsonSaveFileName(saveFileName);

            bool makeBackUp = FileManager.FileExists(jsonSaveFileName);

            // Dictionary<behaviour's GUID, Dictionary<field's name, field as object>> 
            Dictionary<string, Dictionary<string, object>> saveDictionary;
            if(appendSaves && LoadDecodeSaveFile(jsonSaveFileName, out string saveJson))
            {
                saveDictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(saveJson);
            }
            else
            {
                saveDictionary = new Dictionary<string, Dictionary<string, object>>();
            }

            ForEachSaveField(
                forEachSaveableBehaviour: behaviour =>
                {
                    saveDictionary.Remove(behaviour.Guid);
                },
                forEachSaveField: (behaviour, _, memberInfo) =>
                {
                    if(!saveDictionary.ContainsKey(behaviour.Guid))
                    {
                        saveDictionary.Add(behaviour.Guid, new Dictionary<string, object>());
                        saveDictionary[behaviour.Guid].Add("name", behaviour.name);
                    }

                    saveDictionary[behaviour.Guid].Add(memberInfo.Name, GetMemberValue(memberInfo, behaviour));
                });

            string jsonSave = JsonConvert.SerializeObject(saveDictionary, Formatting.Indented);

            if(makeBackUp)
            {
                FileManager.MoveFile(jsonSaveFileName, jsonSaveFileName + ".bak");
            }

            jsonSave = encode ? EncodeString(jsonSave, key) : jsonSave;

            if(FileManager.WriteToFile(jsonSaveFileName, jsonSave))
            {
                if(Log)
                {
                    Debug.Log("Save successful: " + jsonSaveFileName);
                }
                OnAfterSave?.Invoke(true);
            }
            else
            {
                OnAfterSave?.Invoke(false);
            }
        }

        /// <summary>
        /// Load from JSON file and update SaveFields.
        /// </summary>
        /// <param name="saveFileName"></param>
        public void Load(string saveFileName = "")
        {
            OnBeforeLoad?.Invoke();
            string jsonSaveFileName = GetJsonSaveFileName(saveFileName);

            if(LoadDecodeSaveFile(jsonSaveFileName, out string saveJson))
            {
                // Dictionary<behaviour's GUID, Dictionary<field's name, field as object>> 
                Dictionary<string, Dictionary<string, object>> saveDictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(saveJson);
                ForEachSaveField((behaviour, _, memberInfo) =>
                {
                    string guid = behaviour.Guid;
                    string memberName = memberInfo.Name;
                    if(saveDictionary.TryGetValue(guid, out Dictionary<string, object> savedFieldsDictionary))
                    {
                        if(savedFieldsDictionary.ContainsKey(memberName))
                        {
                            object fieldSavedValue = savedFieldsDictionary[memberName];
                            Type underlyingType = GetUnderlyingType(memberInfo);
                            if(fieldSavedValue != null)
                            {
                                object fieldSavedValueAfterConvertion = (fieldSavedValue as JToken).ToObject(underlyingType);
                                SetMemberValue(memberInfo, fieldSavedValueAfterConvertion, behaviour);
                            }
                            else
                            {
                                SetMemberValue(memberInfo, null, behaviour);
                                if(Log)
                                {
                                    Debug.Log(memberName + " from save '" + jsonSaveFileName + "' has a null value.");
                                }
                            }
                        }
                    }
                });

                if(Log)
                {
                    Debug.Log("Load successful: " + jsonSaveFileName);
                }
                OnAfterLoad?.Invoke(true);
            }
            else
            {
                if(Log)
                {
                    Debug.LogWarning("Save file is not in a valid format or does not exist. Save path: " + jsonSaveFileName);
                }
                OnAfterLoad?.Invoke(false);
            }
        }

        private void ForEachSaveField(Action<SaveableBehaviour, SaveFieldAttribute, MemberInfo> forEachSaveField, Action<SaveableBehaviour> forEachSaveableBehaviour = null)
        {
            List<SaveableBehaviour> behaviours = GetSaveableBehaviours();
            behaviours.ForEach(behaviour =>
            {
                forEachSaveableBehaviour?.Invoke(behaviour);
                IEnumerable<MemberInfo> objectFields = behaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Cast<MemberInfo>();
                IEnumerable<MemberInfo> objectProperties = behaviour.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Cast<MemberInfo>();
                List<MemberInfo> allObjectFieldsAndProperties = objectFields.Concat(objectProperties).ToList();

                allObjectFieldsAndProperties.ForEach(memberInfo =>
                {
                    SaveFieldAttribute saveField = memberInfo.GetCustomAttribute<SaveFieldAttribute>();
                    if(saveField != null)
                    {
                        forEachSaveField(behaviour, saveField, memberInfo);
                    }
                });
            });
        }

        private List<SaveableBehaviour> GetSaveableBehaviours()
        {
            return Resources.FindObjectsOfTypeAll(typeof(SaveableBehaviour))
                .Select(behaviour => (SaveableBehaviour)behaviour)
                .Where(behaviour => IsGameObjectOnScene(behaviour.gameObject) && !behaviour.TurnOffSavingAndLoadingForThisBehaviour)
                .ToList();
        }

        public static JsonSerializer GetJsonSerializerWithCustomJsonConverters()
        {
            JsonSerializer jsonSerializerCustomConverters = new JsonSerializer();
            GetCustomJsonConverters().ToList().ForEach(customConverter =>
            {
                jsonSerializerCustomConverters.Converters.Add(customConverter);
            });

            return jsonSerializerCustomConverters;
        }

        public static JsonConverter[] GetCustomJsonConverters()
        {
            List<JsonConverter> customJsonConverters = new List<JsonConverter>();
            customJsonConverters.Add(new ReferenceableScriptableObjectJsonConverter());
            customJsonConverters.Add(new RuntimeTypedComplexKeyDictionaryJsonConverter());

            return customJsonConverters.ToArray();
        }

        private static bool IsGameObjectOnScene(GameObject gameObject) => !EditorUtility.IsPersistent(gameObject.transform.root.gameObject) && !(gameObject.hideFlags == HideFlags.NotEditable || gameObject.hideFlags == HideFlags.HideAndDontSave);

        public static Dictionary<TKey, TValue> GetDictionaryFromList<TKey, TValue>(List<KeyValuePair<TKey, TValue>> list) => list.ToDictionary(x => x.Key, x => x.Value);

        private string GetJsonSaveFileName(string currentSaveFileName)
        {
            return (string.IsNullOrEmpty(currentSaveFileName) ? CurrentSaveFile : currentSaveFileName) + ".json";
        }

        private bool IsValidJson(string json)
        {
            try
            {
                JToken token = JObject.Parse(json);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool LoadDecodeSaveFile(string filename, out string json)
        {
            bool fileLoaded = FileManager.LoadFromFile(filename, out string saveJson);

            if(!fileLoaded)
            {
                json = saveJson;
                return false;
            }

            if(IsValidJson(saveJson))
            {
                json = saveJson;
                return true;
            }

            string decodedJson = DecodeString(saveJson, key);

            if(IsValidJson(decodedJson))
            {
                json = decodedJson;
                return true;
            }

            json = null;
            return false;
        }

        private string EncodeString(string text, string key)
        {
            byte[] textBytes = Encoding.Default.GetBytes(text);
            byte[] xor = new byte[textBytes.Length];

            for(int i = 0; i < textBytes.Length; i++)
            {
                xor[i] = (byte)(textBytes[i] ^ key[i % key.Length]);
            }

            return Convert.ToBase64String(xor);
        }

        private string DecodeString(string text, string key)
        {
            byte[] textBytes = Convert.FromBase64String(text);
            byte[] xor = new byte[textBytes.Length];

            for(int i = 0; i < textBytes.Length; i++)
            {
                xor[i] = (byte)(textBytes[i] ^ key[i % key.Length]);
            }

            return Encoding.Default.GetString(xor);
        }

        public static Type GetUnderlyingType(MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)memberInfo).FieldType,
                MemberTypes.Property => ((PropertyInfo)memberInfo).PropertyType,
                _ => throw new ArgumentException("Input MemberInfo must be of Type: FieldInfo or PropertyInfo."),
            };
        }

        public static object GetMemberValue(MemberInfo memberInfo, SaveableBehaviour behaviourContainingMember)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)memberInfo).GetValue(behaviourContainingMember),
                MemberTypes.Property => ((PropertyInfo)memberInfo).GetValue(behaviourContainingMember),
                _ => throw new ArgumentException("Input MemberInfo must be of Type: FieldInfo or PropertyInfo."),
            };
        }

        public static void SetMemberValue(MemberInfo memberInfo, object memberValue, SaveableBehaviour behaviourContainingMember)
        {
            switch(memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(behaviourContainingMember, memberValue);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(behaviourContainingMember, memberValue);
                    break;
                default:
                    throw new ArgumentException("Input MemberInfo must be of Type: FieldInfo or PropertyInfo.");
            }
        }
    }
}