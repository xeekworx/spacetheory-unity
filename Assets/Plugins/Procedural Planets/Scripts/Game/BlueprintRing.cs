using System.Collections.Generic;
using UnityEngine;
using ProceduralPlanets.SimpleJSON;

namespace ProceduralPlanets
{
    /// <summary>
    /// This is the class of ring blueprints.
    /// Derived from Bluprint class which contains some core classes, lists, and methods common to all types of blueprints.
    ///
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    [System.Serializable]
    public class BlueprintRing : Blueprint
    {
        const string BLUEPRINT_RING_VERSION = "1.0";

        /// <summary>
        /// Sets up all Blueprint Property Floats, Colors, and Materials for Ring Blueprint.
        /// Reset() is called when component is added and if Inspector > Cog wheel > Reset is clicked in the Editor.
        /// </summary>
        void Reset()
        {
            // Ensure there is a parent planet
            if (transform.parent == null)
            {
                Debug.LogError("There is no parent object to this ring blueprint. This game object must be a child to a gameobject with the Planet Blueprint (or derived class) component. Aborting.");
                return;
            }

            // Ensure there is manager parent of the planet blueprint
            if (transform.parent.parent == null)
            {
                Debug.LogError("There is no manager parent object to the planet blueprint parent. Aborting.");
                return;
            }

            // Get a reference to the Manager
            PlanetManager _manager = transform.parent.parent.GetComponent<PlanetManager>();

            // Ensure the parent has the Manager component
            if (_manager == null)
            {
                Debug.LogError("The parent of this game object must have the Manager component. Aborting.");
                return;
            }

            AddBlueprintPropertyFloat("innerRadius", "Inner Radius", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("outerRadius", "Outer Radius", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyMaterial("ringMaterial", "Ring Material", PlanetManager.Instance.ringMaterials.ToArray());
            AddBlueprintPropertyFloat("gradientDiffuse", "Gradient Diffuse", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("gradientAlpha", "Gradient Alpha", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("diffuseHue", "Hue", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("diffuseSaturation", "Saturation", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("diffuseLightness", "Lightness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("alphaContrast", "Alpha Contrast", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("alphaLuminosity", "Alpha Luminosity", 0.0f, 1.0f, true, false);

            // Refresh the lists of blueprints in the Manager
            _manager.RefreshLists();
        }

        /// <summary>
        /// Exports this ring blueprint to a JSON string.
        /// </summary>
        /// <param name="_easyRead"></param>
        /// <returns>JSON formated string with ring blueprint configuration.</returns>
        public string ExportToJSON(bool _easyRead = true)

        {// Initialize the JSON object
            var N = JSON.Parse("{}");

            // Set the base parameters
            N["ring"]["version"] = BLUEPRINT_RING_VERSION;

            // Set all the Blueprint floats, materials, and colors
            foreach (BlueprintPropertyFloat _p in blueprintPropertyFloats)
            {
                N["ring"]["propertyFloats"][_p.key]["min"].AsFloat = _p.minValue;
                N["ring"]["propertyFloats"][_p.key]["max"].AsFloat = _p.maxValue;
            }
            foreach (BlueprintPropertyMaterial _p in blueprintPropertyMaterials)
            {
                N["ring"]["propertyMaterials"][_p.key]["mask"] = _p.mask;
            }
            foreach (BlueprintPropertyColor _p in blueprintPropertyColors)
            {
                N["ring"]["propertyColors"][_p.key]["r"] = _p.color.r;
                N["ring"]["propertyColors"][_p.key]["g"] = _p.color.g;
                N["ring"]["propertyColors"][_p.key]["b"] = _p.color.b;
                N["ring"]["propertyColors"][_p.key]["hueRange"] = _p.hueRange;
                N["ring"]["propertyColors"][_p.key]["saturationRange"] = _p.saturationRange;
                N["ring"]["propertyColors"][_p.key]["brightnessRange"] = _p.brightnessRange;
            }

            // If "easyread" is desired...
            if (_easyRead)
                // Return JSON string with two space indentation and linefeeds
                return N.ToString(2);
            else
                // Resturn compact JSON strnigs without indentation and linefeed
                return N.ToString();
        }

        /// <summary>
        /// Imports a ring blueprint from a JSON string.
        /// </summary>
        /// <param name="_jsonString"></param>
        /// <returns>Error message if import failed.</returns>
        public string ImportFromJSON(string _jsonString)
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
            if (N["ring"] == null) _error = "Error: There is no ring parameter.";

            // Handle validation errors by logging and returning them.
            if (_error != "")
            {
                Debug.LogError(_error);
                return _error;
            }

            // Import property floats, colors, and materials
            foreach (BlueprintPropertyFloat _p in blueprintPropertyFloats)
            {
                if (_p.displayAsInt)
                {
                    _p.minValue = N["ring"]["propertyFloats"][_p.key]["min"].AsInt;
                    _p.maxValue = N["ring"]["propertyFloats"][_p.key]["max"].AsInt;
                }
                else
                {
                    _p.minValue = N["ring"]["propertyFloats"][_p.key]["min"].AsFloat;
                    _p.maxValue = N["ring"]["propertyFloats"][_p.key]["max"].AsFloat;
                }
            }
            foreach (BlueprintPropertyMaterial _p in blueprintPropertyMaterials)
            {
                _p.mask = N["ring"]["propertyMaterials"][_p.key]["mask"];

            }
            foreach (BlueprintPropertyColor _p in blueprintPropertyColors)
            {
                _p.color = new Color(N["ring"]["propertyColors"][_p.key]["r"], N["ring"]["propertyColors"][_p.key]["g"], N["ring"]["propertyColors"][_p.key]["b"]);
                _p.hueRange = N["ring"]["propertyColors"][_p.key]["hueRange"];
                _p.saturationRange = N["ring"]["propertyColors"][_p.key]["saturationRange"];
                _p.brightnessRange = N["ring"]["propertyColors"][_p.key]["brightnessRange"];
            }

            // Return empty string if import was successful
            return "";
        }
    }
}
