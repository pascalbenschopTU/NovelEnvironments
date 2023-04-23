using System.Collections;
using UnityEngine;


public class Forest
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
            new Keyframe(0, 0.18f), new Keyframe(1.0f, 0.8f)    
        );

        curve.preWrapMode = WrapMode.Clamp;
        curve.postWrapMode = WrapMode.Clamp;

        return curve;
    }

    private static Gradient setGradient()
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] gradientColorKeys = new GradientColorKey[5];
        gradientColorKeys[0].color = new Color(0.0f, 0.96f, 0.96f);
        gradientColorKeys[0].time = 0.0f;
        gradientColorKeys[1].color = new Color(0.05f, 0.96f, 0.05f);
        gradientColorKeys[1].time = 0.3f;
        gradientColorKeys[2].color = new Color(0.62f, 0.34f, 0.0f);
        gradientColorKeys[2].time = 0.75f;
        gradientColorKeys[3].color = new Color(0.04f, 0.04f, 0.04f);
        gradientColorKeys[3].time = 0.85f;
        gradientColorKeys[4].color = new Color(1f, 1f, 1f);
        gradientColorKeys[4].time = 1.0f;

        GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[5];
        gradientAlphaKeys[0].alpha = 1.0f;
        gradientAlphaKeys[0].time = 0.0f;
        gradientAlphaKeys[1].alpha = 1.0f;
        gradientAlphaKeys[1].time = 0.3f;
        gradientAlphaKeys[2].alpha = 1.0f;
        gradientAlphaKeys[2].time = 0.75f;
        gradientAlphaKeys[3].alpha = 1.0f;
        gradientAlphaKeys[3].time = 0.85f;
        gradientAlphaKeys[4].alpha = 1.0f;
        gradientAlphaKeys[4].time = 1.0f;

        gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);

        return gradient;
    }
}
