using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class SetKeywordDrawer : MaterialPropertyDrawer
    {
        string keyword;
        bool state = true;

        public SetKeywordDrawer(string keyword)
        {
            this.keyword = keyword;
        }

        public SetKeywordDrawer(string keyword, string state) : this(keyword)
        {
            bool.TryParse(state, out bool invert);
            this.state = invert;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            EditorGUI.BeginChangeCheck();
            editor.DefaultShaderProperty(position, prop, label);
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(keyword))
                SetKeyword(prop);
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);

            if (string.IsNullOrEmpty(keyword))
                return;

            SetKeyword(prop);
        }

        void SetKeyword(MaterialProperty prop)
        {
            bool enableKeyword = CheckProperty(prop);

            if (enableKeyword == state)
                EnableKeyword(prop.targets, keyword);
            else
                DisableKeyword(prop.targets, keyword);
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
    }
}
