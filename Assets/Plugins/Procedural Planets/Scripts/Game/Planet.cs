using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProceduralPlanets.SimpleJSON;
using Substance.Game;

namespace ProceduralPlanets
{
    /// <summary>
    /// This is the base class for planets.
    /// Solid and Gas planets derive from this class.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    public abstract class Planet : MonoBehaviour
    {
        protected const string PLANET_VERSION = "1.0";

        // Compact JSON-string that caches planet settings since planets are recreated on each play/stop
        public string serializedPlanetCache;

        // Init JSON settings
        public string initJSONSettings = "";

        // Planet Random Seed - affects all aspects of a planet
        public int planetSeed = 0;

        // Planet Variation Seed - affects sub seeds in planet generator for smaller variations of a planet
        public int variationSeed = 0;

        // Planet Blueprint
        public int planetBlueprintIndex = -1;
        public bool planetBlueprintOverride = false;
        public BlueprintPlanet blueprint = null;

        // Flag and timer to indicate that textures are being rebuilt
        protected bool _isBuildingTextures = false;
        protected float _timerStartBuildingTextures = -1f;

        protected List<PropertyFloatAnimation.Animation> _propertyFloatAnimations = new List<PropertyFloatAnimation.Animation>();

        protected List<SubstanceGraph> _shaderGraphsUsed = new List<SubstanceGraph>();

        // Callbacks
        protected List<GameObject> _eventListeners = new List<GameObject>();
        public virtual void AddListener(GameObject _gameObject)
        {
            _eventListeners.Add(_gameObject);
        }

        protected virtual void TriggerOnTextureBuildComplete(float _timeTaken)
        {
            foreach (GameObject _g in _eventListeners)
                if (_g != null)
                  _g.SendMessage("OnTextureBuildComplete", _timeTaken, SendMessageOptions.DontRequireReceiver);
        }
        protected virtual void TriggerOnTextureBuildStart(float _timeStarted)
        {
            foreach (GameObject _g in _eventListeners)
                if (_g != null)
                    _g.SendMessage("OnTextureBuildStart", _timeStarted, SendMessageOptions.DontRequireReceiver);
        }

        // Meshes and renderers
        public int meshLODLevel;
        public MeshFilter meshFilter;
        protected MeshRenderer _meshRenderer;

        public int textureLODLevel = -1;
        public int textureProgressiveStep = -1;

        // External Atmosphere
        protected GameObject _externalAtmosphere;
        protected MeshFilter _externalAtmosphereMeshFilter;
        protected MeshRenderer _externalAtmosphereRenderer;

        // Integer IDs of shader properties for performance
        protected int _shaderID_LocalStarPosition;
        protected int _shaderID_LocalStarColor;
        protected int _shaderID_LocalStarIntensity;
        protected int _shaderID_LocalStarAmbientIntensity;

        // Local Star
        protected LocalStar.ShaderCacheSettings _localStarShaderCacheSettings;
        protected LocalStar _localStarNearestInstance;

        // Float, Color and Material lists and dictionaries. 
        public List<PropertyFloat> propertyFloats = new List<PropertyFloat>(0);
        public List<PropertyColor> propertyColors = new List<PropertyColor>(0);
        public List<PropertyMaterial> propertyMaterials = new List<PropertyMaterial>(0);
        // Since dictionaries cannot be serialized, lists are used for serialization for the editor script and dictionaries are synced for
        // dictionary lookup purposes.
        protected Dictionary<string, PropertyFloat> _dictionaryFloats = new Dictionary<string, PropertyFloat>(0);
        protected Dictionary<string, PropertyColor> _dictionaryColors = new Dictionary<string, PropertyColor>(0);
        protected Dictionary<string, PropertyMaterial> _dictionaryMaterials = new Dictionary<string, PropertyMaterial>(0);

        public abstract bool IsBuildingTextures { get; }

        // Used to keep track of last position to update local star shader light direction if planet has moved.
        protected Vector3 _lastPosition;


        // ABSTRACT METHODS
        // These are required in derived classes and must be exist as overridden Methods

        // Require all derived classes to have a SetPlanetBlueprint Method
        public abstract void SetPlanetBlueprint(int _index = -1, bool _leaveOverride = false, bool _setProperties = true);

        // Require all derived classes to have a GetPlanetBlueprintSeededIndex Method
        protected abstract int GetPlanetBlueprintSeededIndex();

        // Require all derived classes to have Awake where planet is recreated on every start/stop cycle
        protected abstract void Awake();

        // Updates procedural textures
        public abstract void UpdateProceduralTexture(string _textureName);

        /// <summary>
        /// Handles any modified textures by examining the string array to see if the array contains reference to one or more procedural texture(s).
        /// If there is a reference to a texture it means that it has been modified and the rebuild flag for the texture is set to true.
        /// </summary>
        /// <param name="_proceduralTextures"></param>
        protected abstract void HandleModifiedTextures(string[] _proceduralTextures);

        // VIRTUAL METHODS
        // These can be used by the derived classes or overridden if necessary

        /// <summary>
        /// Assembles array with float values to be sent to Manager to be used during texture building.
        /// </summary>
        /// <param name="_map"></param>
        /// <returns></returns>
        protected virtual PropertyFloat[] AssembleFloatArray(string _map)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: AssembleFloatArray(" + _map + ")");

            List<PropertyFloat> _l = new List<PropertyFloat>();
            foreach (KeyValuePair<string, PropertyFloat> _p in _dictionaryFloats)
                if (_p.Value.proceduralTextures != null)
                    if (_p.Value.proceduralTextures.Contains(_map))
                        _l.Add(_p.Value);
            return _l.ToArray();
        }

        protected virtual void SetProceduralMaterialFloats(string _map, SubstanceGraph _proceduralMaterial)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: SetProceduralMaterialFloats(" + _map + ", " + _proceduralMaterial + ")");
            
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
        /// Sets the default properties based on the seed. Optioanally leave or overwrite overridden properties.
        /// </summary>
        /// <param name="_leaveOverride"></param>
        protected virtual void SetDefaultProperties(bool _leaveOverride = false)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: SetDefaultProperties(" + _leaveOverride +")");

