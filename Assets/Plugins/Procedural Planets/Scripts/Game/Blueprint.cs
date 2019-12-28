using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPlanets
{

    /// <summary>
    /// This base class for all blueprints which contains core classes, lists, and methods common to all blueprints.
    /// Planet and Ring blueprites are derived from this class.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    public class Blueprint : MonoBehaviour
    {        
        // Class used to store min/max range of values that will be used when a planet/ring is randomly setting a property float
        [System.Serializable]
        public class BlueprintPropertyFloat
        {
            public string key;
            public string label;
            public float minValue;
            public float maxValue;
            public bool clamp01;
            public bool displayAsInt;
            public float minLimit;
            public float maxLimit;
        }

        // Class used to store hue, saturation and brightness ranges that will be used when a planet/ring is randomly choosing setting a color
        [System.Serializable]
        public class BlueprintPropertyColor
        {
            public string key;
            public string label;
            public Color color;
            public float hueRange;
            public float saturationRange;
            public float brightnessRange;

        }

        // Class used to set a mask filter so only desired materials are used when randomly selected when creating a planet/ring
        [System.Serializable]
        public class BlueprintPropertyMaterial
        {
            public string key;
            public string label;
            public string[] maskDisplay;
            public int mask;
        }

        // Lists to keep track of property floats, colors and materials (the (0) is used so it is initialized during serialization)
        public List<BlueprintPropertyFloat> blueprintPropertyFloats = new List<BlueprintPropertyFloat>(0);
        public List<BlueprintPropertyColor> blueprintPropertyColors = new List<BlueprintPropertyColor>(0);
        public List<BlueprintPropertyMaterial> blueprintPropertyMaterials = new List<BlueprintPropertyMaterial>(0);

        /// <summary>
        /// Adds a BlueprintPropertyFloat which is used by derived classes in the Reset() method to add all the settings for a particular planet/ring type.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_min"></param>
        /// <param name="_max"></param>
        /// <param name="_clamp01"></param>
        /// <param name="_displayAsInt"></param>
        protected virtual void AddBlueprintPropertyFloat(string _key, string _label, float _min, float _max, bool _clamp01, bool _displayAsInt)
        {
            BlueprintPropertyFloat _p = new BlueprintPropertyFloat();
            _p.key = _key;
            _p.label = _label;
            _p.minValue = _min;
            _p.minLimit = _min;
            _p.maxValue = _max;
            _p.maxLimit = _max;
            _p.clamp01 = _clamp01;
            _p.displayAsInt = _displayAsInt;
            blueprintPropertyFloats.Add(_p);
        }

        /// <summary>
        /// Adds a BlueprintPropertyColor which is used by derived classes in the Reset() method to add all the settings for a particular planet/ring type.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_color"></param>
        /// <param name="_hueRange"></param>
        /// <param name="_saturationRange"></param>
        /// <param name="_brightnessRange"></param>
        protected virtual void AddBlueprintPropertyColor(string _key, string _label, Color _color, float _hueRange, float _saturationRange, float _brightnessRange)
        {
            BlueprintPropertyColor _p = new BlueprintPropertyColor();
            _p.key = _key;
            _p.label = _label;
            _p.color = _color;
            _p.hueRange = _hueRange;
            _p.saturationRange = _saturationRange;
            _p.brightnessRange = _brightnessRange;
            blueprintPropertyColors.Add(_p);
        }

        /// <summary>
        /// Adds a BlueprintPropertyMaterial which is used by derived classes in the Reset() method to add all the settings for a particular planet/ring type.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_label"></param>
        /// <param name="_materials"></param>
        protected virtual void AddBlueprintPropertyMaterial(string _key, string _label, Substance.Game.SubstanceGraph[] _materials)
        {
            BlueprintPropertyMaterial _p = new BlueprintPropertyMaterial();
            _p.key = _key;
            _p.label = _label;
            _p.maskDisplay = new string[_materials.Length];
            for (int _i = 0; _i < _materials.Length; _i++)
            {
                _p.maskDisplay[_i] = _materials[_i].name;
            }
            _p.mask = -1;
            blueprintPropertyMaterials.Add(_p);
        }

        /// <summary>
        /// Gets the minimum value of a PropertyFloat (this will be the lowest/min value of Random.Range when planet/ring is random setting a property.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Minimum float value to be used by Random.Range when setting a planet float property.</returns>
        public virtual float GetMin(string _key)
        {
            return FindBlueprintPropertyFloat(_key).minValue;
        }

        /// <summary>
        /// Gets the maximum value of a PropertyFloat (this will be the highest/max value of Random.Range when planet/ring is random setting a property.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Maximum float value to be used by Random.Range when setting a planet float property.</returns>
        public virtual float GetMax(string _key)
        {
            return FindBlueprintPropertyFloat(_key).maxValue;
        }

        /// <summary>
        /// Gets the base color of a Property Color (hue, saturation, and brightness ranges will be relative to this color).
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Base color of a PropertyColor.</returns>
        public virtual Color GetColor(string _key)
        {
            return FindBlueprintPropertyColor(_key).color;
        }

        /// <summary>
        /// Gets the Hue range that wil determine the random min/max value of hue from the base color of a ProeprtyColor.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Hue Range (float) for min/max value of hue relative to base color of PropertyColor.</returns>
        public virtual float GetHueRange(string _key)
        {
            return FindBlueprintPropertyColor(_key).hueRange;
        }

        /// <summary>
        /// Gets the Saturationrange that wil determine the random min/max value of saturation from the base color of a ProeprtyColor.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Saturation Range (float) for min/max value of saturation relative to base color of PropertyColor.</returns>
        public virtual float GetSaturationRange(string _key)
        {
            return FindBlueprintPropertyColor(_key).saturationRange;
        }

        /// <summary>
        /// Gets the Brightness range that wil determine the random min/max value of brightness from the base color of a ProeprtyColor.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Brightness Range (float) for min/max value of brigtness relative to base color of PropertyColor.</returns>
        public virtual float GetBrightnessRange(string _key)
        {
            return FindBlueprintPropertyColor(_key).brightnessRange;
        }

        /// <summary>
        /// Gets the material mask to be used when materials is randomly selected in a PropertyMaterial.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Integer mask value of material mask.</returns>
        public virtual int GetMaterialMask(string _key)
        {
            return FindBlueprintPropertyMaterial(_key).mask;
        }

        /// <summary>
        /// Gets the length mask string array.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Integer value of the mask length.</returns>
        public virtual int GetMaterialLength(string _key)
        {
            return FindBlueprintPropertyMaterial(_key).maskDisplay.Length;
        }

        /// <summary>
        /// Finds a BlueprintPropertyFloat by key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>BlueprintPropertyFloat</returns>
        protected virtual BlueprintPropertyFloat FindBlueprintPropertyFloat(string _key)
        {
            // Iterate through all the blueprint property floats
            foreach (BlueprintPropertyFloat _p in blueprintPropertyFloats)
                // If the key is found, return the BlueprintPropertyFloat
                if (_p.key == _key) return _p;

            // No key was found - display warning and return null
            Debug.LogWarning("Can't find BlueprintPropertyFloat: " + _key);
            return null;
        }

        /// <summary>
        /// Finds a BlueprintPropertyColor by key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>BlueprintPropertyColor</returns>
        protected virtual BlueprintPropertyColor FindBlueprintPropertyColor(string _key)
        {
            // Iterate through all the blueprint property colors
            foreach (BlueprintPropertyColor _p in blueprintPropertyColors)
            {
                // If the key is found, return the BlueprintPropertyColor
                if (_p.key == _key) return _p;
            }


            // No key was found - display warning and return null
            Debug.LogWarning("Can't find BlueprintPropertyColor: " + _key);
            return null;
        }

        /// <summary>
        /// Finds a BlueprintPropertyMaterial by key.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>BlueprintPropertyMaterial</returns>
        protected virtual BlueprintPropertyMaterial FindBlueprintPropertyMaterial(string _key)
        {
            // Iterate through all the blueprint property materials
            foreach (BlueprintPropertyMaterial _p in blueprintPropertyMaterials)
                // If the key is found, return the BlueprintPropertyMaterial
                if (_p.key == _key) return _p;

            // No key was found - display warning and return null
            Debug.LogWarning("Can't find BlueprintPropertyMaterial: " + _key);
            return null;
        }

    }
}
