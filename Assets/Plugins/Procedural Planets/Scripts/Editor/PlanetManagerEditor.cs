using UnityEngine;
using UnityEditor;

namespace ProceduralPlanets
{
    /// <summary>
    /// Custom inspector editor for PlanetManager.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    
    [CustomEditor(typeof(PlanetManager))]
    public class PlanetManagerEditor : Editor
    {
        // Constants
        const float WIDTH_BLUEPRINT_TYPE = 35f;
        const float WIDTH_BLUEPRINT_NAME = 110f;
        const float WIDTH_BUTTON = 20f;
        const float HEIGHT_BUTTON = 14f;
        const float WIDTH_BLUEPRINT_PERCENT = 50f;

//        [System.NonSerialized]
        // Private Variables        
        private readonly string[] lodTriangles = { "8", "32", "128", "512", "2048", "8192", "32768" };
        private readonly string[] lodResolution = { "16 x 16", "32 x 32", "64 x 64", "128 x 128", "256 x 256", "512 x 512", "1024 x 1024", "2048 x 2048" };

        // Reference to target object
        private PlanetManager _script;

        private bool _showHelp;
        bool _showMeshLOD;
        bool _showTextureStatic;
        bool _showTextureStaticSeparate;
        bool _showTextureProgressive;
        bool _showTextureProgressiveSeparate;
        bool _showTextureLOD;
        bool _showTextureLODSeparate;

        int _meshSteps;
        int _progressiveSteps;
        int _textureSteps;

        float _test;
        Texture2D _texSolid;

        // Serialized Object
        SerializedObject _target;

        // Serialized Properties    
        SerializedProperty _debug_level;
        SerializedProperty _masterSeed;
        SerializedProperty _cameraLOD;
        SerializedProperty _listSolidPlanetBlueprints;
        SerializedProperty _listGasPlanetBlueprints;
        SerializedProperty _solidCompositionMaterials;
        SerializedProperty _solidBiomeMaterials;
        SerializedProperty _solidCloudsMaterials;
        SerializedProperty _solidCitiesMaterials;
        SerializedProperty _solidLavaMaterials;
        SerializedProperty _solidPolarIceMaterials;
        SerializedProperty _ringMaterials;
        SerializedProperty _gasMaterials;
        SerializedProperty _substanceDuplicates;

        SerializedProperty _meshDetailMode;
        SerializedProperty _meshStaticSubdivisions;
        SerializedProperty _meshLODSteps;
        SerializedProperty _meshLODSubdivisions;
        SerializedProperty _meshLODPlanetSizes;

        SerializedProperty _textureDetailMode;

        SerializedProperty _textureStaticCommon;
        SerializedProperty _textureStaticComposition;
        SerializedProperty _textureStaticBiome;
        SerializedProperty _textureStaticClouds;
        SerializedProperty _textureStaticCities;
        SerializedProperty _textureStaticLava;
        SerializedProperty _textureStaticPolarIce;
        SerializedProperty _textureStaticGas;

        SerializedProperty _textureProgressiveSteps;
        SerializedProperty _textureProgressiveCommon;
        SerializedProperty _textureProgressiveComposition;
        SerializedProperty _textureProgressiveBiome;
        SerializedProperty _textureProgressiveClouds;
        SerializedProperty _textureProgressiveCities;
        SerializedProperty _textureProgressiveLava;
        SerializedProperty _textureProgressivePolarIce;
        SerializedProperty _textureProgressiveGas;

        SerializedProperty _textureLODSteps;
        SerializedProperty _textureLODCommon;
        SerializedProperty _textureLODComposition;
        SerializedProperty _textureLODClouds;
        SerializedProperty _textureLODBiome;
        SerializedProperty _textureLODCities;
        SerializedProperty _textureLODLava;
        SerializedProperty _textureLODPolarIce;
        SerializedProperty _textureLODGas;
        SerializedProperty _textureLODPlanetSizes;

        SerializedProperty _showEditorMeshLOD;
        SerializedProperty _showEditorTextureLOD;

        GUIStyle _header;

        public class LodSliderState
        {
            public int Index { get; set; }            
        }

