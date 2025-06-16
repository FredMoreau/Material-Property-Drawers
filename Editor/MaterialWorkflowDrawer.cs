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

        public MaterialWorkflowDrawer() { }

        public MaterialWorkflowDrawer(string metallicMapScaleProp, string specularMapProp, string specularColorProp, string smoothnessProp, string smoothnessSourceProp)
        {
            this.metallicMapScaleProp = metallicMapScaleProp;
            this.specularMapProp = specularMapProp;
            this.specularColorProp = specularColorProp;
            this.smoothnessProp = smoothnessProp;
            this.smoothnessSourceProp = smoothnessSourceProp;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor) => 0;

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
                if (specularSetup) // specular mode
                {
                    //editor.TexturePropertyTwoLines(new GUIContent(specularMapProperty.displayName), specularMapProperty, specularColorProperty, new GUIContent(smoothnessProperty.displayName), smoothnessProperty);
                    editor.TexturePropertySingleLine(new GUIContent(specularMapProperty.displayName), specularMapProperty, specularColorProperty);
                    EditorGUI.indentLevel += 2;
                    editor.ShaderProperty(smoothnessProperty, smoothnessProperty.displayName);
                    EditorGUI.indentLevel += 1;
                    // TODO: make property field work with material variants override
                    // TODO: set the property to Specular Alpha (0) when transparent
                    using (new EditorGUI.DisabledScope(transparent))
                        smoothnessSourceProperty.floatValue = EditorGUILayout.Popup("Source", (int)smoothnessSourceProperty.floatValue, new string[] { "Specular Alpha", "Albedo Alpha" });
                }
                else // metallic mode
                {
                    //editor.TexturePropertyTwoLines(new GUIContent(label), prop, metallicMapScaleProperty, new GUIContent(smoothnessProperty.displayName), smoothnessProperty);
                    editor.TexturePropertySingleLine(new GUIContent(label), prop, metallicMapScaleProperty);
                    EditorGUI.indentLevel += 2;
                    editor.ShaderProperty(smoothnessProperty, smoothnessProperty.displayName);
                    EditorGUI.indentLevel += 1;
                    using (new EditorGUI.DisabledScope(transparent))
                        smoothnessSourceProperty.floatValue = EditorGUILayout.Popup("Source", (int)smoothnessSourceProperty.floatValue, new string[] { "Metallic Alpha", "Albedo Alpha" });
                }
                EditorGUI.indentLevel -= 3;
                if (EditorGUI.EndChangeCheck() && metallicMapScaleProperty != null)
                {
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
