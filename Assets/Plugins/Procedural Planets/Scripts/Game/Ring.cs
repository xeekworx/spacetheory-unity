using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ProceduralPlanets.SimpleJSON;

namespace ProceduralPlanets
{
    /// <summary>
    /// This is the class for ring.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    [ExecuteInEditMode]
    public class Ring : MonoBehaviour
    {
        const string RING_VERSION = "1.0";

        // Compact JSON-string that caches ring settings since planets are recreated on each play/stop
        public string serializedRingCache = null;

        // Float and Material lists and dictionaries. 
        public List<PropertyMaterial> propertyMaterials = new List<PropertyMaterial>(0);       
        public List<PropertyFloat> propertyFloats = new List<PropertyFloat>(0);

        // Since dictionaries cannot be serialized, lists are used for serialization for the editor script and dictionaries are synced for
        // dictionary lookup purposes.
        private Dictionary<string, PropertyMaterial> _dictionaryMaterials = new Dictionary<string, PropertyMaterial>();
        private Dictionary<string, PropertyFloat> _dictionaryFloats = new Dictionary<string, PropertyFloat>(0);

        // Procedural Materials used by the ring
        public Substance.Game.SubstanceGraph proceduralMaterial;

        // Textures used by the ring
        private Texture2D _textureRing;

        // String format alternatives for ring export
        public enum StringFormat { JSON_EASY_READ, JSON_COMPACT, JSON_ESCAPED, JSON_BASE64 }
 
        // Random seed
        public int seed;

        // Reference to the blueprint for this ring
        public BlueprintRing blueprintRing;

        // Private Variables
        // There are two of each ring object, mesh, renderer, and materials because the ring is split in two sections to
        // allow proper sorting when infront and behind semitransparent atmosphere.
        private GameObject[] _ring = new GameObject[2];
        private Mesh[] _mesh = new Mesh[2];
        private MeshFilter[] _meshFilter = new MeshFilter[2];
        private MeshRenderer[] _meshRenderer = new MeshRenderer[2];
        private Material[] _material = new Material[2];

        // Planet variables
        private Planet _planet;
        private float _planetRadius;

        // Flags
        public bool rebuildTextures = false;
        public bool rebuildRingNeeded = false;

        // Integer IDs of shader properties for performance
        private int _shaderID_LocalStarPosition;
        private int _shaderID_LocalStarColor;
        private int _shaderID_LocalStarIntensity;
        private int _shaderID_LocalStarAmbientIntensity;
        private int _shaderID_PlanetRadius;
        private int _shaderID_PlanetPosition;

        // Local Star
        private LocalStar.ShaderCacheSettings _localStarShaderCacheSettings;
        private LocalStar _localStarNearestInstance;

        void Reset()
        {
            if (gameObject.GetComponent<Planet>() != null)
            {
                Debug.LogError("You can't add this ring component directly to a planet. It must be a child object. Aborting and removing component.");
                DestroyImmediate(this);
                return;
            }
        }

        /// <summary>
        /// Creates the ring and adds all necessary property materials and floats
        /// </summary>
        void Awake()
        {
            if (gameObject.GetComponent<Planet>() != null)
            {
                Debug.LogError("You can't add this ring component directly to a planet. It must be a child object. Aborting and removing component.");
                DestroyImmediate(this);
                return;
            }

            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: Awake()");
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("- Ring Version: " + RING_VERSION);

            // Set Shader property int IDs for increased performance when updating property parameters
            _shaderID_LocalStarPosition = Shader.PropertyToID("_LocalStarPosition");
            _shaderID_LocalStarColor = Shader.PropertyToID("_LocalStarColor");
            _shaderID_LocalStarIntensity = Shader.PropertyToID("_LocalStarIntensity");
            _shaderID_LocalStarAmbientIntensity = Shader.PropertyToID("_LocalStarAmbientIntensity");
            _shaderID_PlanetRadius = Shader.PropertyToID("_PlanetRadius");
            _shaderID_PlanetPosition = Shader.PropertyToID("_PlanetPosition");

            // Ensure that there is a LocalStar in the scene.
            if (FindObjectOfType<LocalStar>() == null)
                Debug.LogWarning("There is no LocalStar in the scene. Planet and ring will not be lit. Create a game object and add the LocalStar component. The position of the local star game object will be the light source.");

            // Ensure that this ring has a parent planet transform
            if (transform.parent == null)
            {
                Debug.LogWarning("There is no parent planet transform to this ring. Aborting ring creation.");
                return;
            }

            // Get the parent planet component
            if (transform.parent.GetComponent<Planet>() != null)
                _planet = transform.parent.GetComponent<Planet>();

            // Ensure ring parent planet has a planet component
            if (_planet == null)
            {
                Debug.LogError("Ring parent has no Planet component (or component derived from Planet class). Aborting.");
                return;
            }
            else
            {
                // Set the planet radius (used for shadow size) - this is static at the moment and it'll scale with planet scale
                _planetRadius = PlanetManager.CONST_MESH_RADIUS + 1.0f;
            }


            // If blueprint has not been set prior to awake being called...
            if (blueprintRing == null)
            {
                // Try to obtain ring blueprint from planet blueprint
                //GetRingBlueprintFromPlanetBlueprint();
                blueprintRing = _planet.GetRingBlueprint();  
                if (blueprintRing == null)
                {
                    // If ring blueprint could not be retrieved, throw error and disable ring component.
                    Debug.LogError("Ring Blueprint not set and could not be retrieved. Disabling component.");
                    gameObject.SetActive(false);
                    return;
                }                    
            }
                
            // Clear properties
            propertyFloats.Clear();
            propertyMaterials.Clear();

            AddPropertyMaterial("ringMaterial", "Ring Material*", PlanetManager.Instance.ringMaterials.ToArray(), 10, new string[] { "Ring" });

            // Update dictionaries (for materials a this stage)
            UpdateDictionariesIfNeeded(true);

            // Set default properties (for materials at this stage)
            SetDefaultProperties();

            // Get a vacatnt instance of the material
            proceduralMaterial = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["ringMaterial"].GetPropertyMaterial(), gameObject, "Ring");
            //proceduralMaterial = _dictionaryMaterials["ringMaterial"].GetPropertyMaterial();

