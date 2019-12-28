using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPlanets
{
    /// <summary>
    /// Custom inspector for gas planet blueprints.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    
    [CustomEditor(typeof(BlueprintGasPlanet))]
    public class BlueprintPlanetGasEditor : Editor
    {
        // Constants
        private const float LABEL_WIDTH = 145;
        private const float VALUE_WIDTH = 95;
        private const float PERCENT_WIDTH = 60;

        // Reference to target object
        private BlueprintGasPlanet _script;

        // Serialized Object
        SerializedObject _target;

        // Serialized Properties        
        SerializedProperty _blueprintPropertyFloats;
        SerializedProperty _blueprintPropertyColors;
        SerializedProperty _blueprintPropertyMaterials;
        SerializedProperty _ringProbability;

        // Unity cannot serialize dictionaries so lists are translated to dictionaries in the editor script and vice versa
        Dictionary<string, int> _indexFloat = new Dictionary<string, int>();
        Dictionary<string, int> _indexColor = new Dictionary<string, int>();
        Dictionary<string, int> _indexMaterial = new Dictionary<string, int>();

        void OnEnable()
        {
            // Get the target object
            _target = new SerializedObject(target);

            // Get a reference to the target script component
            _script = (BlueprintGasPlanet)target;

            // Find the properties of the target
            _blueprintPropertyFloats = _target.FindProperty("blueprintPropertyFloats");
            _blueprintPropertyColors = _target.FindProperty("blueprintPropertyColors");
            _blueprintPropertyMaterials = _target.FindProperty("blueprintPropertyMaterials");
            _ringProbability = _target.FindProperty("ringProbability");
        }


        /// <summary>
        /// Displays and allows interaction in a custom inspector for Gas Planets Blueprints
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Call target update
            _target.Update();

            // Update the index dictionaries
            UpdateIndex();
            EditorGUILayout.LabelField("PLANET BLUEPRINT TOOLS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if (GUILayout.Button(new GUIContent("Create Planet", "Create planet using this blueprint")))
                PlanetManager.CreatePlanet(Vector3.zero, -1, _script.gameObject.name);
            EditorGUILayout.Space();

            if (GUILayout.Button("Export Blueprint to Clipboard (JSON)"))
            {
                GUIUtility.systemCopyBuffer = _script.ExportToJSON();
                EditorUtility.DisplayDialog("Finished", "Planet Blueprint was saved to clipboard", "Close");
            }

            if (GUILayout.Button("Import Blueprint from Clipboard (JSON)"))
            {
                string _str = GUIUtility.systemCopyBuffer;
                _script.ImportFromJSON(_str);
                _target.Update();
                _target.ApplyModifiedProperties();
                _target.Update();
            }

            EditorGUILayout.Space();

            if (_script.transform.Find("Ring") == null)
            {
                if (GUILayout.Button(new GUIContent("Create Planetary Ring", "Add planetary ring to this blueprint")))
                    _script.CreateRing();
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Remove Planetary Ring", "Remove planetary ring from this blueprint")))
                {
                    DestroyImmediate(_script.transform.Find("Ring").gameObject);
                }
                if (GUILayout.Button(new GUIContent("Edit Planetary Ring", "Edit planetary ring of this blueprint")))
                    Selection.activeGameObject = _script.transform.Find("Ring").gameObject;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _ringProbability.floatValue = EditorGUILayout.Slider("Probability to have Ring", _ringProbability.floatValue, 0f, 1f);
                EditorGUILayout.LabelField((_ringProbability.floatValue * 100).ToString("F1") + "%", GUILayout.MaxWidth(PERCENT_WIDTH));
                GUILayout.EndHorizontal();

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            // PLANET SETTINGS
            EditorGUILayout.LabelField("PLANET SETTINGS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            RenderPropertyMaterial("gas");
            RenderPropertyFloat("horizontalTiling");
            RenderPropertyFloat("verticalTiling");
            RenderPropertyFloat("turbulence");
            RenderPropertyFloat("turbulenceScale");
            RenderPropertyFloat("turbulenceDisorder");
            RenderPropertyFloat("separation");
            RenderPropertyFloat("blurriness");
            RenderPropertyFloat("smoothness");            
            RenderPropertyFloat("palette");
            RenderPropertyFloat("detail");
            RenderPropertyFloat("detailOffset");
            RenderPropertyFloat("contrast");
            RenderPropertyFloat("hue");
            RenderPropertyFloat("saturation");
            RenderPropertyFloat("brightness");
            RenderPropertyFloat("banding");
            RenderPropertyFloat("solidness");
            RenderPropertyFloat("faintness");
            RenderPropertyColor("faintnessColor");
            RenderPropertyFloat("roughness");
            RenderPropertyFloat("stormMaskIndex");
            RenderPropertyFloat("stormSquash");
            RenderPropertyColor("stormColor");
            RenderPropertyFloat("stormTint");
            RenderPropertyFloat("stormScale");
            RenderPropertyFloat("stormNoise");
            RenderPropertyColor("atmosphereColor");
            RenderPropertyFloat("atmosphereFalloff");
            RenderPropertyColor("twilightColor");
            
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            _target.ApplyModifiedProperties();
        }

        /// <summary>
        /// Updates the dictionaries used since Unity do not allow serialization of dictionaries (translation to/from lists are needed)
        /// </summary>
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

        /// <summary>
        /// Renders a PropertyFloat in inspector base on the key
        /// </summary>
        /// <param name="_key"></param>
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

        /// <summary>
        /// Gets the SerializedProperty of a PropertyFloat based on the key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>SerializedProperty of PropertyFloat</returns>
        SerializedProperty GetBlueprintPropertyFloat(string _key)
        {
            int _i = 0;
            if (!_indexFloat.TryGetValue(_key, out _i)) return null;
            return _blueprintPropertyFloats.GetArrayElementAtIndex(_i);
        }

        /// <summary>
        /// Renders a editor fields for PropertyColor in the inspector
        /// </summary>
        /// <param name="_key"></param>
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

        /// <summary>
        /// Gets the SerializedProperty of a PropertyColor based on the key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>SerializedProperty of PropertyColor</returns>
        SerializedProperty GetBlueprintPropertyColor(string _key)
        {
            int _i = 0;
            if (!_indexColor.TryGetValue(_key, out _i)) return null;
            return _blueprintPropertyColors.GetArrayElementAtIndex(_i);
        }

        /// <summary>
        /// Renders custom inspector for PropertyMaterial in inspector. 
        /// </summary>
        /// <param name="_key"></param>
        void RenderPropertyMaterial(string _key)
        {
            SerializedProperty _s = GetBlueprintPropertyMaterial(_key);
            EditorGUILayout.BeginHorizontal();
            _s.FindPropertyRelative("mask").intValue = EditorGUILayout.MaskField(_s.FindPropertyRelative("label").stringValue, _s.FindPropertyRelative("mask").intValue, GetRelativeStringArray(_s, "maskDisplay"));
            EditorGUILayout.EndHorizontal();

        }

        /// <summary>
        /// Gets the SerializedProperty of a PropertyMaterial based on the key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>SerializedProperty of PropertyMaterial</returns>
        SerializedProperty GetBlueprintPropertyMaterial(string _key)
        {
            int _i = 0;
            if (!_indexMaterial.TryGetValue(_key, out _i)) return null;
            return _blueprintPropertyMaterials.GetArrayElementAtIndex(_i);
        }

        /// <summary>
        /// Gets the relative string array of a SerializedProperty
        /// </summary>
        /// <param name="_s"></param>
        /// <param name="_relative"></param>
        /// <returns>String array of relative to a SerializedProperty</returns>
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
