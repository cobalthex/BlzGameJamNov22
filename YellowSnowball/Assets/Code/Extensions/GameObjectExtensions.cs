using System.Text;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void HideRenderers(this GameObject thisObject)
    {
        Renderer[] renderers = thisObject.GetComponentsInChildren<Renderer>();
        foreach(var renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    public static void SetTransparent(this GameObject thisObject, float alpha)
    {
        Renderer[] renderers = thisObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            var mats = renderers[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                if (!mats[j].HasProperty("_Color"))
                    continue;
                var color = mats[j].color;
                color.a = alpha;
                mats[j].color = color;

                // Change from opaque to transparent
                mats[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mats[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mats[j].SetInt("_ZWrite", 0);
                mats[j].DisableKeyword("_ALPHATEST_ON");
                mats[j].DisableKeyword("_ALPHABLEND_ON");
                mats[j].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mats[j].renderQueue = 3000;
            }
        }
    }

    public static void SetOpaque(this GameObject thisObject)
    {
        Renderer[] renderers = thisObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            var mats = renderers[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                if (!mats[j].HasProperty("_Color"))
                    continue;
                var color = mats[j].color;
                color.a = 1.0f;
                mats[j].color = color;

                // Change from opaque to transparent
                mats[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mats[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mats[j].SetInt("_ZWrite", 1);
                mats[j].DisableKeyword("_ALPHATEST_ON");
                mats[j].DisableKeyword("_ALPHABLEND_ON");
                mats[j].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mats[j].renderQueue = -1;
            }
        }
    }
}