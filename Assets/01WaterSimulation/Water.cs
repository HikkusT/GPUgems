using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

	public float MedWavelenght;
	public float MedAmplitude;

	public Material WaterMat;

	int numWaves = 0;
	List<Vector4> waveData = new List<Vector4>();
	List<Vector4> waveDirections = new List<Vector4>();

	void Start () {
		for (int i = 0; i < 4; i ++)
			SpawnWave();
	}
	
	void Update () {
		WaterMat.SetInt("_NumWaves", numWaves);
		WaterMat.SetVectorArray("_WaveData", waveData);
		WaterMat.SetVectorArray("_WaveDirections", waveDirections);
	}

	void SpawnWave()
	{
		float randomFactor = Random.Range(0.5f, 2f);
		float wavelenght = randomFactor * MedWavelenght;
		float amplitude = randomFactor * MedAmplitude;
		float frequency = 2 * Mathf.PI / wavelenght;
		float speed = Mathf.Sqrt(9.8f * wavelenght / (2f * Mathf.PI));
		float phi = speed * frequency;
		
		waveData.Add(new Vector4(amplitude, frequency, phi, 1));
		waveDirections.Add(new Vector4(1, 1, 0, 0));
		numWaves++;
	}
}
