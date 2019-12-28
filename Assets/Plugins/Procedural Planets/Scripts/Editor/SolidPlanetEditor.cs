
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


namespace ProceduralPlanets
{
    /// <summary>
    /// Custom inspector editor for Solid Planets.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    [CustomEditor(typeof(SolidPlanet))]
    [CanEditMultipleObjects]
    public class SolidPlanetEditor : Editor
    {
        // Constants
        const float LABEL_WIDTH = 145;
        const float VALUE_WIDTH = 90;
        const float MODIFY_DELAY = 0.5f;

        // Flags
        bool _resetOverrides;
        bool _newSeed;
        bool _newVariation;
        bool _newPlanetBlueprint;
        float _modifyTimestamp;
        bool _modifyWait;
        bool _modifiedTextureBiome1;
        bool _modifiedTextureBiome2;
        bool _modifiedTextureClouds;
        bool _modifiedTextureCities;
        bool _modifiedTextureMaps;
        bool _modifiedTextureLava;
        bool _modifiedTextureLookups;
        bool _modifiedShader;
        bool _updateSerializedPlanetCacheRequired;

        bool _hasRing;

        // Color of modified settings in inspector
        private Color _colorModified = new Color(1.0f, 0.7f, 0.4f);

        //Reference to the target component (needed for some direct method calls)
        private SolidPlanet _script;

        // Serialized Object
        SerializedObject _target;

        // Serialized Properties
        SerializedProperty _serializedPlanetCache;
        SerializedProperty _rebuildMapsNeeded;
        SerializedProperty _rebuildBiome1Needed;
        SerializedProperty _rebuildBiome2Needed;
        SerializedProperty _rebuildCitiesNeeded;
        SerializedProperty _rebuildCloudsNeeded;
        SerializedProperty _rebuildLavaNeeded;
        SerializedProperty _rebuildLookupsNeeded;
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

        private bool _showPlanetBaking;

