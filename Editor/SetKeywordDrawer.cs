using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class SetKeywordDrawer : CustomMaterialPropertyDrawerBase
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
            if (!isVisible)
                return;

            using (new EditorGUI.DisabledScope(!isEnabled || (prop.propertyFlags & ShaderPropertyFlags.PerRendererData) != 0))
            {
                EditorGUI.BeginChangeCheck();
                editor.DefaultShaderProperty(position, prop, label);
                if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(keyword))
                    SetKeyword(prop, keyword, state);
            }
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);

            if (string.IsNullOrEmpty(keyword))
                return;

            SetKeyword(prop, keyword, state);
        }
    }
}
