using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostEffectsController : MonoBehaviour
{
    [SerializeField] Shader postShader;
    [SerializeField] Vector2 redBaseOffset = new Vector2(0.04f, 0);
    [SerializeField] Vector2 blueBaseOffset = new Vector2(0.04f, 0);
    Material postEffectMaterial;

    private void Awake() {
        if (postEffectMaterial == null) { postEffectMaterial = new Material(postShader); }
        SetAberrationOffsets(redBaseOffset, blueBaseOffset);
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        SetAberrationOffsets(redBaseOffset, blueBaseOffset);
        RenderTexture renderTexture = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        Graphics.Blit(src, renderTexture, postEffectMaterial);
        Graphics.Blit(renderTexture, dest);
        RenderTexture.ReleaseTemporary(renderTexture);
    }
    public void SetAberrationOffsets(Vector2 redOffset, Vector2 blueOffset) {
        // blueOffset += blueBaseOffset;
        // redOffset += redBaseOffset;
        // blueOffset *= 0.1f;
        // redOffset *= 0.1f;
        // Debug.Log("red: "+redOffset+", blue: "+blueOffset);
        postEffectMaterial.SetFloat("_xRedOffset", redOffset.x);
        postEffectMaterial.SetFloat("_yRedOffset", redOffset.y);
        postEffectMaterial.SetFloat("_xBlueOffset", blueOffset.x);
        postEffectMaterial.SetFloat("_yBlueOffset", blueOffset.y);
    }
}
