using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	
	public GameObject settingsMenu, islandMenu;
	public Button viewIslandButton;
	
	[Header("Settings Overview")]
	public Toggle animateToggle;
	public Toggle generateBiomesToggle;
	public Toggle generateVolcanoToggle;
	public Toggle placePropsToggle;
	public Toggle smoothTerraceToggle;
	public Toggle jiggleVerticesToggle;
	
	[Header("World Shape Settings")]
	public Toggle randomSeedToggle;
	public InputField seedInputField;
	public Slider xSizeSlider;
	public Slider zSizeSlider;
	public Slider xCenterSlider;
	public Slider zCenterSlider;
	public InputField seaLevelInputField;
	public InputField falloffRateInputField;
	
	public void ViewIsland()
	{
		settingsMenu.SetActive(false);
		islandMenu.SetActive(true);
	}
	
	public void GenerateIsland()
	{
		ViewIsland();
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
		
		//World Shape Settings
		mg.useRandomSeed = randomSeedToggle.isOn;
		mg.seed = int.Parse(seedInputField.text);
		mg.xSize = (int) xSizeSlider.value;
		mg.zSize = (int) zSizeSlider.value;
		mg.xCenter = (int) xCenterSlider.value;
		if(mg.xCenter > mg.xSize)//center can't be outside of bounds
			mg.xCenter = mg.xSize/2;
		mg.zCenter = (int) zCenterSlider.value;
		if(mg.zCenter > mg.zSize)//center can't be outside of bounds
			mg.zCenter = mg.zSize/2;
		mg.seaLevel = float.Parse(seaLevelInputField.text);
		mg.distanceFromCenterFalloffRate = float.Parse(falloffRateInputField.text);
		
		mg.StartIslandGeneration();
	}
	
}
