using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Victor.Tools
{
	public struct RevealerFilter
	{
		public bool raycast { get; set; }
		public bool overlap { get; set; }
		public bool handles { get; set; }

		public static RevealerFilter revealerFilter { get; } = new RevealerFilter
		{
			raycast = true,
			overlap = true,
			handles = true,
		};
	}
}