            // Add floats
            AddPropertyFloat("innerRadius", "Inner Radius", 1.0f, 10.0f, true, false, 7234, false, null);        
            AddPropertyFloat("outerRadius", "Outer Radius", 1.0f, 10.0f, true, false, 8543, false, null);            
            AddPropertyFloat("gradientDiffuse", "Gradient Diffuse", 0.0f, 1.0f, true, false, 30, false, new string[] { "Ring" }, proceduralMaterial, "GradientDiffuse", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("gradientAlpha", "Gradient Alpha", 0.0f, 1.0f, true, false, 70, false, new string[] { "Ring" }, proceduralMaterial, "GradientAlpha", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("diffuseHue", "Hue", 0.0f, 1.0f, true, false, 400, false, new string[] { "Ring" }, proceduralMaterial, "DiffuseHue", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("diffuseSaturation", "Saturation", 0.0f, 1.0f, true, false, 125, false, new string[] { "Ring" }, proceduralMaterial, "DiffuseSaturation", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("diffuseLightness", "Lightness", 0.0f, 1.0f, true, false, 543, false, new string[] { "Ring" }, proceduralMaterial, "DiffuseLightness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("alphaContrast", "Alpha Contrast", 0.0f, 1.0f, true, false, 2342, false, new string[] { "Ring" }, proceduralMaterial, "AlphaContrast", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("alphaLuminosity", "Alpha Luminosity", 0.0f, 1.0f, true, false, 6532, false, new string[] { "Ring" }, proceduralMaterial, "AlphaLuminosity", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);

            // Update dictionaries (again, now with all Float and Color properties too) 
            UpdateDictionariesIfNeeded(true);

            // Set default properties based on seed (this time for all properties)
            SetDefaultProperties();

            // Load ring settings from cache (if this is not a new ring) - this overwrites default settings if changes have been made
            if (serializedRingCache != null)
                if (serializedRingCache.Length > 0)
                    ImportFromJSON(serializedRingCache);

            // Create materials
            CreateMaterials();

            // Update dictionaries
            UpdateDictionariesIfNeeded(true);

            // Create meshes
            CreateMeshes();

            // Update shader for planet lighting
            UpdateShaderLocalStar(true);

            // Rebuild textures
            RebuildTextures(true);
        }

        /// <summary>
        /// Update is called every frame and it rotates the ring in relation to the camera to ensure atmosphere transparency sorting works.
        /// </summary>
        void Update()
        {
            // Find the main camera transform acting as target
            Transform _target = Camera.main.transform;

            // Get the target position Vector3
            Vector3 _targetPostition = new Vector3(_target.position.x, transform.position.y, _target.position.z);

            // Make the ring transform look at the camera along and rotate around the up axis
            transform.LookAt(_targetPostition, Vector3.up);

            // Get the main camera transform forward direction vector3
            Vector3 _cameraForward = Camera.main.transform.forward;

            // Get the sign of the cross product of the negative transform and camera forward vector
            int _sign = Vector3.Cross(-transform.forward, _cameraForward).y < 0 ? -1 : 1;

            // Get the angle between the negative transform foward and camera forward
            float _angle = Vector3.Angle(-transform.forward, _cameraForward);

            // Multiply angle by the sign 
            _angle *= _sign;

            // Set the new forward of the ring transform
            Vector3 _newForward = Vector3.Lerp(transform.forward, new Vector3(-Camera.main.transform.forward.x, 0, -Camera.main.transform.forward.z), 0.5f);            
            if (_newForward != Vector3.zero)
                transform.forward = _newForward;

            // Rotate the ring negative 80 degrees, making it most probable that the ring and atmosphere sorts correctly
            // It's 80 degrees and not 90 because the close part of the ring mesh covers 160 degrees (80 is half of that) and the far side of the ring covers 200 degrees
            transform.Rotate(new Vector3(0.0f, -80.0f, 0.0f));
            
            // Update shader based on local star parameters (only if changed, hence force is set to false)
            UpdateShaderLocalStar(false);

            // Rebuild ring textures if needed
            if (RebuildTexturesNeeded())
                RebuildTextures();
        }

        /// <summary>
        /// Adds a float property that affects a single Procedural Material
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_minValue"></param>
        /// <param name="_maxValue"></param>
        /// <param name="_clamp01"></param>
        /// <param name="_displayAsInt"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_variation"></param>
        /// <param name="_proceduralTextures"></param>
        /// <param name="_materials"></param>
        /// <param name="_shaderProperty"></param>
        /// <param name="_method"></param>
        /// <param name="_dataType"></param>
        protected virtual void AddPropertyFloat(string _key, string _label, float _minValue, float _maxValue, bool _clamp01, bool _displayAsInt, int _seedOffset, bool _variation, string[] _proceduralTextures, Material _materials, string _shaderProperty = null, PropertyFloat.Method _method = PropertyFloat.Method.VALUE, PropertyFloat.DataType _dataType = PropertyFloat.DataType.FLOAT)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: AddPropertyFloat(" + _key + "," + _label + "," + _minValue + "," + _maxValue + "," + _clamp01 + "," + _displayAsInt + "," + _seedOffset + "," + _variation + "," + _proceduralTextures + "," + _materials + "," + _shaderProperty + "," + _method + "," + _dataType + ")");
            AddPropertyFloat(_key, _label, _minValue, _maxValue, _clamp01, _displayAsInt, _seedOffset, _variation, _proceduralTextures, new Material[] { _materials }, _shaderProperty, _method, _dataType);
        }


        /// <summary>
        /// Adds a float property that affects a single Procedural Material
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_minValue"></param>
        /// <param name="_maxValue"></param>
        /// <param name="_clamp01"></param>
        /// <param name="_displayAsInt"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_variation"></param>
        /// <param name="_proceduralTextures"></param>
        /// <param name="_substanceGraph"></param>
        /// <param name="_shaderProperty"></param>
        /// <param name="_method"></param>
        /// <param name="_dataType"></param>
        protected virtual void AddPropertyFloat(string _key, string _label, float _minValue, float _maxValue, bool _clamp01, bool _displayAsInt, int _seedOffset, bool _variation, string[] _proceduralTextures, Substance.Game.SubstanceGraph _substanceGraph, string _shaderProperty = null, PropertyFloat.Method _method = PropertyFloat.Method.VALUE, PropertyFloat.DataType _dataType = PropertyFloat.DataType.FLOAT)
        {
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: AddPropertyFloat(" + _key + "," + _label + "," + _minValue + "," + _maxValue + "," + _clamp01 + "," + _displayAsInt + "," + _seedOffset + "," + _variation + "," + _proceduralTextures + "," + _substanceGraph + "," + _shaderProperty + "," + _method + "," + _dataType + ")");
            AddPropertyFloat(_key, _label, _minValue, _maxValue, _clamp01, _displayAsInt, _seedOffset, _variation, _proceduralTextures, new Substance.Game.SubstanceGraph[] { _substanceGraph }, _shaderProperty, _method, _dataType);
        }


        /// <summary>
        /// Adds a float property that affects multiple Procedural Materials.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_minValue"></param>
        /// <param name="_maxValue"></param>
        /// <param name="_clamp01"></param>
        /// <param name="_displayAsInt"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_variation"></param>
        /// <param name="_proceduralTextures"></param>
        /// <param name="_materials"></param>
        /// <param name="_shaderProperty"></param>
        /// <param name="_method"></param>
        /// <param name="_dataType"></param>
        protected virtual void AddPropertyFloat(string _key, string _label, float _minValue, float _maxValue, bool _clamp01, bool _displayAsInt, int _seedOffset, bool _variation, string[] _proceduralTextures = null, Material[] _materials = null, string _shaderProperty = null, PropertyFloat.Method _method = PropertyFloat.Method.VALUE, PropertyFloat.DataType _dataType = PropertyFloat.DataType.FLOAT)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: AddPropertyFloat(" + _key + "," + _label + "," + _minValue + "," + _maxValue + "," + _clamp01 + "," + _displayAsInt + "," + _seedOffset + "," + _variation + "," + _proceduralTextures + "," + _materials + "," + _shaderProperty + "," + _method + "," + _dataType + ")");
            PropertyFloat _p = new PropertyFloat(_key, _label, _minValue, _maxValue, _clamp01, _displayAsInt, _seedOffset, _variation, _proceduralTextures, _materials, _shaderProperty, _method, _dataType);
            propertyFloats.Add(_p);
        }

        /// <summary>
        /// Adds a float property that affects multiple Procedural Materials.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_minValue"></param>
        /// <param name="_maxValue"></param>
        /// <param name="_clamp01"></param>
        /// <param name="_displayAsInt"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_variation"></param>
        /// <param name="_proceduralTextures"></param>
        /// <param name="_substanceGraphs"></param>
        /// <param name="_shaderProperty"></param>
        /// <param name="_method"></param>
        /// <param name="_dataType"></param>
        protected virtual void AddPropertyFloat(string _key, string _label, float _minValue, float _maxValue, bool _clamp01, bool _displayAsInt, int _seedOffset, bool _variation, string[] _proceduralTextures, Substance.Game.SubstanceGraph[] _substanceGraphs, string _shaderProperty = null, PropertyFloat.Method _method = PropertyFloat.Method.VALUE, PropertyFloat.DataType _dataType = PropertyFloat.DataType.FLOAT)
        {
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: AddPropertyFloat(" + _key + "," + _label + "," + _minValue + "," + _maxValue + "," + _clamp01 + "," + _displayAsInt + "," + _seedOffset + "," + _variation + "," + _proceduralTextures + "," + _substanceGraphs + "," + _shaderProperty + "," + _method + "," + _dataType + ")");
            PropertyFloat _p = new PropertyFloat(_key, _label, _minValue, _maxValue, _clamp01, _displayAsInt, _seedOffset, _variation, _proceduralTextures, _substanceGraphs, _shaderProperty, _method, _dataType);
            propertyFloats.Add(_p);
        }

        /// <summary>
        /// Answers the question if a PropertyMaterial exists based on property key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>True/False</returns>
        bool PropertyMaterialExists(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: AddPropertyExists(" + _key + ")");
            foreach (PropertyMaterial _p in propertyMaterials)
                if (_p.key == _key)
                    return true;
            return false;
        }

        /// <summary>
        /// Adds a material property.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_materials"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_proceduralTextures"></param>
        void AddPropertyMaterial(string _key, string _label, Substance.Game.SubstanceGraph[] _materials, int _seedOffset, string[] _proceduralTextures = null)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: AddPropertyMaterial(" + _key + "," + _label + "," + _materials + "," + _seedOffset + "," + _proceduralTextures + "," + _material + ")");
            PropertyMaterial _p = new PropertyMaterial(_key, _label, _materials, _seedOffset, _proceduralTextures);
            propertyMaterials.Add(_p);
        }

        /// <summary>
        /// Updates the dictionaries to contains the Float and Material values of the property lists.
        /// Unity does not support serialized dictionaries so this component need to keep values in the dictionary for the game
        /// and in the list for serialization for the custom inspector in the editor script.
        /// </summary>
        /// <param name="_force"></param
        void UpdateDictionariesIfNeeded(bool _force = false)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: UpdateDictionariesIfNeeded(" + _force + ")");

            // If force update is not set and dictionary already contains values - return
            if (!_force &&
                _dictionaryFloats.Count > 0 &&
                _dictionaryMaterials.Count > 0) return;

            _dictionaryFloats.Clear();
            foreach (PropertyFloat _p in propertyFloats)
                _dictionaryFloats.Add(_p.key, _p);

            _dictionaryMaterials.Clear();
            foreach (PropertyMaterial _p in propertyMaterials)
                _dictionaryMaterials.Add(_p.key, _p);
        }

