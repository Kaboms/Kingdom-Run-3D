using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
	[SerializeField]
	private PlatformItems _items;
	public PlatformItems Items
	{
		get { return _items; }
		private set { _items = value; }
	}

	[Serializable]
	public class PlatformData
	{
		public GameObject[] Columns = new GameObject[3];
		public GameObject this[int key]
		{
			get => Columns[key];
			set => Columns[key] = value;
		}
	}
	public PlatformData[] Data;

	private void OnDrawGizmos()
	{
		for (float i = -0.25f; i <= 0.25; i += 0.25f)
		{
			Gizmos.DrawLine(transform.TransformPoint(i, 0.75f, 0.5f), transform.TransformPoint(new Vector3(i, 0.75f, -0.5f)));
		}

		for (float i = 0.45f; i > -0.5f; i -= 0.05f)
		{
			Gizmos.DrawLine(transform.TransformPoint(-0.25f, 0.75f, i), transform.TransformPoint(new Vector3(0.25f, 0.75f, i)));
		}
	}
}
 