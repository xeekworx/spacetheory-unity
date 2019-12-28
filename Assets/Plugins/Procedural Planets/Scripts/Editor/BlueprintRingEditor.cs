using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPlanets
{
    /// <summary>
    /// Custom inspector for ring blueprints.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    /// 
    [CustomEditor(typeof(BlueprintRing))]
    public class BlueprintRingEditor : Editor
    {
        private const float LABEL_WIDTH = 145;
        private const float VALUE_WIDTH = 85;

        private BlueprintRing _script;

        // Serialized Object
        SerializedObject _target;

        // Serialized Properties        
        SerializedProperty _blueprintPropertyFloats;
        Dictionary<string, int> _indexFloat = new Dictionary<string, int>();

        SerializedProperty _blueprintPropertyColors;
        Dictionary<string, int> _indexColor = new Dictionary<string, int>();

        SerializedProperty _blueprintPropertyMaterials;
        Dictionary<string, int> _indexMaterial = new Dictionary<string, int>();

        void OnEnable()
        {

            _target = new SerializedObject(target);

            _script = (BlueprintRing)target;

            _blueprintPropertyFloats = _target.FindProperty("blueprintPropertyFloats");
            _blueprintPropertyColors = _target.FindProperty("blueprintPropertyColors");
            _blueprintPropertyMaterials = _target.FindProperty("blueprintPropertyMaterials");
        }

        public override void OnInspectorGUI()
        {

            _target.Update();

            UpdateIndex();
            EditorGUILayout.LabelField("RING BLUEPRINT TOOLS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (GUILayout.Button("Export Ring Blueprint to Clipboard (JSON)"))
            {
                GUIUtility.systemCopyBuffer = _script.ExportToJSON();
                EditorUtility.DisplayDialog("Finished", "Ring Blueprint was saved to clipboard", "Close");
            }

            if (GUILayout.Button("Import Ring Blueprint from Clipboard (JSON)"))
            {
                _script.ImportFromJSON(GUIUtility.systemCopyBuffer);
                _target.Update();
                _target.ApplyModifiedProperties();
                _target.Update();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();


            // RING SETTINGS
            EditorGUILayout.LabelField("RING SETTINGS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyMaterial("ringMaterial");
            RenderPropertyFloat("gradientDiffuse");
            RenderPropertyFloat("gradientAlpha");
            RenderPropertyFloat("diffuseHue");
            RenderPropertyFloat("diffuseSaturation");
            RenderPropertyFloat("diffuseLightness");
            RenderPropertyFloat("alphaContrast");
            RenderPropertyFloat("alphaLuminosity");
            EditorGUILayout.Space();
            RenderPropertyFloat("innerRadius");
            RenderPropertyFloat("outerRadius");
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            _target.ApplyModifiedProperties();
        }


        void UpdateIndex()
        {
            _indexFloat.Clear();
            for (int _i = 0; _i < _blueprintPropertyFloats.arraySize; _i++)
            {
                SerializedProperty property = _blueprintPropertyFloats.GetArrayElementAtIndex(_i);
                _indexFloat.Add(property.FindPropertyRelative("key").stringValue, _i);
            }

            _indexColor.Clear();
            for (int _i = 0; _i < _blueprintPropertyColors.arraySize; _i++)
            {
                SerializedProperty property = _blueprintPropertyColors.GetArrayElementAtIndex(_i);
                _indexColor.Add(property.FindPropertyRelative("key").stringValue, _i);
            }

            _indexMaterial.Clear();
            for (int _i = 0; _i < _blueprintPropertyMaterials.arraySize; _i++)
            {
                SerializedProperty property = _blueprintPropertyMaterials.GetArrayElementAtIndex(_i);
                _indexMaterial.Add(property.FindPropertyRelative("key").stringValue, _i);
            }
        }

        void RenderPropertyFloat(string _key)
        {
            SerializedProperty _s = GetBlueprintPropertyFloat(_key);
            EditorGUILayout.BeginHorizontal();
            float _min = _s.FindPropertyRelative("minValue").floatValue;
            float _max = _s.FindPropertyRelative("maxValue").floatValue;
            EditorGUILayout.LabelField(_s.FindPropertyRelative("label").stringValue, GUILayout.MaxWidth(LABEL_WIDTH));
            EditorGUILayout.MinMaxSlider(ref _min, ref _max, _s.FindPropertyRelative("minLimit").floatValue, _s.FindPropertyRelative("maxLimit").floatValue);
            if (_s.FindPropertyRelative("displayAsInt").boolValue)
            {
                EditorGUILayout.LabelField(_min.ToString("F0") + " - " + _max.ToString("F0"), GUILayout.MaxWidth(VALUE_WIDTH));
            }
            else
            {
                EditorGUILayout.LabelField(_min.ToString("F2") + " - " + _max.ToString("F2"), GUILayout.MaxWidth(VALUE_WIDTH));
            }

            _s.FindPropertyRelative("minValue").floatValue = _min;
            _s.FindPropertyRelative("maxValue").floatValue = _max;
            EditorGUILayout.EndHorizontal();

        }

        SerializedProperty GetBlueprintPropertyFloat(string _key)
        {
            int _i = 0;
            if (!_indexFloat.TryGetValue(_key, out _i)) return null;
            return _blueprintPropertyFloats.GetArrayElementAtIndex(_i);
        }

        void RenderPropertyColor(string _key)
        {
            SerializedProperty _s = GetBlueprintPropertyColor(_key);
            _s.FindPropertyRelative("color").colorValue = EditorGUILayout.ColorField(_s.FindPropertyRelative("label").stringValue, _s.FindPropertyRelative("color").colorValue);
            EditorGUI.indentLevel++;
            _s.FindPropertyRelative("hueRange").floatValue = EditorGUILayout.Slider("Hue Range", _s.FindPropertyRelative("hueRange").floatValue, 0.0f, 1.0f);
            _s.FindPropertyRelative("saturationRange").floatValue = EditorGUILayout.Slider("Saturation Range", _s.FindPropertyRelative("saturationRange").floatValue, 0.0f, 1.0f);
            _s.FindPropertyRelative("brightnessRange").floatValue = EditorGUILayout.Slider("Brightness Range", _s.FindPropertyRelative("brightnessRange").floatValue, 0.0f, 1.0f);
            EditorGUI.indentLevel--;
        }

        SerializedProperty GetBlueprintPropertyColor(string _key)
        {
            int _i = 0;
            if (!_indexColor.TryGetValue(_key, out _i)) return null;
            return _blueprintPropertyColors.GetArrayElementAtIndex(_i);
        }

        void RenderPropertyMaterial(string _key)
        {
            SerializedProperty _s = GetBlueprintPropertyMaterial(_key);
            EditorGUILayout.BeginHorizontal();
            _s.FindPropertyRelative("mask").intValue = EditorGUILayout.MaskField(_s.FindPropertyRelative("label").stringValue, _s.FindPropertyRelative("mask").intValue, GetRelativeStringArray(_s, "maskDisplay"));
            EditorGUILayout.EndHorizontal();
        }

        SerializedProperty GetBlueprintPropertyMaterial(string _key)
        {
            int _i = 0;
            if (!_indexMaterial.TryGetValue(_key, out _i)) return null;
            return _blueprintPropertyMaterials.GetArrayElementAtIndex(_i);
        }

        string[] GetRelativeStringArray(SerializedProperty _s, string _relative)
        {
            string[] _string = new string[_s.FindPropertyRelative(_relative).arraySize];
            for (int _i = 0; _i < _s.FindPropertyRelative(_relative).arraySize; _i++)
            {
                _string[_i] = _s.FindPropertyRelative(_relative).GetArrayElementAtIndex(_i).stringValue;
            }
            return _string;
        }
    }
}
