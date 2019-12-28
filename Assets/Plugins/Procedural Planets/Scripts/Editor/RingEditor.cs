using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace ProceduralPlanets
{
    /// <summary>
    /// Custom inspector editor for Ring.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    /// 
    [CustomEditor(typeof(Ring))]
    [CanEditMultipleObjects]
    public class RingEditor : Editor
    {
        // Constants
        private const float MODIFY_DELAY = 0.5f;

        // Flags
        private bool _resetOverrides = false;
        private bool _updateSerializedRingCacheRequired = false;
        private bool _newSeed = false;
        private bool _modifiedRingTexture;
        private float _modifyTimestamp;
        private bool _modifyWait = false;

        // Color of modified settings in inspector
        private Color _colorModified = new Color(1.0f, 0.7f, 0.4f);

        //Reference to the target component (needed for some direct method calls)
        private Ring _script;

        // Serialized Object
        SerializedObject _target;

        // Serialized Properties
        SerializedProperty _serializedRingCache;
        SerializedProperty _seed;
        SerializedProperty _blueprintRing;
        SerializedProperty _rebuildRingNeeded;

        SerializedProperty _propertyFloats;        
        SerializedProperty _propertyMaterials;

        // Unity cannot serialize dictionaries so lists are translated to dictionaries in the editor script and vise versa
        Dictionary<string, int> _indexFloats = new Dictionary<string, int>();
        Dictionary<string, int> _indexMaterials = new Dictionary<string, int>();

        void OnEnable()
        {
            // Get the target object
            _target = new SerializedObject(target);

            // Ensure editor application run Update method (doesn't happen every frame, only when OnInspectorGUI() is called.
            EditorApplication.update += Update;

            // Find the properties of the target
            _serializedRingCache = _target.FindProperty("serializedRingCache");
            _seed = _target.FindProperty("seed");
            _blueprintRing = _target.FindProperty("blueprintRing");
            _propertyFloats = _target.FindProperty("propertyFloats");
            _propertyMaterials = _target.FindProperty("propertyMaterials");
            _rebuildRingNeeded = _target.FindProperty("rebuildRingNeeded");
        }

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

            _script = (Ring) target;

            EditorGUILayout.ObjectField(_blueprintRing);
            // RANDOM
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            _seed.intValue = EditorGUILayout.DelayedIntField("Ring Seed", _seed.intValue);
            if (GUILayout.Button("Random")) _seed.intValue = UnityEngine.Random.Range(0, int.MaxValue - 1000000);
            if (EditorGUI.EndChangeCheck())
            {
                _newSeed = true;
                _updateSerializedRingCacheRequired = true;
                _target.ApplyModifiedProperties();
                _target.Update();
                _modifiedRingTexture = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            RenderPropertyMaterial("ringMaterial");
            if (EditorGUI.EndChangeCheck())
            {
                _target.ApplyModifiedProperties();
                _target.Update();
                _updateSerializedRingCacheRequired = true;
                _modifiedRingTexture = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            RenderPropertyFloat("innerRadius");
            RenderPropertyFloat("outerRadius");
            if (EditorGUI.EndChangeCheck())
            {
                _target.ApplyModifiedProperties();
                _target.Update();
                //_script.RecreateMesh();
                _script.CreateMeshes();
                _updateSerializedRingCacheRequired = true;
            }

            // Appearance
            RenderPropertyFloat("gradientDiffuse");
            RenderPropertyFloat("gradientAlpha");
            RenderPropertyFloat("diffuseHue");
            RenderPropertyFloat("diffuseSaturation");
            RenderPropertyFloat("diffuseLightness");
            RenderPropertyFloat("alphaContrast");
            RenderPropertyFloat("alphaLuminosity");

            if (_modifiedRingTexture)
            {
                if (!_modifyWait)
                {
                    _modifyTimestamp = Time.realtimeSinceStartup;
                    _modifyWait = true;
                }
            }

            // Apply the modified properties        
            if (_newSeed)
            {
                _target.Update();
                //_script.RecreateMesh();
                _script.CreateMeshes();
                _newSeed = false;
                Repaint();
            }
            else
            {
                _target.ApplyModifiedProperties();
            }

            if (_updateSerializedRingCacheRequired)
            {
                _target.Update();
                _serializedRingCache.stringValue = _script.ExportToJSON(Ring.StringFormat.JSON_COMPACT);
                _target.ApplyModifiedProperties();
                _updateSerializedRingCacheRequired = false;
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

            _indexMaterials.Clear();
            for (int _i = 0; _i < _propertyMaterials.arraySize; _i++)
            {
                SerializedProperty property = _propertyMaterials.GetArrayElementAtIndex(_i);
                _indexMaterials.Add(property.FindPropertyRelative("key").stringValue, _i);
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

            if (_newSeed)
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
                HandleModifiedTextures(_s);
                _updateSerializedRingCacheRequired = true;
            }

            if (_s.FindPropertyRelative("overrideRandom").boolValue)
            {
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _target.ApplyModifiedProperties();
                    _script.SetPropertyFloat(_key);
                    _target.Update();
                    HandleModifiedTextures(_s);
                    _updateSerializedRingCacheRequired = true;
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
        /// Renders custom inspector for Propertymaterial in inspector. 
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

            if (_newSeed)
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
            {
                _string[_i] = _s.FindPropertyRelative(_relative).GetArrayElementAtIndex(_i).stringValue;
            }
            return _string;
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
                        case "Ring":
                            _modifiedRingTexture = true;
                            break;
                    }
                }
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

                if (_modifiedRingTexture)
                {
                    _rebuildRingNeeded.boolValue = true;
                    _modifiedRingTexture = false;
                }
                _modifyTimestamp = 0f;
                _modifyWait = false;
                _target.ApplyModifiedProperties();
            }
        }
    }
}
