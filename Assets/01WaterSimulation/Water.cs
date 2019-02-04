using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

	public float MedWavelenght;
	public float MedAmplitude;

	public Material WaterMat;

	int numWaves = 0;
	Vector4[] waveData = new Vector4[20];
	Vector4[] waveDirections = new Vector4[20];

	void Start () {
		CreateWave();
	}
	
	void Update () {
		WaterMat.SetInt("_NumWaves", numWaves);
		WaterMat.SetVectorArray("_WaveData", waveData);
		WaterMat.SetVectorArray("_WaveDirections", waveDirections);
	}

	void CreateWave()
	{
		float wavelenght = Random.Range(MedWavelenght/2f, MedWavelenght * 2f);
		float amplitude = MedAmplitude * wavelenght / MedWavelenght;
		float frequency = 2 * Mathf.PI / wavelenght;
		float speed = Mathf.Sqrt(9.8f * wavelenght / (2f * Mathf.PI));
		float phi = speed * frequency;
		
		waveData[numWaves] = new Vector4(amplitude, frequency, phi, 1);
		waveDirections[numWaves] = new Vector4(1, 1, 0, 0);
		numWaves++;
	}
}
