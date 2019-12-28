using UnityEngine;

namespace ProceduralPlanets
{
    /// <summary>
    /// This is derived from the BlueprintPlanet and contains specific types of properties for solid planet blueprints.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    public class BlueprintSolidPlanet : BlueprintPlanet
    {
        /// <summary>
        /// Sets up all Blueprint Property Floats, Colors, and Materials for Solid Planet Blueprints.
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
            AddBlueprintPropertyMaterial("composition", "Composition", PlanetManager.Instance.solidCompositionMaterials.ToArray());
            AddBlueprintPropertyFloat("alienization", "Alienization", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("specularColor", "Specular Color", new Color(0.4f, 0.4f, 0.05f, 1.0f), 0.05f, 0.05f, 0.05f);

            AddBlueprintPropertyFloat("continentSeed", "Continent Seed", 0, 255, false, true);
            AddBlueprintPropertyFloat("continentSize", "Continent Size", 0f, 1f, true, false);
            AddBlueprintPropertyFloat("continentComplexity", "Continent Complexity", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyFloat("coastalDetail", "Coastal Detail", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("coastalReach", "Coastal Reach", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyFloat("liquidLevel", "Liquid Level", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("liquidColor", "LiquidColor", new Color(0.0f, 0.0f, 0.3f, 1.0f), 0.02f, 0.3f, 0.3f);
            AddBlueprintPropertyFloat("liquidOpacity", "Liquid Opacity", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("liquidShallow", "Shallow Distance", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("liquidSpecularPower", "Specular Power", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyMaterial("polarIce", "Polar Ice", PlanetManager.Instance.solidPolarIceMaterials.ToArray());
            AddBlueprintPropertyFloat("polarCapAmount", "Polar Caps", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("iceColor", "Ice Color", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.1f, 0.1f, 0.1f);

            AddBlueprintPropertyColor("atmosphereColor", "Atmosphere Color", new Color(0.2f, 0.75f, 1.0f, 1.0f), 0.2f, 0.2f, 0.2f);
            AddBlueprintPropertyFloat("atmosphereExternalSize", "External Size", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("atmosphereExternalDensity", "External Density", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("atmosphereInternalDensity", "Internal Density", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("twilightColor", "Twilight Color", new Color(0.25f, 0.2f, 0.05f, 1.0f), 0.05f, 0.2f, 0.2f);

            AddBlueprintPropertyMaterial("clouds", "Clouds", PlanetManager.Instance.solidCloudsMaterials.ToArray());
            AddBlueprintPropertyFloat("cloudsOpacity", "Clouds Opacity", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("cloudsColor", "Clouds Color", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.1f, 0.1f, 0.1f);
            AddBlueprintPropertyFloat("cloudsCoverage", "Clouds Coverage", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("cloudsSeed", "Clouds Seed", 0, 255, false, true);
            AddBlueprintPropertyFloat("cloudsLayer1", "Clouds Layer 1", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("cloudsLayer2", "Clouds Layer 2", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("cloudsLayer3", "Clouds Layer 3", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("cloudsSharpness", "Clouds Sharpness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("cloudsRoughness", "Clouds Roughness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("cloudsTiling", "Clouds Tiling", 1, 40, false, true);
            AddBlueprintPropertyFloat("cloudsSpeed", "Clouds Speed", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("cloudsHeight", "Clouds height", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("cloudsShadow", "Clouds Shadow", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyMaterial("lava", "Lava", PlanetManager.Instance.solidLavaMaterials.ToArray());
            AddBlueprintPropertyFloat("lavaAmount", "Lava Amount", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("lavaComplexity", "Lava Complexity", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("lavaFrequency", "Lava Frequency", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("lavaDetail", "Lava Detail", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("lavaReach", "Lava Reach", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("lavaColorVariation", "Color Variation", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("lavaFlowSpeed", "Flow Speed", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("lavaGlowAmount", "Glow Amount", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyColor("lavaGlowColor", "Glow Color", new Color(1.0f, 0.4f, 0.0f, 1.0f), 0.2f, 0.2f, 0.4f);

            AddBlueprintPropertyFloat("surfaceTiling", "Surface Tiling", 1, 30, false, true);
            AddBlueprintPropertyFloat("surfaceRoughness", "Surface Roughness", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyFloat("compositionSeed", "Composition Seed", 0, 255, false, true);
            AddBlueprintPropertyFloat("compositionTiling", "Composition Tiling", 1, 10, false, true);
            AddBlueprintPropertyFloat("compositionChaos", "Composition Chaos", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("compositionBalance", "Composition Balance", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("compositionContrast", "Composition Contrast", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyFloat("biome1Seed", "Biome 1 Seed", 0, 255, false, true);
            AddBlueprintPropertyMaterial("biome1Type", "Biome 1 Type", PlanetManager.Instance.solidBiomeMaterials.ToArray());
            AddBlueprintPropertyFloat("biome1Chaos", "Chaos", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1Balance", "Balance", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1Contrast", "Contrast", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1ColorVariation", "Color Variation", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1Saturation", "Saturation", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1Brightness", "Brightness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1SurfaceBump", "Surface Bump", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1CratersSmall", "Small Craters", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1CratersMedium", "Medium Craters", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1CratersLarge", "Large Craters", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1CratersErosion", "Crater Erosion", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1CratersDiffuse", "Craters Diffuse", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1CratersBump", "Craters Bump", 0.0f, 1.0f, true, false);
            //AddBlueprintPropertyFloat("biome1CanyonsDiffuse", "Canyons Diffuse", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome1CanyonsBump", "Canyons Bump", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyFloat("biome2Seed", "Biome 1 Seed", 0, 255, false, true);
            AddBlueprintPropertyMaterial("biome2Type", "Biome 2 Type", PlanetManager.Instance.solidBiomeMaterials.ToArray());
            AddBlueprintPropertyFloat("biome2Chaos", "Chaos", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2Balance", "Balance", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2Contrast", "Contrast", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2ColorVariation", "Color Variation", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2Saturation", "Saturation", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2Brightness", "Brightness", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2SurfaceBump", "Surface Bump", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2CratersSmall", "Small Craters", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2CratersMedium", "Medium Craters", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2CratersLarge", "Large Craters", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2CratersErosion", "Crater Erosion", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2CratersDiffuse", "Craters Diffuse", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2CratersBump", "Craters Bump", 0.0f, 1.0f, true, false);
            //AddBlueprintPropertyFloat("biome2CanyonsDiffuse", "Canyons Diffuse", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("biome2CanyonsBump", "Canyons Bump", 0.0f, 1.0f, true, false);

            AddBlueprintPropertyMaterial("cities", "Cities", PlanetManager.Instance.solidCitiesMaterials.ToArray());
            AddBlueprintPropertyFloat("citiesSeed", "Random Seed", 0, 255, false, true);
            AddBlueprintPropertyFloat("citiesPopulation", "Population", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("citiesAdvancement", "Advancement", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("citiesGlow", "Glow", 0.0f, 1.0f, true, false);
            AddBlueprintPropertyFloat("citiesTiling", "Tiling", 1, 10, false, true);
            AddBlueprintPropertyColor("citiesColor", "Night Light Color", new Color(1.0f, 1.0f, 0.95f, 1.0f), 0.05f, 0.05f, 0.05f);

            // Refresh the lists of blueprints in the Manager
            _manager.RefreshLists();                
        }
    }    
}