        void OnEnable()
        {            
            _header = new GUIStyle();
            _header.normal.textColor = Color.white;
            _header.fontStyle = FontStyle.Bold;
            _header.fontSize = 12;
            _header.fixedHeight = 20;

            // Get the target object
            _target = new SerializedObject(target);

            // Get a reference to the target script component
            _script = (PlanetManager)target;

            // Create generic ring blueprint if necessary (if it doesn't already exist)
            _script.CreateGenericBlueprint();

            // Find the properties of the target
            _debug_level = _target.FindProperty("_debugLevel");
            _listSolidPlanetBlueprints = _target.FindProperty("listSolidPlanetBlueprints");
            _listGasPlanetBlueprints = _target.FindProperty("listGasPlanetBlueprints");
            _solidCompositionMaterials = _target.FindProperty("solidCompositionMaterials");
            _solidBiomeMaterials = _target.FindProperty("solidBiomeMaterials");
            _solidCloudsMaterials = _target.FindProperty("solidCloudsMaterials");
            _solidCitiesMaterials = _target.FindProperty("solidCitiesMaterials");
            _solidLavaMaterials = _target.FindProperty("solidLavaMaterials");
            _solidPolarIceMaterials = _target.FindProperty("solidPolarIceMaterials");
            _gasMaterials = _target.FindProperty("gasMaterials");
            _ringMaterials = _target.FindProperty("ringMaterials");
            _substanceDuplicates = _target.FindProperty("substanceDuplicates");

            _cameraLOD = _target.FindProperty("_cameraLOD");
            _meshDetailMode = _target.FindProperty("_meshDetailMode");
            _meshLODSteps = _target.FindProperty("_meshLODSteps");
            _meshStaticSubdivisions = _target.FindProperty("_meshStaticSubdivisions");            
            _meshLODSubdivisions = _target.FindProperty("_meshLODSubdivisions");
            _meshLODPlanetSizes = _target.FindProperty("_meshLODPlanetSizes");

            _textureDetailMode = _target.FindProperty("_textureDetailMode");

            _textureStaticCommon = _target.FindProperty("_textureStaticCommon");
            _textureStaticComposition = _target.FindProperty("_textureStaticComposition");
            _textureStaticClouds = _target.FindProperty("_textureStaticClouds");
            _textureStaticBiome = _target.FindProperty("_textureStaticBiome");
            _textureStaticCities = _target.FindProperty("_textureStaticCities");
            _textureStaticLava = _target.FindProperty("_textureStaticLava");
            _textureStaticPolarIce = _target.FindProperty("_textureStaticPolarIce");
            _textureStaticGas = _target.FindProperty("_textureStaticGas");

            _textureProgressiveSteps = _target.FindProperty("_textureProgressiveSteps");
            _textureProgressiveCommon = _target.FindProperty("_textureProgressiveCommon");
            _textureProgressiveComposition = _target.FindProperty("_textureProgressiveComposition");
            _textureProgressiveClouds = _target.FindProperty("_textureProgressiveClouds");
            _textureProgressiveBiome = _target.FindProperty("_textureProgressiveBiome");
            _textureProgressiveCities = _target.FindProperty("_textureProgressiveCities");
            _textureProgressiveLava = _target.FindProperty("_textureProgressiveLava");
            _textureProgressivePolarIce = _target.FindProperty("_textureProgressivePolarIce");
            _textureProgressiveGas = _target.FindProperty("_textureProgressiveGas");

            _textureLODSteps = _target.FindProperty("_textureLODSteps");
            _textureLODCommon = _target.FindProperty("_textureLODCommon");
            _textureLODComposition = _target.FindProperty("_textureLODComposition");
            _textureLODClouds = _target.FindProperty("_textureLODClouds");
            _textureLODBiome = _target.FindProperty("_textureLODBiome");
            _textureLODCities = _target.FindProperty("_textureLODCities");
            _textureLODLava = _target.FindProperty("_textureLODLava");
            _textureLODPolarIce = _target.FindProperty("_textureLODPolarIce");
            _textureLODGas = _target.FindProperty("_textureLODGas");
            _textureLODPlanetSizes = _target.FindProperty("_textureLODPlanetSizes");

            _showEditorMeshLOD = _target.FindProperty("showEditorMeshLOD");
            _showEditorTextureLOD = _target.FindProperty("showEditorTextureLOD");

            _texSolid = new Texture2D(2, 2);
            Color[] _cols = new Color[4] { Color.white, Color.white, Color.white, Color.white };
            _texSolid.SetPixels(_cols);
            _texSolid.Apply();
        }

