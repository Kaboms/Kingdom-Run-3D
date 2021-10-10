using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreFeatures.MessageBus;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
	public void OnRestart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
