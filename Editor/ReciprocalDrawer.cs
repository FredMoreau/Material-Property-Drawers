using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class ReciprocalDrawer : CustomMaterialPropertyDrawerBase
    {
        public ReciprocalDrawer() { }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            switch (prop.propertyType)
            {
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
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
                        newValue = EditorGUI.FloatField(position, label, 1 / currentValue);
                        break;
                    case ShaderPropertyType.Range:
                        newValue = EditorGUI.Slider(position, label, 1 / currentValue, prop.rangeLimits.x, prop.rangeLimits.y);
                        break;
                    default:
                        newValue = currentValue;
                        break;
                }

                if (EditorGUI.EndChangeCheck())
                    prop.floatValue = 1 / newValue;

                EditorGUI.showMixedValue = false;

                MaterialEditor.EndProperty();
            }
            editor.EndAnimatedCheck();
        }
    }
}
