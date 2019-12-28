using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace ProceduralPlanets
{
    /// <summary>
    /// Custom inspector editor for Gas Planets.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    
    [CustomEditor(typeof(GasPlanet))]
    [CanEditMultipleObjects]
    public class GasPlanetEditor : Editor
    {
        // Constants
        const float LABEL_WIDTH = 145;
        const float VALUE_WIDTH = 90;
        const float MODIFY_DELAY = 0.0f;

        // Flags
        bool _resetOverrides;
        bool _newSeed;
        bool _newVariation;
        bool _newPlanetBlueprint;
        float _modifyTimestamp;
        bool _modifyWait;
        bool _modifiedTextureMaps;
        bool _modifiedStormMask;
        bool _modifiedShader;
        bool _updateSerializedPlanetCacheRequired;

        bool _hasRing;

        // Color of modified settings in inspector
        private Color _colorModified = new Color(1.0f, 0.7f, 0.4f);

        //Reference to the target component (needed for some direct method calls)
        private GasPlanet _script;

        // Serialized Object
        SerializedObject _target;

        // Serialized Properties
        SerializedProperty _serializedPlanetCache;
        SerializedProperty _rebuildMapsNeeded;
        SerializedProperty _rebuildStormMaskNeeded;
        SerializedProperty _updateShaderNeeded;

        SerializedProperty _planetBlueprintIndex;
        SerializedProperty _planetBlueprintOverride;

        SerializedProperty _planetSeed;
        SerializedProperty _variationSeed;

        SerializedProperty _propertyFloats;
        SerializedProperty _propertyColors;
        SerializedProperty _propertyMaterials;

        SerializedProperty _textureProgressiveStep;

        // Unity cannot serialize dictionaries so lists are translated to dictionaries in the editor script and vice versa
        Dictionary<string, int> _indexFloats = new Dictionary<string, int>();
        Dictionary<string, int> _indexColors = new Dictionary<string, int>();
        Dictionary<string, int> _indexMaterials = new Dictionary<string, int>();

        void OnEnable()
        {
            // Get the target object
            _target = new SerializedObject(target);

            // Ensure editor application run Update method (doesn't happen every frame, only when OnInspectorGUI() is called.
            EditorApplication.update += Update;

            // Find the properties of the target
            _serializedPlanetCache = _target.FindProperty("serializedPlanetCache");
            _rebuildMapsNeeded = _target.FindProperty("_rebuildMapsNeeded");
            _rebuildStormMaskNeeded = _target.FindProperty("_rebuildStormMaskNeeded");
            _updateShaderNeeded = _target.FindProperty("_updateShaderNeeded");
            _propertyFloats = _target.FindProperty("propertyFloats");
            _propertyColors = _target.FindProperty("propertyColors");
            _propertyMaterials = _target.FindProperty("propertyMaterials");
            _planetSeed = _target.FindProperty("planetSeed");
            _variationSeed = _target.FindProperty("variationSeed");
            _planetBlueprintIndex = _target.FindProperty("planetBlueprintIndex");
            _planetBlueprintOverride = _target.FindProperty("planetBlueprintOverride");
            _textureProgressiveStep = _target.FindProperty("textureProgressiveStep");
        }

        void OnDisable()
        {
            // Remove Update method event
            EditorApplication.update -= Update;
        }

        /// <summary>
        /// Displays and allows interaction in a custom inspector for Gas Planets
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                if (_script != null)
                    _hasRing = _script.HasRing();
            }

            // Editing prefabs that are not instantiated in a scene is not supported.
            if (EditorUtility.IsPersistent(Selection.activeObject))
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("You must instantiate planet prefab in a scene to edit properties.", MessageType.Info);
                EditorGUILayout.Separator();
                EditorGUILayout.Space();
                return;
            }

            // Update the serialized object
            _target.Update();

            UpdateIndex();

            _script = (GasPlanet)target;

            EditorGUILayout.LabelField("PLANET", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            // RANDOM
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            _planetSeed.intValue = EditorGUILayout.DelayedIntField("Planet Seed", _planetSeed.intValue);
            if (GUILayout.Button("Random")) _planetSeed.intValue = UnityEngine.Random.Range(0, int.MaxValue - 1000000);
            if (EditorGUI.EndChangeCheck())
            {
                _textureProgressiveStep.intValue = -1;
                _newSeed = true;
                _target.ApplyModifiedProperties();
                _target.Update();
                _modifiedTextureMaps = true;
                _modifiedStormMask = true;
            }
            EditorGUILayout.EndHorizontal();
            _resetOverrides = EditorGUILayout.Toggle("Reset Overrides", _resetOverrides);

            // VARIANT
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            _variationSeed.intValue = EditorGUILayout.IntField("Variant", _variationSeed.intValue);
            if (GUILayout.Button("Random")) _variationSeed.intValue = UnityEngine.Random.Range(0, 10000);
            if (EditorGUI.EndChangeCheck())
            {
                _newVariation = true;
                _target.ApplyModifiedProperties();
                _target.Update();
                _modifiedTextureMaps = true;
                _modifiedStormMask = true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("PLANET TOOLS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (GUILayout.Button("Export to Clipboard (JSON)"))
            {
                GUIUtility.systemCopyBuffer = _script.ExportToJSON(SimpleJSON.StringFormat.JSON_EASY_READ);
                EditorUtility.DisplayDialog("Finished", "Planet JSON was saved to clipboard", "Close");
            }
            if (GUILayout.Button("Export to Clipboard (Escaped JSON)"))
            {
                GUIUtility.systemCopyBuffer = _script.ExportToJSON(SimpleJSON.StringFormat.JSON_ESCAPED);
                EditorUtility.DisplayDialog("Finished", "Planet Escaped JSON string was saved to clipboard", "Close");
            }
            if (GUILayout.Button("Export to Clipboard (Base64 JSON)"))
            {
                GUIUtility.systemCopyBuffer = _script.ExportToJSON(SimpleJSON.StringFormat.JSON_BASE64);
                EditorUtility.DisplayDialog("Finished", "Planet Base64 string was saved to clipboard", "Close");
            }

            if (GUILayout.Button("Import from Clipboard (JSON / Base64)"))
            {
                _script.ImportFromJSON(GUIUtility.systemCopyBuffer, true);
                _target.Update();
                _target.ApplyModifiedProperties();
                _target.Update();
                _modifiedTextureMaps = true;
                _modifiedStormMask = true;
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Force Overrides", "Change all properties as being overridden")))
            {
                for (int _i = 0; _i < _propertyFloats.arraySize; _i++)
                    _propertyFloats.GetArrayElementAtIndex(_i).FindPropertyRelative("overrideRandom").boolValue = true;
                for (int _i = 0; _i < _propertyColors.arraySize; _i++)
                    _propertyColors.GetArrayElementAtIndex(_i).FindPropertyRelative("overrideRandom").boolValue = true;
                for (int _i = 0; _i < _propertyMaterials.arraySize; _i++)
                    _propertyMaterials.GetArrayElementAtIndex(_i).FindPropertyRelative("overrideRandom").boolValue = true;
            }
            if (GUILayout.Button(new GUIContent("Remove Overrides", "Remove all property overrides execept blueprint")))
            {
                for (int _i = 0; _i < _propertyFloats.arraySize; _i++)
                    _propertyFloats.GetArrayElementAtIndex(_i).FindPropertyRelative("overrideRandom").boolValue = false;
                for (int _i = 0; _i < _propertyColors.arraySize; _i++)
                    _propertyColors.GetArrayElementAtIndex(_i).FindPropertyRelative("overrideRandom").boolValue = false;
                for (int _i = 0; _i < _propertyMaterials.arraySize; _i++)
                    _propertyMaterials.GetArrayElementAtIndex(_i).FindPropertyRelative("overrideRandom").boolValue = false;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (_hasRing)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Edit Rings", "Shortcut to select the child ring gameobject")))
                {
                    Selection.activeGameObject = _script.GetRing().gameObject;
                }

                if (GUILayout.Button(new GUIContent("Remove Rings", "Destroys / removes planetary ring object from this planet")))
                {
                    if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to destroy (remove) the rings for this planet?", "Yes", "Cancel"))
                    {
                        _script.DestroyRing();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Add Planetary Rings", "Adds a planetary ring object to this planet")))
                {
                    _script.CreateRing();
                }
            }
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Bake Static Planet Prefab", "Create a prefab with static textures")))
            {

                Texture2D[] _tex2DArray = _script.GetBakedTextures();
                BakePlanetAsset(_tex2DArray, _script.GetRing());
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // PLANET BLUEPRINT
            EditorGUILayout.LabelField("PLANET BLUEPRINT", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            PlanetManager.Instance.RefreshLists();
            List<string> _l = new List<string>();
            foreach (BlueprintGasPlanet _cs in PlanetManager.Instance.listGasPlanetBlueprints)
                _l.Add(_cs.name);
            Color _orgGUIColor = GUI.color;
            EditorGUILayout.BeginHorizontal();

            if (_newSeed)
            {
                if (_resetOverrides || !_planetBlueprintOverride.boolValue)
                {
                    _script.SetPlanetBlueprint();
                    _updateSerializedPlanetCacheRequired = true;
                    _planetBlueprintOverride.boolValue = false;
                }
            }
            else
            {
                if (_planetBlueprintOverride.boolValue)
                {
                    GUI.color = _colorModified;
                }

                EditorGUI.BeginChangeCheck();
                _planetBlueprintIndex.intValue = EditorGUILayout.Popup("Planet Blueprint", _planetBlueprintIndex.intValue, _l.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    _planetBlueprintOverride.boolValue = true;
                    _target.ApplyModifiedProperties();
                    _target.Update();
                    _script.SetPlanetBlueprint(_planetBlueprintIndex.intValue, true, true);
                    _newPlanetBlueprint = true;
                    _modifiedTextureMaps = true;
                    _modifiedShader = true;
                    _updateSerializedPlanetCacheRequired = true;
                }

                if (_planetBlueprintOverride.boolValue)
                {
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        _script.SetPlanetBlueprint(-1, true, true);
                        _newPlanetBlueprint = true;
                    }
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.Button(" ", GUILayout.Width(20));
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = _orgGUIColor;
                EditorGUI.indentLevel--;
            }
            // END PLANET BLUEPRINT

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("PLANET SETTINGS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyMaterial("gas");
            RenderPropertyFloat("gasSeed");
            RenderPropertyFloat("horizontalTiling");
            RenderPropertyFloat("verticalTiling");
            RenderPropertyFloat("turbulence");
            RenderPropertyFloat("turbulenceScale");
            RenderPropertyFloat("turbulenceDisorder");
            RenderPropertyFloat("separation");
            RenderPropertyFloat("smoothness");
            RenderPropertyFloat("blurriness");
            RenderPropertyFloat("hue");
            RenderPropertyFloat("saturation");
            RenderPropertyFloat("brightness");
            RenderPropertyFloat("contrast");
            RenderPropertyFloat("palette");
            RenderPropertyFloat("detail");
            RenderPropertyFloat("detailOffset");
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
            RenderPropertyFloat("atmosphereFalloff");
            RenderPropertyColor("atmosphereColor");
            RenderPropertyColor("twilightColor");
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (_modifiedShader)
            {
                _updateShaderNeeded.boolValue = true;
                _modifiedShader = false;
            }

            if (_modifiedTextureMaps)
            {
                if (!_modifyWait)
                {
                    _modifyTimestamp = Time.realtimeSinceStartup;
                    _modifyWait = true;
                }
            }

            if (_modifiedStormMask)
            {
                _rebuildStormMaskNeeded.boolValue = true;
                _modifiedStormMask = false;

            }

            // Apply the modified properties        
            if (_newSeed || _newVariation || _newPlanetBlueprint)
            {
                _target.Update();
                _newSeed = false;
                _newVariation = false;
                _newPlanetBlueprint = false;
                Repaint();
            }
            else
            {
                _target.ApplyModifiedProperties();
            }

            if (_updateSerializedPlanetCacheRequired)
            {
                _target.Update();
                _serializedPlanetCache.stringValue = _script.ExportToJSON(SimpleJSON.StringFormat.JSON_COMPACT);
                _target.ApplyModifiedProperties();
                _updateSerializedPlanetCacheRequired = false;
            }
        }

        /// <summary>
        /// Update runs in editor (only when changes are made) and a time delay is used to ensure that updates are not called too frequently which would otherwise lag the editor.
        /// </summary>
        void Update()
        {
            if (_modifyWait && Time.realtimeSinceStartup > _modifyTimestamp + MODIFY_DELAY)
            {
                _target.Update();

                if (_modifiedTextureMaps)
                {
                    _rebuildMapsNeeded.boolValue = true;
                    _modifiedTextureMaps = false;
                }

                _modifyTimestamp = 0f;
                _modifyWait = false;
                _target.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Renders a PropertyFloat in inspector base on the key
        /// </summary>
        /// <param name="_key"></param>
        void RenderPropertyFloat(string _key)
        {
            bool _GUIstate = GUI.enabled;
            Color _orgGUIColor = GUI.color;
            EditorGUILayout.BeginHorizontal();
            SerializedProperty _s = GetPropertyFloat(_key);
            if (_s == null)
            {
                Debug.LogError("PropertyFloat not found: " + _key);
                return;
            }

            if (_newSeed || _newVariation || _newPlanetBlueprint)
            {
                if ((_newSeed && _resetOverrides) || !_s.FindPropertyRelative("overrideRandom").boolValue)
                {
                    _script.SetPropertyFloat(_key);
                    _s.FindPropertyRelative("overrideRandom").boolValue = false;
                }
                return;
            }

            if (_s.FindPropertyRelative("overrideRandom").boolValue)
            {
                GUI.color = _colorModified;
            }

            EditorGUI.BeginChangeCheck();

            if (_s.FindPropertyRelative("clamp01").boolValue)
            {
                _s.FindPropertyRelative("value").floatValue = EditorGUILayout.Slider(_s.FindPropertyRelative("label").stringValue, _s.FindPropertyRelative("value").floatValue, 0.0f, 1.0f);
            }
            else
            {
                if (_s.FindPropertyRelative("displayAsInt").boolValue)
                {
                    _s.FindPropertyRelative("value").floatValue = EditorGUILayout.Slider(_s.FindPropertyRelative("label").stringValue, Mathf.RoundToInt(_s.FindPropertyRelative("value").floatValue), Mathf.RoundToInt(_s.FindPropertyRelative("minValue").floatValue), Mathf.RoundToInt(_s.FindPropertyRelative("maxValue").floatValue));
                }
                else
                {
                    _s.FindPropertyRelative("value").floatValue = EditorGUILayout.Slider(_s.FindPropertyRelative("label").stringValue, _s.FindPropertyRelative("value").floatValue, _s.FindPropertyRelative("minValue").floatValue, _s.FindPropertyRelative("maxValue").floatValue);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                _s.FindPropertyRelative("overrideRandom").boolValue = true;
                _updateSerializedPlanetCacheRequired = true;
                _modifiedShader = true;
                HandleModifiedTextures(_s);
            }

            if (_s.FindPropertyRelative("overrideRandom").boolValue)
            {
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _target.ApplyModifiedProperties();
                    _script.SetPropertyFloat(_key);
                    _target.Update();
                    _updateSerializedPlanetCacheRequired = true;
                    HandleModifiedTextures(_s);
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(" ", GUILayout.Width(20));
                GUI.enabled = true;
            }

            EditorGUILayout.EndHorizontal();
            GUI.color = _orgGUIColor;
            GUI.enabled = _GUIstate;
        }

        /// <summary>
        /// Gets the SerializedProperty of a PropertyFloat based on the key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>SerializedProperty of PropertyFloat</returns>
        SerializedProperty GetPropertyFloat(string _key)
        {
            int _i = 0;
            if (!_indexFloats.TryGetValue(_key, out _i)) return null;
            return _propertyFloats.GetArrayElementAtIndex(_i);
        }

        /// <summary>
        /// Renders a editor fields for PropertyColor in the inspector
        /// </summary>
        /// <param name="_key"></param>
        void RenderPropertyColor(string _key)
        {
            bool _GUIstate = GUI.enabled;
            Color _orgGUIColor = GUI.color;
            EditorGUILayout.BeginHorizontal();

            SerializedProperty _s = GetPropertyColor(_key);
            if (_s == null)
            {
                Debug.LogError("PropertyColor not found.");
                return;
            }

            if (_newSeed || _newPlanetBlueprint)
            {
                if ((_newSeed && _resetOverrides) || !_s.FindPropertyRelative("overrideRandom").boolValue)
                {
                    _script.SetPropertyColor(_key);
                    _s.FindPropertyRelative("overrideRandom").boolValue = false;
                }
                return;
            }

            if (_s.FindPropertyRelative("overrideRandom").boolValue)
            {
                GUI.color = _colorModified;
            }

            EditorGUI.BeginChangeCheck();

            _s.FindPropertyRelative("color").colorValue = EditorGUILayout.ColorField(_s.FindPropertyRelative("label").stringValue, _s.FindPropertyRelative("color").colorValue);

            if (EditorGUI.EndChangeCheck())
            {
                _s.FindPropertyRelative("overrideRandom").boolValue = true;
                _modifiedShader = true;
                _updateSerializedPlanetCacheRequired = true;
            }

            if (_s.FindPropertyRelative("overrideRandom").boolValue)
            {
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _target.ApplyModifiedProperties();
                    _script.SetPropertyColor(_key);
                    _target.Update();
                    _updateSerializedPlanetCacheRequired = true;
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(" ", GUILayout.Width(20));
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            GUI.color = _orgGUIColor;
            GUI.enabled = _GUIstate;
        }

        /// <summary>
        /// Gets the SerializedProperty of a PropertyColor based on the key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>SerializedProperty of PropertyColor</returns>
        SerializedProperty GetPropertyColor(string _key)
        {
            int _i = 0;
            if (!_indexColors.TryGetValue(_key, out _i)) return null;
            return _propertyColors.GetArrayElementAtIndex(_i);
        }

        /// <summary>
        /// Renders custom inspector for PropertyMaterial in inspector. 
        /// </summary>
        /// <param name="_key"></param>
        void RenderPropertyMaterial(string _key)
        {
            bool _GUIstate = GUI.enabled;
            Color _orgGUIColor = GUI.color;
            EditorGUILayout.BeginHorizontal();
            SerializedProperty _s = GetPropertyMaterial(_key);
            if (_s == null)
            {
                Debug.LogError("PropertyMaterial not found.");
                return;
            }

            if (_newSeed || _newPlanetBlueprint)
            {
                if ((_newSeed && _resetOverrides) || !_s.FindPropertyRelative("overrideRandom").boolValue)
                {
                    _script.SetPropertyMaterial(_key);
                    _s.FindPropertyRelative("overrideRandom").boolValue = false;
                }
                return;
            }

            if (_s.FindPropertyRelative("overrideRandom").boolValue)
            {
                GUI.color = _colorModified;
            }

            EditorGUI.BeginChangeCheck();

            _s.FindPropertyRelative("value").intValue = EditorGUILayout.Popup(_s.FindPropertyRelative("label").stringValue, _s.FindPropertyRelative("value").intValue, GetRelativeStringArray(_s, "popupDisplay"));

            if (EditorGUI.EndChangeCheck())
            {
                _script.OverridePropertyMaterial(_key, _s.FindPropertyRelative("value").intValue);
                _s.FindPropertyRelative("overrideRandom").boolValue = true;
                _updateSerializedPlanetCacheRequired = true;
                HandleModifiedTextures(_s);
            }

            if (_s.FindPropertyRelative("overrideRandom").boolValue)
            {
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _target.ApplyModifiedProperties();
                    _script.SetPropertyMaterial(_key);
                    _target.Update();
                    HandleModifiedTextures(_s);
                    _updateSerializedPlanetCacheRequired = true;
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(" ", GUILayout.Width(20));
                GUI.enabled = true;
            }

            EditorGUILayout.EndHorizontal();
            GUI.color = _orgGUIColor;
            GUI.enabled = _GUIstate;
        }

        /// <summary>
        /// Handles any modified textures of - if the Property has one or more proceduralTexture reference the procedural textures need to be rebuilt.
        /// </summary>
        /// <param name="_s"></param>
        void HandleModifiedTextures(SerializedProperty _s)
        {
            if (_s.FindPropertyRelative("proceduralTextures").arraySize > 0)
            {
                for (int _i = 0; _i < _s.FindPropertyRelative("proceduralTextures").arraySize; _i++)
                {
                    switch (_s.FindPropertyRelative("proceduralTextures").GetArrayElementAtIndex(_i).stringValue)
                    {
                        case "Maps":
                            _modifiedTextureMaps = true;
                            break;
                        case "StormMask":
                            _modifiedStormMask = true;
                            break;

                    }
                }
            }
        }

        /// <summary>
        /// Updates the dictionaries used since Unity do not allow serialization of dictionaries (translation to/from lists are needed)
        /// </summary>
        void UpdateIndex()
        {
            _indexFloats.Clear();
            for (int _i = 0; _i < _propertyFloats.arraySize; _i++)
            {
                SerializedProperty property = _propertyFloats.GetArrayElementAtIndex(_i);
                _indexFloats.Add(property.FindPropertyRelative("key").stringValue, _i);
            }

            _indexColors.Clear();
            for (int _i = 0; _i < _propertyColors.arraySize; _i++)
            {
                SerializedProperty property = _propertyColors.GetArrayElementAtIndex(_i);
                _indexColors.Add(property.FindPropertyRelative("key").stringValue, _i);
            }

            _indexMaterials.Clear();
            for (int _i = 0; _i < _propertyMaterials.arraySize; _i++)
            {
                SerializedProperty property = _propertyMaterials.GetArrayElementAtIndex(_i);
                _indexMaterials.Add(property.FindPropertyRelative("key").stringValue, _i);
            }
        }

        /// <summary>
        /// Gets the SerializedProperty of a PropertyMaterial based on the key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>SerializedProperty of PropertyMaterial</returns>
        SerializedProperty GetPropertyMaterial(string _key)
        {
            int _i = 0;
            if (!_indexMaterials.TryGetValue(_key, out _i)) return null;
            return _propertyMaterials.GetArrayElementAtIndex(_i);
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
                _string[_i] = _s.FindPropertyRelative(_relative).GetArrayElementAtIndex(_i).stringValue;
            return _string;
        }

        void BakePlanetAsset(Texture2D[] _tex2DArray, Ring _ring = null)
        {
            string _directory = "/Procedural Planets/BakedPlanets/GasPlanet_" + _planetSeed.intValue.ToString() + "_" + _variationSeed.intValue.ToString();

            int _i = 0;        
            while (System.IO.Directory.Exists(Application.dataPath + _directory + "_" + _i.ToString()))
            {
                _i++;
                // avoid hang
                if (_i > 999) break;
            }
            _directory = _directory + "_" + _i.ToString();

            if (!System.IO.Directory.Exists(Application.dataPath + _directory))
                System.IO.Directory.CreateDirectory(Application.dataPath + _directory);

            if (!System.IO.Directory.Exists(Application.dataPath + _directory + "/Assets"))
                System.IO.Directory.CreateDirectory(Application.dataPath + _directory + "/Assets");

            byte[] _bytes = _tex2DArray[0].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureMaps.png", _bytes);
            _bytes = _tex2DArray[1].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TexturePalette.png", _bytes);
            _bytes = _tex2DArray[2].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureBodyNormal.png", _bytes);
            _bytes = _tex2DArray[3].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureCapNormal.png", _bytes);
            _bytes = _tex2DArray[4].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureStormMask.png", _bytes);
            if (_ring != null)
            {

                Texture2D _texRing = _ring.GetBakedTexture();
                _bytes = _texRing.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureRing.png", _bytes);
                UnityEngine.Object.DestroyImmediate(_texRing);
            }                
            UnityEngine.Object.DestroyImmediate(_tex2DArray[0]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[1]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[2]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[3]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[4]);

            EditorCoroutine.start(ImportAsset(_directory, _ring));
        }


        IEnumerator ImportAsset(string _directory, Ring _ring)
        {
            _hasRing = false;
            if (_ring != null)
                _hasRing = true;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureMaps.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TexturePalette.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureBodyNormal.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureCapNormal.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureStormMask.png"))
                yield return null;            
            
            if (_hasRing)
            {
                while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureRing.png"))
                    yield return null;
            }

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureMaps.png", ImportAssetOptions.ForceUpdate);

            TextureImporter _ti;

            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureMaps.png");
                _ti.sRGBTexture = false;
                EditorUtility.SetDirty(_ti);
                _ti.SaveAndReimport();
            }

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TexturePalette.png", ImportAssetOptions.ForceUpdate);
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TexturePalette.png");
                _ti.sRGBTexture = false;
                EditorUtility.SetDirty(_ti);
                _ti.SaveAndReimport();
            }                
                    
            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureBodyNormal.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureBodyNormal.png");            
            _ti.textureType = TextureImporterType.NormalMap;
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();
                   
            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureCapNormal.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureCapNormal.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureType = TextureImporterType.NormalMap;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureStormMask.png", ImportAssetOptions.ForceUpdate);
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureStormMask.png");
                _ti.sRGBTexture = false;
                EditorUtility.SetDirty(_ti);
                _ti.SaveAndReimport();
            }

            if (_hasRing)
            {
                AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureRing.png", ImportAssetOptions.ForceUpdate);
                if (PlayerSettings.colorSpace == ColorSpace.Linear)
                {
                    _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureRing.png");
                    _ti.sRGBTexture = false;
                    EditorUtility.SetDirty(_ti);
                    _ti.SaveAndReimport();
                }
            }

            Texture2D _tex2DMaps = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureMaps.png");
            Texture2D _tex2DPalette = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TexturePalette.png");
            Texture2D _tex2DBodyNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureBodyNormal.png");            
            Texture2D _tex2DCapNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureCapNormal.png");
            Texture2D _tex2DStormMask = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureStormMask.png");

            Texture2D _tex2DRing = new Texture2D(2,2);
            if (_hasRing)
                _tex2DRing = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureRing.png");

            GameObject _goPlanet = new GameObject();
            GameObject _goRing;
            Material _planetMaterial;
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _planetMaterial = new Material(Shader.Find("ProceduralPlanets/GasPlanetLinear"));
            else
                _planetMaterial = new Material(Shader.Find("ProceduralPlanets/GasPlanetGamma"));

            if (_hasRing)
            {
                _goRing = new GameObject();
                _goRing.name = "Ring";
                _goRing.transform.SetParent(_goPlanet.transform);
                RingStatic _rs = _goRing.AddComponent<RingStatic>();                
                _rs.materials[0] = new Material(Shader.Find("ProceduralPlanets/Ring"));
                _rs.materials[1] = new Material(Shader.Find("ProceduralPlanets/Ring"));
                _rs.materials[0].SetTexture("_MainTex", _tex2DRing);
                _rs.materials[1].SetTexture("_MainTex", _tex2DRing);
                _rs.materials[0].renderQueue = 2800;
                _rs.materials[1].renderQueue = 3200;
                _rs.ringInnerRadius = _ring.GetPropertyFloatLerp("innerRadius") * 6.0f;
                _rs.ringOuterRadius = _ring.GetPropertyFloatLerp("outerRadius") * 6.0f;
                AssetDatabase.CreateAsset(_rs.materials[0], "Assets" + _directory + "/Assets/RingClose.mat");
                AssetDatabase.CreateAsset(_rs.materials[1], "Assets" + _directory + "/Assets/RingFar.mat");
                System.IO.StreamWriter _writerRing = new System.IO.StreamWriter("Assets" + _directory + "/Assets/jsonRingSettings.txt", false);
                _writerRing.WriteLine(_ring.ExportToJSON(Ring.StringFormat.JSON_EASY_READ));
                _writerRing.Close();
                AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/jsonRingSettings.txt", ImportAssetOptions.ForceUpdate);
            }

            _planetMaterial.SetTexture("_BodyTexture", _tex2DMaps);
            _planetMaterial.SetTexture("_CapTexture", _tex2DMaps);
            _planetMaterial.SetTexture("_PaletteLookup", _tex2DPalette);
            _planetMaterial.SetTexture("_BodyNormal", _tex2DBodyNormal);
            _planetMaterial.SetTexture("_CapNormal", _tex2DCapNormal);
            _planetMaterial.SetTexture("_StormMask", _tex2DStormMask);
            _target.Update();
            int _hTiling = (int)GetPropertyFloat("horizontalTiling").FindPropertyRelative("value").floatValue;
            int _vTiling = (int)GetPropertyFloat("verticalTiling").FindPropertyRelative("value").floatValue;
            float _stormIndex = (int)(GetPropertyFloat("stormMaskIndex").FindPropertyRelative("value").floatValue * (float)(_hTiling * _vTiling));
            _planetMaterial.SetInt("_HTiling", _hTiling);
            _planetMaterial.SetInt("_VTiling", _vTiling);            
            _planetMaterial.SetFloat("_Banding", Mathf.Lerp(GetPropertyFloat("banding").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("banding").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("banding").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_Solidness", Mathf.Lerp(GetPropertyFloat("solidness").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("solidness").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("solidness").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_Faintness", Mathf.Lerp(GetPropertyFloat("faintness").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("faintness").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("faintness").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_StormMaskIndex", (int) _stormIndex);
            _planetMaterial.SetFloat("_StormTint", Mathf.Lerp(GetPropertyFloat("stormTint").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("stormTint").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("stormTint").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_AtmosphereFalloff", Mathf.Lerp(GetPropertyFloat("atmosphereFalloff").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("atmosphereFalloff").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("atmosphereFalloff").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetColor("_ColorTwilight", GetPropertyColor("twilightColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetColor("_AtmosphereColor", GetPropertyColor("atmosphereColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetColor("_StormColor", GetPropertyColor("stormColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetColor("_FaintnessColor", GetPropertyColor("faintnessColor").FindPropertyRelative("color").colorValue);
            MeshRenderer _mr = _goPlanet.AddComponent<MeshRenderer>();
            _goPlanet.AddComponent<MeshFilter>();
            _mr.material = _planetMaterial;
            _goPlanet.AddComponent<GasPlanetStatic>();
            
            AssetDatabase.CreateAsset(_planetMaterial, "Assets" + _directory + "/Assets/GasPlanet.mat");
            //PrefabUtility.CreatePrefab("Assets" + _directory + "/GasPlanetPrefab.prefab", _goPlanet);
            PrefabUtility.SaveAsPrefabAsset(_goPlanet, "Assets" + _directory + "/GasPlanetPrefab.prefab");

            System.IO.StreamWriter _writer = new System.IO.StreamWriter("Assets" + _directory + "/Assets/jsonPlanetSettings.txt", false);
            _writer.WriteLine(_script.ExportToJSON(SimpleJSON.StringFormat.JSON_EASY_READ));
            _writer.Close();
            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/jsonPlanetSettings.txt", ImportAssetOptions.ForceUpdate);
            DestroyImmediate(_goPlanet);
            Debug.Log("Planet Prefab Created: " + _directory);
        }


    }

}
