using UnityEngine;

namespace ProceduralPlanets
{
    /// <summary>
    /// This is derived from the BlueprintPlanet and contains specific types of properties for gas planet blueprints.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>    
    public class BlueprintGasPlanet : BlueprintPlanet
    {
        /// <summary>
        /// Sets up all Blueprint Property Floats, Colors, and Materials for Gas Planet Blueprints.
        /// Reset() is called when component is added and if Inspector > Cog wheel > Reset is clicked in the Editor.
        /// </summary>
        void Reset()
        {
            // Ensure there is a parent
            if (transform.parent == null)
            {
                Debug.LogError("There is no parent object to this blueprint. This game object must be a child to a gameobject with the Manager component. Aborting.");
                return;
            }

            // Get a reference to the Manager
            PlanetManager _manager = transform.parent.GetComponent<PlanetManager>();

            // Ensure the parent has the Manager component
            if (_manager == null)
            {
                Debug.LogError("The parent of this game object must have the Manager component. Aborting.");
                return;
            }

            // Add the blueprint Property Floats, Colors, and Materials
            AddBlueprintPropertyMaterial("gas", "Gas", PlanetManager.Instance.gasMaterials.ToArray());
            AddBlueprintPropertyColor("twilightColor", "Twilight Color", new Color(0.15f, 0.0f, 0.15f, 1.0f), 0.05f, 0.05f, 0.05f);

            AddBlueprintPropertyFloat("gasSeed", "Gas Seed", 0, 255, false, true);
            AddBlueprintPropertyFloat("turbulenceSeed", "Turbulence Seed", 0, 255, false, true);
            AddBlueprintPropertyFloat("horizontalTiling", "Horizontal Tiling", 1, 10, false, true);
            AddBlueprintPropertyFloat("verticalTiling", "Vertical Tiling", 1, 10, false, true);

            AddBlueprintPropertyFloat("turbulence", "Turbulence", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("turbulenceScale", "Turbulence Scale", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("turbulenceDisorder", "Turbulence Disorder", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("smoothness", "Smoothness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("blurriness", "Blurrinenss", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("separation", "Separation", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("palette", "Palette", 1, 8, false, true);
            AddBlueprintPropertyFloat("detail", "Detail", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("detailOffset", "Detail Offset", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("contrast", "Contrast", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("hue", "Hue", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("saturation", "Saturation", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("brightness", "Brightness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("banding", "Banding", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("solidness", "Solidness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("faintness", "Faintness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("faintnessColor", "Faintness Color", new Color(0.5f, 0.5f, 0.5f, 1.0f), 0.2f, 0.2f, 0.2f);
            AddBlueprintPropertyFloat("roughness", "Roughness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("stormMaskIndex", "Storm Mask Index", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("stormSquash", "Storm Squash", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("stormColor", "Storm Color", new Color(0.78f, 0.13f, 0.28f, 1.0f), 0.05f, 0.05f, 0.05f);
            AddBlueprintPropertyFloat("stormTint", "Storm Tint", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("stormScale", "Storm Scale", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("stormNoise", "Storm Noise", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("atmosphereColor", "Atmosphere Color", new Color (0.48f, 0.48f, 0.3f, 1.0f), 0.05f, 0.05f, 0.05f);
            AddBlueprintPropertyFloat("atmosphereFalloff", "Atmosphere Falloff", 1.0f, 20.0f, true, false);

            // Refresh the lists of blueprints in the Manager
            _manager.RefreshLists();
        }
    }
}
