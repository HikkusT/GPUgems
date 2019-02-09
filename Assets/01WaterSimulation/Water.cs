using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

	enum WaveState {rising, dying}

	public float MedWavelenght;
	public float MedAmplitude;
	public float Decay;
	public float WindCone;

	public Material WaterMat;

	int numWaves = 0;
	List<Vector4> waveData = new List<Vector4>();
	List<Vector4> waveDirections = new List<Vector4>();
	List<float> waveIntensity = new List<float>();
	List<WaveState> waveState = new List<WaveState>();
	Vector2 windDir;

	void Start () {
		FillLists(20);
		windDir = Random.insideUnitCircle;
		StartCoroutine(SpawnInitialWaves());
	}
	
	void Update () {
		UpdateWind();
		UpdateWaves();
		Debug.Log(waveIntensity.Count.ToString());

		WaterMat.SetInt("_NumWaves", numWaves);
		WaterMat.SetVectorArray("_WaveData", waveData);
		WaterMat.SetVectorArray("_WaveDirections", waveDirections);
		WaterMat.SetFloatArray("_WaveIntensity", waveIntensity);
	}

	void UpdateWind()
	{
		float angle  = Mathf.Deg2Rad * Random.Range(0, 10) * Time.deltaTime;
		windDir = RotateVector2D(windDir, angle); 
	}

	void UpdateWaves()
	{
		for(int i = 0; i < numWaves; i ++)
		{
			if(waveState[i] == WaveState.dying)
			{
				waveIntensity[i] -= Decay * Time.deltaTime;
				if (waveIntensity[i] < 0.01f)
				{
					KillWave(i);
					SpawnWave();
				}
			}
			else
			{
				waveIntensity[i] += Decay * Time.deltaTime;
				if (waveIntensity[i] > 0.99f)
					waveState[i] = WaveState.dying;
			}
		}
	}

	Vector2 GetRandomWaveDir()
	{
		float angle = Mathf.Deg2Rad * Random.Range(- WindCone, WindCone);

		return RotateVector2D(windDir, angle);
	}

	void SpawnWave()
	{
		float randomFactor = Random.Range(0.5f, 2f);
		float wavelenght = randomFactor * MedWavelenght;
		float amplitude = randomFactor * MedAmplitude;
		float frequency = 2 * Mathf.PI / wavelenght;
		float speed = Mathf.Sqrt(9.8f * wavelenght / (2f * Mathf.PI));
		float phi = speed * frequency;
		
		waveData[numWaves] = new Vector4(amplitude, frequency, speed, 2);
		waveIntensity[numWaves] = 0f;
		Vector2 waveDir = GetRandomWaveDir();
		waveDirections[numWaves] = new Vector4(waveDir.x, waveDir.y, 0, 1);
		waveState.Add(WaveState.rising);
		numWaves++;
	}

	void KillWave(int i)
	{
		RemoveAtIndex(i);
		numWaves --;
	}

	IEnumerator SpawnInitialWaves()
	{
		for (int i = 0; i < 4; i ++)
		{
			SpawnWave();
			yield return new WaitForSeconds(Random.Range(2f, 5f));
		}
	}

	Vector2 RotateVector2D (Vector2 vector, float angle)
	{
		float x = vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle);
		float y = vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle);

		return new Vector2(x, y);
	}

	void FillLists(int size)
	{
		for (int i = 0; i < size; i ++)
		{
			waveData.Add(new Vector4(0, 0, 0, 0));
			waveIntensity.Add(0);
			waveDirections.Add(new Vector4(0, 0, 0, 0));
		}
	}

	void RemoveAtIndex(int i)
	{
		waveData.RemoveAt(i);
		waveDirections.RemoveAt(i);
		waveIntensity.RemoveAt(i);
		waveState.RemoveAt(i);

		waveData.Add(new Vector4(0, 0, 0, 0));
		waveIntensity.Add(0);
		waveDirections.Add(new Vector4(0, 0, 0, 0));
	}
}
