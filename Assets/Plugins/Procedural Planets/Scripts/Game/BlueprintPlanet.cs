using UnityEngine;
using ProceduralPlanets.SimpleJSON;

namespace ProceduralPlanets
{

    /// <summary>
    /// This is the parent class of planet blueprints containing some of the core variables, classes and methods.
    /// Derived from Bluprint class which contains some core classes, lists, and methods common to all types of blueprints.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    public class BlueprintPlanet : Blueprint
    {
        const string BLUEPRINT_PLANET_VERSION = "1.0";

        // Probability (0.0 - 1.0) of this blueprint being used when spawning planet. Probability value of all blueprints are combined to calculate a percentage.
        public float probability = 0.5f;

        // Probability of this planet blueprint spawning a planetary ring (if a child ring blueprint exists)
        public float ringProbability = 0.5f;

        /// <summary>
        /// Creates a ring as a child to this planet blueprint.
        /// </summary>
        public virtual void CreateRing()
        {
            GameObject _go = new GameObject();
            _go.name = "Ring";
            _go.transform.parent = transform;
            _go.AddComponent<BlueprintRing>();
        }

        /// <summary>
        /// Answers the question if planet blueprint has a ring or not.
        /// </summary>
        /// <returns>True (ring exists) / False (no ring exists)</returns>
        public virtual bool HasRing()
        {
            if (transform.Find("Ring") == null) return false;
            if (transform.Find("Ring").GetComponent<BlueprintRing>() == null) return false;
            return true;
        }

        /// <summary>
        /// Gets the blueprint ring if one exists.
        /// </summary>
        /// <returns>BlueprintRing</returns>
        public virtual BlueprintRing GetRing()
        {
            if (HasRing())
                return transform.Find("Ring").GetComponent<BlueprintRing>();
            return null;
        }

        /// <summary>
        /// Exports this planet blueprint to a JSON string.
        /// </summary>
        /// <param name="_easyRead"></param>
        /// <returns>JSON formated string with planet blueprint configuration.</returns>
        public virtual string ExportToJSON(bool _easyRead = true)
        {            
            // Initialize the JSON object
            var N = JSON.Parse("{}");

            // Set the base parameters
            N["category"] = "blueprint";
            N["type"] = GetType().Name;
            N["name"] = gameObject.name;
            N["planet"]["version"] = BLUEPRINT_PLANET_VERSION;
            N["planet"]["probability"] = probability;
            N["planet"]["ringProbability"] = ringProbability;

            // Set all the Blueprint floats, materials, and colors
            foreach (BlueprintPropertyFloat _p in blueprintPropertyFloats)
            {
                N["planet"]["propertyFloats"][_p.key]["min"].AsFloat = _p.minValue;
                N["planet"]["propertyFloats"][_p.key]["max"].AsFloat = _p.maxValue;
            }
            foreach (BlueprintPropertyMaterial _p in blueprintPropertyMaterials)
            {
                N["planet"]["propertyMaterials"][_p.key]["mask"] = _p.mask;
            }
            foreach (BlueprintPropertyColor _p in blueprintPropertyColors)
            {
                N["planet"]["propertyColors"][_p.key]["r"] = _p.color.r;
                N["planet"]["propertyColors"][_p.key]["g"] = _p.color.g;
                N["planet"]["propertyColors"][_p.key]["b"] = _p.color.b;
                N["planet"]["propertyColors"][_p.key]["hueRange"] = _p.hueRange;
                N["planet"]["propertyColors"][_p.key]["saturationRange"] = _p.saturationRange;
                N["planet"]["propertyColors"][_p.key]["brightnessRange"] = _p.brightnessRange;
            }

            // Initialize return string
            string _str = "";

            // If "easyread" is desired...
            if (_easyRead)
                // Use two space indentation with linefeeds
                _str = N.ToString(2);
            else
                // Use compact format with no spaces or linefeeds
                _str = N.ToString();

            // Remove the last closing squiggly bracket }  (we may need to add a ring to this JSON string)
            _str = _str.Substring(0, _str.Length - 1);

            // If blueprint has a ring...
            if (HasRing())
            {
                // Get the JSON configuration of the ring blueprint
                string _ring = GetRing().ExportToJSON(_easyRead);

                // Remove first and last squiggly brackets {} from the string                
                _ring = _ring.Substring(1, _ring.Length - 2);

                // Add a comma and the ring JSON to the return string
                _str += "," + _ring;
            }

            // Add closing bracket, remove excessive linefeed and return the JSON string
            return (_str + "}").Replace("\r\n,", ",");
        }

        /// <summary>
        /// Imports a planet blueprint from a JSON string.
        /// </summary>
        /// <param name="_jsonString"></param>
        /// <returns>Error message if import failed.</returns>
        public virtual string ImportFromJSON(string _jsonString)
        {

            // Initialize string for porential errors
            string _error = "";

            // Create the JSON object by parsing the string supplied as a parameter
            var N = JSON.Parse(_jsonString);

            // If JSON object is null - log and return error
            if (N == null)
            {
                _error = "Error: Could not parse JSON.";
                Debug.LogError(_error);
                return _error;
            }

            // Validate to ensure this is a blueprint.
            if (N["category"] != "blueprint") _error = "Error: Not a blueprint JSON.";

            // Validate that the blue print type exists.
            if (N["type"] != GetType().Name) _error = "Error: JSON blueprint does not contain the same planet type as you are trying to import to.";

            // Handle validation errors by logging and returning them.
            if (_error != "")
            {
                Debug.LogError(_error);
                return _error;
            }

            // Import base parameters
            probability = N["planet"]["probability"].AsFloat;
            ringProbability = N["planet"]["ringProbability"].AsFloat;

            // Import property floats, colors, and materials
            foreach (BlueprintPropertyFloat _p in blueprintPropertyFloats)
            {
                if (_p.displayAsInt)
                {
                    _p.minValue = N["planet"]["propertyFloats"][_p.key]["min"].AsInt;
                    _p.maxValue = N["planet"]["propertyFloats"][_p.key]["max"].AsInt;
                }
                else
                {
                    _p.minValue = N["planet"]["propertyFloats"][_p.key]["min"].AsFloat;
                    _p.maxValue = N["planet"]["propertyFloats"][_p.key]["max"].AsFloat;
                }
            }
            foreach (BlueprintPropertyMaterial _p in blueprintPropertyMaterials)
            {
                _p.mask = N["planet"]["propertyMaterials"][_p.key]["mask"];
            }
            foreach (BlueprintPropertyColor _p in blueprintPropertyColors)
            {
                _p.color = new Color(N["planet"]["propertyColors"][_p.key]["r"], N["planet"]["propertyColors"][_p.key]["g"], N["planet"]["propertyColors"][_p.key]["b"]);
                _p.hueRange = N["planet"]["propertyColors"][_p.key]["hueRange"];
                _p.saturationRange = N["planet"]["propertyColors"][_p.key]["saturationRange"];
                _p.brightnessRange = N["planet"]["propertyColors"][_p.key]["brightnessRange"];
            }

            // If the blueprint JSON string contains a ring...
            if (N["ring"] != null)
            {
                // If no ring exists, create it
                if (!HasRing()) CreateRing();

                // Import the configuration of the ring from the JSON string
                GetRing().ImportFromJSON(_jsonString);
            }
            else
            {
                // If a ring exists and there is no ring in the JSON string - destroy the ring of this blueprint
                if (HasRing()) Destroy(GetRing().gameObject);
            }

            // Return empty string if import was successful
            return "";
        }
    }
}
