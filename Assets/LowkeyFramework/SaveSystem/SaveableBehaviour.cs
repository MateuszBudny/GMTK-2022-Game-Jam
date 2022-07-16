using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowkeyFramework.AttributeSaveSystem
{
	// a part of this code has been borrowed from: https://github.com/UnityTechnologies/open-project-1

	public abstract class SaveableBehaviour : MonoBehaviour
	{
		[SerializeField, ReadOnly]
		private string guid;

		public string Guid => guid;

		/// <summary>
		/// Might be used in some rare situations, e.g. when object should be destroyed, but it is waiting for its turn, because Destroy() works on the end of the frame and using DestroyImmidiate() is not recommended.
		/// </summary>
		public bool TurnOffSavingAndLoadingForThisBehaviour { get; set; } = false;

#if UNITY_EDITOR
		void OnValidate()
		{
			if (string.IsNullOrEmpty(guid))
			{
				guid = System.Guid.NewGuid().ToString();
				Debug.LogWarning($"New GUID ({Guid}) has been assigned to {gameObject.name}.");
			}
		}
#endif
	}
}
