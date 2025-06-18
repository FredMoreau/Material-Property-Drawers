using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class NormalMapDrawer : CustomMaterialPropertyDrawerBase
    {
        string strengthProperty, keyword;

        public NormalMapDrawer() { }

        public NormalMapDrawer(string strengthProperty)
        {
            this.strengthProperty = strengthProperty;
        }

        public NormalMapDrawer(string strengthProperty, string keyword) : this(strengthProperty)
        {
            this.keyword = keyword;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            base.GetPropertyHeight(prop, label, editor);
            return 0;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!isVisible)
                return;

            if (prop.propertyType != ShaderPropertyType.Texture)
            {
                editor.DefaultShaderProperty(prop, label);
                return;
            }

            var properties = MaterialEditor.GetMaterialProperties(prop.targets);
            MaterialProperty strengthProp = FindProperty(strengthProperty, properties);

            bool showStrengthProperty = strengthProp != null && prop.textureValue != null;

            using (new EditorGUI.DisabledScope(!isEnabled || (prop.propertyFlags & ShaderPropertyFlags.PerRendererData) != 0))
            {
                EditorGUI.BeginChangeCheck();
                editor.TexturePropertySingleLine(new GUIContent(label), prop, showStrengthProperty ? strengthProp : null);
                if (EditorGUI.EndChangeCheck() && strengthProp != null && !string.IsNullOrEmpty(keyword))
                    SetKeyword(prop, strengthProp);
            }
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);

            if (prop.propertyType != ShaderPropertyType.Texture)
                return;

            if (string.IsNullOrEmpty(keyword) || string.IsNullOrEmpty(strengthProperty))
                return;

            var properties = MaterialEditor.GetMaterialProperties(prop.targets);
            MaterialProperty strengthProp = FindProperty(strengthProperty, properties);

            SetKeyword(prop, strengthProp);
        }

        void SetKeyword(MaterialProperty prop, MaterialProperty strengthProp)
        {
            if (prop.textureValue == null || strengthProp.floatValue == 0)
                DisableKeyword(prop.targets, keyword);
            else
                EnableKeyword(prop.targets, keyword);
        }
    }
}
