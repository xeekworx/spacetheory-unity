/*  
    Purpose specific classes to store so called PropertyFloat, PropertyColor and PropertyMaterial.
    Planets and rings rely on parameters that are randomly selected between minimum and maximum values as specified in blueprints
    and these Properties are used specify min/max along with references to affected textures, shaders, and whether or not interpolation should be used etc.
    Properties are referenced by a key (to simplify editor code, make scripting easier, and to make JSON import/export easier.

    Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
*/

using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPlanets
{
    /// <summary>
    /// Used to keep track of animated floats.
    /// </summary>
    public class PropertyFloatAnimation
    {

        public class Animation
        {
            public string key;
            public float source;
            public float destination;
            public float duration;
            public float delay;
            public float timer;

            public Animation(string _key, float _source, float _destination, float _duration, float _delay)
            {
                key = _key;
                source = _source;
                destination = _destination;
                duration = _duration;
                delay = _delay;
                timer = Time.time;
            }

            public float GetAnimatedValue()
            {
                if (Time.time < timer + delay)
                    return source;
                if (Time.time > timer + delay + duration)
                    return destination;
                return Mathf.Lerp(source, destination, (Time.time - timer - delay) / duration);
            }

            public bool HasExpired()
            {
                if (Time.time > timer + delay + duration)
                    return true;
                return false;           
            }
        }
    }

    /// <summary>
    /// PropertyFloat contains floats with a minimum/maximum range. It contains reference to any procedural textures, materials, and shaders that are affected by this float.
    /// </summary>
    [System.Serializable]
    public class PropertyFloat
    {
        public string key;
        public string label;
        public float minValue;
        public float maxValue;
        public float value;
        public bool overrideRandom;
        public int seedOffset;
        public bool clamp01;
        public bool displayAsInt;
        public bool variation;
        public string[] proceduralTextures = new string[0];
        public Material[] materials;
        public Substance.Game.SubstanceGraph[] substanceGraphs;
        public string shaderProperty;
        public enum Method { VALUE, LERP }
        public Method shaderMethod;
        public enum DataType { INT, FLOAT }
        public DataType shaderDataType;

        /// <summary>
        /// Gets a linear interpolated value by using the actual value and interpolating between the min and max value.
        /// </summary>
        /// <returns></returns>
        public float GetPropertyLerp()
        {
            return Mathf.Lerp(minValue, maxValue, value);
        }

        /// <summary>
        /// Sets a Random Property Float based on a random seed and the min and max value.
        /// </summary>
        /// <param name="_seed"></param>
        /// <param name="_minValue"></param>
        /// <param name="_maxValue"></param>
        /// <returns></returns>
        public float SetPropertyFloat(int _seed, float _minValue, float _maxValue)
        {
            value = SeededRandomFloat(_seed, _minValue, _maxValue);
            overrideRandom = false;
            return value;
        }

        /// <summary>
        /// Get a random float between a min and max range using a specific seed (without affecting Random.State)
        /// </summary>
        /// <param name="_seed"></param>
        /// <param name="_minValue"></param>
        /// <param name="_maxValue"></param>
        /// <returns>Random float value using a specific random seed and min/max range.</returns>
        public float SeededRandomFloat(int _seed, float _minValue, float _maxValue)
        {
            // Store the random state so we can restore it later
            Random.State _oldState = Random.state;
            // Initialize the random state with a specific seed
            Random.InitState(_seed);

            // Get a random float value between the min and max range
            value = Random.Range(_minValue, _maxValue);

            // Restore the random state
            Random.state = _oldState;

            // Return the seeded random float
            return value;
        }

        /// <summary>
        /// Overrides a float with a specific value.
        /// </summary>
        /// <param name="_value"></param>
        public void OverridePropertyFloat(float _value)
        {
            value = _value;
            overrideRandom = true;
        }

        /// <summary>
        /// Constructor to define a PropertyFloat.
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
        /// <param name="_shaderDataType"></param>
        public PropertyFloat(string _key, string _label, float _minValue, float _maxValue, bool _clamp01, bool _displayAsInt, int _seedOffset, bool _variation, string[] _proceduralTextures = null, Material[] _materials = null, string _shaderProperty = null, Method _method = Method.VALUE, DataType _shaderDataType = DataType.FLOAT)
        {
            key = _key;
            label = _label;
            minValue = _minValue;
            maxValue = _maxValue;
            clamp01 = _clamp01;
            displayAsInt = _displayAsInt;
            seedOffset = _seedOffset;
            // Procedural textures affected by this PropertyFloat
            proceduralTextures = _proceduralTextures;
            variation = _variation;
            materials = _materials;
            // Shader property affected by this property
            shaderProperty = _shaderProperty;
            shaderMethod = _method;
            shaderDataType = _shaderDataType;

        }

        /// <summary>
        /// Constructor to define a PropertyFloat.
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
        /// <param name="_shaderDataType"></param>
        public PropertyFloat(string _key, string _label, float _minValue, float _maxValue, bool _clamp01, bool _displayAsInt, int _seedOffset, bool _variation, string[] _proceduralTextures = null, Substance.Game.SubstanceGraph[] _substanceGraphs = null, string _shaderProperty = null, Method _method = Method.VALUE, DataType _shaderDataType = DataType.FLOAT)
        {
            key = _key;
            label = _label;
            minValue = _minValue;
            maxValue = _maxValue;
            clamp01 = _clamp01;
            displayAsInt = _displayAsInt;
            seedOffset = _seedOffset;
            // Procedural textures affected by this PropertyFloat
            proceduralTextures = _proceduralTextures;
            variation = _variation;
            substanceGraphs = _substanceGraphs;
            // Shader property affected by this property
            shaderProperty = _shaderProperty;
            shaderMethod = _method;
            shaderDataType = _shaderDataType;

        }
    }

    
    /// <summary>
    /// PropertyColor contains a base color with specified ranges for hue, saturation and brightness. It contains reference to any shaders that are affected by this Color.
    /// </summary>
    [System.Serializable]
    public class PropertyColor
    {
        public string key;
        public string label;
        public Color baseColor;
        public float hueRange;
        public float saturationRange;
        public float brightnessRange;
        public Color color;
        public bool overrideRandom;
        public int seedOffset;
        public bool variation;
        public Material[] materials;
        public string shaderProperty;

        /// <summary>
        /// Sets a random Property Color based on a random seed, a base color, and hue, saturation, and brightness ranges.
        /// </summary>
        /// <param name="_seed"></param>
        /// <param name="_baseColor"></param>
        /// <param name="_hueRange"></param>
        /// <param name="_saturationRange"></param>
        /// <param name="_brightnessRange"></param>
        /// <returns>Random color using a specific random seed, base color and ranges for hue, saturation and brightness.</returns>
        public Color SetPropertyColor(int _seed, Color _baseColor, float _hueRange, float _saturationRange, float _brightnessRange)
        {
            color = GetPropertySeededColor(_seed, _baseColor, _hueRange, _saturationRange, _brightnessRange);
            overrideRandom = false;
            return color;
        }

        /// <summary>
        /// Gets a random seeded color based on specific seed, base color, and ranges for hue, saturation and brightness.
        /// </summary>
        /// <param name="_seed"></param>
        /// <param name="_baseColor"></param>
        /// <param name="_hueRange"></param>
        /// <param name="_saturationRange"></param>
        /// <param name="_brightnessRange"></param>
        /// <returns>Random color using a specific random seed, base color and ranges for hue, saturation and brightness.</returns>
        public Color GetPropertySeededColor(int _seed, Color _baseColor, float _hueRange, float _saturationRange, float _brightnessRange)
        {
            // Store the random state so we can restore it later
            Random.State _oldState = Random.state;

            // Initialize the random state with a specific seed
            Random.InitState(_seed);

            // Initialize hue, saturation, and brightness values
            float _hue = 0f;
            float _saturation = 0f;
            float _brightness = 0f;

            // Convert the baseColor to  HSV (hue, saturation and brightness) values
            Color.RGBToHSV(_baseColor, out _hue, out _saturation, out _brightness);

            // Set the hue to base color and a random deviation within hue range
            _hue = Mathf.Clamp01(Random.Range(_hue - _hueRange / 2.0f, _hue + _hueRange / 2.0f));

            // Set the saturation to base color and a random deviation within saturation range
            _saturation = Mathf.Clamp01(Random.Range(_saturation - _saturationRange / 2.0f, _saturation + _saturationRange / 2.0f));

            // Set the brightness to base color and a random deviation within brightness range
            _brightness = Mathf.Clamp01(Random.Range(_brightness - _brightnessRange / 2.0f, _brightness + _brightnessRange / 2.0f));

            // Restore the random state
            Random.state = _oldState;

            // Return the new random color as a RGB color
            return Color.HSVToRGB(_hue, _saturation, _brightness);
        }

        /// <summary>
        /// Overrides a PropertyColor with a specific color.
        /// </summary>
        /// <param name="_color"></param>
        public void OverridePropertyColor(Color _color)
        {
            color = _color;
            overrideRandom = true;
        }

        /// <summary>
        /// Constructor to define a PropertyColor.
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
        public PropertyColor(string _key, string _label, Color _baseColor, float _hueRange, float _saturationRange, float _brightnessRange, int _seedOffset, bool _variation, Material[] _materials = null, string _shaderProperty = null)
        {
            key = _key;
            label = _label;
            baseColor = _baseColor;
            hueRange = _hueRange;
            saturationRange = _saturationRange;
            brightnessRange = _brightnessRange;
            seedOffset = _seedOffset;
            variation = _variation;
            materials = _materials;
            // Shader properties affected by this property
            shaderProperty = _shaderProperty;
        }

    }

    /// <summary>
    /// PropertyMaterial contains reference to a procedural material and reference to any procedural textures and shaders that are affected by this material.
    /// </summary>
    [System.Serializable]
    public class PropertyMaterial
    {
        public string key;
        public string label;
        public int value;
        public string[] popupDisplay;
        public Substance.Game.SubstanceGraph[] materials;
        public bool overrideRandom;
        public int seedOffset;
        public string[] proceduralTextures = new string[0];
        public Material material;
        public string shaderProperty;

        /// <summary>
        /// Gets the procedural material of this PropertyMaterial
        /// </summary>
        /// <returns></returns>
        public Substance.Game.SubstanceGraph GetPropertyMaterial()
        {
            return materials[value];
        }

        /// <summary>
        /// Sets the PropertyMaterial based on random seed and the length and filter mask specified.
        /// </summary>
        /// <param name="_seed"></param>
        /// <param name="_length"></param>
        /// <param name="_mask"></param>
        /// <returns>Index value of the PropertyMaterial.</returns>
        public int SetPropertyMaterial(int _seed, int _length, int _mask)
        {
            // Get the random index value based on seed, length and mask
            int _randomIndex = GetPropertySeededMaterialIndex(_seed, _length, _mask);
            // Create a selection list since we need to convert a masked list of materials to a full list of materials
            List<int> _selection = GetSelectionList(_length, _mask);
            // Get the maximum value in the mask (masks utilize power of two)
            value = Util.MaxInMask(_selection[_randomIndex]) - 1;
            // Ensure override is set to false since we're getting this via random
            overrideRandom = false;
            // Return the index
            return _randomIndex;
        }

        /// <summary>
        /// Gets a seeded material index based on seed, length and mask.
        /// </summary>
        /// <param name="_seed"></param>
        /// <param name="_length"></param>
        /// <param name="_mask"></param>
        /// <returns>Index of material</returns>
        public int GetPropertySeededMaterialIndex(int _seed, int _length, int _mask)
        {            
            // Store the random state so we can restore it later
            Random.State _oldState = Random.state;

            // Initialize the random state with a specific seed
            Random.InitState(_seed);

            // Get the random index value between 0 and the size of the masked selection list
            int _randomIndex = Random.Range(0, GetSelectionList(_length, _mask).Count);

            // Restore random state
            Random.state = _oldState;

            // Return the seeded random index value of the material
            return _randomIndex;
        }

        /// <summary>
        /// Gets a seeded material value based on seed, length and mask based on a random index.
        /// </summary>
        /// <param name="_seed"></param>
        /// <param name="_length"></param>
        /// <param name="_mask"></param>
        /// <returns>Integer value of the material based on a random index.</returns>
        public int GetPropertySeededMaterialValue(int _seed, int _length, int _mask)
        {
            // Get a random index value
            int _randomIndex = GetPropertySeededMaterialIndex(_seed, _length, _mask);

            // Create a masked selection list of materials to choose from
            List<int> _selection = GetSelectionList(_length, _mask);

            // Return integer value of material based on a random index
            return Util.MaxInMask(_selection[_randomIndex]) - 1;
        }

        /// <summary>
        /// Generates a selection list based on length and mask values.
        /// </summary>
        /// <param name="_length"></param>
        /// <param name="_mask"></param>
        /// <returns></returns>
        public List<int> GetSelectionList(int _length, int _mask)
        {
            List<int> _selection = new List<int>();
            if (_mask == -1)
            {
                // Mask is set to everything
                int _v = 1;
                for (int _i = 0; _i < _length; _i++)
                {
                    if ((_v & _mask) == _v)
                    {
                        _selection.Add(_v);
                    }

                    _v *= 2;
                }
            }

            if (_mask > 0)
            {
                // Mask is set to nothing with items added
                int _maxInMask = Util.MaxInMask(_mask);
                int _v = 1;
                for (int _i = 0; _i < _maxInMask; _i++)
                {
                    if ((_v & _mask) == _v) _selection.Add(_v);
                    _v *= 2;
                }
            }
            if (_mask < -1)
            {
                // Mask is set to everything with items removed
                _mask = Mathf.Abs(_mask) - 1;
                int _v = 1;
                for (int _i = 0; _i < _length; _i++)
                {
                    if ((_v & _mask) != _v) _selection.Add(_v);
                    _v *= 2;
                }
            }
            return _selection;
        }

        /// <summary>
        /// Gets a material index by name.
        /// </summary>
        /// <param name="_name"></param>
        /// <returns>Index value of material</returns>
        public int GetMaterialIndexByName(string _name)
        {
            // Iterate through materials
            for (int _i = 0; _i < materials.Length; _i++)
                // If a material in the list matches the name, return the index value
                if (materials[_i].name == _name) return _i;

            // No material was found, return -1
            return -1;
        }

        /// <summary>
        /// Override a PropertyMaterial with a specific value.
        /// </summary>
        /// <param name="_value"></param>
        public void OverridePropertyMaterial(int _value)
        {
            value = _value;
            overrideRandom = true;
        }

        /// <summary>
        /// Constructor to define a PropertyMaterial.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_materials"></param>
        /// <param name="_seedOffset"></param>
        /// <param name="_proceduralTextures"></param>
        /// <param name="_material"></param>
        /// <param name="_shaderProperty"></param>
        public PropertyMaterial(string _key, string _label, Substance.Game.SubstanceGraph[] _materials, int _seedOffset, string[] _proceduralTextures = null, Material _material = null, string _shaderProperty = null)
        {
            key = _key;
            label = _label;
            materials = _materials;
            // Array of procedural textures affected by this material
            proceduralTextures = _proceduralTextures;
            // Array to be displayed in the inspector of filtered materials
            popupDisplay = new string[materials.Length];
            seedOffset = _seedOffset;
            // Populate the array of filtered names to be displayed in inspector
            for (int _i = 0; _i < materials.Length; _i++)
                popupDisplay[_i] = materials[_i].name;
            material = _material;
            // Shader properties affected by this material
            shaderProperty = _shaderProperty;
        }
    }
}
