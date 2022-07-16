using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class SaveTest : MonoBehaviour
{
    private Transform Transform => transform;
    private Vector3 Position => transform.position;



    [JsonProperty]
    public int aa = 5;
}
