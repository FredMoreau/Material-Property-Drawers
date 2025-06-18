using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public class MaterialWorkflowDrawer : CustomMaterialPropertyDrawerBase
    {
        string metallicMapScaleProp, specularMapProp, specularColorProp, smoothnessProp, smoothnessSourceProp;
        const string specularModeKeyword = "_SPECULAR_SETUP";
        const string surfaceTypeTransparent = "_SURFACE_TYPE_TRANSPARENT";
        const string metallicSpecGlossMapKeyword = "_METALLICSPECGLOSSMAP";
        const string smoothnessTextureAlbedoChannelA = "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A";

        bool specularMode = false, transparentType = false;

        public MaterialWorkflowDrawer() { }

        public MaterialWorkflowDrawer(string metallicMapScaleProp, string specularMapProp, string specularColorProp, string smoothnessProp, string smoothnessSourceProp)
        {
            this.metallicMapScaleProp = metallicMapScaleProp;
            this.specularMapProp = specularMapProp;
            this.specularColorProp = specularColorProp;
            this.smoothnessProp = smoothnessProp;
            this.smoothnessSourceProp = smoothnessSourceProp;
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
            MaterialProperty metallicMapScaleProperty = FindProperty(metallicMapScaleProp, properties);
            MaterialProperty specularMapProperty = FindProperty(specularMapProp, properties);
            MaterialProperty specularColorProperty = FindProperty(specularColorProp, properties);
            MaterialProperty smoothnessProperty = FindProperty(smoothnessProp, properties);
            MaterialProperty smoothnessSourceProperty = FindProperty(smoothnessSourceProp, properties);

            Material target = (Material)editor.target;
            bool specularSetup = target.IsKeywordEnabled(specularModeKeyword);
            bool transparent = target.IsKeywordEnabled(surfaceTypeTransparent);

            using (new EditorGUI.DisabledScope(!isEnabled || (prop.propertyFlags & ShaderPropertyFlags.PerRendererData) != 0))
            {
                EditorGUI.BeginChangeCheck();

                string[] smoothnessSourceLabels = specularSetup ? new string[] { "Specular Alpha", "Albedo Alpha" } : new string[] { "Metallic Alpha", "Albedo Alpha" };

                if (specularSetup) // specular mode
                {
                    editor.TexturePropertySingleLine(new GUIContent(specularMapProperty.displayName), specularMapProperty, specularColorProperty);
                    EditorGUI.indentLevel += 2;
                    editor.ShaderProperty(smoothnessProperty, smoothnessProperty.displayName);
                    EditorGUI.indentLevel += 1;
                }
                else // metallic mode
                {
                    editor.TexturePropertySingleLine(new GUIContent(label), prop, metallicMapScaleProperty);
                    EditorGUI.indentLevel += 2;
                    editor.ShaderProperty(smoothnessProperty, smoothnessProperty.displayName);
                    EditorGUI.indentLevel += 1;
                }

                // DONE: make property field work with material variants override
                // DONE: set the property to Metallic/Specular Alpha (0) when transparent
                // TODO: make it properly display mixed values
                if (transparent)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.Popup("Source", 0, smoothnessSourceLabels);
                    }
                }
                else
                {
                    var smoothnessSourceRect = EditorGUILayout.GetControlRect();
                    editor.BeginAnimatedCheck(smoothnessSourceRect, smoothnessSourceProperty);
                    MaterialEditor.BeginProperty(smoothnessSourceRect, smoothnessSourceProperty);
                    EditorGUI.showMixedValue = smoothnessSourceProperty.hasMixedValue;
                    smoothnessSourceProperty.floatValue = EditorGUI.Popup(smoothnessSourceRect, "Source", (int)smoothnessSourceProperty.floatValue, smoothnessSourceLabels);
                    EditorGUI.showMixedValue = false;
                    MaterialEditor.EndProperty();
                    editor.EndAnimatedCheck();
                }

                EditorGUI.indentLevel -= 3;
                if (EditorGUI.EndChangeCheck() || specularMode != specularSetup || transparentType != transparent)
                {
                    if ((specularSetup && specularMapProperty.textureValue != null) ||
                        (!specularSetup && prop.textureValue != null))
                        target.EnableKeyword(metallicSpecGlossMapKeyword);
                    else
                        target.DisableKeyword(metallicSpecGlossMapKeyword);

                    if (smoothnessSourceProperty.floatValue > 0 && !transparent)
                        target.EnableKeyword(smoothnessTextureAlbedoChannelA);
                    else
                        target.DisableKeyword(smoothnessTextureAlbedoChannelA);

                    specularMode = specularSetup;
                    transparentType = transparent;
                }
            }
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);

            var properties = MaterialEditor.GetMaterialProperties(prop.targets);
            MaterialProperty metallicMapScaleProperty = FindProperty(metallicMapScaleProp, properties);
            MaterialProperty specularMapProperty = FindProperty(specularMapProp, properties);
            MaterialProperty specularColorProperty = FindProperty(specularColorProp, properties);
            MaterialProperty smoothnessProperty = FindProperty(smoothnessProp, properties);
            MaterialProperty smoothnessSourceProperty = FindProperty(smoothnessSourceProp, properties);

            for (int i = 0; i < prop.targets.Length; i++)
            {
                Material target = (Material)prop.targets[i];
                bool specularSetup = target.IsKeywordEnabled(specularModeKeyword);

                if ((specularSetup && specularMapProperty.textureValue != null) ||
                        (!specularSetup && prop.textureValue != null))
                    target.EnableKeyword(metallicSpecGlossMapKeyword);
                else
                    target.DisableKeyword(metallicSpecGlossMapKeyword);

                if (smoothnessSourceProperty.floatValue > 0)
                    target.EnableKeyword(smoothnessTextureAlbedoChannelA);
                else
                    target.DisableKeyword(smoothnessTextureAlbedoChannelA);
            }
        }
    }
}
