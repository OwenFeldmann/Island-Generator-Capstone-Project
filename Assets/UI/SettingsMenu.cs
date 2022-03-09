using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	
	public GameObject settingsMenu, islandMenu;
	
	[Header("Settings Overview")]
	public Toggle animateToggle;
	public Toggle generateBiomesToggle;
	public Toggle generateVolcanoToggle;
	public Toggle placePropsToggle;
	public Toggle smoothTerraceToggle;
	public Toggle jiggleVerticesToggle;
	
	public void GenerateIsland()
	{
		settingsMenu.SetActive(false);
		islandMenu.SetActive(true);
		
		ApplySettingsAndGenerate();
		
	}
	
	private void ApplySettingsAndGenerate()
	{
		GameObject ig = GameObject.Find("IslandGenerator");
		MeshGenerator mg = ig.GetComponent<MeshGenerator>();
		BiomeGenerator bg = ig.GetComponent<BiomeGenerator>();
		VolcanoGenerator vg = ig.GetComponent<VolcanoGenerator>();
		
		//Settings Overview
		mg.animateGeneration = animateToggle.isOn;
		bg.generateBiomes = generateBiomesToggle.isOn;
		vg.generateVolcano = generateVolcanoToggle.isOn;
		mg.generateProps = placePropsToggle.isOn;
		mg.smoothTerraceVertices = smoothTerraceToggle.isOn;
		mg.jiggleVertices = jiggleVerticesToggle.isOn;
		
		mg.StartIslandGeneration();
	}
	
}
