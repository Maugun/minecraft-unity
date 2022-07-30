using System.Collections.Generic;
using UnityEngine;

public class Noise {
    System.Random _prng;
    float _maxPossibleHeight = 0;
    Vector2[] _octaveOffsets;
    NoiseSettings _settings;


    public Noise(NoiseSettings settings, Vector2 chunkPos) {
        _settings = settings;
        _maxPossibleHeight = 0;
        _octaveOffsets = new Vector2[_settings.octaves];

        float amplitude = 1;
        _prng = new System.Random(_settings.seed);
        for (int i = 0; i < _settings.octaves; i++) {
            float offsetX = _prng.Next(-100000, 100000) + _settings.offset.x + chunkPos.x;
            float offsetY = _prng.Next(-100000, 100000) + _settings.offset.y + chunkPos.y;
            _octaveOffsets[i] = new Vector2(offsetX, offsetY);

            _maxPossibleHeight += amplitude;
            amplitude *= _settings.persistance;
        }

        if (_settings.scale <= 0) {
            _settings.scale = 0.0001f;
        }
    }

    public float Perlin(float x, float y) {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < _settings.octaves; i++) {
            float sampleX = (x + _octaveOffsets[i].x) / _settings.scale * frequency;
            float sampleY = (y + _octaveOffsets[i].y) / _settings.scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= _settings.persistance;
            frequency *= _settings.lacunarity;
        }

        float normalizedHeight = (noiseHeight + 1) / (_maxPossibleHeight / 0.9f);
        noiseHeight = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);

        return noiseHeight;
    }

    public static float Perlin2D(int xPos, int yPos, float scale, Vector2 offset) {
        float x = (xPos + 0.1f) * scale + offset.x;
        float y = (yPos + 0.1f) * scale + offset.y;

        return Mathf.PerlinNoise(x, y);
    }

    public static float Perlin3D(float x, float y, float z, float scale) {
        x = (x + 0.1f) * scale;
        y = (y + 0.1f) * scale;
        z = (z + 0.1f) * scale;

        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}

[System.Serializable]
public class NoiseSettings {

    public string name = "Noise";
    public float scale = 200;

    public int octaves = 2;
    [Range(0, 1)]
    public float persistance = .9f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}
