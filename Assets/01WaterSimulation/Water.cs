using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

	public float MedWavelenght;
	public float MedAmplitude;
	public float Decay;
	public float WindCone;

	public Material WaterMat;

	int numWaves = 0;
	List<Vector4> waveData = new List<Vector4>();
	List<Vector4> waveDirections = new List<Vector4>();
	Vector2 windDir;

	void Start () {
		windDir = Random.insideUnitCircle;
		for (int i = 0; i < 4; i ++)
			SpawnWave();
	}
	
	void Update () {
		UpdateWaves();

		WaterMat.SetInt("_NumWaves", numWaves);
		WaterMat.SetVectorArray("_WaveData", waveData);
		WaterMat.SetVectorArray("_WaveDirections", waveDirections);
	}

	void UpdateWaves()
	{
		for(int i = 0; i < numWaves; i ++)
		{
			Vector4 wave = waveData[i];
			wave.w -= Decay * Time.deltaTime;
			if (wave.w < 0.01f)
				KillWave(i);
			else
				waveData[i] = wave;
		}
	}

	Vector2 GetRandomWaveDir()
	{
		float angle  = Mathf.Deg2Rad * Random.Range(- WindCone, WindCone);
		float x = windDir.x * Mathf.Cos(angle) - windDir.y * Mathf.Sin(angle);
		float y = windDir.x * Mathf.Sin(angle) + windDir.y * Mathf.Cos(angle);
		return new Vector2(x, y);
	}

	void SpawnWave()
	{
		float randomFactor = Random.Range(0.5f, 2f);
		float wavelenght = randomFactor * MedWavelenght;
		float amplitude = randomFactor * MedAmplitude;
		float frequency = 2 * Mathf.PI / wavelenght;
		float speed = Mathf.Sqrt(9.8f * wavelenght / (2f * Mathf.PI));
		float phi = speed * frequency;
		
		waveData.Add(new Vector4(amplitude, frequency, speed, 1));
		Vector2 waveDir = GetRandomWaveDir();
		waveDirections.Add(new Vector4(waveDir.x, waveDir.y, 0, 1));
		numWaves++;
	}

	void KillWave(int i)
	{
		waveData.RemoveAt(i);
		waveDirections.RemoveAt(i);
		numWaves --;
	}
}
