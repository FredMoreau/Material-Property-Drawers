using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class MultiplyDrawer : CustomMaterialPropertyDrawerBase
    {
        float multiplier = 1;

        public MultiplyDrawer() { }

        public MultiplyDrawer(float multiplier)
        {
            this.multiplier = multiplier;
        }

        public MultiplyDrawer(float multiplier, string negative)
        {
            bool.TryParse(negative, out bool invert);
            this.multiplier = multiplier * (invert? -1 : 1);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            switch (prop.propertyType)
            {
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                case ShaderPropertyType.Int:
                    CustomGUI(position, prop, label, editor);
                    break;
                default:
                    editor.DefaultShaderProperty(position, prop, label);
                    break;
            }
        }

        void CustomGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            editor.BeginAnimatedCheck(position, prop);
            using (new EditorGUI.DisabledScope((prop.propertyFlags & ShaderPropertyFlags.PerRendererData) != 0))
            {
                MaterialEditor.BeginProperty(position, prop);
                float labelWidth = EditorGUIUtility.labelWidth;

                float currentValue = prop.floatValue;

                EditorGUI.showMixedValue = prop.hasMixedValue;

                float newValue;

                EditorGUI.BeginChangeCheck();
                switch (prop.propertyType)
                {
                    case ShaderPropertyType.Float:
                        newValue = EditorGUI.FloatField(position, label, currentValue / multiplier);
                        if (EditorGUI.EndChangeCheck())
                            prop.floatValue = newValue * multiplier;
                        break;
                    case ShaderPropertyType.Range:
                        newValue = EditorGUI.Slider(position, label, currentValue / multiplier, prop.rangeLimits.x, prop.rangeLimits.y);
                        if (EditorGUI.EndChangeCheck())
                            prop.floatValue = newValue * multiplier;
                        break;
                    case ShaderPropertyType.Int:
                        newValue = EditorGUI.IntField(position, label, (int)(currentValue / multiplier));
                        if (EditorGUI.EndChangeCheck())
                            prop.intValue = (int)(newValue * multiplier);
                        break;
                }

                EditorGUI.showMixedValue = false;

                MaterialEditor.EndProperty();
            }
            editor.EndAnimatedCheck();
        }
    }
}
