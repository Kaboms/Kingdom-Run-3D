using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Platform))]
public class PlatformEditor : Editor
{
	private GameObject selectedItem;

	private void OnSceneGUI()
	{
		Platform platform = target as Platform;
		Transform transform = platform.transform;

		for (int i = 0; i < platform.Data.Length; i++)
		{
			float z = -0.45f + 0.05f * i;
			for (int j = 0; j < platform.Data[i].Columns.Length; j++)
			{
				float x = -0.25f + (0.25f * j);

				Handles.color = (platform.Data[i][j] == null) ? Color.blue : Color.red;

				if ((selectedItem == null && platform.Data[i][j] != null) || selectedItem != null)
				{
					if (Handles.Button(transform.TransformPoint(x, 1, z), Quaternion.identity, 0.4f, 0.4f, Handles.CubeHandleCap))
					{
						if (platform.Data[i][j] == null)
						{
							GameObject enemy = Instantiate(selectedItem);
							enemy.transform.localPosition = transform.TransformPoint(x, 1, z);
							enemy.transform.parent = transform.parent;

							platform.Data[i][j] = enemy;

						}
						else
						{
							DestroyImmediate(platform.Data[i][j]);
							platform.Data[i][j] = null;
						}

						EditorUtility.SetDirty(target);
					}
				}
			}
		}

		DrawGui();
	}

	private void DrawGui()
	{
		Platform platform = target as Platform;

		Handles.BeginGUI();

		if (platform.Items == null)
			return;

		if (platform.Items.EnemyPrefabs != null)
		{
			int y = 0;
			for (int i = 0; i < platform.Items.EnemyPrefabs.Count; i++)
			{
				y = 55 * i;

				GameObject enemy = platform.Items.EnemyPrefabs[i];
				if (GUI.Button(new Rect(10, y, 100, 50), enemy.name))
					selectedItem = enemy;
			}

			if (GUI.Button(new Rect(10, y + 55, 100, 50), platform.Items.GoldPrefab.name))
				selectedItem = platform.Items.GoldPrefab;
		}

		if (selectedItem)
			GUI.Label(new Rect(150, 0, 100, 50), selectedItem.name);


		if (GUI.Button(new Rect(10, 400, 100, 50), "Clear"))
		{

			for (int i = 0; i < platform.Data.Length; i++)
			{
				for (int j = 0; j < platform.Data[i].Columns.Length; j++)
				{
					if (platform.Data[i][j])
						DestroyImmediate(platform.Data[i][j]);
				}
			}

			for (int i = 0; i < platform.Data.Length; i++)
			{
				platform.Data[i] = new Platform.PlatformData();
			}

			EditorUtility.SetDirty(target);
		}

		Handles.EndGUI();
	}
}