        /// <summary>
        /// Sets the default properties based on the seed. Optioanally leave or overwrite overridden properties.
        /// </summary>
        /// <param name="_leaveOverride"></param>
        void SetDefaultProperties(bool _leaveOverride = false)
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: SetDefaultProperties(" + _leaveOverride + ")");

            // Iterate through property floats and materials and set values based on a random seed
            foreach (PropertyFloat _p in propertyFloats)
                if (!_leaveOverride || (_leaveOverride && !_p.overrideRandom))
                    SetPropertyFloat(_p.key);
            foreach (PropertyMaterial _p in propertyMaterials)
                if (!_leaveOverride || (_leaveOverride && !_p.overrideRandom))
                    SetPropertyMaterial(_p.key);
        }

        /// <summary>
        /// Sets a PropertyFloat based on random seed.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Float that was set by random.</returns>
        public float SetPropertyFloat(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: SetPropertyFloat(" + _key + ")");

            // Ensure that the dictionary is updated, otherwise it may not contain the key
            UpdateDictionariesIfNeeded();

            // Get reference to the PropertyFloat based on key
            PropertyFloat _p = _dictionaryFloats[_key];

            // Set the seeded property float based on blueprint ranges without affecting current random state
            float _value = _p.SetPropertyFloat(seed + _p.seedOffset, blueprintRing.GetMin(_key), blueprintRing.GetMax(_key));

            // If property float affects ring radius...
            if (_key == "innerRadius" || _key == "outerRadius")
                // Recreate the ringmeshes
                CreateMeshes();

            // If value should be displayed as int, cast it to int to ensure it's rounded off
            if (_p.displayAsInt) _p.value = (int)_p.value;

            // Handle (rebuild) any textures that are affected by this property float
            if (_p.proceduralTextures != null) HandleModifiedTextures(_p.proceduralTextures);

            // Return the random value that was set.
            return _value;
        }

