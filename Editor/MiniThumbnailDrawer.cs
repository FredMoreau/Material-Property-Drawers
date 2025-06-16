using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class MiniThumbnailDrawer : CustomMaterialPropertyDrawerBase
    {
        public MiniThumbnailDrawer() { }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!isVisible)
                return;

            using (new EditorGUI.DisabledScope(!isEnabled || (prop.propertyFlags & ShaderPropertyFlags.PerRendererData) != 0))
            {
                if (prop.propertyType != ShaderPropertyType.Texture)
                    editor.DefaultShaderProperty(position, prop, label);
                else
                    editor.TexturePropertyMiniThumbnail(position, prop, label, null);
            }
        }
    }
}
