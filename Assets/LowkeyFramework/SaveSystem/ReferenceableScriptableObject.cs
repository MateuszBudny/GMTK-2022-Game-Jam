using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LowkeyFramework.AttributeSaveSystem
{
	public abstract class ReferenceableScriptableObject : ScriptableObject
	{
		[SerializeField, ReadOnly]
		private string guid;

		public string Guid => guid;

#if UNITY_EDITOR
		void OnValidate()
		{
			if (string.IsNullOrEmpty(guid))
			{
				guid = System.Guid.NewGuid().ToString();
				Debug.LogWarning($"New GUID ({Guid}) has been assigned to SO: {name}.");

				EditorApplication.delayCall += () =>
				{
					AssetDatabase.Refresh();
					EditorUtility.SetDirty(this);
					AssetDatabase.SaveAssets();
				};
			}
		}
#endif
	}
}
