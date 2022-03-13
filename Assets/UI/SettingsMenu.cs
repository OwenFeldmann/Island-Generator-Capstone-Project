using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	
	public GameObject settingsMenu, islandMenu, settingsExplanationMenu, howToExportFbxMenu;
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
	public Slider xNoiseScaleSlider;
	public Slider zNoiseScaleSlider;
	public InputField seaLevelInputField;
	public InputField falloffRateInputField;
	
	[Header("Biome Settings")]
	public InputField biomeCountInputField;
	public InputField biomeSpreadInputField;
	
	[Header("Volcano Settings")]
	public Toggle randomCenterToggle;
	public Slider volcanoXCenterSlider;
	public Slider volcanoZCenterSlider;
	public InputField lavaLevelInputField;
	public InputField rimRadiusInputField;
	public InputField iterationsInputField;
	public InputField iterationStrengthInputField;
	public InputField startRadiusInputField;
	public InputField endRadiusInputField;
	public InputField startSpreadInputField;
	public InputField endSpreadInputField;
	
	[Header("Misc Settings")]
	public InputField propAttemptsInputField;
	public InputField terraceHeightInputField;
	
	public void ViewIsland()
	{
		settingsMenu.SetActive(false);
		islandMenu.SetActive(true);
	}
	
	public void ViewSettingsExplanation()
	{
		settingsExplanationMenu.SetActive(true);
		settingsMenu.SetActive(false);
	}
	
	public void ViewHowToExportFbxMenu()
	{
		howToExportFbxMenu.SetActive(true);
		settingsMenu.SetActive(false);
	}
	
	public void BackToSettings()
	{
		settingsMenu.SetActive(true);
		settingsExplanationMenu.SetActive(false);
		howToExportFbxMenu.SetActive(false);
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
		mg.xCenter = (int) (xCenterSlider.value * mg.xSize);//slider [0,1] is the percent of the island
		mg.zCenter = (int) (zCenterSlider.value * mg.zSize);
		mg.noiseXScale = xNoiseScaleSlider.value;
		mg.noiseZScale = zNoiseScaleSlider.value;
		mg.seaLevel = Mathf.Abs(float.Parse(seaLevelInputField.text));
		mg.distanceFromCenterFalloffRate = float.Parse(falloffRateInputField.text);
		
		//Biome Settings
		bg.biomesToPlace = int.Parse(biomeCountInputField.text);
		if(bg.biomesToPlace < 1)
			bg.biomesToPlace = 1;
		bg.maxBiomeSpread = Mathf.Abs(float.Parse(biomeSpreadInputField.text));
		
		//Volcano Settings
		vg.chooseRandomCenter = randomCenterToggle.isOn;
		vg.centerX = volcanoXCenterSlider.value * mg.xSize;//slider [0,1] is the percent of the island
		vg.centerZ = volcanoZCenterSlider.value * mg.zSize;
		vg.lavaLevel = float.Parse(lavaLevelInputField.text);
		vg.rimRadius = Mathf.Abs(float.Parse(rimRadiusInputField.text));
		vg.iterations = Mathf.Abs(int.Parse(iterationsInputField.text));
		vg.iterationStrength = Mathf.Abs(float.Parse(iterationStrengthInputField.text));
		vg.iterationStartRadius = Mathf.Abs(float.Parse(startRadiusInputField.text));
		vg.iterationEndRadius = Mathf.Abs(float.Parse(endRadiusInputField.text));
		vg.startSpread = Mathf.Abs(float.Parse(startSpreadInputField.text));
		vg.endSpread = Mathf.Abs(float.Parse(endSpreadInputField.text));
		
		if(vg.iterationStartRadius < vg.iterationEndRadius){//ensure start radius is larger than the end radius
			float temp = vg.iterationStartRadius;
			vg.iterationStartRadius = vg.iterationEndRadius;
			vg.iterationEndRadius = temp;
		}
		
		if(vg.startSpread < vg.endSpread){//ensure start spread is larger than the end spread
			float temp = vg.startSpread;
			vg.startSpread = vg.endSpread;
			vg.endSpread = temp;
		}
		
		//Misc Settings
		mg.propsToTryToPlace = Mathf.Abs(int.Parse(propAttemptsInputField.text));
		mg.terraceHeight = Mathf.Abs(float.Parse(terraceHeightInputField.text));
		
		mg.StartIslandGeneration();
	}
	
}