            foreach (PropertyFloat _p in propertyFloats)
                if (!_leaveOverride || (_leaveOverride && !_p.overrideRandom))
                    SetPropertyFloat(_p.key);
            foreach (PropertyColor _p in propertyColors)
                if (!_leaveOverride || (_leaveOverride && !_p.overrideRandom))
                    SetPropertyColor(_p.key);
            foreach (PropertyMaterial _p in propertyMaterials)
                if (!_leaveOverride || (_leaveOverride && !_p.overrideRandom))
                    SetPropertyMaterial(_p.key);
        }

        /// <summary>
        /// Sets a property float to the value determined by the random seed.
        /// Public because editor script calls this method.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>The float value that was set by the random seed</returns>
        public virtual float SetPropertyFloat(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: SetPropertyFloat(" + _key + ")");

            // Ensure that the dictionary is updated, otherwise it may not contain the key
            UpdateDictionariesIfNeeded();

            // Get the reference to the float property from the dictionary
            PropertyFloat _p = _dictionaryFloats[_key];

            // Set the seed to the planetSeed and unique seed offset for this property
            int _seed = planetSeed + _p.seedOffset;

            // If this particular property is affected by the variation seed, add the variation seed value
            if (_p.variation) _seed += variationSeed;

            // Set the property to a value based on the seed between the Minimum and Maximum value of the property for this particular configuration template
            _p.SetPropertyFloat(_seed, blueprint.GetMin(_key), blueprint.GetMax(_key));

            // If the value is integer, round it by casting it to int
            if (_p.displayAsInt) _p.value = (int)_p.value;

            // If this property affects any procedural textures, ensure to handle them so they get updated
            if (_p.proceduralTextures != null) HandleModifiedTextures(_p.proceduralTextures);

            // If this is not property that affects procedural textures it must affect the shader, so update the shader
            if (_p.materials != null && _p.shaderProperty != null) UpdatePropertyFloatShader(_p);

            // Return the value that was just determined and set by the random seed
            return _p.value;
        }

        /// <summary>
        /// Updates the planet shader if affected by the PropertyFloat.
        /// </summary>
        /// <param name="_propertyFloat"></param>
        protected virtual void UpdatePropertyFloatShader(PropertyFloat _propertyFloat)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: UpdatePropertyFloatShader(" + _propertyFloat + ")");

            // If this PropertyFloat has no affected material or the shaderProperty is set to null, return
            if (_propertyFloat.materials == null || _propertyFloat.shaderProperty == null) return;
            
            // Iterate through materials that this PropertyFloat affects
            foreach (Material _m in _propertyFloat.materials)
            {
                // Ensure the material is not null
                if (_m != null)
                {
                    if (_propertyFloat.shaderDataType == PropertyFloat.DataType.FLOAT)
                    {
                        // If data type is a float, set the value or interpolated value between min/max 
                        if (_propertyFloat.shaderMethod == PropertyFloat.Method.VALUE)
                            _m.SetFloat(_propertyFloat.shaderProperty, _propertyFloat.value);
                        if (_propertyFloat.shaderMethod == PropertyFloat.Method.LERP)
                            _m.SetFloat(_propertyFloat.shaderProperty, _propertyFloat.GetPropertyLerp());
                    }
                    if (_propertyFloat.shaderDataType == PropertyFloat.DataType.INT)
                    {
                        // If data type is n int, set the value or interpolated value between min/max 
                        if (_propertyFloat.shaderMethod == PropertyFloat.Method.VALUE)
                            _m.SetInt(_propertyFloat.shaderProperty, (int)_propertyFloat.value);
                        if (_propertyFloat.shaderMethod == PropertyFloat.Method.LERP)
                            _m.SetInt(_propertyFloat.shaderProperty, (int)_propertyFloat.GetPropertyLerp());
                    }
                }
            }
        }

        /// <summary>
        /// Sets a property color to the color determined by the random seed.
        /// Public because editor script calls this method.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Color determined by random seed</returns>
        public virtual Color SetPropertyColor(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: SetPropertyColor" + _key + ")");

            // Ensure that the dictionary is updated, otherwise it may not contain the key
            UpdateDictionariesIfNeeded();

            // Get the reference to the color property from the dictionary
            PropertyColor _p = _dictionaryColors[_key];

            // Set the seed to the planetSeed and unique seed offset for this property
            int _seed = planetSeed + _p.seedOffset;

            // Set the property to a color based on the seed and hue/saturation/brightness ranges of the property for this particular configuration template
            Color _color = _p.SetPropertyColor(_seed, blueprint.GetColor(_p.key), blueprint.GetHueRange(_p.key), blueprint.GetSaturationRange(_p.key), blueprint.GetBrightnessRange(_p.key));

            // If this is not property that affects procedural textures it must affect the shader, so update the shader
            if (_p.materials != null && _p.shaderProperty != null) UpdatePropertyColorShader(_p);

            // Return the color that was just determined and set by the random seed
            return _color;
        }

        /// <summary>
        /// Updates the planet shader if affected by the PropertyColor
        /// </summary>
        /// <param name="_propertyColor"></param>
        protected virtual void UpdatePropertyColorShader(PropertyColor _propertyColor)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: UpdatePropertyColorShader(" + _propertyColor + ")");

            // If this PropertyColor has no affected material or the shaderProperty is set to null, return
            if (_propertyColor.materials == null || _propertyColor.shaderProperty == null) return;

            // Iterate through materials that this property affects
            foreach (Material _m in _propertyColor.materials)
            {
                // Ensure the material is not null and set the color in the shader
                if (_m != null)
                    _m.SetColor(_propertyColor.shaderProperty, _propertyColor.color);
            }
        }

        /// <summary>
        /// Sets a property material to the material determined by the random seed.
        /// Public because editor script calls this method.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Integer index of material</returns>
        public virtual int SetPropertyMaterial(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: SetPropertyMaterial(" + _key + ")");

            // Ensure that the dictionary is updated, otherwise it may not contain the key
            UpdateDictionariesIfNeeded();

            // Get the reference to the material property from the dictionary
            PropertyMaterial _p = _dictionaryMaterials[_key];

            // Set the seed to the planetSeed and unique seed offset for this property
            int _seed = planetSeed + _p.seedOffset;
           
            // Set the property to a material index based on the seed between the material mask of the property for this particular configuration template
            int _value = _p.SetPropertyMaterial(_seed, blueprint.GetMaterialLength(_key), blueprint.GetMaterialMask(_key));

            // If this property affects any procedural textures, ensure to handle them so they get updated
            if (_p.proceduralTextures != null) HandleModifiedTextures(_p.proceduralTextures);

            // Return the integer index of the material determined by the random seed
            return _value;
        }

        /// <summary>
        /// Updates the dictionaries to contains the Float,Color,Material values of the property lists.
        /// Unity does not support serialized dictionaries so this component need to keep values in the dictionary for the game
        /// and in the list for serialization for the custom inspector in the editor script.
        /// </summary>
        /// <param name="_force"></param>
        protected virtual void UpdateDictionariesIfNeeded(bool _force = false)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: UpdateDictionariesIfNeeded(" + _force + ")");

            // If force update is not set and dictionary already contains values - return
            if (!_force &&
                _dictionaryFloats.Count > 0 &&
                _dictionaryColors.Count > 0 &&
                _dictionaryMaterials.Count > 0) return;

            // Clear the Float dictionary and populate it from the list of min max properties
            _dictionaryFloats.Clear();
            foreach (PropertyFloat _p in propertyFloats)
                _dictionaryFloats.Add(_p.key, _p);

            // Clear the Color dictionary and populate it from the list of color properties
            _dictionaryColors.Clear();
            foreach (PropertyColor _p in propertyColors)
                _dictionaryColors.Add(_p.key, _p);

            // Clear the Material dictionary and populate it from the list of material properties
            _dictionaryMaterials.Clear();
            foreach (PropertyMaterial _p in propertyMaterials)
                _dictionaryMaterials.Add(_p.key, _p);
        }
        /// <summary>
        /// Generates lookup textures with a sharp transition.
        /// </summary>
        /// <param name="_level"></param>
        /// <param name="_detail"></param>
        /// <returns>Texture2D</returns>
        protected virtual Texture2D GenerateLookupTexture(float _level, int _detail = 1024)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GenerateLookupTexture(" + _level + "," + _detail + ")");

            Texture2D _texture = new Texture2D(_detail, 4);
            Color32[] _col32 = new Color32[_detail * 4];
            for (int _x = 0; _x < _detail; _x++)
            {
                if (_x < (int)(_level * _detail))
                {
                    _col32[_x] = new Color32(0, 0, 0, 255);
                    _col32[_x + _detail] = new Color32(0, 0, 0, 255);
                    _col32[_x + _detail * 2] = new Color32(0, 0, 0, 255);
                    _col32[_x + _detail * 3] = new Color32(0, 0, 0, 255);
                }
                else {
                    _col32[_x] = new Color32(255, 255, 255, 255);
                    _col32[_x + _detail] = new Color32(255, 255, 255, 255);
                    _col32[_x + _detail * 2] = new Color32(255, 255, 255, 255);
                    _col32[_x + _detail * 3] = new Color32(255, 255, 255, 255);
                }
            }
            _texture.SetPixels32(_col32);
            _texture.wrapMode = TextureWrapMode.Clamp;
            _texture.Apply();
            return _texture;
        }

        /// <summary>
        /// Genereates smooth lookup textures with a gradient transition between lowLimit and upLimit.
        /// </summary>
        /// <param name="_level"></param>
        /// <param name="_lowLimit"></param>
        /// <param name="_upLimit"></param>
        /// <returns>Texture2D</returns>
        protected virtual Texture2D GenerateLookupSmoothTexture(float _level, float _lowLimit = -1.0f, float _upLimit = -1.0f)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GenerateSmoothLookupTexture(" + _level + "," + _lowLimit + "," + _upLimit + ")");

            Texture2D _texture = new Texture2D(256, 4);
            Color32[] _col32 = new Color32[256 * 4];
            for (int _x = 0; _x < 256; _x++)
            {
                if (_lowLimit < 0 && _upLimit < 0)
                {
                    if (_x < (int)(_level * 256.0f))
                    {
                        _col32[_x] = new Color32(0, 0, 0, 255);
                        _col32[_x + 256] = new Color32(0, 0, 0, 255);
                        _col32[_x + 256*2] = new Color32(0, 0, 0, 255);
                        _col32[_x + 256*3] = new Color32(0, 0, 0, 255);
                    }
                    else {
                        _col32[_x] = new Color32(255, 255, 255, 255);
                    }
                }
                else {
                    float _val = (float)_x / 256.0f;
                    float _low = _level - _lowLimit;
                    float _high = _level + _upLimit;

                    if (_val <= _low)
                    {
                        _col32[_x] = new Color32(0, 0, 0, 255);
                        _col32[_x + 256] = new Color32(0, 0, 0, 255);
                        _col32[_x + 256 * 2] = new Color32(0, 0, 0, 255);
                        _col32[_x + 256 * 3] = new Color32(0, 0, 0, 255);
                    }
                    if (_val >= _high)
                    {
                        _col32[_x] = new Color32(255, 255, 255, 255);
                        _col32[_x + 256] = new Color32(255, 255, 255, 255);
                        _col32[_x + 256 * 2] = new Color32(255, 255, 255, 255);
                        _col32[_x + 256 * 3] = new Color32(255, 255, 255, 255);
                    }
                    if (_val > _low && _val < _high)
                    {
                        _val = Mathf.Lerp(0.0f, 1.0f, (_val - _low) / (_high - _low));
                        byte _byte = (byte)(_val * 255.0f);
                        _col32[_x] = new Color32(_byte, _byte, _byte, 255);
                        _col32[_x + 256] = new Color32(_byte, _byte, _byte, 255);
                        _col32[_x + 256 * 2] = new Color32(_byte, _byte, _byte, 255);
                        _col32[_x + 256 * 3] = new Color32(_byte, _byte, _byte, 255);
                    }
                }
            }
            _texture.SetPixels32(_col32);
            _texture.wrapMode = TextureWrapMode.Clamp;
            _texture.Apply();
            return _texture;
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
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: AddPropertyFloat(" + _key + "," + _label + "," + _minValue + "," + _maxValue + "," + _clamp01 + "," + _displayAsInt + "," + _seedOffset + "," + _variation + "," + _proceduralTextures + "," + _materials + "," + _shaderProperty + "," + _method + "," + _dataType + ")");

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
        /// <param name="_substanceGraphs"></param>
        /// <param name="_shaderProperty"></param>
        /// <param name="_method"></param>
        /// <param name="_dataType"></param>
        protected virtual void AddPropertyFloat(string _key, string _label, float _minValue, float _maxValue, bool _clamp01, bool _displayAsInt, int _seedOffset, bool _variation, string[] _proceduralTextures, Substance.Game.SubstanceGraph _substanceGraphs, string _shaderProperty = null, PropertyFloat.Method _method = PropertyFloat.Method.VALUE, PropertyFloat.DataType _dataType = PropertyFloat.DataType.FLOAT)
        {
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: AddPropertyFloat(" + _key + "," + _label + "," + _minValue + "," + _maxValue + "," + _clamp01 + "," + _displayAsInt + "," + _seedOffset + "," + _variation + "," + _proceduralTextures + "," + _substanceGraphs + "," + _shaderProperty + "," + _method + "," + _dataType + ")");

            AddPropertyFloat(_key, _label, _minValue, _maxValue, _clamp01, _displayAsInt, _seedOffset, _variation, _proceduralTextures, new Substance.Game.SubstanceGraph[] { _substanceGraphs }, _shaderProperty, _method, _dataType);
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
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: AddPropertyFloat(" + _key + "," + _label + "," + _minValue + "," + _maxValue + "," + _clamp01 + "," + _displayAsInt + "," + _seedOffset + "," + _variation + "," + _proceduralTextures + "," + _materials + "," + _shaderProperty + "," + _method + "," + _dataType + ")");

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
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: AddPropertyFloat(" + _key + "," + _label + "," + _minValue + "," + _maxValue + "," + _clamp01 + "," + _displayAsInt + "," + _seedOffset + "," + _variation + "," + _proceduralTextures + "," + _substanceGraphs+ "," + _shaderProperty + "," + _method + "," + _dataType + ")");

            PropertyFloat _p = new PropertyFloat(_key, _label, _minValue, _maxValue, _clamp01, _displayAsInt, _seedOffset, _variation, _proceduralTextures, _substanceGraphs, _shaderProperty, _method, _dataType);
            propertyFloats.Add(_p);
        }

        /// <summary>
        /// Adds a Color property that affects a single Procedural Material.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_baseColor"></param>
        /// <param name="_hueRange"></param>
        /// <param name="_saturationRange"></param>
        /// <param name="_brightnessRange"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_variation"></param>
        /// <param name="_material"></param>
        /// <param name="_shaderProperty"></param>
        protected virtual void AddPropertyColor(string _key, string _label, Color _baseColor, float _hueRange, float _saturationRange, float _brightnessRange, int _seedOffset, bool _variation, Material _material = null, string _shaderProperty = null)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: AddPropertyColor(" + _key + "," + _label + "," + _baseColor + "," + _hueRange + "," + _saturationRange + "," + _brightnessRange + "," + _seedOffset + "," + _variation + "," + _material + "," + _shaderProperty + ")");

            AddPropertyColor(_key, _label, _baseColor, _hueRange, _saturationRange, _brightnessRange, _seedOffset, _variation, new Material[] { _material }, _shaderProperty);
        }

        /// <summary>
        /// Adds a Color property that affects multiple Procedural Materials.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_baseColor"></param>
        /// <param name="_hueRange"></param>
        /// <param name="_saturationRange"></param>
        /// <param name="_brightnessRange"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_variation"></param>
        /// <param name="_materials"></param>
        /// <param name="_shaderProperty"></param>
        protected virtual void AddPropertyColor(string _key, string _label, Color _baseColor, float _hueRange, float _saturationRange, float _brightnessRange, int _seedOffset, bool _variation, Material[] _materials = null, string _shaderProperty = null)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: AddPropertyColor(" + _key + "," + _label + "," + _baseColor + "," + _hueRange + "," + _saturationRange + "," + _brightnessRange + "," + _seedOffset + "," + _variation + "," + _materials + "," + _shaderProperty + ")");

            PropertyColor _p = new PropertyColor(_key, _label, _baseColor, _hueRange, _saturationRange, _brightnessRange, _seedOffset, _variation, _materials, _shaderProperty);
            propertyColors.Add(_p);
        }

        /// <summary>
        /// Adds a Material property that affects a single Procedural Material.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_materials"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_proceduralTextures"></param>
        /// <param name="_material"></param>
        /// <param name="_shaderProperty"></param>
        protected virtual void AddPropertyMaterial(string _key, string _label, Substance.Game.SubstanceGraph[] _materials, int _seedOffset, string[] _proceduralTextures = null, Material _material = null, string _shaderProperty = null)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: AddPropertyMaterial(" + _key + "," + _label + "," + _materials + "," + _seedOffset + "," + _proceduralTextures + "," + _material + "," + _shaderProperty + ")");

            PropertyMaterial _p = new PropertyMaterial(_key, _label, _materials, _seedOffset, _proceduralTextures, _material, _shaderProperty);
            propertyMaterials.Add(_p);
        }

        /// <summary>
        /// Gets the property value as determined by the random seed using the key as argument. 
        /// See GetPropertySeededFloat(PropertyFloat _p) method for details.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Float value determined by the random seed</returns>
        protected virtual float GetPropertySeededFloat(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GetPropertySeededFloat(" + _key + ")");

            // Get the reference to the float property.
            PropertyFloat _p = _dictionaryFloats[_key];

            // Return the seeded value for the property. See GetPropertySeededValue(PropertyFloat _p) method for details.
            return GetPropertySeededFloat(_p);
        }

        /// <summary>
        /// Gets the property value as determined by the random seed using the PropertyFloat as argument.
        /// </summary>
        /// <param name="_propertyFloat"></param>
        /// <returns>Float value determined by the random seed</returns>
        protected virtual float GetPropertySeededFloat(PropertyFloat _propertyFloat)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GetPropertySeededFloat(" + _propertyFloat + ")");

            // Calculate the seed based on the planet seed and the seed offset
            int _seed = planetSeed + _propertyFloat.seedOffset;

            // If property is affected by the variation seed, take it into account
            if (_propertyFloat.variation) _seed += variationSeed;

            // Get the seeded random value within the range specified for the planet blueprint
            float _value = _propertyFloat.SeededRandomFloat(_seed, blueprint.GetMin(_propertyFloat.key), blueprint.GetMax(_propertyFloat.key));

            // Return the value
            return _value;
        }

        /// <summary>
        /// Overrides a property value. By default this updates the serialized cache for the planet.
        /// Public any script should be able to override a property of the planet (e.g. manually set or animate liquid level etc.)
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        /// <param name="_updateSerializedPlanetCache"></param>
        public virtual void OverridePropertyFloat(string _key, float _value, bool _updateSerializedPlanetCache = true)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: OverridePropertyFloat(" + _key + "," + _value + "," + _updateSerializedPlanetCache + ")");

            // Get the reference to the PropertyFloat
            PropertyFloat _p = _dictionaryFloats[_key];

            // Override the property value with specified value
            _p.OverridePropertyFloat(_value);

            // If this is an int value, cast to int to round off
            if (_p.displayAsInt) _p.value = (int)_p.value;

            // If this property affects any procedural materials, handle (and rebuild) the affected texture(s)
            if (_p.proceduralTextures != null) HandleModifiedTextures(_p.proceduralTextures);

            // If this is not property that affects procedural textures it must affect the shader, so update the shader
            if (_p.materials != null && _p.shaderProperty != null) UpdatePropertyFloatShader(_p);

            // Update the serialized planet cache (needed since the planets are recreated on each play/stop event)
            if (_updateSerializedPlanetCache) serializedPlanetCache = ExportToJSON(StringFormat.JSON_COMPACT);
        }

        /// <summary>
        /// Answers if property float is overridden based on key (returns true/false)
        /// Public because any script should be able to find out if a property of the planet is overridden
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>true/false if property is overridden</returns>
        public virtual bool IsPropertyFloatOverridden(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: IsPropertyFloatOverridden(" + _key + ")");

            PropertyFloat _p = _dictionaryFloats[_key];
            return _p.overrideRandom;
        }

        /// <summary>
        /// Gets the float value based on key (returns current value regardless whether it is determined by seed or overridden)
        /// Public because any script should be able to find out value of a property of the planet
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Float value of property</returns>
        public virtual float GetPropertyFloat(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GetPropertyFloat(" + _key + ")");

            if (!_dictionaryFloats.ContainsKey(_key))
            {
                Debug.LogError($"The property float key '{_key}' did not exist in the dictionary. The dictionary contains {_dictionaryFloats.Keys.Count} keys and {_dictionaryFloats.Values.Count} values. ");
            }

            PropertyFloat _p = _dictionaryFloats[_key];
            return _p.value;
        }


        /// <summary>
        /// Gets the property color as determined by the random seed using the key as argument. 
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Color determined by the random seed</returns>
        protected virtual Color GetPropertySeededColor(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GetPropertySeededColor(" + _key + ")");

            // Get the reference to the color property.
            PropertyColor _p = _dictionaryColors[_key];

            // Calculate the seed based on the planet seed and the seed offset
            int _seed = planetSeed + _p.seedOffset;

            // Get the seeded random color within the hue/saturation/brightness ranges specified for the planet blueprint
            Color _color = _p.GetPropertySeededColor(_seed, blueprint.GetColor(_p.key), blueprint.GetHueRange(_p.key), blueprint.GetSaturationRange(_p.key), blueprint.GetBrightnessRange(_p.key));

            // Return the color
            return _color;

        }

        /// <summary>
        /// Overrides a property color. By default this updates the serialized cache for the planet.
        /// Public because any script should be able to override a property of the planet (e.g. manually set or animate liquid level etc.)
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_color"></param>
        /// <param name="_updateSerializedPlanetCache"></param>
        public virtual void OverridePropertyColor(string _key, Color _color, bool _updateSerializedPlanetCache = true)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: OverridePropertyColor(" + _key + "," + _color + "," + _updateSerializedPlanetCache + ")");

            // Get the reference to the PropertyColor
            PropertyColor _p = _dictionaryColors[_key];

            // Override the property color with specified color
            _p.OverridePropertyColor(_color);

            // If this is not property that affects procedural textures it must affect the shader, so update the shader            
            if (_p.materials != null && _p.shaderProperty != null) UpdatePropertyColorShader(_p);

            // Update the serialized planet cache (needed since the planets are recreated on each play/stop event)
            if (_updateSerializedPlanetCache) serializedPlanetCache = ExportToJSON(StringFormat.JSON_COMPACT);
        }

        /// <summary>
        /// Answers if property color is overridden based on key (returns true/false)
        /// Public any script should be able to find out if a property of the planet is overridden
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public virtual bool IsPropertyColorOverridden(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: IsPropertyColorOverridden(" + _key + ")");

            PropertyColor _p = _dictionaryColors[_key];
            return _p.overrideRandom;
        }

        /// <summary>
        /// Gets the color value based on key (returns current value regardless whether it is determined by seed or overridden)
        /// Public because any script should be able to find out color of a property of the planet
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Color of the property</returns>
        public virtual Color GetPropertyColor(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GetPropertyColor(" + _key + ")");

            PropertyColor _p = _dictionaryColors[_key];
            return _p.color;
        }


        /// <summary>
        /// Gets the property material (integer index) as determined by the random seed using the key as argument. 
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Integer index of material</returns>
        protected virtual int GetPropertySeededMaterial(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GetPropertySeededMaterial(" + _key + ")");

            // Get the reference to the material property.
            PropertyMaterial _p = _dictionaryMaterials[_key];

            // Calculate the seed based on the planet seed and the seed offset
            int _seed = planetSeed + _p.seedOffset;

            // Get the seeded random material index value within the material mask specified for the planet blueprint
            return _p.GetPropertySeededMaterialValue(_seed, blueprint.GetMaterialLength(_key), blueprint.GetMaterialMask(_key));
        }

        /// <summary>
        /// Overrides a property material. By default this updates the serialized cache for the planet.
        /// Public because any script should be able to override a property of the planet (e.g. manually set or animate liquid level etc.)
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        /// <param name="_updateSerializedPlanetCache"></param>
        public virtual void OverridePropertyMaterial(string _key, int _value, bool _updateSerializedPlanetCache = true)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: OverridePropertyMaterial(" + _key + "," + _value + "," + _updateSerializedPlanetCache + ")");

            // Get the reference to the PropertyMaterial
            PropertyMaterial _p = _dictionaryMaterials[_key];

            // Override the property material with specified material index value
            _p.OverridePropertyMaterial(_value);

            // If this property affects any procedural materials, handle (and rebuild) the affected texture(s)
            if (_p.proceduralTextures != null) HandleModifiedTextures(_p.proceduralTextures);

            // Update the serialized planet cache (needed since the planets are recreated on each play/stop event)
            if (_updateSerializedPlanetCache) serializedPlanetCache = ExportToJSON(StringFormat.JSON_COMPACT);
        }

        /// <summary>
        /// Answers if property material is overridden based on key (returns true/false)
        /// Public any script should be able to find out if a property of the planet is overridden
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>true/false if property is overridden</returns>
        public virtual bool IsPropertyMaterialOverridden(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: IsPropertyMaterialOverridden(" + _key + ")");

            PropertyMaterial _p = _dictionaryMaterials[_key];
            return _p.overrideRandom;
        }

        /// <summary>
        /// Gets the material index integer value based on key (returns current value regardless whether it is determined by seed or overridden)
        /// Public because any script should be able to find out color of a property of the planet
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public virtual int GetPropertyMaterial(string _key)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: GetPropertyMaterial(" + _key + ")");

            PropertyMaterial _p = _dictionaryMaterials[_key];
            return _p.value;
        }

        /// <summary>
        /// Creates a ring using based on blueprint random values or using optional jSON string.
        /// </summary>
        /// <param name="_jsonString"></param>
        public virtual void CreateRing(string _jsonString = null)
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Planet.cs: CreateRing(" + _jsonString + ")");

            // Verify if blueprint has planetary ring blueprint
            BlueprintRing _blueprintRing = null;
            if (blueprint.transform.Find("Ring") != null)
            {
                _blueprintRing = blueprint.transform.Find("Ring").GetComponent<BlueprintRing>();
            }
            else
            {
                //Planet doesn't have ring blueprint specified - use generic ring blueprint
                _blueprintRing = PlanetManager.Instance.GetRingBlueprintByPlanetBlueprintName(PlanetManager.GENERIC_PLANET_BLUEPRINT_NAME);
            }

            if (_blueprintRing != null)
            {
                // Create the new ring gameobject
                GameObject _ringGameObject = new GameObject();

                // Deactivate the gameobject to prevent Awake() initialization to be called prior to planet having an assigned blueprint
                _ringGameObject.SetActive(false);

                // Set the parent of the ring transform to planet transform
                _ringGameObject.transform.parent = transform;

                // Give the ring gameobject the name "Ring"
                _ringGameObject.name = "Ring";

                // Reset the local position to zero to ensure it's in the same center location as the planet
                _ringGameObject.transform.localPosition = Vector3.zero;

                // Add the Ring component class
                Ring _ring = _ringGameObject.AddComponent<Ring>();

                // Set the blueprint of the ring and give it a random seed
                //_ring.SetBlueprintRing(_blueprintRing, Random.Range(0, 65535), false);
                _ring.SetBlueprintRing(_blueprintRing, 0, false);

                // Set the serialized ring cache (if any)
                _ring.serializedRingCache = _jsonString;

                // Set the ring gameobject to active to execute Awake() method which will set configure ring according to seed/blueprint/json-string
                _ringGameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Answers the question whether or not the planet has a planetary ring
        /// </summary>
        /// <returns>True/False</returns>
        public virtual bool HasRing()
        {
            if ((int) PlanetManager.DebugLevel > 2) Debug.Log("Planet.cs: HasRing()");

            if (transform.Find("Ring") == null) return false;
            if (transform.Find("Ring").GetComponent<Ring>() == null) return false;
            return true;
        }

        /// <summary>
        /// Gets the ring component of a planetary ring of this planet.
        /// </summary>
        /// <returns></returns>
        public virtual Ring GetRing()
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Planet.cs: GetRing()");

            if (HasRing())
                return transform.Find("Ring").GetComponent<Ring>();
            return null;
        }

        /// <summary>
        /// Destroy ring if planet has one (child object must be named "Ring")
        /// </summary>
        public virtual void DestroyRing()
        {
            if (!HasRing())
                return;

#if UNITY_EDITOR
            DestroyImmediate(transform.Find("Ring").gameObject);
#endif
#if !UNITY_EDITOR
            Destroy(transform.Find("Ring").gameObject);
#endif

        }

        /// <summary>
        /// Exports the planet to JSON string in different formats (easy to read / compact / "escaped" (i.e. slashes are replaced for copy/paste
        /// compatibility) and base64 encoding. The planet and variation seed is included in the exported string but also all property values even if they
        /// are not overridden (the reason being that if the planet blueprint index hierarchy changes or updates to ProceduralPlanet asset affects the random
        /// seed, all values that used to be default seed values will instead be set to override.
        /// Public so other scripts can export planet.
        /// </summary>
        /// <param name="_format"></param>
        /// <returns></returns>
        public virtual string ExportToJSON(StringFormat _format = StringFormat.JSON_EASY_READ)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: ExportToJSON(" + _format + ")");

            if (blueprint == null) return "";

            // Construct the JSON string and include all planet properties
            var N = JSON.Parse("{}");

            // Set all the base JSON properties
            N["category"] = "planet";
            N["type"] = N["type"] = GetType().Name;
            N["version"] = PLANET_VERSION;
            N["planet"]["planetSeed"] = planetSeed;
            N["planet"]["variationSeed"] = variationSeed;
            N["planet"]["blueprintIndex"] = planetBlueprintIndex;
            N["planet"]["blueprintName"] = blueprint.name;

            // Iterate and add all property floats, materials, and colors to the JSON string.
            foreach (PropertyFloat _p in propertyFloats)
            {
                N["planet"]["propertyFloats"][_p.key].AsFloat = _p.value;
            }
            foreach (PropertyMaterial _p in propertyMaterials)
            {
                N["planet"]["propertyMaterials"][_p.key]["index"] = _p.value;
                N["planet"]["propertyMaterials"][_p.key]["name"] = _p.materials[_p.value].name;
            }
            foreach (PropertyColor _p in propertyColors)
            {
                N["planet"]["propertyColors"][_p.key]["r"] = _p.color.r;
                N["planet"]["propertyColors"][_p.key]["g"] = _p.color.g;
                N["planet"]["propertyColors"][_p.key]["b"] = _p.color.b;
            }

            // Format the JSON string
            string _str;

            if (_format == StringFormat.JSON_EASY_READ)
                _str = N.ToString(2);
            else
                _str = N.ToString();

            _str = _str.Substring(0, _str.Length - 1);

            // If a ring exists, export the ring and add it to the JSON string
            if (HasRing())
            {
                string _ring = GetRing().ExportToJSON((_format == StringFormat.JSON_EASY_READ) ? Ring.StringFormat.JSON_EASY_READ : Ring.StringFormat.JSON_COMPACT);
                _ring = _ring.Substring(1, _ring.Length - 2);
                _str += "," + _ring;
            }

            // Close the string properly
            _str = (_str + "}").Replace("\r\n,", ",");

            // Encode or Escape the string if required
            if (_format == StringFormat.JSON_BASE64)
                _str = _str.ToBase64();
            if (_format == StringFormat.JSON_ESCAPED)
                _str = _str.EscapeString();
                      
            // Return the JSON string
            return _str;
        }

        /// <summary>
        /// Import planet properties from JSON string (supports easy to read/compact/escaped/base64 encoded formats).
        /// Public so other scripts can call action to import planet settings.
        /// </summary>
        /// <param name="_jsonString"></param>
        /// <returns>Error message string (empty if successful)</returns>
        public virtual string ImportFromJSON(string _jsonString, bool _createRing = false)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("Planet.cs: ImportFromJSON(" + _jsonString + "," + _createRing + ")");

            // Verify if the string is Base64 encoded, if so - decode it
            if (_jsonString.IsBase64()) _jsonString = _jsonString.FromBase64();

            // Replace escaped backslashes with real backslashes
            _jsonString = _jsonString.Replace("&quot;", "\"");

            // Initialize string for porential errors
            string _error = "";

            // Parse the JSON string
            var N = JSON.Parse(_jsonString);

            // Validate JSON to ensure it's a planet string and that it contains required properties.
            if (N == null)
            {
                _error += "Corrupt - could not parse JSON.\r\n";
                Debug.LogError(_error);
                return _error;
            }
            if (N["category"] != "planet") _error += "Not a planet category JSON string.\r\n";
            if (System.Type.GetType("ProceduralPlanets." + N["type"]) == null) _error += "No planet blueprint class exists for this planet type (" + N["type"] + ").\r\n";
            if (N["planet"]["planetSeed"] == null) _error += "Error: planet.planetSeed missing\r\n";
            if (N["planet"]["variationSeed"] == null) _error += "Error: planet.variationSeed missing\r\n";
            if (N["planet"]["blueprintIndex"] == null) _error += "Error: planet.blueprintIndex missing\r\n";
            if (N["planet"]["blueprintName"] == null) _error += "Error: planet.blueprintName missing\r\n";

            // Get planet blueprint index from manager
            int _newIndex = PlanetManager.Instance.GetPlanetBlueprintIndexByName(N["planet"]["blueprintName"]);

            // If index was not found the manager is not configured with a planet blueprint that matches the name used when this planet was exported.
            if (_newIndex == -1)
                _error += "Error: planet.blueprintName (" + N["planet"]["blueprintName"] + ") does not exist in Manager list of planet blueprints. You must create a planet blueprint under Manager gameobject with this name (and blueprint class type: " + System.Type.GetType(N["type"]) + ") for the import to work.\r\n";

            // Handle validation errors
            if (_error != "")
            {
                Debug.LogError(_error);
                return _error;
            }

            // Set the planet and variation seed
            planetSeed = N["planet"]["planetSeed"];
            variationSeed = N["planet"]["variationSeed"];

            // If the order of the seeded planet blueprint index does not match it needs to be overridden...
            if (PlanetManager.Instance.GetPlanetBlueprintNameByIndex(N["planet"]["blueprintIndex"], blueprint) == PlanetManager.Instance.GetPlanetBlueprintNameByIndex(GetPlanetBlueprintSeededIndex(), blueprint))
            {
                // Seeded value name equals index value name, using seeded random
                SetPlanetBlueprint();
            }
            else
            {
                // Seeded value does not equal index value name, forcing override
                SetPlanetBlueprint(_newIndex);
            }

            // Import the float values of all float properties
            foreach (PropertyFloat _p in propertyFloats)
            {
                // Only import values that exist in the import JSON string
                if (N["planet"]["propertyFloats"][_p.key] != null)
                {
                    // If float property should be displayed as int...
                    if (_p.displayAsInt)
                    {
                        // Integer Values
                        // If randomly seeded value equals the value of the JSON string...
                        if ((int)GetPropertySeededFloat(_p) == N["planet"]["propertyFloats"][_p.key].AsInt)
                            // Set the property value to randomly seeded value (which is the same value as specified in the string)
                            SetPropertyFloat(_p.key);
                        else
                            // Force override of the value because the seeded random value does no longer match the value in the JSON string
                            OverridePropertyFloat(_p.key, N["planet"]["propertyFloats"][_p.key].AsInt, false);
                    }
                    else
                    {
                        // Float Values
                        // If the randomly seeded float value is approximately the same as the value specified in the string...
                        if (Mathf.Abs(GetPropertySeededFloat(_p) - N["planet"]["propertyFloats"][_p.key].AsFloat) < 0.0001f)
                            // Set the property value to randomly seeded value (which is the same value as specified in the string)
                            SetPropertyFloat(_p.key);
                        else
                            // Force override of the value because the seeded random value does no longer match the value in the JSON string
                            OverridePropertyFloat(_p.key, N["planet"]["propertyFloats"][_p.key].AsFloat, false);
                    }
                }
            }

            // Import the procedural materials
            foreach (PropertyMaterial _p in propertyMaterials)
            {
                // Validate the materials
                if (N["planet"]["propertyMaterials"][_p.key]["index"] == null) return "Error: propertyMaterials." + _p.key + ".index missing";
                if (N["planet"]["propertyMaterials"][_p.key]["name"] == null) return "Error: propertyMaterials." + _p.key + ".name missing";

                // Check if material exists by name since material index may change based on config
                if (_p.GetMaterialIndexByName(N["planet"]["propertyMaterials"][_p.key]["name"]) == -1) return "Error: propertyMaterials." + _p.key + ".name (" + N["planet"]["propertyMaterials"][_p.key]["name"] + ") does not exist in Manager list of materials.";

                // Check if material name is the same for the index saved and the index in editor now
                if (N["planet"]["propertyMaterials"][_p.key]["name"] == _p.materials[N["planet"]["propertyMaterials"][_p.key]["index"]].name)
                {
                    // Yes, indices are the same, check if material index is the same as the seeded value
                    if (N["planet"]["propertyMaterials"][_p.key]["index"] == GetPropertySeededMaterial(_p.key))
                    {
                        // Yes, it's seeded and therefore not overridden
                        SetPropertyMaterial(_p.key);
                    }
                    else
                    {
                        // No, it's not seeded so override it
                        OverridePropertyMaterial(_p.key, N["planet"]["propertyMaterials"][_p.key]["index"], false);
                    }
                }
                else
                {
                    // No, indices are not the same, force override:
                    OverridePropertyMaterial(_p.key, N["planet"]["propertyMaterials"][_p.key]["index"], false);
                }
            }

            // Import the colors
            foreach (PropertyColor _p in propertyColors)
            {
                // Validate the colors
                if (N["planet"]["propertyColors"][_p.key]["r"] == null) return "Error: planet.propertyColors." + _p.key + ".r missing";
                if (N["planet"]["propertyColors"][_p.key]["g"] == null) return "Error: planet.propertyColors." + _p.key + ".g missing";
                if (N["planet"]["propertyColors"][_p.key]["b"] == null) return "Error: planet.propertyColors." + _p.key + ".b missing";

                // Get the color determined by the random seed
                Color _seededColor = GetPropertySeededColor(_p.key);

                // If the color created by the random seed matches the color in the JSON string...
                if (Mathf.Abs(N["planet"]["propertyColors"][_p.key]["r"] - _seededColor.r) < 0.0001f &&
                    Mathf.Abs(N["planet"]["propertyColors"][_p.key]["g"] - _seededColor.g) < 0.0001f &&
                    Mathf.Abs(N["planet"]["propertyColors"][_p.key]["b"] - _seededColor.b) < 0.0001f)
                {
                    // Set the color to be determined by the random seed (since it matches the values of the color in the imported string)
                    SetPropertyColor(_p.key);
                }
                else
                {
                    // Force the color values to be overridden since the color values no longer match what is determined by the random seed.
                    OverridePropertyColor(_p.key, new Color(N["planet"]["propertyColors"][_p.key]["r"], N["planet"]["propertyColors"][_p.key]["g"], N["planet"]["propertyColors"][_p.key]["b"], 1.0f), false);
                }
            }
            
            if (_createRing)
            {
                // If the JSON string contains a ring....
                if (N["ring"] != null)
                {
                    // If the object already has a ring - destroy it
                    if (HasRing())
                        DestroyImmediate(GetRing().gameObject);

                    // Create a ring using settings in JSON string
                    CreateRing(_jsonString);
                }
                else
                {
                    // the JSON does not contain a ring so destroy the ring if this planet has one
                    //if (HasRing())
                    //  DestroyImmediate(GetRing().gameObject);
                }
            }

            // Update the serialized planet cache
            serializedPlanetCache = N.ToString();

            // Return empty string if import was successful.
            return "";
        }

        /// <summary>
        /// Gets the blueprint for the ring based on the planet blueprint child ring blueprint.
        /// </summary>
        /// <returns></returns>
        public virtual BlueprintRing GetRingBlueprint()
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("Planet.cs: GetRingBlueprint()");

            if (blueprint == null && planetBlueprintIndex == -1)
            {
                Debug.LogError("Planet has no blueprint or blueprintIndex set.");
                return null;
            }

            if (blueprint == null && planetBlueprintIndex != -1)
                SetPlanetBlueprint(planetBlueprintIndex, true, false);

            if (blueprint == null)
            {
                Debug.LogError("Planet had no blueprint and blueprintIndex could not be used to set blueprint.");
                return null;
            }

            if (blueprint.transform.Find("Ring") == null)
            {
                Debug.LogError("Ring component parent planet blueprint does not have a ring blueprint.");
                return null;
            }

            if (blueprint.transform.Find("Ring").GetComponent<BlueprintRing>() == null)
            {
                Debug.LogError("Error: Ring component parent planet blueprint does not have a ring blueprint.");
                return null;
            }

            if (blueprint.transform.Find("Ring").GetComponent<BlueprintRing>() == null)
            {
                Debug.LogError("Error: Ring component parent planet blueprint does not have a ring blueprint component.");
                return null;
            }

            return blueprint.transform.Find("Ring").GetComponent<BlueprintRing>();
        }

        public float GetLODPercent()
        {
            if (PlanetManager.cameraLOD == null)
            {
                Debug.LogWarning("PlanetManager.cameraLOD is null. Set to property to valid camera for LOD calculations. Aborting and returning 100% for LOD level.");
                return 100f;
            }
            float _distance = Vector3.Distance(PlanetManager.cameraLOD.transform.position, transform.position);
            float _frustumHeightAtDistance = 2.0f * _distance * Mathf.Tan(PlanetManager.cameraLOD.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float _planetDiameter = PlanetManager.CONST_MESH_RADIUS * 2.0f * transform.lossyScale.x;
            float _planetToScreenHeightPercent = (_planetDiameter / _frustumHeightAtDistance);
            //float _pixelsDiameter = _planetToScreenHeightPercent * Screen.height;
            return _planetToScreenHeightPercent;
        }


        public void SetSharedMesh(Mesh _mesh)
        {
            if (meshFilter == null)
                meshFilter = gameObject.GetComponent<MeshFilter>();

            meshFilter.sharedMesh = _mesh;
            if (_externalAtmosphereMeshFilter != null)
                _externalAtmosphereMeshFilter.sharedMesh = meshFilter.sharedMesh;
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (PlanetManager.Instance.showEditorMeshLOD)
            {
                Color _old = Gizmos.color;  
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position);
                Gizmos.color = _old;
            }

            if (PlanetManager.Instance.showEditorTextureLOD)
            {
                DrawString("Texture LOD Lvl " + textureLODLevel, transform.position);
            }
#endif
        }

        static void DrawString(string text, Vector3 worldPos)
        {
#if UNITY_EDITOR
            GUIStyle _gs = new GUIStyle();
            _gs.fontSize = 16;
            _gs.normal.textColor = Color.white;

            UnityEditor.Handles.BeginGUI();
            GUI.color = _gs.normal.textColor;
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
            Vector2 size =  _gs.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text, _gs);
            UnityEditor.Handles.EndGUI();
#endif
        }

        public virtual void OnEnable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
#endif
        }

        public virtual void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Update;
#endif
        }


        protected virtual void Update()
        {

        }

        public void Animate(string _key, float _source, float _destination, float _duration, float _delay)
        {
            PropertyFloatAnimation.Animation _pfaa = new PropertyFloatAnimation.Animation(_key, _source, _destination, _duration, _delay);
            _propertyFloatAnimations.Add(_pfaa);
        }

        public abstract void CacheProceduralProperty(string _property, bool _value);

    }

}

