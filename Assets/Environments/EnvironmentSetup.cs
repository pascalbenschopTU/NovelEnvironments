using System.Collections;
using UnityEngine;


public class EnvironmentSetup
{
    public string meshTag;

    public Material terrainMaterial;
    public Material wallMaterial;

    public AnimationCurve animationCurve;

    public float scale;
    public int octaves;
    public float lacunarity;

    public Gradient gradient;

    public EnvironmentSetup(string meshTag, Material terrainMaterial, Material wallMaterial, AnimationCurve animationCurve, float scale, int octaves, float lacunarity, Gradient gradient)
    {
        this.meshTag = meshTag;
        this.terrainMaterial = terrainMaterial;
        this.wallMaterial = wallMaterial;
        this.animationCurve = animationCurve;
        this.scale = scale;
        this.octaves = octaves;
        this.lacunarity = lacunarity;
        this.gradient = gradient;
    }
}
