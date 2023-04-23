using System.Collections;
using UnityEngine;


public class Snow
{
    public static string meshTag = "GrassFloor";

    public static Material terrainMaterial = (Material)Resources.Load("Materials/TerrainMaterial", typeof(Material));
    public static Material wallMaterial = (Material)Resources.Load("Materials/GlassWalls", typeof(Material));

    public static AnimationCurve heightCurve = setCurve();

    public static float scale = 100;
    public static int octaves = 5;
    public static float lacunarity = 2;

    public static Gradient gradient = setGradient();

    public static EnvironmentSetup GetEnvironmentSetup()
    {
        return new EnvironmentSetup(
                meshTag, terrainMaterial, wallMaterial, heightCurve, scale, octaves, lacunarity, gradient
            );
    }
    private static AnimationCurve setCurve()
    {
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0, 0.0f), new Keyframe(0.84f, 1.20f)    
        );

        curve.preWrapMode = WrapMode.Clamp;
        curve.postWrapMode = WrapMode.Clamp;

        return curve;
    }

    private static Gradient setGradient()
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] gradientColorKeys = new GradientColorKey[5];
        gradientColorKeys[0].color = new Color(1f, 1f, 1f);
        gradientColorKeys[0].time = 0.0f;

        GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[5];
        gradientAlphaKeys[0].alpha = 1.0f;
        gradientAlphaKeys[0].time = 0.0f;

        gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);

        return gradient;
    }
}
