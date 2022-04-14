/*
Copyright (C) 2022 Owen Feldmann

This file is part of Island Generator.

Island Generator is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License 
as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Island Generator is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Island Generator. If not, see <https://www.gnu.org/licenses/>.
*/

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
		
		GetComponent<SettingsMenu>().viewIslandButton.interactable = true;
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
