using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class UnderWaterEffect : MonoBehaviour
{
    public Material _mat;

    [Range(0.001f, 0.1f)]
    public float pixelOffset;
    [Range(0.1f,20f)]
    public float noiseScale;
    [Range(0.1f, 20f)]
    public float noiseFrecuency;
    [Range(0.1f, 30f)]
    public float noiseSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _mat.SetFloat("_NoiseFrequency", noiseFrecuency);
        _mat.SetFloat("_NoiseScale", noiseScale);
        _mat.SetFloat("_NoiseSpeed", noiseSpeed);
        _mat.SetFloat("_PixelOffset", pixelOffset);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _mat);
    }
}
