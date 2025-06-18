using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyDrawers
{
    public abstract class CustomMaterialPropertyDrawerBase : MaterialPropertyDrawer
    {
        protected bool isVisible = true, isEnabled = true;

        /// <summary>
        /// Call base.GetPropertyHeight when overriding as it handles extra attributes for visibility and enabled states.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="label"></param>
        /// <param name="editor"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            GetVisibility(prop, editor);
            return isVisible ? base.GetPropertyHeight(prop, label, editor) : 0;
        }

        void GetVisibility(MaterialProperty prop, MaterialEditor editor)
        {
            Material material = (Material)editor.target;
            Shader shader = material.shader;
            int pIndex = shader.FindPropertyIndex(prop.name);
            string[] pAttr = shader.GetPropertyAttributes(pIndex);
            string visibleAttr = null;
            string enableAttr = null;
            for (int i = 0; i < pAttr.Length; i++)
            {
                if (pAttr[i].StartsWith("VisibleIf"))
                {
                    visibleAttr = pAttr[i];
                    break;
                }
                else if (pAttr[i].StartsWith("EnableIf"))
                {
                    enableAttr = pAttr[i];
                    break;
                }
            }
            if (!string.IsNullOrEmpty(visibleAttr) && GetAttribute(visibleAttr, "VisibleIf", out string parameters))
            {
                string[] param = parameters.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (param.Length != 0)
                {
                    var properties = MaterialEditor.GetMaterialProperties(prop.targets);
                    MaterialProperty otherProp = FindProperty(param[0], properties);
                    if (otherProp != null)
                    {
                        isVisible = CheckProperty(otherProp) ^ (param.Length == 2 && param[1] == "true");
                    }
                    else
                    {
                        LocalKeyword keyword = shader.keywordSpace.FindKeyword(param[0]);
                        if (keyword != null)
                            isVisible = material.IsKeywordEnabled(keyword) ^ (param.Length == 2 && param[1] == "true");
                    }
                }
            }
            if (!string.IsNullOrEmpty(enableAttr) && GetAttribute(enableAttr, "EnableIf", out string eParameters))
            {
                string[] param = eParameters.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (param.Length != 0)
                {
                    var properties = MaterialEditor.GetMaterialProperties(prop.targets);
                    MaterialProperty otherProp = FindProperty(param[0], properties);

                    if (otherProp != null)
                    {
                        isEnabled = CheckProperty(otherProp) ^ (param.Length == 2 && param[1] == "true");
                    }
                    else
                    {
                        LocalKeyword keyword = shader.keywordSpace.FindKeyword(param[0]);
                        if (keyword != null)
                            isEnabled = material.IsKeywordEnabled(keyword) ^ (param.Length == 2 && param[1] == "true");
                    }
                }
            }
        }

        protected static bool GetAttribute(string input, string attribute, out string parameters)
        {
            string escapedPrefix = Regex.Escape(attribute);
            string pattern = $"^{escapedPrefix}\\(([^,()]+)(?:,([^,()]+))?\\)$";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string part1 = match.Groups[1].Value.Trim();
                string part2 = match.Groups[2].Success ? match.Groups[2].Value.Trim() : null;

                parameters = part2 == null ? part1 : $"{part1},{part2}";
                return true;
            }
            parameters = null;
            return false;
        }

        protected static bool CheckProperty(MaterialProperty prop) => prop.propertyType switch
        {
            ShaderPropertyType.Color => prop.colorValue.maxColorComponent > 0 || prop.colorValue.a > 0,
            ShaderPropertyType.Vector => prop.vectorValue.magnitude > 0,
            ShaderPropertyType.Float => prop.floatValue > 0,
            ShaderPropertyType.Range => prop.floatValue > 0,
            ShaderPropertyType.Texture => prop.textureValue != null,
            ShaderPropertyType.Int => prop.intValue > 0,
            _ => false
        };

        protected static MaterialProperty FindProperty(string propertyName, MaterialProperty[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
                if (properties[i] != null && properties[i].name == propertyName)
                    return properties[i];

            return null;
        }

        protected static void SetKeyword(MaterialProperty prop, string keyword, bool state)
        {
            bool enableKeyword = CheckProperty(prop);

            if (enableKeyword == state)
                EnableKeyword(prop.targets, keyword);
            else
                DisableKeyword(prop.targets, keyword);
        }

        protected static void EnableKeyword(UnityEngine.Object[] mats, string keyword)
        {
            for (int i = 0; i < mats.Length; i++)
                if (mats[i] is Material material)
                    material.EnableKeyword(keyword);
        }

        protected static void DisableKeyword(UnityEngine.Object[] mats, string keyword)
        {
            for (int i = 0; i < mats.Length; i++)
                if (mats[i] is Material material)
                    material.DisableKeyword(keyword);
        }
    }
}