        public static float[] LodSlider(Rect _controlRect, float[] _values, GUIStyle _style)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            var state = (LodSliderState)GUIUtility.GetStateObject(typeof(LodSliderState), controlID);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.Repaint:
                    {
                        
                        int _x = _style.margin.left + _style.padding.left;
                        for (int _i = 0; _i <= _values.Length; _i++)
                        {
                            int _pos = 0;
                            if (_i < _values.Length)
                                _pos = (int)Mathf.Lerp(0f, _controlRect.width, 1.0f - _values[_i]) + _style.margin.left + _style.padding.left;
                            else
                                _pos = (int) _controlRect.width + _style.margin.left + _style.padding.left;

                            Rect _outerRect = new Rect(_x, _controlRect.y, _pos - _x, _controlRect.height);
                            GUI.color = Color.Lerp(new Color(0f, 0.9f, 1.0f), new Color(0f, 0.4f, 0.5f), ((float)_i / (float)_values.Length));
                            GUI.DrawTexture(_outerRect, _style.normal.background);

                            Rect _innerRect = new Rect(_x+2, _controlRect.y +2, _pos - _x-4, _controlRect.height -4);
                            GUI.color = Color.Lerp(new Color(0f, 0.8f, 1.0f), new Color(0f, 0.3f, 0.4f), ((float)_i / (float)_values.Length));
                            GUI.DrawTexture(_innerRect, _style.normal.background);


                            GUI.color = _style.normal.textColor;
                            string _pct = " 100%";
                            if (_i > 0) _pct = " " + ((_values[_i - 1]) * 100).ToString("N1") + "%";
                            GUI.Label(_outerRect, new GUIContent(" LOD" + _i.ToString() + "\n" + _pct));

                            GUI.color = Color.white;
                            _x = _pos;
                        }

                        _x = _style.margin.left + _style.padding.left;
                        for (int _i = 0; _i < _values.Length; _i++)
                        {
                            int _pos = (int)Mathf.Lerp(0f, _controlRect.width, _values[_i]) + _style.margin.left + _style.padding.left;
                            Rect targetRect = new Rect(_controlRect) { x = _x, width = _pos - _x };
                            Rect controlRect = new Rect(targetRect.x + targetRect.width - 2, targetRect.y - 2, 4, _controlRect.height + 4);
                            EditorGUIUtility.AddCursorRect(controlRect, MouseCursor.ResizeHorizontal);
                            _x = _pos;
                        }
                        
                        break;
                    }
                case EventType.MouseDown:
                    {
                        if (_controlRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                        {
                            int _x = _style.margin.left + _style.padding.left;
                            state.Index = -1;
                            for (int _i = 0; _i < _values.Length; _i++)
                            {
                                int _pos = (int)Mathf.Lerp(0f, _controlRect.width, _values[_i]) + _style.margin.left + _style.padding.left;
                                Rect targetRect = new Rect(_controlRect) { x = _x, width = _pos - _x };
                                Rect controlRect = new Rect(targetRect.x + targetRect.width - 4, targetRect.y - 2, 8, _controlRect.height + 4);

                                if (Event.current.mousePosition.x > controlRect.x && Event.current.mousePosition.x <= controlRect.x + controlRect.width)
                                {
                                    state.Index = _i;
                                }
                            }

                            GUIUtility.hotControl = controlID;
                        }
                        break;
                    }

                case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == controlID)
                            GUIUtility.hotControl = 0;                        
                        break;
                    }
            }

            if (Event.current.isMouse && GUIUtility.hotControl == controlID)
            {
                if (state.Index >= 0)
                {
                    float relativeX = Event.current.mousePosition.x - _controlRect.x;

                    _values[state.Index] = Mathf.Clamp01(relativeX / _controlRect.width);
                    if (state.Index > 0)
                    {
                        if (Mathf.Clamp01(relativeX / _controlRect.width) <= _values[state.Index - 1] + 0.005f)
                            _values[state.Index] = _values[state.Index - 1] + 0.005f;
                    }
                    if (state.Index < _values.Length -1)
                    {
                        if (Mathf.Clamp01(relativeX / _controlRect.width) >= _values[state.Index + 1] - 0.005f)
                            _values[state.Index] = _values[state.Index + 1] - 0.005f;
                    }

                    GUI.changed = true;
                    Event.current.Use();
                }

            }

            return _values;
        }


        /// <summary>
        /// Displays and allows interaction in a custom inspector for Manager
        /// </summary>
        public override void OnInspectorGUI()
        {            
            // Layout Phase
            if (Event.current.type == EventType.Layout)
            {
                if (_meshDetailMode.enumValueIndex == 1)
                    _showMeshLOD = true;
                else
                    _showMeshLOD = false;

                if (_textureDetailMode.enumValueIndex == 0)
                    _showTextureStatic = true;
                else
                    _showTextureStatic = false;

                if (_textureDetailMode.enumValueIndex == 1)
                    _showTextureStaticSeparate = true;
                else
                    _showTextureStaticSeparate = false;

                if (_textureDetailMode.enumValueIndex == 2)
                    _showTextureProgressive = true;
                else
                    _showTextureProgressive = false;

                if (_textureDetailMode.enumValueIndex == 3)
                    _showTextureProgressiveSeparate = true;
                else
                    _showTextureProgressiveSeparate = false;

                if (_textureDetailMode.enumValueIndex == 4)
                    _showTextureLOD = true;
                else
                    _showTextureLOD = false;

                if (_textureDetailMode.enumValueIndex == 5)
                    _showTextureLODSeparate = true;
                else
                    _showTextureLODSeparate = false;

                _meshSteps = _meshLODSteps.intValue;
                _progressiveSteps = _textureProgressiveSteps.intValue;
                _textureSteps = _textureLODSteps.intValue;

            }

            _script.RefreshLists();
            _script.RefreshBlueprintDictionary();
            _target.Update();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("PROCEDURAL PLANETS MANAGER", _header);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorIcons.IconHelp(), GUILayout.Width(20), GUILayout.Height(14)))
            {
                _showHelp = !_showHelp;
                return;
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            if (GUILayout.Button("Create Random Planet")) CreatePlanet();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("PLANET BLUEPRINTS", _header);

            float _total = 0f;

            for (int i = 0; i < _listSolidPlanetBlueprints.arraySize; i++)
                _total += ((BlueprintSolidPlanet)_listSolidPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue).probability;

            for (int i = 0; i < _listGasPlanetBlueprints.arraySize; i++)
                _total += ((BlueprintGasPlanet)_listGasPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue).probability;
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TYPE", EditorStyles.boldLabel, GUILayout.Width(WIDTH_BLUEPRINT_TYPE));
            EditorGUILayout.LabelField("BLUEPRINT", EditorStyles.boldLabel, GUILayout.Width(WIDTH_BLUEPRINT_NAME));
            EditorGUILayout.LabelField("TOOLS", EditorStyles.boldLabel, GUILayout.Width(8 + WIDTH_BUTTON * 3));
            GUI.Label(EditorGUILayout.GetControlRect(false), "PROBABILITY", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();      

            // Solid Planet Blueprints
            for (int i = 0; i < _listSolidPlanetBlueprints.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                float _value = ((BlueprintSolidPlanet)(_listSolidPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).probability;
                EditorGUILayout.LabelField("Solid", GUILayout.Width(WIDTH_BLUEPRINT_TYPE));
                ((BlueprintSolidPlanet)(_listSolidPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject.name = EditorGUILayout.TextField(((BlueprintSolidPlanet)(_listSolidPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject.name, GUILayout.Width(WIDTH_BLUEPRINT_NAME));
                if (GUILayout.Button(new GUIContent(EditorIcons.IconEdit(), "Edit this blueprint"), GUILayout.Width(WIDTH_BUTTON), GUILayout.Height(HEIGHT_BUTTON)))
                    Selection.activeGameObject = ((BlueprintSolidPlanet)(_listSolidPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject;
                if (GUILayout.Button(new GUIContent(EditorIcons.IconPlusInCircle(), "Create planet using this blueprint"), GUILayout.Width(WIDTH_BUTTON), GUILayout.Height(HEIGHT_BUTTON)))
                    PlanetManager.CreatePlanet(Vector3.zero, - 1, ((BlueprintSolidPlanet)(_listSolidPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject.name);
                if (GUILayout.Button(new GUIContent(EditorIcons.IconDelete(), "Delete this blueprint"), GUILayout.Width(WIDTH_BUTTON), GUILayout.Height(HEIGHT_BUTTON)))
                {
                    if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to delete the blueprint?", "Yes", "Cancel"))
                    {
                        DestroyImmediate(((BlueprintSolidPlanet)(_listSolidPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject);
                        _script.RefreshLists();
                        _target.ApplyModifiedProperties();
                        return;
                    }
                }
                GUILayout.Space(5);
                _value = GUI.HorizontalSlider(EditorGUILayout.GetControlRect(false), _value, 0.0F, 1.0f);
                ((BlueprintSolidPlanet)(_listSolidPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).probability = _value;
                EditorGUILayout.LabelField(((_value / _total ) * 100).ToString("F1").Replace("NaN", "0") + "%", GUILayout.Width(WIDTH_BLUEPRINT_PERCENT));
                    
                EditorGUILayout.EndHorizontal();
            }
            // Gas Planet Blueprints
            for (int i = 0; i < _listGasPlanetBlueprints.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                float _value = ((BlueprintGasPlanet)(_listGasPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).probability;
                EditorGUILayout.LabelField("Gas", GUILayout.Width(WIDTH_BLUEPRINT_TYPE));
                ((BlueprintGasPlanet)(_listGasPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject.name = EditorGUILayout.TextField(((BlueprintGasPlanet)(_listGasPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject.name, GUILayout.Width(WIDTH_BLUEPRINT_NAME));
                if (GUILayout.Button(new GUIContent(EditorIcons.IconEdit(), "Edit this blueprint"), GUILayout.Width(WIDTH_BUTTON), GUILayout.Height(HEIGHT_BUTTON)))
                    Selection.activeGameObject = ((BlueprintGasPlanet)(_listGasPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject;
                if (GUILayout.Button(new GUIContent(EditorIcons.IconPlusInCircle(), "Create planet using this blueprint"), GUILayout.Width(WIDTH_BUTTON), GUILayout.Height(HEIGHT_BUTTON)))
                    PlanetManager.CreatePlanet(Vector3.zero, -1, ((BlueprintGasPlanet)(_listGasPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject.name);
                if (GUILayout.Button(new GUIContent(EditorIcons.IconDelete(), "Delete this blueprint"), GUILayout.Width(WIDTH_BUTTON), GUILayout.Height(HEIGHT_BUTTON)))
                {
                    if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to delete the blueprint?", "Yes", "Cancel"))
                    {
                        DestroyImmediate(((BlueprintGasPlanet)(_listGasPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).gameObject);
                        _script.RefreshLists();
                        _target.ApplyModifiedProperties();
                        return;
                    }
                }
                GUILayout.Space(5);
                _value = GUI.HorizontalSlider(EditorGUILayout.GetControlRect(false), _value, 0.0F, 1.0f);
                ((BlueprintGasPlanet)(_listGasPlanetBlueprints.GetArrayElementAtIndex(i).objectReferenceValue)).probability = _value;
                EditorGUILayout.LabelField(((_value / _total) * 100).ToString("F1").Replace("NaN", "0") + "%", GUILayout.Width(WIDTH_BLUEPRINT_PERCENT));

                EditorGUILayout.EndHorizontal();
            }
        
            if (EditorGUI.EndChangeCheck())
                _script.RefreshBlueprintDictionary();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New Solid Planet Blueprint"))
            {
                CreateSolidPlanetBlueprint();
                return;
            }
            if (GUILayout.Button("New Gas Planet Blueprint"))
            {
                CreateGasPlanetBlueprint();
                return;
            }
            EditorGUILayout.EndHorizontal();

            // Blueprint Tools

            EditorGUILayout.Space();            
            EditorGUILayout.LabelField("BLUEPRINT TOOLS", EditorStyles.boldLabel);

            if (GUILayout.Button("Export All Blueprints to Clipboard (JSON)"))
                ExportAllBlueprintsToClipboard();
            
            if (GUILayout.Button("Import Blueprints from Clipboard (JSON)"))
                if (EditorUtility.DisplayDialog("Confirmation", "This will overwrite any blueprints that have the same name as imported blueprints.", "Proceed", "Cancel"))
                    ImportAllBlueprintsFromClipboard();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            // Procedural Materials
            EditorGUILayout.LabelField("PROCEDURAL MATERIALS", _header);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("SOLID PLANETS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_solidCompositionMaterials, true);
            EditorGUILayout.PropertyField(_solidBiomeMaterials, true);
            EditorGUILayout.PropertyField(_solidCloudsMaterials, true);
            EditorGUILayout.PropertyField(_solidCitiesMaterials, true);
            EditorGUILayout.PropertyField(_solidLavaMaterials, true);
            EditorGUILayout.PropertyField(_solidPolarIceMaterials, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GAS PLANETS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_gasMaterials, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("RINGS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_ringMaterials, true);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_substanceDuplicates, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            if (_showMeshLOD || _showTextureLOD || _showTextureLODSeparate)
            {
                EditorGUILayout.PropertyField(_cameraLOD, new GUIContent("LOD Camera", "Camera used to calculate size for Level of Detail (LOD). Defaults to Camera.main"));
            }

            EditorGUILayout.LabelField("MESH DETAIL", _header);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_meshDetailMode);
            if (EditorGUI.EndChangeCheck())
            {
                _target.ApplyModifiedProperties();
                _script.RecreateProceduralMeshes();
            }
                

            EditorGUI.BeginChangeCheck();

            GUIStyle _gs = new GUIStyle();
            _gs.normal.background = _texSolid;
            _gs.margin = EditorStyles.inspectorDefaultMargins.margin;
            _gs.padding = EditorStyles.inspectorDefaultMargins.padding;
            _gs.normal.textColor = Color.black;

            if (_showMeshLOD)
            {
                EditorGUI.BeginChangeCheck();
                _meshLODSteps.intValue = EditorGUILayout.IntSlider("Levels", _meshLODSteps.intValue, 2, 5);
                if (EditorGUI.EndChangeCheck())
                {
                    _target.ApplyModifiedProperties();
                    _script.RecreateProceduralMeshes();
                }
                _showEditorMeshLOD.boolValue = EditorGUILayout.Toggle("Show LOD Editor", _showEditorMeshLOD.boolValue);

                EditorGUILayout.Space();

                float[] _values = new float[_meshLODSteps.intValue -1];

                for (int _i = 0; _i < _values.Length; _i++)
                    _values[_i] = _meshLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue;

                _values = LodSlider(EditorGUILayout.GetControlRect(false, GUILayout.Height(30)), _values, _gs);

                for (int _i = 0; _i < _values.Length; _i++)
                    _meshLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue = _values[_i];


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(14);
                GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), "LOD LEVEL", EditorStyles.boldLabel);
                GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(80)), "TRIANGLES", EditorStyles.boldLabel);
                GUI.Label(EditorGUILayout.GetControlRect(false), "PLANET SCREEN %", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();
                
                for (int _i = 0; _i < _meshSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (highest)";
                    if (_i == _meshSteps - 1)
                        _label = "Level " + (_meshSteps - 1) + " (lowest)";

                    GUILayout.Space(14);
                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _meshLODSubdivisions.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup("", _meshLODSubdivisions.GetArrayElementAtIndex(_i).intValue, lodTriangles, GUILayout.Width(80));
                    GUILayout.Space(5);
                    if (_i != 0)
                    {
                        _meshLODPlanetSizes.GetArrayElementAtIndex(_i-1).floatValue = GUI.HorizontalSlider(EditorGUILayout.GetControlRect(false), _meshLODPlanetSizes.GetArrayElementAtIndex(_i-1).floatValue, 1.0f, 0.0f);
                        EditorGUILayout.LabelField(((_meshLODPlanetSizes.GetArrayElementAtIndex(_i-1).floatValue) * 100).ToString("F1").Replace("NaN", "0") + "%", GUILayout.Width(WIDTH_BLUEPRINT_PERCENT));
                    }                    

                    if (_i < _meshSteps - 2)
                        if (_meshLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue < _meshLODPlanetSizes.GetArrayElementAtIndex(_i + 1).floatValue)
                            _meshLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue = _meshLODPlanetSizes.GetArrayElementAtIndex(_i + 1).floatValue;

                    if (_i > 0 && _i < _meshSteps -1)
                        if (_meshLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue > _meshLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue)
                            _meshLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue = _meshLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue;

                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUI.indentLevel+=2;
                _meshStaticSubdivisions.intValue = EditorGUILayout.Popup("Mesh Triangles", _meshStaticSubdivisions.intValue, lodTriangles);
                EditorGUI.indentLevel-=2;
            }
            if (EditorGUI.EndChangeCheck())
            {
                _target.ApplyModifiedProperties();
                _script.RecreateProceduralMeshes();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("TEXTURE DETAIL", _header);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_textureDetailMode, new GUIContent("Resolution"));
            if (EditorGUI.EndChangeCheck())
            {
                _target.ApplyModifiedProperties();
                _script.RebuildAllPlanetTextures(true);
            }

            if (_showTextureStatic)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                _textureStaticCommon.intValue = EditorGUILayout.Popup("All Textures", _textureStaticCommon.intValue, lodResolution);
                if (EditorGUI.EndChangeCheck())
                {
                    _target.ApplyModifiedProperties();
                    _script.RebuildAllPlanetTextures();
                }
                EditorGUI.indentLevel--;
            }

            if (_showTextureStaticSeparate)
            {
                EditorGUI.indentLevel ++;
                EditorGUI.BeginChangeCheck();
                _textureStaticComposition.intValue = EditorGUILayout.Popup("Composition", _textureStaticComposition.intValue, lodResolution);
                _textureStaticBiome.intValue = EditorGUILayout.Popup("Biome", _textureStaticBiome.intValue, lodResolution);
                _textureStaticClouds.intValue = EditorGUILayout.Popup("Clouds", _textureStaticClouds.intValue, lodResolution);
                _textureStaticCities.intValue = EditorGUILayout.Popup("Cities", _textureStaticCities.intValue, lodResolution);
                _textureStaticLava.intValue = EditorGUILayout.Popup("Lava", _textureStaticLava.intValue, lodResolution);
                _textureStaticPolarIce.intValue = EditorGUILayout.Popup("PolarIce", _textureStaticPolarIce.intValue, lodResolution);
                _textureStaticGas.intValue = EditorGUILayout.Popup("Gas", _textureStaticGas.intValue, lodResolution);
                if (EditorGUI.EndChangeCheck())
                {
                    _target.ApplyModifiedProperties();
                    _script.RebuildAllPlanetTextures();
                }
                EditorGUI.indentLevel --;
            }

            if (_showTextureProgressive)
            {
                EditorGUI.indentLevel++;
                _textureProgressiveSteps.intValue = EditorGUILayout.IntSlider("Steps", _textureProgressiveSteps.intValue, 2, 5);
                for (int _i = 0; _i < _progressiveSteps; _i++)
                {
                    string _label = "Step " + _i;
                    if (_i == 0)
                        _label = "Step 0 (First)";
                    if (_i == _progressiveSteps - 1)
                        _label = "Step " + (_progressiveSteps - 1) + " (Final)";
                    _textureProgressiveCommon.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_label, _textureProgressiveCommon.GetArrayElementAtIndex(_i).intValue, lodResolution);

                }
                EditorGUI.indentLevel--;
            }

            if (_showTextureProgressiveSeparate)
            {
                EditorGUI.indentLevel ++;
                _textureProgressiveSteps.intValue = EditorGUILayout.IntSlider("Steps", _textureProgressiveSteps.intValue, 2, 5);
                EditorGUILayout.LabelField("Composition");
                EditorGUI.indentLevel++;
                for (int _i = 0; _i < _progressiveSteps; _i++) 
                {
                    string _label = "Step " + _i;
                    if (_i == 0)
                        _label = "Step 0 (First)";
                    if (_i == _progressiveSteps - 1)
                        _label = "Step " + (_progressiveSteps - 1) + " (Final)";
                    _textureProgressiveComposition.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_label, _textureProgressiveComposition.GetArrayElementAtIndex(_i).intValue, lodResolution);

                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Biome");
                EditorGUI.indentLevel++;
                for (int _i = 0; _i < _progressiveSteps; _i++)
                {
                    string _label = "Step " + _i;
                    if (_i == 0)
                        _label = "Step 0 (First)";
                    if (_i == _progressiveSteps - 1)
                        _label = "Step " + (_progressiveSteps - 1) + " (Final)";
                    _textureProgressiveBiome.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_label, _textureProgressiveBiome.GetArrayElementAtIndex(_i).intValue, lodResolution);

                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Clouds");
                EditorGUI.indentLevel++;
                for (int _i = 0; _i < _progressiveSteps; _i++)
                {
                    string _label = "Step " + _i;
                    if (_i == 0)
                        _label = "Step 0 (First)";
                    if (_i == _progressiveSteps - 1)
                        _label = "Step " + (_progressiveSteps - 1) + " (Final)";
                    _textureProgressiveClouds.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_label, _textureProgressiveClouds.GetArrayElementAtIndex(_i).intValue, lodResolution);

                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Cities");
                EditorGUI.indentLevel++;
                for (int _i = 0; _i < _progressiveSteps; _i++)
                {
                    string _label = "Step " + _i;
                    if (_i == 0)
                        _label = "Step 0 (First)";
                    if (_i == _progressiveSteps - 1)
                        _label = "Step " + (_progressiveSteps - 1) + " (Final)";
                    _textureProgressiveCities.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_label, _textureProgressiveCities.GetArrayElementAtIndex(_i).intValue, lodResolution);

                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Lava");
                EditorGUI.indentLevel++;
                for (int _i = 0; _i < _progressiveSteps; _i++)
                {
                    string _label = "Step " + _i;
                    if (_i == 0)
                        _label = "Step 0 (First)";
                    if (_i == _progressiveSteps - 1)
                        _label = "Step " + (_progressiveSteps - 1) + " (Final)";
                    _textureProgressiveLava.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_label, _textureProgressiveLava.GetArrayElementAtIndex(_i).intValue, lodResolution);

                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("PolarIce");
                EditorGUI.indentLevel++;
                for (int _i = 0; _i < _progressiveSteps; _i++)
                {
                    string _label = "Step " + _i;
                    if (_i == 0)
                        _label = "Step 0 (First)";
                    if (_i == _progressiveSteps - 1)
                        _label = "Step " + (_progressiveSteps - 1) + " (Final)";
                    _textureProgressivePolarIce.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_label, _textureProgressivePolarIce.GetArrayElementAtIndex(_i).intValue, lodResolution);

                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Gas");
                EditorGUI.indentLevel++;
                for (int _i = 0; _i < _progressiveSteps; _i++)
                {
                    string _label = "Step " + _i;
                    if (_i == 0)
                        _label = "Step 0 (First)";
                    if (_i == _progressiveSteps - 1)
                        _label = "Step " + (_progressiveSteps - 1) + " (Final)";
                    _textureProgressiveGas.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_label, _textureProgressiveGas.GetArrayElementAtIndex(_i).intValue, lodResolution);

                }
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel --;
            }

            if (_showTextureLOD ||_showTextureLODSeparate)
            {
                _textureLODSteps.intValue = EditorGUILayout.IntSlider("Levels", _textureLODSteps.intValue, 2, 5);
                _showEditorTextureLOD.boolValue = EditorGUILayout.Toggle("Show LOD Editor", _showEditorTextureLOD.boolValue);

                EditorGUILayout.Space();

                float[] _values = new float[_textureLODSteps.intValue - 1];

                for (int _i = 0; _i < _values.Length; _i++)
                    _values[_i] = _textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue;

                _values = LodSlider(EditorGUILayout.GetControlRect(false, GUILayout.Height(30)), _values, _gs);

                for (int _i = 0; _i < _values.Length; _i++)
                    _textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue = _values[_i];
            }

            if (_showTextureLOD)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(14);
                GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), "LOD LEVEL", EditorStyles.boldLabel);
                GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), "RESOLUTION", EditorStyles.boldLabel);
                GUI.Label(EditorGUILayout.GetControlRect(false), "PLANET SCREEN %", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (lowest)";

                    GUILayout.Space(14);
                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _textureLODCommon.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup("", _textureLODCommon.GetArrayElementAtIndex(_i).intValue, lodResolution, GUILayout.Width(100));
                    GUILayout.Space(5);
                    if (_i != 0)
                    {
                        _textureLODPlanetSizes.GetArrayElementAtIndex(_i-1).floatValue = GUI.HorizontalSlider(EditorGUILayout.GetControlRect(false), _textureLODPlanetSizes.GetArrayElementAtIndex(_i-1).floatValue, 1.0f, 0.0f);
                        EditorGUILayout.LabelField(((_textureLODPlanetSizes.GetArrayElementAtIndex(_i-1).floatValue) * 100).ToString("F1").Replace("NaN", "0") + "%", GUILayout.Width(WIDTH_BLUEPRINT_PERCENT));
                    }

                    if (_i < _textureSteps - 2)
                        if (_textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue < _textureLODPlanetSizes.GetArrayElementAtIndex(_i + 1).floatValue)
                            _textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue = _textureLODPlanetSizes.GetArrayElementAtIndex(_i + 1).floatValue;

                    if (_i > 0 && _i < _textureSteps - 1)
                        if (_textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue > _textureLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue)
                            _textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue = _textureLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue;

                    EditorGUILayout.EndHorizontal();
                }         
            }

            if (_showTextureLODSeparate)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), "LOD LEVEL", EditorStyles.boldLabel);
                GUI.Label(EditorGUILayout.GetControlRect(false), "PLANET SCREEN %", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (lowest)";

                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    GUILayout.Space(5);
                    if (_i != 0)
                    {
                        _textureLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue = GUI.HorizontalSlider(EditorGUILayout.GetControlRect(false), _textureLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue, 1.0f, 0.0f);
                        EditorGUILayout.LabelField(((_textureLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue) * 100).ToString("F1").Replace("NaN", "0") + "%", GUILayout.Width(WIDTH_BLUEPRINT_PERCENT));
                    }

                    if (_i < _textureSteps - 2)
                        if (_textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue < _textureLODPlanetSizes.GetArrayElementAtIndex(_i + 1).floatValue)
                            _textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue = _textureLODPlanetSizes.GetArrayElementAtIndex(_i + 1).floatValue;

                    if (_i > 0 && _i < _textureSteps - 1)
                        if (_textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue > _textureLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue)
                            _textureLODPlanetSizes.GetArrayElementAtIndex(_i).floatValue = _textureLODPlanetSizes.GetArrayElementAtIndex(_i - 1).floatValue;

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Composition");
                EditorGUI.indentLevel--;

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (Highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (Lowest)";

                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _textureLODComposition.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_textureLODComposition.GetArrayElementAtIndex(_i).intValue, lodResolution);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Biome");
                EditorGUI.indentLevel--;

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (Highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (Lowest)";

                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _textureLODBiome.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_textureLODBiome.GetArrayElementAtIndex(_i).intValue, lodResolution);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Clouds");
                EditorGUI.indentLevel--;

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (Highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (Lowest)";

                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _textureLODClouds.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_textureLODClouds.GetArrayElementAtIndex(_i).intValue, lodResolution);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Cities");
                EditorGUI.indentLevel--;

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (Highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (Lowest)";

                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _textureLODCities.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_textureLODCities.GetArrayElementAtIndex(_i).intValue, lodResolution);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Lava");
                EditorGUI.indentLevel--;

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (Highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (Lowest)";

                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _textureLODLava.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_textureLODLava.GetArrayElementAtIndex(_i).intValue, lodResolution);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("PolarIce");
                EditorGUI.indentLevel--;

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (Highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (Lowest)";

                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _textureLODPolarIce.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_textureLODPolarIce.GetArrayElementAtIndex(_i).intValue, lodResolution);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Gas");
                EditorGUI.indentLevel--;

                for (int _i = 0; _i < _textureSteps; _i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    string _label = "Level " + _i;
                    if (_i == 0)
                        _label = "Level 0 (Highest)";
                    if (_i == _textureSteps - 1)
                        _label = "Level " + (_textureSteps - 1) + " (Lowest)";

                    GUI.Label(EditorGUILayout.GetControlRect(false, GUILayout.Width(100)), _label);
                    _textureLODGas.GetArrayElementAtIndex(_i).intValue = EditorGUILayout.Popup(_textureLODGas.GetArrayElementAtIndex(_i).intValue, lodResolution);
                    EditorGUILayout.EndHorizontal();
                }
            }
           
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("DEBUGGING", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_debug_level, new GUIContent("Debug Level"));


            _target.ApplyModifiedProperties();
        }

        /// <summary>
        /// Creates a random planet at vector 0,0,0
        /// </summary>
        public void CreatePlanet()
        {
            PlanetManager.CreatePlanet(Vector3.zero);
        }

        /// <summary>
        /// Creates a solid planet blueprint.
        /// </summary>
        public void CreateSolidPlanetBlueprint()
        {
            GameObject _go = new GameObject();
            _go.name = _script.GetUniqueBlueprintName();
            _go.transform.parent = _script.transform;
            _go.AddComponent<BlueprintSolidPlanet>();
            _script.RefreshLists();
        }

        /// <summary>
        /// Creates a gas planet blueprint.
        /// </summary>
        public void CreateGasPlanetBlueprint()
        {
            GameObject _go = new GameObject();
            _go.name = _script.GetUniqueBlueprintName();
            _go.transform.parent = _script.transform;
            _go.AddComponent<BlueprintGasPlanet>();
            _script.RefreshLists();
        }

        /// <summary>
        /// Updfates procedural textures based on texture name.
        /// </summary>
        /// <param name="_textureName"></param>
        void UpdateProceduralTexture(string _textureName)
        {
            SolidPlanet[] _sp = FindObjectsOfType<SolidPlanet>();
            foreach (SolidPlanet _p in _sp)
                _p.UpdateProceduralTexture(_textureName);

            /* DISABLED UNTIL IMPLEMENTED

            GasPlanet[] _gp = FindObjectsOfType<GasPlanet>();
            foreach (GasPlanet _p in _gp)
                _p.UpdateProceduralTexture(_textureName);
            */
        }

        /// <summary>
        /// Exports all planet (and child ring) blueprints to clipboard as a JSON string with each blueprint being a unique item.
        /// </summary>
        void ExportAllBlueprintsToClipboard()
        {
            // Use the JSON export method in the target script
            _script.ExportAllBlueprintsToClipboard();
        }

        /// <summary>
        /// Imports planet (and child ring) blueprints from clipboard (must be a valid JSON string with blueprints as unique numbered items.
        /// </summary>
        void ImportAllBlueprintsFromClipboard()
        {
            // Use the JSON import method in the target script
            _script.ImportBlueprintsFromClipboard();
        }
    }
}
