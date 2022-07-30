using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTester : MonoBehaviour {

    public int chunkSize = 16;
    public List<NoiseSettings> noiseSettings = new List<NoiseSettings>();

    void Start() {
        //Noise noise1 = new Noise(noiseSettings[0]);

        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                //      Debug.Log(noise1.Perlin(x, y));
            }
        }
    }
}