        void OnEnable()
        {
            // Get the target object
            _target = new SerializedObject(target);

            // Ensure editor application run Update method (doesn't happen every frame, only when OnInspectorGUI() is called.
            EditorApplication.update += Update;

            // Find the properties of the target
            _serializedPlanetCache = _target.FindProperty("serializedPlanetCache");
            _rebuildMapsNeeded = _target.FindProperty("_rebuildMapsNeeded");
            _rebuildBiome1Needed = _target.FindProperty("_rebuildBiome1Needed");
            _rebuildBiome2Needed = _target.FindProperty("_rebuildBiome2Needed");
            _rebuildCitiesNeeded = _target.FindProperty("_rebuildCitiesNeeded");
            _rebuildCloudsNeeded = _target.FindProperty("_rebuildCloudsNeeded");
            _rebuildLavaNeeded = _target.FindProperty("_rebuildLavaNeeded");
            _rebuildLookupsNeeded = _target.FindProperty("_rebuildLookupsNeeded");
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
        /// Displays and allows interaction in a custom inspector for Solid Planets
        /// </summary>
        public override void OnInspectorGUI()
        {
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

            _script = (SolidPlanet)target;

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
                _modifiedTextureBiome1 = true;
                _modifiedTextureBiome2 = true;
                _modifiedTextureCities = true;
                _modifiedTextureClouds = true;
                _modifiedTextureLava = true;
                _modifiedTextureLookups = true;
                _modifiedTextureMaps = true;
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
                _modifiedTextureBiome1 = true;
                _modifiedTextureBiome2 = true;
                _modifiedTextureCities = true;
                _modifiedTextureClouds = true;
                _modifiedTextureLava = true;
                _modifiedTextureLookups = true;
                _modifiedTextureMaps = true;
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
                _modifiedTextureBiome1 = true;
                _modifiedTextureBiome2 = true;
                _modifiedTextureCities = true;
                _modifiedTextureClouds = true;
                _modifiedTextureLava = true;
                _modifiedTextureLookups = true;
                _modifiedTextureMaps = true;
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
            _showPlanetBaking = EditorGUILayout.Foldout(_showPlanetBaking, new GUIContent("Bake Planet To Prefab"));
            if (_showPlanetBaking)
            {
                if (GUILayout.Button(new GUIContent("Bake Static Planet Prefab", "Create a prefab with static textures")))
                {

                    Texture2D[] _tex2DArray = _script.GetBakedTextures();
                    BakePlanetAsset(_tex2DArray, _script.GetRing());
                }
            }


            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // PLANET BLUEPRINT
            EditorGUILayout.LabelField("PLANET BLUEPRINT", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            PlanetManager.Instance.RefreshLists();
            List<string> _l = new List<string>();
            foreach (BlueprintSolidPlanet _cs in PlanetManager.Instance.listSolidPlanetBlueprints)
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
                    _modifiedTextureBiome1 = true;
                    _modifiedTextureBiome2 = true;
                    _modifiedTextureCities = true;
                    _modifiedTextureClouds = true;
                    _modifiedTextureLava = true;
                    _modifiedTextureLookups = true;
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
            RenderPropertyFloat("alienization");
            RenderPropertyColor("specularColor");
            EditorGUI.indentLevel--;

            // CONTINENTS
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("CONTINENTS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyFloat("continentSeed");
            RenderPropertyFloat("continentSize");
            RenderPropertyFloat("continentComplexity");
            EditorGUI.indentLevel--;

            // COASTS
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("COAST LINES", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyFloat("coastalDetail");
            RenderPropertyFloat("coastalReach");
            EditorGUI.indentLevel--;

            // LIQUID 
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("LIQUID", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyFloat("liquidLevel");
            if (GetPropertyFloat("liquidLevel").FindPropertyRelative("value").floatValue < 0.001f)
                GUI.enabled = false;
            RenderPropertyColor("liquidColor");
            RenderPropertyFloat("liquidOpacity");
            RenderPropertyFloat("liquidShallow");
            RenderPropertyFloat("liquidSpecularPower");
            GUI.enabled = true;
            EditorGUI.indentLevel--;

            // ICE
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ICE", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyMaterial("polarIce");
            RenderPropertyFloat("polarCapAmount");
            RenderPropertyColor("iceColor");
            EditorGUI.indentLevel--;

            // ATMOSPHERE
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ATMOSPHERE", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyColor("atmosphereColor");
            RenderPropertyFloat("atmosphereExternalSize");
            RenderPropertyFloat("atmosphereExternalDensity");
            RenderPropertyFloat("atmosphereInternalDensity");
            RenderPropertyColor("twilightColor");
            EditorGUI.indentLevel--;

            // CLOUDS
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("CLOUDS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyFloat("cloudsOpacity");
            if (GetPropertyFloat("cloudsOpacity").FindPropertyRelative("value").floatValue < 0.0001f)
                GUI.enabled = false;
            RenderPropertyMaterial("clouds");
            RenderPropertyFloat("cloudsSeed");
            RenderPropertyColor("cloudsColor");
            RenderPropertyFloat("cloudsRoughness");
            RenderPropertyFloat("cloudsCoverage");
            RenderPropertyFloat("cloudsLayer1");
            RenderPropertyFloat("cloudsLayer2");
            RenderPropertyFloat("cloudsLayer3");
            RenderPropertyFloat("cloudsSharpness");
            RenderPropertyFloat("cloudsTiling");
            RenderPropertyFloat("cloudsSpeed");
            RenderPropertyFloat("cloudsHeight");
            RenderPropertyFloat("cloudsShadow");
            GUI.enabled = true;
            EditorGUI.indentLevel--;

            // LAVA
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("MOLTEN LAVA", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyFloat("lavaAmount");
            if (GetPropertyFloat("lavaAmount").FindPropertyRelative("value").floatValue < 0.00001f)
                GUI.enabled = false;
            RenderPropertyMaterial("lava");
            RenderPropertyFloat("lavaComplexity");
            RenderPropertyFloat("lavaFrequency");
            RenderPropertyFloat("lavaDetail");
            RenderPropertyFloat("lavaReach");
            RenderPropertyFloat("lavaColorVariation");
            RenderPropertyFloat("lavaFlowSpeed");
            RenderPropertyFloat("lavaGlowAmount");
            RenderPropertyColor("lavaGlowColor");
            GUI.enabled = true;
            EditorGUI.indentLevel--;

            // SURFACE
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("SURFACES", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyFloat("surfaceRoughness");
            RenderPropertyFloat("surfaceTiling");
            EditorGUI.indentLevel--;

            // COMPOSITION
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("COMPOSITION", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyMaterial("composition");
            RenderPropertyFloat("compositionSeed");
            RenderPropertyFloat("compositionTiling");
            RenderPropertyFloat("compositionChaos");
            RenderPropertyFloat("compositionBalance");
            RenderPropertyFloat("compositionContrast");
            EditorGUI.indentLevel--;

            // BIOME 1
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("BIOME 1", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyFloat("biome1Seed");
            RenderPropertyMaterial("biome1Type");
            RenderPropertyFloat("biome1Chaos");
            RenderPropertyFloat("biome1Balance");
            RenderPropertyFloat("biome1Contrast");
            RenderPropertyFloat("biome1ColorVariation");
            RenderPropertyFloat("biome1Saturation");
            RenderPropertyFloat("biome1Brightness");
            RenderPropertyFloat("biome1CratersSmall");
            RenderPropertyFloat("biome1CratersMedium");
            RenderPropertyFloat("biome1CratersLarge");
            RenderPropertyFloat("biome1CratersErosion");
            RenderPropertyFloat("biome1CratersDiffuse");
            //RenderPropertyFloat("biome1CanyonsDiffuse");
            RenderPropertyFloat("biome1SurfaceBump");
            RenderPropertyFloat("biome1CratersBump");
            RenderPropertyFloat("biome1CanyonsBump");
            EditorGUI.indentLevel--;

            // BIOME 2
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("BIOME 2", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyFloat("biome2Seed");
            RenderPropertyMaterial("biome2Type");
            RenderPropertyFloat("biome2Chaos");
            RenderPropertyFloat("biome2Balance");
            RenderPropertyFloat("biome2Contrast");
            RenderPropertyFloat("biome2ColorVariation");
            RenderPropertyFloat("biome2Saturation");
            RenderPropertyFloat("biome2Brightness");
            RenderPropertyFloat("biome2CratersSmall");
            RenderPropertyFloat("biome2CratersMedium");
            RenderPropertyFloat("biome2CratersLarge");
            RenderPropertyFloat("biome2CratersErosion");
            RenderPropertyFloat("biome2CratersDiffuse");
            //RenderPropertyFloat("biome2CanyonsDiffuse");
            RenderPropertyFloat("biome2SurfaceBump");
            RenderPropertyFloat("biome2CratersBump");
            RenderPropertyFloat("biome2CanyonsBump");
            EditorGUI.indentLevel--;

            // CITIES
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("CITIES & POPULATION", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RenderPropertyMaterial("cities");
            RenderPropertyFloat("citiesSeed");
            RenderPropertyFloat("citiesPopulation");
            RenderPropertyFloat("citiesAdvancement");
            RenderPropertyFloat("citiesGlow");
            RenderPropertyFloat("citiesTiling");
            RenderPropertyColor("citiesColor");
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (_modifiedShader)
            {
                _updateShaderNeeded.boolValue = true;
                _modifiedShader = false;
            }

            if (_modifiedTextureBiome1 || _modifiedTextureBiome2 || _modifiedTextureCities || _modifiedTextureClouds || _modifiedTextureLava || _modifiedTextureMaps)
            {
                if (!_modifyWait)
                {
                    _modifyTimestamp = Time.realtimeSinceStartup;
                    _modifyWait = true;
                }
            }

            if (_modifiedTextureLookups)
            {
                _rebuildLookupsNeeded.boolValue = true;
                _modifiedTextureLookups = false;
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
                if (_modifiedTextureCities)
                {
                    _rebuildCitiesNeeded.boolValue = true;
                    _modifiedTextureCities = false;
                }
                if (_modifiedTextureClouds)
                {
                    _rebuildCloudsNeeded.boolValue = true;
                    _modifiedTextureClouds = false;
                }
                if (_modifiedTextureLava)
                {
                    _rebuildLavaNeeded.boolValue = true;
                    _modifiedTextureLava = false;
                }
                if (_modifiedTextureBiome1)
                {
                    _rebuildBiome1Needed.boolValue = true;
                    _modifiedTextureBiome1 = false;
                }
                if (_modifiedTextureBiome2)
                {
                    _rebuildBiome2Needed.boolValue = true;
                    _modifiedTextureBiome2 = false;
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
                    _s.FindPropertyRelative("value").floatValue = EditorGUILayout.Slider(_s.FindPropertyRelative("label").stringValue, (int)_s.FindPropertyRelative("value").floatValue, (int)_s.FindPropertyRelative("minValue").floatValue, (int)_s.FindPropertyRelative("maxValue").floatValue);
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
                        case "Lava":
                            _modifiedTextureLava = true;
                            break;
                        case "Clouds":
                            _modifiedTextureClouds = true;
                            break;
                        case "Cities":
                            _modifiedTextureCities = true;
                            break;
                        case "Biome1":
                            _modifiedTextureBiome1 = true;
                            break;
                        case "Biome2":
                            _modifiedTextureBiome2 = true;
                            break;
                        case "Lookups":
                            _modifiedTextureLookups = true;
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
            string _directory = "/Procedural Planets/BakedPlanets/SolidPlanet_" + _planetSeed.intValue.ToString() + "_" + _variationSeed.intValue.ToString();

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
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureBiome1DiffSpec.png", _bytes);
            _bytes = _tex2DArray[2].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureBiome1Normal.png", _bytes);
            _bytes = _tex2DArray[3].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureBiome2DiffSpec.png", _bytes);
            _bytes = _tex2DArray[4].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureBiome2Normal.png", _bytes);
            _bytes = _tex2DArray[5].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureIceDiffuse.png", _bytes);
            _bytes = _tex2DArray[6].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureCities.png", _bytes);
            _bytes = _tex2DArray[7].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureClouds.png", _bytes);
            _bytes = _tex2DArray[8].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureLavaDiffuse.png", _bytes);
            _bytes = _tex2DArray[9].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureLavaFlow.png", _bytes);
            _bytes = _tex2DArray[10].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureLookupLiquid.png", _bytes);
            _bytes = _tex2DArray[11].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureLookupLava.png", _bytes);
            _bytes = _tex2DArray[12].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureLookupLavaGlow.png", _bytes);
            _bytes = _tex2DArray[13].EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + _directory + "/Assets/TextureLookupPolar.png", _bytes);
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
            UnityEngine.Object.DestroyImmediate(_tex2DArray[5]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[6]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[7]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[8]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[9]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[10]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[11]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[12]);
            UnityEngine.Object.DestroyImmediate(_tex2DArray[13]);

            EditorCoroutine.start(ImportAsset(_directory, _ring));
        }


        IEnumerator ImportAsset(string _directory, Ring _ring)
        {
            _hasRing = false;
            if (_ring != null)
                _hasRing = true;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureMaps.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureBiome1DiffSpec.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureBiome1Normal.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureBiome2DiffSpec.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureBiome2Normal.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureCities.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureClouds.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureIceDiffuse.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureLavaDiffuse.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureLavaFlow.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureLookupLiquid.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureLookupLava.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureLookupLavaGlow.png"))
                yield return null;

            while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureLookupPolar.png"))
                yield return null;

            if (_hasRing)
            {
                while (!System.IO.File.Exists(Application.dataPath + _directory + "/Assets/TextureRing.png"))
                    yield return null;
            }

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureMaps.png", ImportAssetOptions.ForceUpdate);

            TextureImporter _ti;

            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureMaps.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();
            
            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureBiome1DiffSpec.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureBiome1DiffSpec.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureBiome1Normal.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureBiome1Normal.png");
            _ti.textureType = TextureImporterType.NormalMap;
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureBiome2DiffSpec.png", ImportAssetOptions.ForceUpdate);            
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureBiome2DiffSpec.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureBiome2Normal.png", ImportAssetOptions.ForceUpdate);        
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureBiome2Normal.png");
            _ti.textureType = TextureImporterType.NormalMap;
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureIceDiffuse.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureIceDiffuse.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureCities.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureCities.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureClouds.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureClouds.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureLavaDiffuse.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureLavaDiffuse.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureLavaFlow.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureLavaFlow.png");
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _ti.sRGBTexture = false;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureLookupLiquid.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureLookupLiquid.png");
            _ti.wrapMode = TextureWrapMode.Clamp;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureLookupLava.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureLookuplava.png");
            _ti.wrapMode = TextureWrapMode.Clamp;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureLookupLavaGlow.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureLookupLavaGlow.png");
            _ti.wrapMode = TextureWrapMode.Clamp;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureLookupPolar.png", ImportAssetOptions.ForceUpdate);
            _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureLookupPolar.png");
            _ti.wrapMode = TextureWrapMode.Clamp;
            _ti.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(_ti);
            _ti.SaveAndReimport();

            if (_hasRing)
            {
                AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/TextureRing.png", ImportAssetOptions.ForceUpdate);
                _ti = (TextureImporter)AssetImporter.GetAtPath("Assets" + _directory + "/Assets/TextureRing.png");
                if (PlayerSettings.colorSpace == ColorSpace.Linear)
                    _ti.sRGBTexture = false;
                _ti.textureCompression = TextureImporterCompression.Uncompressed;
                EditorUtility.SetDirty(_ti);
                _ti.SaveAndReimport();
            }



            Texture2D _tex2DMaps = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureMaps.png");
            Texture2D _tex2DBiome1DiffSpec= AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureBiome1DiffSpec.png");
            Texture2D _tex2DBiome1Normal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureBiome1Normal.png");
            Texture2D _tex2DBiome2DiffSpec = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureBiome2DiffSpec.png");
            Texture2D _tex2DBiome2Normal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureBiome2Normal.png");
            Texture2D _tex2DIceDiffuse  = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureIceDiffuse.png");
            Texture2D _tex2DCities = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureCities.png");
            Texture2D _tex2DClouds = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureClouds.png");
            Texture2D _tex2DLavaDiffuse = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureLavaDiffuse.png");
            Texture2D _tex2DLavaFlow = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureLavaFlow.png");
            Texture2D _tex2DLookupLiquid = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureLookupLiquid.png");
            Texture2D _tex2DLookupLava = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureLookupLava.png");
            Texture2D _tex2DLookupLavaGlow = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureLookupLavaGlow.png");
            Texture2D _tex2DLookupPolar = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureLookupPolar.png");

            Texture2D _tex2DRing = new Texture2D(2, 2);
            if (_hasRing)
                _tex2DRing = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + _directory + "/Assets/TextureRing.png");

            GameObject _goPlanet = new GameObject();            
            GameObject _goRing;
            Material _planetMaterial;
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                _planetMaterial = new Material(Shader.Find("ProceduralPlanets/SolidPlanetLinear"));
            else
                _planetMaterial = new Material(Shader.Find("ProceduralPlanets/SolidPlanetGamma"));

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

            GameObject _goAtmosphere = new GameObject();
            _goAtmosphere.name = "ExternalAtmosphere";
            _goAtmosphere.transform.SetParent(_goPlanet.transform);
            _goAtmosphere.AddComponent<MeshFilter>();
            MeshRenderer _mra = _goAtmosphere.AddComponent<MeshRenderer>();
            Material _atmosphereMaterial = new Material(Shader.Find("ProceduralPlanets/Atmosphere"));
            _mra.sharedMaterial = _atmosphereMaterial;

            _planetMaterial.SetTexture("_TexMaps", _tex2DMaps);
            _planetMaterial.SetTexture("_TexBiome1DiffSpec", _tex2DBiome1DiffSpec);
            _planetMaterial.SetTexture("_TexBiome1Normal", _tex2DBiome1Normal);
            _planetMaterial.SetTexture("_TexBiome2DiffSpec", _tex2DBiome2DiffSpec);
            _planetMaterial.SetTexture("_TexBiome2Normal", _tex2DBiome2Normal);
            _planetMaterial.SetTexture("_TexIceDiffuse", _tex2DIceDiffuse);
            _planetMaterial.SetTexture("_TexCities", _tex2DCities);
            _planetMaterial.SetTexture("_TexClouds", _tex2DClouds);
            _planetMaterial.SetTexture("_TexLavaDiffuse", _tex2DLavaDiffuse);
            _planetMaterial.SetTexture("_TexLavaFlow", _tex2DLavaFlow);
            _planetMaterial.SetTexture("_TexLookupLiquid", _tex2DLookupLiquid);
            _planetMaterial.SetTexture("_TexLookupLava", _tex2DLookupLava);
            _planetMaterial.SetTexture("_TexLookupLavaGlow", _tex2DLookupLavaGlow);
            _planetMaterial.SetTexture("_TexLookupPolar", _tex2DLookupPolar);

            _target.Update();

            _planetMaterial.SetColor("_ColorSpecular", GetPropertyColor("specularColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetInt("_TilingHeightBase", (int) (Mathf.Lerp(GetPropertyFloat("continentSize").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("continentSize").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("continentSize").FindPropertyRelative("value").floatValue)));
            _planetMaterial.SetInt("_TilingHeightDetail", (int) (Mathf.Lerp(GetPropertyFloat("coastalDetail").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("coastalDetail").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("coastalDetail").FindPropertyRelative("value").floatValue)));
            _planetMaterial.SetFloat("_DetailHeight", Mathf.Lerp(GetPropertyFloat("coastalReach").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("coastalReach").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("coastalReach").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetColor("_ColorLiquid", GetPropertyColor("liquidColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetFloat("_LiquidOpacity", Mathf.Lerp(GetPropertyFloat("liquidOpacity").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("liquidOpacity").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("liquidOpacity").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_SpecularPowerLiquid", Mathf.Lerp(GetPropertyFloat("liquidSpecularPower").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("liquidSpecularPower").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("liquidSpecularPower").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetColor("_ColorIce", GetPropertyColor("iceColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetColor("_ColorAtmosphere", GetPropertyColor("atmosphereColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetFloat("_AtmosphereFalloff", Mathf.Lerp(GetPropertyFloat("atmosphereInternalDensity").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("atmosphereInternalDensity").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("atmosphereInternalDensity").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetColor("_ColorTwilight", GetPropertyColor("twilightColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetFloat("_CloudOpacity", Mathf.Lerp(GetPropertyFloat("cloudsOpacity").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("cloudsOpacity").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("cloudsOpacity").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetColor("_ColorClouds", GetPropertyColor("cloudsColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetInt("_TilingClouds", (int) (GetPropertyFloat("cloudsTiling").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_CloudSpeed", Mathf.Lerp(GetPropertyFloat("cloudsSpeed").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("cloudsOpacity").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("cloudsOpacity").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_CloudHeight", Mathf.Lerp(GetPropertyFloat("cloudsHeight").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("cloudsHeight").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("cloudsHeight").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_CloudShadow", Mathf.Lerp(GetPropertyFloat("cloudsShadow").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("cloudsShadow").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("cloudsShadow").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetInt("_TilingLavaBase", (int) (GetPropertyFloat("lavaFrequency").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetInt("_TilingLavaDetail", (int) (GetPropertyFloat("lavaDetail").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_DetailLava", Mathf.Lerp(GetPropertyFloat("lavaReach").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("lavaReach").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("lavaReach").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_LavaFlowSpeed", Mathf.Lerp(GetPropertyFloat("lavaFlowSpeed").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("lavaFlowSpeed").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("lavaFlowSpeed").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetColor("_ColorLavaGlow", GetPropertyColor("lavaGlowColor").FindPropertyRelative("color").colorValue);
            _planetMaterial.SetInt("_TilingSurface", (int) (GetPropertyFloat("surfaceTiling").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetFloat("_SurfaceRoughness", Mathf.Lerp(GetPropertyFloat("surfaceRoughness").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("surfaceRoughness").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("surfaceRoughness").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetInt("_TilingBiome", (int) (GetPropertyFloat("compositionTiling").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetInt("_TilingCities", (int) (GetPropertyFloat("citiesTiling").FindPropertyRelative("value").floatValue));
            _planetMaterial.SetColor("_ColorCities", GetPropertyColor("citiesColor").FindPropertyRelative("color").colorValue);

            _atmosphereMaterial.SetColor("_ColorAtmosphere", GetPropertyColor("atmosphereColor").FindPropertyRelative("color").colorValue);
            _atmosphereMaterial.SetFloat("_Size", Mathf.Lerp(GetPropertyFloat("atmosphereExternalSize").FindPropertyRelative("minValue").floatValue, GetPropertyFloat("atmosphereExternalSize").FindPropertyRelative("maxValue").floatValue, GetPropertyFloat("atmosphereExternalSize").FindPropertyRelative("value").floatValue));
          
            float _aedMin = GetPropertyFloat("atmosphereExternalDensity").FindPropertyRelative("minValue").floatValue;
            float _aedMax = GetPropertyFloat("atmosphereExternalDensity").FindPropertyRelative("maxValue").floatValue;
            float _aedVal = GetPropertyFloat("atmosphereExternalDensity").FindPropertyRelative("value").floatValue;
            float _aesVal = GetPropertyFloat("atmosphereExternalSize").FindPropertyRelative("value").floatValue;
            _atmosphereMaterial.SetFloat("_Falloff", Mathf.Lerp(_aedMin * (1 + _aesVal), _aedMax * (1 + _aesVal), _aedVal));

            MeshRenderer _mr = _goPlanet.AddComponent<MeshRenderer>();
            _goPlanet.AddComponent<MeshFilter>();
            _mr.material = _planetMaterial;
            _goPlanet.AddComponent<SolidPlanetStatic>();

            AssetDatabase.CreateAsset(_planetMaterial, "Assets" + _directory + "/Assets/SolidPlanet.mat");
            AssetDatabase.CreateAsset(_atmosphereMaterial, "Assets" + _directory + "/Assets/Atmosphere.mat");            
            PrefabUtility.SaveAsPrefabAsset(_goPlanet, "Assets" + _directory + "/SolidPlanetPrefab.prefab");

            System.IO.StreamWriter _writer = new System.IO.StreamWriter("Assets" + _directory + "/Assets/jsonPlanetSettings.txt", false);
            _writer.WriteLine(_script.ExportToJSON(SimpleJSON.StringFormat.JSON_EASY_READ));
            _writer.Close();
            AssetDatabase.ImportAsset("Assets" + _directory + "/Assets/jsonPlanetSettings.txt", ImportAssetOptions.ForceUpdate);
            DestroyImmediate(_goPlanet);
            Debug.Log("Planet Prefab Created: " + _directory);
        }

    }

}
