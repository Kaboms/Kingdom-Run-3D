using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlatformItems", menuName = "ScriptableObjects/PlatformItems", order = 1)]
public class PlatformItems : ScriptableObject
{
	public GameObject GoldPrefab;
	public List<GameObject> EnemyPrefabs = new List<GameObject>();
}
