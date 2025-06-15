using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class InlineDrawer : MaterialPropertyDrawer
    {
        string extraProperty1, extraProperty2, keyword;
        bool[] propertyChecks = new[] { true, false, false };
        public InlineDrawer() { }

        public InlineDrawer(string extraProperty1)
        {
            this.extraProperty1 = extraProperty1;
        }

        public InlineDrawer(string extraProperty1, string extraProperty2) : this(extraProperty1)
        {
            this.extraProperty2 = extraProperty2;
        }

        public InlineDrawer(string extraProperty1, string extraProperty2, string keyword) : this(extraProperty1, extraProperty2)
        {
            this.keyword = keyword;
        }

        public InlineDrawer(string extraProperty1, string extraProperty2, string keyword, string check1, string check2, string check3) : this(extraProperty1, extraProperty2, keyword)
        {
            this.propertyChecks = new[] { bool.Parse(check1), bool.Parse(check2), bool.Parse(check3) };
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor) => 0;

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            var properties = MaterialEditor.GetMaterialProperties(prop.targets);
            MaterialProperty prop1 = FindProperty(extraProperty1, properties);
            MaterialProperty prop2 = FindProperty(extraProperty2, properties);

            EditorGUI.BeginChangeCheck();
            editor.TexturePropertySingleLine(new GUIContent(label), prop, prop1, prop2);
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(keyword))
                SetKeyword(prop, prop1, prop2);
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);

            if (string.IsNullOrEmpty(keyword))
                return;

            var properties = MaterialEditor.GetMaterialProperties(prop.targets);
            MaterialProperty prop1 = FindProperty(extraProperty1, properties);
            MaterialProperty prop2 = FindProperty(extraProperty2, properties);

            SetKeyword(prop, prop1, prop2);
        }

        bool CheckProperty(MaterialProperty prop) => prop.propertyType switch
        {
            ShaderPropertyType.Color => prop.colorValue.maxColorComponent > 0 || prop.colorValue.a > 0,
            ShaderPropertyType.Vector => prop.vectorValue.magnitude > 0,
            ShaderPropertyType.Float => prop.floatValue > 0,
            ShaderPropertyType.Range => prop.floatValue > 0,
            ShaderPropertyType.Texture => prop.textureValue != null,
            ShaderPropertyType.Int => prop.intValue > 0,
            _ => false
        };

        void SetKeyword(MaterialProperty prop, MaterialProperty extraProperty1 = null, MaterialProperty extraProperty2 = null)
        {
            bool enableKeyword = propertyChecks[0] ? CheckProperty(prop) : true;
            if (enableKeyword && propertyChecks[1] && extraProperty1 != null)
                enableKeyword &= CheckProperty(extraProperty1);
            if (enableKeyword && propertyChecks[2] && extraProperty2 != null)
                enableKeyword &= CheckProperty(extraProperty2);

            if (enableKeyword)
                EnableKeyword(prop.targets, keyword);
            else
                DisableKeyword(prop.targets, keyword);
        }

        void EnableKeyword(UnityEngine.Object[] mats, string keyword)
        {
            for (int i = 0; i < mats.Length; i++)
                if (mats[i] is Material material)
                    material.EnableKeyword(keyword);
        }

        void DisableKeyword(UnityEngine.Object[] mats, string keyword)
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
