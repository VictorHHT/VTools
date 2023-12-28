using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Victor.Tools
{
	[Serializable]
	public struct RevealerHit
	{
		public GameObject gameObject;
		public Vector3? point;
		public float? distance;

		public RevealerHit(GameObject gameObject)
		{
			this.gameObject = gameObject;
			this.distance = default;
			this.point = default;
		}
    }
}