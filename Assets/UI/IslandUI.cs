using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandUI : MonoBehaviour
{
	public Text generationText;
	public GameObject generationPanel;
	public GameObject settingsMenu, islandMenu;
	
	public void OpenSettingsMenu()
	{
		//stop generating if in process.
		MonoBehaviour[] scripts = GameObject.Find("IslandGenerator").GetComponents<MonoBehaviour>();
		foreach(MonoBehaviour s in scripts)
		{
			s.StopAllCoroutines();
		}
			
		settingsMenu.SetActive(true);
		islandMenu.SetActive(false);	
	}
	
	public void SetGenerationText(string text)
	{
		generationText.text = text;
	}
	
	public void SetGenerationPanelActive(bool active)
	{
		generationPanel.SetActive(active);
	}
}
