using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class MiniThumbnailDrawer : MaterialPropertyDrawer
    {
        public MiniThumbnailDrawer() { }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (prop.propertyType != ShaderPropertyType.Texture)
                editor.DefaultShaderProperty(position, prop, label);
            else
                editor.TexturePropertyMiniThumbnail(position, prop, label, null);
        }
    }
}
