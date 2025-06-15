using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class NormalMapDrawer : MaterialPropertyDrawer
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

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor) => 0;

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (prop.propertyType != ShaderPropertyType.Texture)
            {
                editor.DefaultShaderProperty(prop, label);
                return;
            }

            var properties = MaterialEditor.GetMaterialProperties(prop.targets);
            MaterialProperty strengthProp = FindProperty(strengthProperty, properties);

            bool showStrengthProperty = strengthProp != null && prop.textureValue != null;

            EditorGUI.BeginChangeCheck();
            editor.TexturePropertySingleLine(new GUIContent(label), prop, showStrengthProperty ? strengthProp : null);
            if (EditorGUI.EndChangeCheck() && strengthProp != null && !string.IsNullOrEmpty(keyword))
                SetKeyword(prop, strengthProp);
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

        void EnableKeyword(Object[] mats, string keyword)
        {
            for (int i = 0; i < mats.Length; i++)
                if (mats[i] is Material material)
                    material.EnableKeyword(keyword);
        }

        void DisableKeyword(Object[] mats, string keyword)
        {
            for (int i = 0; i < mats.Length; i++)
                if (mats[i] is Material material)
                    material.DisableKeyword(keyword);
        }

        static MaterialProperty FindProperty(string propertyName, MaterialProperty[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
                if (properties[i] != null && properties[i].name == propertyName)
                    return properties[i];

            return null;
        }
    }
}