        /// <summary>
        /// Overrides a property value. By default this updates the serialized cache for the ring.
        /// Public any script should be able to override a property of the ring
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        /// <param name="_updateSerializedRingCache"></param>
        public void OverridePropertyFloat(string _key, float _value, bool _updateSerializedRingCache = true)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: OverridePropertyFloat(" + _key + "," + _value + "," + _updateSerializedRingCache + ")");

            // Get the reference to the PropertyFloat
            PropertyFloat _p = _dictionaryFloats[_key];

            // Override the property value with specified value
            _p.OverridePropertyFloat(_value);

            // If property affects mesh size, recreate the meshes
            if (_key == "innerRadius" || _key == "outerRadius")
                //RecreateMesh();
                CreateMeshes();

            // // If this is an int value, cast to int to round off
            if (_p.displayAsInt) _p.value = (int)_p.value;

            // If this property affects any procedural materials, handle (and rebuild) the affected texture(s)
            if (_p.proceduralTextures != null) HandleModifiedTextures(_p.proceduralTextures);

            // Update the serialized planet cache (needed since the planets are recreated on each play/stop event)
            if (_updateSerializedRingCache) serializedRingCache = ExportToJSON(StringFormat.JSON_COMPACT);
        }

        /// <summary>
        /// Answers if property float is overridden based on key (returns true/false)
        /// Public because any script should be able to find out if a property of the planet is overridden
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>true/false if property is overridden</returns>
        public bool IsPropertyFloatOverridden(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: IsPropertyFloatOverridden(" + _key + ")");

            PropertyFloat _p = _dictionaryFloats[_key];
            return _p.overrideRandom;
        }

        /// <summary>
        /// Gets the float value based on key (returns current value regardless whether it is determined by seed or overridden)
        /// Public because any script should be able to find out value of a property of the planet
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Float value of property</returns>
        public float GetPropertyFloat(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: GetPropertyFloat(" + _key + ")");

            PropertyFloat _p = _dictionaryFloats[_key];
            return _p.value;
        }

        /// <summary>
        /// Gets the property value as determined by the random seed using the key as argument. 
        /// See GetPropertySeededFloat(PropertyFloat _p) method for details.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Float value determined by the random seed</returns>
        public float GetPropertySeededFloat(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: GetPropertySeededFloat(" + _key + ")");

            PropertyFloat _p = _dictionaryFloats[_key];
            return GetPropertySeededFloat(_p);
        }

        /// <summary>
        /// Gets the property value as determined by the random seed using the PropertyFloat as argument.
        /// </summary>
        /// <param name="_propertyFloat"></param>
        /// <returns>Float value determined by the random seed</returns>
        public float GetPropertySeededFloat(PropertyFloat _p)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: GetPropertySeededFloat(" + _p + ")");

            float _value = _p.SeededRandomFloat(seed + _p.seedOffset, blueprintRing.GetMin(_p.key), blueprintRing.GetMax(_p.key));
            return _value;
        }

        /// <summary>
        /// Sets a property material to the material determined by the random seed.
        /// Public because editor script calls this method.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Integer index of material</returns>
        public int SetPropertyMaterial(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: SetPropertyMaterial(" + _key + ")");

            // Ensure that the dictionary is updated, otherwise it may not contain the key
            UpdateDictionariesIfNeeded();

            // Get the reference to the material property from the dictionary
            PropertyMaterial _p = _dictionaryMaterials[_key];

            // Set the property to a material index based on the seed between the material mask of the property for this particular configuration template
            int _value = _p.SetPropertyMaterial(seed + _p.seedOffset, blueprintRing.GetMaterialLength(_key), blueprintRing.GetMaterialMask(_key));

            // If this property affects any procedural textures, ensure to handle them so they get updated
            if (_p.proceduralTextures != null) HandleModifiedTextures(_p.proceduralTextures);

            // Return the integer index of the material determined by the random seed
            return _value;
        }

        /// <summary>
        /// Overrides a property material. By default this updates the serialized cache for the ring.
        /// Public because any script should be able to override a property of the ring (e.g. manually set or animate.)
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        /// <param name="_updateSerializedRingCache"></param>
        public void OverridePropertyMaterial(string _key, int _value, bool _updateSerializedRingCache = true)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: OverridePropertyMaterial(" + _key + "," + _value + "," + _updateSerializedRingCache + ")");

            // Get the reference to the PropertyMaterial
            PropertyMaterial _p = _dictionaryMaterials[_key];

            // Override the property material with specified material index value
            _p.OverridePropertyMaterial(_value);

            // If this property affects any procedural materials, handle (and rebuild) the affected texture(s)
            if (_p.proceduralTextures != null) HandleModifiedTextures(_p.proceduralTextures);

            // Update the serialized ring cache (needed since the rings are recreated on each play/stop event)
            if (_updateSerializedRingCache) serializedRingCache = ExportToJSON(StringFormat.JSON_COMPACT);
        }

        /// <summary>
        /// Answers if property material is overridden based on key (returns true/false)
        /// Public any script should be able to find out if a property of the ring is overridden
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>true/false if property is overridden</returns>
        public bool IsPropertyMaterialOverridden(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: IsPropertyMaterialOverridden(" + _key + ")");

            PropertyMaterial _p = _dictionaryMaterials[_key];
            return _p.overrideRandom;
        }

        /// <summary>
        /// Gets the material index integer value based on key (returns current value regardless whether it is determined by seed or overridden)
        /// Public because any script should be able to find out color of a property of the planet
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int GetPropertyMaterial(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: GetPropertyMaterial(" + _key + ")");

            PropertyMaterial _p = _dictionaryMaterials[_key];
            return _p.value;
        }

        /// <summary>
        /// Gets the property material (integer index) as determined by the random seed using the key as argument. 
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Integer index of material</returns>
        public int GetPropertySeededMaterial(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: GetPropertySeededMaterial(" + _key + ")");

            // Get the reference to the materialproperty.
            PropertyMaterial _p = _dictionaryMaterials[_key];

            // Get the seeded random material index value within the material mask specified for the ring blueprint
            return _p.GetPropertySeededMaterialIndex(seed + _p.seedOffset, blueprintRing.GetMaterialLength(_key), blueprintRing.GetMaterialMask(_key));
        }

        /// <summary>
        /// Sets the ring blueprint.
        /// </summary>
        /// <param name="_blueprintRing"></param>
        /// <param name="_seed"></param>
        /// <param name="_updateProperties"></param>
        public void SetBlueprintRing(BlueprintRing _blueprintRing, int _seed = 0, bool _updateProperties = true)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: SetBlueprintRing(" + _blueprintRing + ")");

            // Update dictionaries if needed
            UpdateDictionariesIfNeeded();

            // Set the blueprint
            blueprintRing = _blueprintRing;

            // Set the seed
            seed = _seed;

            // Update properties and recreate textures and meshes if necessary
            if (_updateProperties)
            {
                UpdateProperties();
                rebuildRingNeeded = true;
                CreateMeshes();
            }
        }

        /// <summary>
        /// Handles any modified textures by examining the string array to see if the array contains reference to one or more procedural texture(s).
        /// If there is a reference to a texture it means that it has been modified and the rebuild flag for the texture is set to true.
        /// </summary>
        /// <param name="_proceduralTextures"></param>
        public void HandleModifiedTextures(string[] _proceduralTextures)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: HandleModifiedTextures(" + _proceduralTextures + ")");

            foreach (string _s in _proceduralTextures)
            {
                switch (_s)
                {
                    case "Ring":
                        rebuildRingNeeded = true;
                        break;
                }
            }
        }

        public bool RebuildTexturesNeeded()
        {
            if (rebuildRingNeeded) return true;
            return false;
        }

        public void RebuildTextures(bool _force = false)
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: RebuildTextures(" + _force + ")");

            UpdateDictionariesIfNeeded();

            if (_force || rebuildRingNeeded)
            {
                UpdateProceduralTexture();
                rebuildRingNeeded = false;
            }
        }

        /// <summary>
        /// Assembles array with float values to be sent to Manager to be used during texture building.
        /// </summary>
        /// <param name="_map"></param>
        /// <returns></returns>
        PropertyFloat[] AssembleFloatArray(string _map)
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: AssembleFloatArray(" + _map + ")");

            List<PropertyFloat> _l = new List<PropertyFloat>();
            foreach (KeyValuePair<string, PropertyFloat> _p in _dictionaryFloats)
                if (_p.Value.proceduralTextures != null)
                    if (_p.Value.proceduralTextures.Contains(_map))
                        _l.Add(_p.Value);
            return _l.ToArray();
        }

        /// <summary>
        /// Gets the procedural material floats, sends them to the procedural material to update graph parameters.
        /// </summary>
        /// <param name="_map"></param>
        /// <param name="_proceduralMaterial"></param>
        protected virtual void SetProceduralMaterialFloats(string _map, Substance.Game.SubstanceGraph _proceduralMaterial)
        {
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("Ring.cs: SetProceduralMaterialFloats(" + _map + ", " + _proceduralMaterial + ")");

            foreach (KeyValuePair<string, PropertyFloat> _p in _dictionaryFloats)
            {
                if (_p.Value.proceduralTextures != null)
                {
                    if (_p.Value.proceduralTextures.Contains(_map))
                    {
                        float _value = _p.Value.value;

                        // If the PropertyFloat is set to LERP - set the value to by using the value to get an interpolated value between the min and max of the property float
                        if (_p.Value.shaderMethod == PropertyFloat.Method.LERP)
                            _value = _p.Value.GetPropertyLerp();

                        // Set the procedural material float to the value to be used when building the texture
                        _proceduralMaterial.SetInputFloat(_p.Value.shaderProperty, _value);
                    }
                }
            }
        }

        /// <summary>
        /// Updates and rebuilds procedural texture.
        /// </summary>
        public void UpdateProceduralTexture()
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: UpdateProceduralTexture()");

            UpdateDictionariesIfNeeded();
            SetProceduralMaterialFloats("Ring", proceduralMaterial);
            proceduralMaterial.QueueForRender();
            _textureRing = proceduralMaterial.GetGeneratedTextures()[0];
            _material[0].SetTexture("_MainTex", _textureRing);
            _material[1].SetTexture("_MainTex", _textureRing);
            _dictionaryMaterials["ringMaterial"].GetPropertyMaterial().QueueForRender();
        }


        /// <summary>
        /// Update properties and set them to their seeded random value
        /// </summary>
        public void UpdateProperties()
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: UpdateProperties()");

            foreach (PropertyFloat _p in propertyFloats)
                SetPropertyFloat(_p.key);
            foreach (PropertyMaterial _p in propertyMaterials)
                SetPropertyMaterial(_p.key);
        }

        /// <summary>
        /// Create ring meshes. There are two because ring is split into two sections with different sort orders to render in front and behind semitransparent planet atmosphere.
        /// </summary>
        public void CreateMeshes()
        {
            UpdateDictionariesIfNeeded();

            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: CreateMeshes()");

            // If ring meshes exist, destroy them because we'll recreate them
            if (transform.Find("MeshClose") != null) DestroyImmediate(transform.Find("MeshClose").gameObject);
            if (transform.Find("MeshFar") != null) DestroyImmediate(transform.Find("MeshFar").gameObject);

            // Create two meshes
            for (int _i = 0; _i < 2; _i++)
            {
                _ring[_i] = new GameObject();
                _ring[_i].transform.parent = transform;
                _ring[_i].transform.localPosition = Vector3.zero;
                _ring[_i].transform.localRotation = Quaternion.identity;                    

                if (_i == 0)
                {
                    // The first mesh is the close mesh and it covers 160 degrees closest to the camera
                    _mesh[_i] = ProceduralRing.Create(_dictionaryFloats["innerRadius"].GetPropertyLerp() * 6.0f, _dictionaryFloats["outerRadius"].GetPropertyLerp() * 6.0f, 200, 160f);
                    _ring[_i].name = "MeshClose";
                }
                else
                {
                    // The second mesh is the far side mesh and it covers 200 degrees on the far side of the planet
                    _mesh[_i] = ProceduralRing.Create(_dictionaryFloats["innerRadius"].GetPropertyLerp() * 6.0f, _dictionaryFloats["outerRadius"].GetPropertyLerp() * 6.0f, 200, 200f);
                    _ring[_i].name = "MeshFar";
                    _ring[_i].transform.Rotate(0.0f, 160.0f, 0.0f);
                }
                _meshFilter[_i] = _ring[_i].AddComponent<MeshFilter>();
                _meshFilter[_i].sharedMesh = _mesh[_i];
                _meshRenderer[_i] = _ring[_i].AddComponent<MeshRenderer>();

                if (_material[_i] == null)
                    CreateMaterials();

                _meshRenderer[_i].sharedMaterial = _material[_i];

            }
        }

        /// <summary>
        /// Creates new materials and sets the render queue.
        /// </summary>
        void CreateMaterials()
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: CreateMaterials()");

            _material[0] = new Material(Shader.Find("ProceduralPlanets/Ring"));
            _material[1] = new Material(Shader.Find("ProceduralPlanets/Ring"));
            _material[1].renderQueue = 2800;
            _material[0].renderQueue = 3200;            
        }



        /// <summary>
        /// Exports the ring to JSON string in different formats (easy to read / compact / "escaped" (i.e. slashes are replaced for copy/paste
        /// compatibility) and base64 encoding. The ring seed is included in the exported string but also all property values even if they
        /// are not overridden (the reason being that if the ring blueprint index hierarchy changes or updates to ProceduralPlanet asset affects the random
        /// seed, all values that used to be default seed values will instead be set to override.
        /// Public so other scripts can export ring.
        /// </summary>
        /// <param name="_format"></param>
        /// <returns></returns>
        public string ExportToJSON(StringFormat _format = StringFormat.JSON_EASY_READ)
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: ExportToJSON(" + _format + ")");

            var N = JSON.Parse("{}");
            N["ring"]["version"] = RING_VERSION;
            N["ring"]["seed"] = seed;
            foreach (PropertyFloat _p in propertyFloats)
            {
                N["ring"]["propertyFloats"][_p.key].AsFloat = _p.value;
            }
            foreach (PropertyMaterial _p in propertyMaterials)
            {
                N["ring"]["propertyMaterials"][_p.key]["index"] = _p.value;
                N["ring"]["propertyMaterials"][_p.key]["name"] = _p.materials[_p.value].name;
            }

            // Format the JSON string
            string _str;
            if (_format == StringFormat.JSON_EASY_READ)
                _str = N.ToString(2);
            else
                _str = N.ToString();
            if (_format == StringFormat.JSON_BASE64)
                _str = _str.ToBase64();
            if (_format == StringFormat.JSON_ESCAPED)
                _str = _str.EscapeString();

            // Return the JSON string
            return _str;
        }

        /// <summary>
        /// Import ring properties from JSON string (supports easy to read/compact/escaped/base64 encoded formats).
        /// Public so other scripts can call action to import ring settings.
        /// </summary>
        /// <param name="_jsonString"></param>
        /// <returns>Error message string (empty if successful)</returns>
        public string ImportFromJSON(string _jsonString)
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Ring.cs: ImportFromJSON(" + _jsonString+ ")");

            // Verify if the string is Base64 encoded, if so - decode it
            if (_jsonString.IsBase64()) _jsonString = _jsonString.FromBase64();

            // Replace escaped backslashes with real backslashes
            _jsonString = _jsonString.Replace("&quot;", "\"");

            // Initialize string for porential errors
            string _error = "";

            // Parse the JSON string
            var N = JSON.Parse(_jsonString);

            // Validate JSON to ensure it's a ring planet string and that it contains required properties.
            if (N == null)
            {
                _error = "Error: Could not parse JSON.\r\n";
                Debug.LogError(_error);
                return _error; 
            }
            if (N["ring"]["seed"] == null) _error += "Error: seed missing.\r\n";

            // Handle validation errors
            if (_error != "")
            {
                Debug.LogError(_error);
                return _error;
            }

            // Get ring blueprint reference from Manager
            BlueprintRing _bpr  = PlanetManager.Instance.GetRingBlueprintByPlanetBlueprintName(_planet.blueprint.name);

            // If index was not found the manager is not configured with a planet blueprint that matches the name used when this planet was exported.
            if (_bpr == null) return "Ring blueprint does not exist in Manager for the planet blueprint. You must create a Ring under the planet blueprint in Manager for the import to work.";

            // Set the ring seed
            seed = N["ring"]["seed"];

            // Import the float values of all float properties
            foreach (PropertyFloat _p in propertyFloats)
            {
                if (N["ring"]["propertyFloats"][_p.key] == null) return "Error: propertyFloat:" + _p.key + " missing";

                // Only import values that exist in the import JSON string
                if (N["ring"]["propertyFloats"][_p.key] != null)
                {
                    // If float property should be displayed as int...
                    if (_p.displayAsInt)
                    {
                        // Integer Values
                        // If randomly seeded value equals the value of the JSON string...
                        if ((int)GetPropertySeededFloat(_p) == N["ring"]["propertyFloats"][_p.key].AsInt)
                            // Set the property value to randomly seeded value (which is the same value as specified in the string)
                            SetPropertyFloat(_p.key);
                        else
                            // Force override of the value because the seeded random value does no longer match the value in the JSON string
                            OverridePropertyFloat(_p.key, N["ring"]["propertyFloats"][_p.key].AsInt);
                    }
                    else
                    {
                        // Float Values
                        // If the randomly seeded float value is approximately the same as the value specified in the string...
                        if (Mathf.Abs(GetPropertySeededFloat(_p) - N["ring"]["propertyFloats"][_p.key].AsFloat) < 0.0001f)
                            // Set the property value to randomly seeded value (which is the same value as specified in the string)
                            SetPropertyFloat(_p.key);
                        else
                            // Force override of the value because the seeded random value does no longer match the value in the JSON string
                            OverridePropertyFloat(_p.key, N["ring"]["propertyFloats"][_p.key].AsFloat);
                    }
                }
                    
            }

            // Import the materials of all material properties
            foreach (PropertyMaterial _p in propertyMaterials)
            {
                if (N["ring"]["propertyMaterials"][_p.key]["index"] == null) return "Error: propertyMaterials:" + _p.key + ".index missing";
                if (N["ring"]["propertyMaterials"][_p.key]["name"] == null) return "Error: propertyMaterials:" + _p.key + ".name missing";
                if (_p.GetMaterialIndexByName(N["ring"]["propertyMaterials"][_p.key]["name"]) == -1) return "Error: propertyMaterials:" + _p.key + ":name (" + N["ring"]["propertyMaterials"][_p.key]["name"] + ") does not exist in Manager list of materials.";

                // Check if material name is the same for the index saved and the index in editor now
                if (N["ring"]["propertyMaterials"][_p.key]["name"] == _p.materials[N["ring"]["propertyMaterials"][_p.key]["index"]].name)
                {
                    // Yes, indices are the same, check if material index is the same as the seeded value
                    if (N["ring"]["propertyMaterials"][_p.key]["index"] == GetPropertySeededMaterial(_p.key))
                    {
                        // Yes, it's seeded and therefore not overridden
                        SetPropertyMaterial(_p.key);
                    }
                    else
                    {
                        // No, it's not seeded so override it
                        OverridePropertyMaterial(_p.key, N["ring"]["propertyMaterials"][_p.key]["index"]);
                    }
                }
                else
                {
                    // No, indices are not the same, force override:
                    OverridePropertyMaterial(_p.key, N["ring"]["propertyMaterials"][_p.key]["index"]);
                }
            }

            // Update the serialized ring cache
            serializedRingCache = N.ToString();

            // Return empty string if import was successful.
            return "";
        }

        /// <summary>
        /// Updates the shader to take into account the properties of a local star in the scene (e.g. position, intensity, color of star).
        /// Used for lighting and shadows.
        /// </summary>
        /// <param name="_forceUpdate"></param>
        void UpdateShaderLocalStar(bool _forceUpdate)
        {
            // Ensure that a star shader setting cache exists, otherwise create one
            if (_localStarShaderCacheSettings == null)
                _localStarShaderCacheSettings = new LocalStar.ShaderCacheSettings();

            // If there is no nearest star instance...
            if (_localStarNearestInstance == null)
            {
                // Find and iterate through the local star objects in the scene
                foreach (LocalStar _ls in FindObjectsOfType<LocalStar>())
                {
                    // If there is no instance, set the first hit to be the nearest star
                    if (_localStarNearestInstance == null) _localStarNearestInstance = _ls;

                    // For subsequent local star, find the nearest one
                    if (Vector3.Distance(_localStarNearestInstance.transform.position, transform.position) < Vector3.Distance(_localStarNearestInstance.transform.position, transform.position))
                        _localStarNearestInstance = _ls;
                }
            }

            // If there are no local stars in the scene, return
            if (_localStarNearestInstance == null) return;

            // Optimize
            for (int _i = 0; _i < 2; _i++)
            {
                _material[_i].SetFloat(_shaderID_PlanetRadius, _planetRadius);
                _material[_i].SetVector(_shaderID_PlanetPosition, _planet.transform.position);
            }
                
            // Detect if if local star position is different from the cache - if so, update the shader with new settings and update the cache
            if (Vector3.Distance(_localStarShaderCacheSettings.position, _localStarNearestInstance.transform.position) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.position = _localStarNearestInstance.transform.position;                
                for (int _i = 0; _i < 2; _i++)
                    _meshRenderer[_i].sharedMaterial.SetVector(_shaderID_LocalStarPosition, _localStarNearestInstance.transform.position);                

            }

            // Detect if if local star color is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.color.r - _localStarNearestInstance.color.r) > 0.0001f ||
                Mathf.Abs(_localStarShaderCacheSettings.color.g - _localStarNearestInstance.color.g) > 0.0001f ||
                Mathf.Abs(_localStarShaderCacheSettings.color.b - _localStarNearestInstance.color.b) > 0.0001f ||
                _forceUpdate)
            {
                _localStarShaderCacheSettings.color = _localStarNearestInstance.color;
                for (int _i = 0; _i < 2; _i++)
                    _meshRenderer[_i].sharedMaterial.SetColor(_shaderID_LocalStarColor, _localStarNearestInstance.color);
            }

            // Detect if if local star intensity is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.intensity - _localStarNearestInstance.intensity) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.intensity = _localStarNearestInstance.intensity;
                for (int _i = 0; _i < 2; _i++)
                    _meshRenderer[_i].sharedMaterial.SetFloat(_shaderID_LocalStarIntensity, _localStarNearestInstance.intensity);
            }

            // Detect if if local star ambient intensity is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.ambientIntensity - _localStarNearestInstance.ambientIntensity) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.ambientIntensity = _localStarNearestInstance.ambientIntensity;
                for (int _i = 0; _i < 2; _i++)
                    _meshRenderer[_i].sharedMaterial.SetFloat(_shaderID_LocalStarAmbientIntensity, _localStarNearestInstance.ambientIntensity);
            }
        }
        
        public float GetPropertyFloatLerp(string _key)
        {
            UpdateDictionariesIfNeeded();
            if (_dictionaryFloats[_key] == null)
            {
                Debug.LogError("GetPropertyFloatLerp(" + _key + "): No key");
                return 0;
            }

            return _dictionaryFloats[_key].GetPropertyLerp();
        }

        /// <summary>
        /// Gets the baked texture - used by editor script to bake planet to prefab with PNG textures
        /// </summary>
        /// <returns>Texture2D ring texture</returns>
        public Texture2D GetBakedTexture()
        {
            Texture2D _tex2D = new Texture2D(2,2);
            RenderTexture _currentActiveRT = RenderTexture.active;
            RenderTexture _rt = new RenderTexture(_textureRing.width, _textureRing.height, 32);
            Graphics.Blit(_textureRing, _rt);
            RenderTexture.active = _rt;
            _tex2D = new Texture2D(_rt.width, _rt.height, TextureFormat.ARGB32, false);
            _tex2D.ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2D.Apply();
            RenderTexture.active = _currentActiveRT;
            DestroyImmediate(_rt);
            return _tex2D;
        }
    }
}

