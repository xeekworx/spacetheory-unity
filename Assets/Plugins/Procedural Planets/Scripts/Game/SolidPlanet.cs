using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ProceduralPlanets.SimpleJSON;
using Substance.Game;

namespace ProceduralPlanets
{
    /// <summary>
    /// Component used by solid planets. Planets are created by using the Inspector on the PlanetManager or via the static public method
    /// <seealso cref="PlanetManager.CreatePlanet(Vector3, int, string, string)"/> in PlanetManager.
    /// 
    /// SolidPlanet is derived from the base class Planet. SolidPlanet are all planets that are not gas planets.
    /// 
    /// Solid Planets are built from multiple textures:
    /// 
    /// Composition (Dictates the layout of continents, shorelines, liquid coverage, lava, polar caps, and biome coverage)<br>
    /// Biome (Each SolidPlanet has two biomes which is the surface material, e.g. Forest, Desert, Tundra, etc.)<br>
    /// Clouds (Cloud coverage of planet)<br>
    /// Cities (Night lights on the dark side of the planet)<br>
    /// Lava (Lava texture of the planet, if Lava is enabled)<br>
    /// PolarIce (Ice material that replaces biomes at the poles of a planet)<br>
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>

    // Execute in edit mode because we want to be able to change planet parameter and rebuild textures in editor
    [ExecuteInEditMode]

    // Require MeshFilter and MeshRenderer for planet
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class SolidPlanet : Planet
    {
        // Flags to indicate if maps need to be rebuilt and if shader needs to be updated with new properties
        [SerializeField] bool _rebuildMapsNeeded;
        [SerializeField] bool _rebuildBiome1Needed;
        [SerializeField] bool _rebuildBiome2Needed;
        [SerializeField] bool _rebuildCitiesNeeded;
        [SerializeField] bool _rebuildCloudsNeeded;
        [SerializeField] bool _rebuildLavaNeeded;
        [SerializeField] bool _rebuildPolarIceNeeded;
        [SerializeField] bool _rebuildLookupsNeeded;
        [SerializeField] bool _updateShaderNeeded;

        int _lodCommon = 0;
        int _lodComposition = 5;
        int _lodBiome = 5;
        int _lodCities = 5;
        int _lodClouds = 5;
        int _lodLava = 5;
        int _lodPolarIce = 5;

        // Procedural materials for planet textures
        SubstanceGraph _proceduralMaterialMaps;
        SubstanceGraph _proceduralMaterialBiome1;
        SubstanceGraph _proceduralMaterialBiome2;
        SubstanceGraph _proceduralMaterialPolarIce;
        SubstanceGraph _proceduralMaterialCities;
        SubstanceGraph _proceduralMaterialClouds;
        SubstanceGraph _proceduralMaterialLava;

        // Textures used by the planet
        Texture2D _textureMaps;
        Texture2D _textureBiome1DiffSpec;
        Texture2D _textureBiome1Normal;
        Texture2D _textureBiome2DiffSpec;
        Texture2D _textureBiome2Normal;
        Texture2D _textureIceDiffuse;
        Texture2D _textureCities;
        Texture2D _textureClouds;
        Texture2D _textureLavaDiffuse;
        Texture2D _textureLavaFlow;

        // Lookup textures for fast shader lookup of liquid, lava and polar cap coverage
        Texture2D _textureLookupLiquid;
        Texture2D _textureLookupLava;
        Texture2D _textureLookupLavaGlow;
        Texture2D _textureLookupPolar;

        // Materials
        Material _material;
        Material _externalAtmosphereMaterial;

        /// <summary>
        /// Gets if the planet is currently generating any procedural textures.
        /// Public so any script can query if planet is currently processing textures. 
        /// </summary>
        /// <returns>True if any planet texture is being processed or False if no planet textures are being processed.</returns>
        public override bool IsBuildingTextures
        {
            get
            {
                // TODO: FIX IS PROCESSING
                return false;
                /*
                if (_proceduralMaterialBiome1 != null)
                    if (_proceduralMaterialBiome1.isProcessing) return true;
                if (_proceduralMaterialBiome2 != null)
                    if (_proceduralMaterialBiome2.isProcessing) return true;
                if (_proceduralMaterialCities != null)
                    if (_proceduralMaterialCities.isProcessing) return true;
                if (_proceduralMaterialClouds != null)
                    if (_proceduralMaterialClouds.isProcessing) return true;
                if (_proceduralMaterialPolarIce != null)
                    if (_proceduralMaterialPolarIce.isProcessing) return true;
                if (_proceduralMaterialLava != null)
                    if (_proceduralMaterialLava.isProcessing) return true;
                if (_proceduralMaterialMaps != null)
                    if (_proceduralMaterialMaps.isProcessing) return true;
                return false;
                */
            }
        }

        /// <summary>
        /// Sets the planet blueprint based on the index value (order of the GameObject of under the PlanetManager gameobject). 
        /// Defaults to seed selected index. Optionally leave overridden values as is.
        /// Public because Manager calls this when creating a new planet.
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_leaveOverride"></param>        
        public override void SetPlanetBlueprint(int _index = -1, bool _leaveOverride = false, bool _setProperties = true)
        {
            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("SolidPlanet.cs: SetPlanetBlueprint(" + _index + "," + _leaveOverride + "," + _setProperties + ")");

            if (_index == -1)
            {
                // If index is set to -1 (default) the random seed will determine planet blueprint.
                planetBlueprintIndex = GetPlanetBlueprintSeededIndex();
                // Set override flag to false since random seed is determining planet blueprint.
                planetBlueprintOverride = false;
            }
            else
            {
                // Set planet blueprint to a specific index value.
                planetBlueprintIndex = _index;
                // Set override flag to true since we are overriding the planet blueprint.
                planetBlueprintOverride = true;
            }

            blueprint = PlanetManager.Instance.GetPlanetBlueprintByIndex(planetBlueprintIndex, this);

            if (_setProperties)
            {
                // Set the default properties for the planet (and forward the override flag)
                SetDefaultProperties(_leaveOverride);

                // Rebuild textures (force rebuild of all textures)
                RebuildTextures(true);

                // Set flag to ensure shader is updated
                _updateShaderNeeded = true;
            }

            serializedPlanetCache = ExportToJSON(StringFormat.JSON_COMPACT);
        }

        /// <summary>
        /// Creates the planet with serialized parameters (this happens every play/stop in editor)
        /// </summary>
        protected override void Awake()
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("SolidPlanet.cs: Awake()");
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("- PlanetVersion: " + PLANET_VERSION);

            // Set Shader property int IDs for increased performance when updating property parameters
            _shaderID_LocalStarPosition = Shader.PropertyToID("_LocalStarPosition");
            _shaderID_LocalStarColor = Shader.PropertyToID("_LocalStarColor");
            _shaderID_LocalStarIntensity = Shader.PropertyToID("_LocalStarIntensity");
            _shaderID_LocalStarAmbientIntensity = Shader.PropertyToID("_LocalStarAmbientIntensity");

            if (planetBlueprintIndex != -1 || blueprint == null)
                SetPlanetBlueprint(planetBlueprintIndex, true, false);

            // Ensure that there is a LocalStar in the scene.
            if (FindObjectOfType<LocalStar>() == null)
                Debug.LogWarning("There is no LocalStar in the scene. Planet will not be lit. Create a game object and add the LocalStar component. The position of the game object will be the light source.");

            // Get reference to the MeshFilter component
            meshFilter = gameObject.GetComponent<MeshFilter>();

            // Force initialization of PlanetManager if it is not yet initialized. We need the procedural mesh.
            if (!PlanetManager.IsInitialized || PlanetManager.MeshLODMeshes == null || PlanetManager.MeshLODMeshes.Length < meshLODLevel)
                PlanetManager.Initialize();

            // Use Mesh with appropriate Level of Detail (LOD) from Manager
            meshFilter.sharedMesh = PlanetManager.MeshLODMeshes[meshLODLevel];

            // Get reference to MeshRenderer Component
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();

            // Create the planet material and set the material for the MeshRenderer component
            if (_material == null)
            {
                if (QualitySettings.activeColorSpace == ColorSpace.Linear)
                    _material = new Material(Shader.Find("ProceduralPlanets/SolidPlanetLinear"));
                else
                    _material = new Material(Shader.Find("ProceduralPlanets/SolidPlanetGamma"));

                _meshRenderer.material = _material;
            }

            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                _material.shader = Shader.Find("ProceduralPlanets/SolidPlanetLinear");
            }                            
            else
            {
                _material.shader = Shader.Find("ProceduralPlanets/SolidPlanetGamma");
            }
                

            // Create or get reference to external atmosphere gameobject
            if (transform.Find("ExternalAtmosphere") == null)
                _externalAtmosphere = new GameObject("ExternalAtmosphere");
            else
                _externalAtmosphere = transform.Find("ExternalAtmosphere").gameObject;

            // Parent atmosphere to planet and hide in hierarchy
            _externalAtmosphere.transform.parent = transform;
            _externalAtmosphere.gameObject.layer = gameObject.layer;
            _externalAtmosphere.transform.localPosition = Vector3.zero;
            _externalAtmosphere.gameObject.hideFlags = HideFlags.HideInHierarchy;
            //_externalAtmosphere.gameObject.hideFlags = HideFlags.None;

            // Create or get reference to atmosphere MeshFilter Component
            if (_externalAtmosphere.GetComponent<MeshFilter>() == null)
                _externalAtmosphereMeshFilter = _externalAtmosphere.AddComponent<MeshFilter>();
            else
                _externalAtmosphereMeshFilter = _externalAtmosphere.GetComponent<MeshFilter>();

            // Use the planet's procedural octahedron sphere mesh as the atmosphere mesh as well
            _externalAtmosphereMeshFilter.sharedMesh = meshFilter.sharedMesh;

            // Create external atmosphere material
            if (_externalAtmosphereMaterial == null)
                _externalAtmosphereMaterial = new Material(Shader.Find("ProceduralPlanets/Atmosphere"));

            // Create or get reference to atmosphere MeshRenderer Component
            if (_externalAtmosphere.GetComponent<MeshRenderer>() == null)
                _externalAtmosphereRenderer = _externalAtmosphere.AddComponent<MeshRenderer>();
            else
                _externalAtmosphereRenderer = _externalAtmosphere.GetComponent<MeshRenderer>();

            // Set atmosphere material
            _externalAtmosphereRenderer.sharedMaterial = _externalAtmosphereMaterial;

            // Disable shadows for atmosphere
            _externalAtmosphereRenderer.receiveShadows = false;
            _externalAtmosphereRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // Clear properties lists
            propertyFloats.Clear();
            propertyMaterials.Clear();
            propertyColors.Clear();

            // Add property materials
            AddPropertyMaterial("composition", "Composition*", PlanetManager.Instance.solidCompositionMaterials.ToArray(), 1, new string[] { "Maps" });
            AddPropertyMaterial("polarIce", "Polar Ice", PlanetManager.Instance.solidPolarIceMaterials.ToArray(), 4321, new string[] { "PolarIce" });
            AddPropertyMaterial("clouds", "Clouds*", PlanetManager.Instance.solidCloudsMaterials.ToArray(), 1, new string[] { "Clouds" });
            AddPropertyMaterial("lava", "Lava*", PlanetManager.Instance.solidLavaMaterials.ToArray(), 1821, new string[] { "Lava" });
            AddPropertyMaterial("biome1Type", "Biome 1 Type*", PlanetManager.Instance.solidBiomeMaterials.ToArray(), 455, new string[] { "Biome1" });
            AddPropertyMaterial("biome2Type", "Biome 2 Type*", PlanetManager.Instance.solidBiomeMaterials.ToArray(), 615, new string[] { "Biome2" });
            AddPropertyMaterial("cities", "Cities*", PlanetManager.Instance.solidCitiesMaterials.ToArray(), 1, new string[] { "Cities" });

            // Update dictionaries (for materials a this stage)
            UpdateDictionariesIfNeeded(true);

            // Set default properties (for materials at this stage)
            SetDefaultProperties();
            
            // Get references to newly created property materials
            _proceduralMaterialMaps = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["composition"].GetPropertyMaterial(), gameObject, "composition");
            _proceduralMaterialBiome1 = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["biome1Type"].GetPropertyMaterial(), gameObject, "biome1Type");
            _proceduralMaterialBiome2 = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["biome2Type"].GetPropertyMaterial(), gameObject, "biome2Type");
            _proceduralMaterialPolarIce = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["polarIce"].GetPropertyMaterial(), gameObject, "polarIce");
            _proceduralMaterialLava = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["lava"].GetPropertyMaterial(), gameObject, "lava");
            _proceduralMaterialCities = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["cities"].GetPropertyMaterial(), gameObject, "cities");
            _proceduralMaterialClouds = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["clouds"].GetPropertyMaterial(), gameObject, "clouds");                

            // Add Float (within a range of min/max) and color properties
            AddPropertyFloat("alienization", "Alienization*", 0.0f, 1.0f, true, false, 2, false, new string[] { "Biome1", "Biome2" }, new SubstanceGraph[] { _proceduralMaterialBiome1, _proceduralMaterialBiome2 }, "Biome_Alienization", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyColor("specularColor", "Specular Color", new Color(0.4f, 0.4f, 0.05f, 1.0f), 0.05f, 0.05f, 0.05f, 5, false, _material, "_ColorSpecular");
            AddPropertyFloat("continentSeed", "Continent Seed*", 0, 255, false, true, 10, true, new string[] { "Maps" }, _proceduralMaterialMaps, "MapHeight_Random_Seed", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("continentSize", "Continent Size", 10, 1, true, false, 30, false, null, _material, "_TilingHeightBase", PropertyFloat.Method.LERP, PropertyFloat.DataType.INT);
            AddPropertyFloat("continentComplexity", "Continent Complexity*", 0.0f, 20.0f, true, false, 20, false, new string[] { "Maps" }, _proceduralMaterialMaps, "MapHeight_Warp", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("coastalDetail", "Coastal Detail", 1, 50, true, false, 40, false, null, _material, "_TilingHeightDetail", PropertyFloat.Method.LERP, PropertyFloat.DataType.INT);
            AddPropertyFloat("coastalReach", "Coastal Reach", 0.0f, 0.2f, true, false, 50, false, null, _material, "_DetailHeight", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("liquidLevel", "Liquid Level", 0.0f, 1.0f, true, false, 60, true, new string[] { "Lookups" });
            AddPropertyColor("liquidColor", "LiquidColor", new Color(0.0f, 0.0f, 0.3f, 1.0f), 0.02f, 0.3f, 0.3f, 70, true, _material, "_ColorLiquid");
            AddPropertyFloat("liquidOpacity", "Liquid Opacity", 0.0f, 1.0f, true, false, 80, false, null, _material, "_LiquidOpacity", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("liquidShallow", "Shallow Distance", 0.0f, 1.0f, true, false, 90, false, new string[] { "Lookups" });
            AddPropertyFloat("liquidSpecularPower", "Specular Power", 1.0f, 50.0f, true, false, 100, false, null, _material, "_SpecularPowerLiquid", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("polarCapAmount", "Polar Caps", 1.0f, 0.2f, true, false, 110, true, new string[] { "Lookups" });
            AddPropertyColor("iceColor", "Ice Color", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.1f, 0.1f, 0.1f, 120, true, _material, "_ColorIce");
            AddPropertyColor("atmosphereColor", "Atmosphere Color", new Color(0.2f, 0.75f, 1.0f, 1.0f), 0.2f, 0.2f, 0.2f, 130, true, new Material[] { _material, _externalAtmosphereMaterial }, "_ColorAtmosphere");
            AddPropertyFloat("atmosphereExternalSize", "External Size", 0.0f, 0.5f, true, false, 140, true);
            AddPropertyFloat("atmosphereExternalDensity", "External Density", 1.7f, 1.2f, true, false, 140, true);
            AddPropertyFloat("atmosphereInternalDensity", "Internal Density", 20.0f, 3.0f, true, false, 150, true, null, _material, "_AtmosphereFalloff", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyColor("twilightColor", "Twilight Color", new Color(0.25f, 0.2f, 0.05f, 1.0f), 0.05f, 0.2f, 0.2f, 160, true, _material, "_ColorTwilight");
            AddPropertyFloat("cloudsOpacity", "Clouds Opacity", 0.0f, 1.0f, true, false, 170, false, null, _material, "_CloudOpacity", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyColor("cloudsColor", "Clouds Color", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.1f, 0.1f, 0.1f, 180, true, _material, "_ColorClouds");
            AddPropertyFloat("cloudsSeed", "Clouds Seed*", 0, 255, false, true, 200, true, new string[] { "Clouds" }, _proceduralMaterialClouds, "$randomseed", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("cloudsCoverage", "Clouds Coverage*", 1.0f, 0.2f, true, false, 190, true, new string[] { "Clouds" }, _proceduralMaterialClouds, "Coverage", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("cloudsLayer1", "Clouds Layer 1*", 0.0f, 1.0f, true, false, 210, true, new string[] { "Clouds" }, _proceduralMaterialClouds, "Layer1_Opacity", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("cloudsLayer2", "Clouds Layer 2*", 0.0f, 1.0f, true, false, 220, true, new string[] { "Clouds" }, _proceduralMaterialClouds, "Layer2_Opacity", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("cloudsLayer3", "Clouds Layer 3*", 0.0f, 1.0f, true, false, 230, true, new string[] { "Clouds" }, _proceduralMaterialClouds, "Layer3_Opacity", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("cloudsSharpness", "Clouds Sharpness*", 0.0f, 1.0f, true, false, 240, false, new string[] { "Clouds" }, _proceduralMaterialClouds, "Sharpness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("cloudsRoughness", "Clouds Roughness*", 0.0f, 1.0f, true, false, 250, false, new string[] { "Clouds" }, _proceduralMaterialClouds, "Roughness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("cloudsTiling", "Clouds Tiling", 1, 40, false, true, 260, false, null, _material, "_TilingClouds", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("cloudsSpeed", "Clouds Speed", 0.0f, 50.0f, true, false, 270, false, null, _material, "_CloudSpeed", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("cloudsHeight", "Clouds height", 0.0f, 15.0f, true, false, 280, false, null, _material, "_CloudHeight", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("cloudsShadow", "Clouds Shadow", 0.0f, 1.0f, true, false, 290, false, null, _material, "_CloudShadow", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("lavaAmount", "Lava Amount", 0.0f, 1f, true, false, 300, true, new string[] { "Lookups" });
            AddPropertyFloat("lavaComplexity", "Lava Complexity*", 10.0f, 20.0f, true, true, 310, true, new string[] { "Maps" }, _proceduralMaterialMaps, "MapLava_Warp", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("lavaFrequency", "Lava Frequency", 1, 10, true, false, 320, true, null, _material, "_TilingLavaBase", PropertyFloat.Method.LERP, PropertyFloat.DataType.INT);
            AddPropertyFloat("lavaDetail", "Lava Detail", 1, 40, true, false, 330, true, null, _material, "_TilingLavaDetail", PropertyFloat.Method.LERP, PropertyFloat.DataType.INT);
            AddPropertyFloat("lavaReach", "Lava Reach", 0.0f, 0.3f, true, false, 340, true, null, _material, "_DetailLava", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("lavaColorVariation", "Color Variation*", 0.48f, 0.6f, true, false, 350, true, new string[] { "Lava" }, _proceduralMaterialLava, "Lava_Hue", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("lavaFlowSpeed", "Flow Speed", 0.0f, 1.0f, true, false, 360, false, null, _material, "_LavaFlowSpeed", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("lavaGlowAmount", "Glow Amount", 0.0f, 0.05f, true, false, 370, true, new string[] { "Lookups" });
            AddPropertyColor("lavaGlowColor", "Glow Color", new Color(1.0f, 0.4f, 0.0f, 1.0f), 0.2f, 0.2f, 0.4f, 380, true, _material, "_ColorLavaGlow");
            AddPropertyFloat("surfaceTiling", "Surface Tiling", 1, 30, false, true, 390, false, null, _material, "_TilingSurface", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("surfaceRoughness", "Surface Roughness", 1.0f, 0.1f, true, false, 400, false, null, _material, "_SurfaceRoughness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("compositionSeed", "Composition Seed*", 0, 255, false, true, 10, true, new string[] { "Maps" }, _proceduralMaterialMaps, "MapBiome_Random_Seed", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("compositionTiling", "Composition Tiling", 1, 10, false, true, 410, true, null, _material, "_TilingBiome", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("compositionChaos", "Composition Chaos*", 1.0f, 10.0f, true, false, 420, true, new string[] { "Maps" }, _proceduralMaterialMaps, "MapBiome_Warp", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("compositionBalance", "Composition Balance*", 0.0f, 1.0f, true, false, 430, true, new string[] { "Maps" }, _proceduralMaterialMaps, "MapBiome_Balance", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("compositionContrast", "Composition Contrast*", 0.0f, 1.0f, true, false, 440, true, new string[] { "Maps" }, _proceduralMaterialMaps, "MapBiome_Contrast", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1Seed", "Biome 1 Seed*", 0, 255, false, true, 450, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "$randomseed", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("biome1Chaos", "Chaos*", 0.0f, 10.0f, true, false, 460, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Biome_Coverage_Warp", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1Balance", "Balance*", 0.0f, 1.0f, true, false, 470, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Biome_Coverage_Balance", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1Contrast", "Contrast*", 0.0f, 1.0f, true, false, 480, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Biome_Coverage_Contrast", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1ColorVariation", "Color Variation*", 0.0f, 1.0f, true, false, 490, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Biome_Hue", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1Saturation", "Saturation*", 0.0f, 1.0f, true, false, 500, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Biome_Saturation", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1Brightness", "Brightness*", 0.3f, 0.7f, true, false, 510, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Biome_Brightness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1SurfaceBump", "Surface Bump*", 0.0f, 0.3f, true, false, 520, false, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Normal_Strength_Surface", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1CratersSmall", "Small Craters*", 0.0f, 1.0f, true, false, 530, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Craters_Small", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1CratersMedium", "Medium Craters*", 0.0f, 1.0f, true, false, 540, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Craters_Medium", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1CratersLarge", "Large Craters*", 0.0f, 1.0f, true, false, 550, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Craters_Large", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1CratersErosion", "Craters Erosion*", 0.0f, 1.0f, true, false, 560, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Crater_Erosion", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1CratersDiffuse", "Craters Diffuse*", 0.0f, 1.0f, true, false, 570, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Diffuse_Strength_Craters", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1CratersBump", "Craters Bump*", 0.0f, 1.0f, true, false, 580, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Normal_Strength_Craters", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            //AddPropertyFloat("biome1CanyonsDiffuse", "Canyons Diffuse*", 0.0f, 1.0f, true, false, 590, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Diffuse_Strength_Canyons", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome1CanyonsBump", "Canyons Bump*", 0.0f, 0.3f, true, false, 600, true, new string[] { "Biome1" }, _proceduralMaterialBiome1, "Normal_Strength_Canyons", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2Seed", "Biome 2 Seed*", 0, 255, false, true, 610, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "$randomseed", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("biome2Chaos", "Chaos*", 0.0f, 10.0f, true, false, 620, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Biome_Coverage_Warp", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2Balance", "Balance*", 0.0f, 1.0f, true, false, 630, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Biome_Coverage_Balance", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2Contrast", "Contrast*", 0.0f, 1.0f, true, false, 640, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Biome_Coverage_Contrast", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2ColorVariation", "Color Variation*", 0.0f, 1.0f, true, false, 650, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Biome_Hue", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2Saturation", "Saturation*", 0.0f, 1.0f, true, false, 660, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Biome_Saturation", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2Brightness", "Brightness*", 0.3f, 0.7f, true, false, 670, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Biome_Brightness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2SurfaceBump", "Surface Bump*", 0.0f, 0.3f, true, false, 680, false, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Normal_Strength_Surface", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2CratersSmall", "Small Craters*", 0.0f, 1.0f, true, false, 690, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Craters_Small", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2CratersMedium", "Medium Craters*", 0.0f, 1.0f, true, false, 700, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Craters_Medium", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2CratersLarge", "Large Craters*", 0.0f, 1.0f, true, false, 710, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Craters_Large", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2CratersErosion", "Craters Erosion*", 0.0f, 1.0f, true, false, 720, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Crater_Erosion", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2CratersDiffuse", "Craters Diffuse*", 0.0f, 1.0f, true, false, 730, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Diffuse_Strength_Craters", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2CratersBump", "Craters Bump*", 0.0f, 1.0f, true, false, 740, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Normal_Strength_Craters", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            //AddPropertyFloat("biome2CanyonsDiffuse", "Canyons Diffuse*", 0.0f, 1.0f, true, false, 750, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Diffuse_Strength_Canyons", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("biome2CanyonsBump", "Canyons Bump*", 0.0f, 0.3f, true, false, 760, true, new string[] { "Biome2" }, _proceduralMaterialBiome2, "Normal_Strength_Canyons", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("citiesSeed", "Random Seed*", 0, 255, false, true, 954, true, new string[] { "Cities" }, _proceduralMaterialCities, "$randomseed", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("citiesPopulation", "Population*", 0.0f, 0.25f, true, false, 780, true, new string[] { "Cities" }, _proceduralMaterialCities, "Population", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("citiesAdvancement", "Advancement*", 0.0f, 1.0f, true, false, 790, true, new string[] { "Cities" }, _proceduralMaterialCities, "Advancement", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("citiesGlow", "Glow*", 0.0f, 1.0f, true, false, 925, true, new string[] { "Cities" }, _proceduralMaterialCities, "Glow", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("citiesTiling", "Tiling", 1, 10, false, true, 800, false, null, _material, "_TilingCities", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyColor("citiesColor", "Night Light Color", new Color(1.0f, 1.0f, 0.95f, 1.0f), 0.05f, 0.05f, 0.05f, 810, true, _material, "_ColorCities");

            // Update dictionaries (again, now with all Float and Color properties too) 
            UpdateDictionariesIfNeeded(true);

            // Set default properties based on seed (this time for all properties)
            SetDefaultProperties();

           
            if (initJSONSettings != "")
            {
                // If initJSON string is set, configure planet according to the init string
                ImportFromJSON(initJSONSettings);
                initJSONSettings = "";
            }
            else
            {
                // Load planet settings from cache (if this is not a new planet) - this overwrites default settings if changes have been made
                if (serializedPlanetCache != null)
                    if (serializedPlanetCache.Length > 0)
                        ImportFromJSON(serializedPlanetCache, false);
            }

            // Update lookup textures (e.g. create lookup textures for liquid level so shader knows where on hight map to apply water)
            UpdateLookupTextures();

            // Update shader for planet lighting
            UpdateShaderLocalStar(true);

            // Force rebuild of planet textures to use all the correct properties for the planet
            RebuildTextures(true);

            _updateShaderNeeded = true;
        }


        /// <summary>
        /// Updates local star position and checks if any textures need to be rebuilt. This happens every frame.
        /// </summary>
        override protected void Update()
        {            
            // Update local star position
            UpdateShaderLocalStar(false);

            // Rebuild any planet textures where procedural properties have been changed. This only executes if autoUpdateTextures is set in ProceduralPlanetManager.
            if (RebuildTexturesNeeded()  && !IsBuildingTextures)
                RebuildTextures();

            // Update shader properties if any shader related properties have been changed
            if (_updateShaderNeeded) UpdateShader();

            if (!IsBuildingTextures && _timerStartBuildingTextures > 0)
            {
                TriggerOnTextureBuildComplete(Time.realtimeSinceStartup - _timerStartBuildingTextures);
                _timerStartBuildingTextures = -1;
            }

            if (IsBuildingTextures && _timerStartBuildingTextures < 0)
            {
                _timerStartBuildingTextures = Time.realtimeSinceStartup;
                TriggerOnTextureBuildStart(Time.realtimeSinceStartup);
            }

            UpdateLODMeshIfNeeded();
            UpdateLODTextureIfNeeded();
        }

        /// <summary>
        /// Updates Level of Detail (LOD) Mesh if needed. This is queried every frame via Update().
        /// </summary>
        void UpdateLODMeshIfNeeded()
        {
            int _appropriateLODLevel = PlanetManager.Instance.GetAppropriateMeshLODLevel(GetLODPercent());

            if (_appropriateLODLevel != meshLODLevel)
            {
                // Mesh doesn't exist yet, wait for a bit and let it be created - it should update soon
                if (_appropriateLODLevel >= PlanetManager.MeshLODMeshes.Length)
                    return;
                
                meshLODLevel = _appropriateLODLevel;
                if ((int) PlanetManager.DebugLevel > 0) Debug.Log("SolidPlanet.cs: UpdateLODMeshIfNeeded() Setting mesh LOD level to: " + meshLODLevel);
                SetSharedMesh(PlanetManager.MeshLODMeshes[meshLODLevel]);
            }
        }

        /// <summary>
        /// Updates Level of Detail (LOD) Texture if needed. This is queried every frame via Update().
        /// Public because PlanetManager updates textures if necessary.
        /// </summary>
        /// <param name="_force"></param>
        public void UpdateLODTextureIfNeeded(bool _force = false)
        {            
            switch (PlanetManager.TextureDetailMode)
            {
                case PlanetManager.TextureDetailModes.Static:
                    if (_lodCommon != PlanetManager.TextureStaticCommon || _force)
                    {                        
                        _lodCommon = PlanetManager.TextureStaticCommon;
                        if ((int) PlanetManager.DebugLevel > 0) Debug.Log("SolidPlanet.cs: UpdateLODTextureIfNeeded() Setting texture static LOD level to: " + _lodCommon);
                        _lodComposition = _lodCommon;
                        _lodBiome = _lodCommon;
                        _lodCities = _lodCommon;
                        _lodClouds = _lodCommon;
                        _lodLava = _lodCommon;
                        _lodPolarIce = _lodCommon;
                        RebuildTextures(true);
                    }
                    break;

                case PlanetManager.TextureDetailModes.Static_Separate:

                    bool _flag = false;

                    if (_lodComposition != PlanetManager.TextureStaticComposition || _force)
                    {
                        _lodComposition = PlanetManager.TextureStaticComposition;
                        _flag = true;
                    }
                        

                    if (_lodBiome != PlanetManager.TextureStaticBiome || _force)
                    {
                        _lodBiome = PlanetManager.TextureStaticBiome;
                        _flag = true;
                    }

                    if (_lodClouds != PlanetManager.TextureStaticClouds || _force)                        
                    {
                        _lodClouds = PlanetManager.TextureStaticClouds;
                        _flag = true;
                    }

                    if (_lodCities != PlanetManager.TextureStaticCities || _force)
                    {
                        _lodCities = PlanetManager.TextureStaticCities;
                        _flag = true;
                    }

                    if (_lodLava != PlanetManager.TextureStaticLava || _force)
                    {
                        _lodLava = PlanetManager.TextureStaticLava;
                        _flag = true;
                    }

                    if (_lodPolarIce != PlanetManager.TextureStaticPolarIce || _force)
                    {
                        _lodPolarIce = PlanetManager.TextureStaticPolarIce;
                        _flag = true;
                    }

                    if (_flag && (int) PlanetManager.DebugLevel > 0) Debug.Log("SolidPlanet.cs: UpdateLODTextureIfNeeded() Setting texture static separate LOD levels to: Composition: " + _lodComposition + ",  Biome: " + _lodBiome + ", Clouds: " + _lodClouds + ", Cities: " + _lodCities + ", Lava: " + _lodLava + ", PolarIce: " + _lodPolarIce);

                    RebuildTextures(true);                    
                    break;

                case PlanetManager.TextureDetailModes.Progressive:

                    if (textureProgressiveStep >= PlanetManager.TextureProgressiveSteps - 1)
                        return;

                    if (IsBuildingTextures)
                        return;

                    textureProgressiveStep++;

                    if (textureProgressiveStep < 0)
                        return;

                    if ((int) PlanetManager.DebugLevel > 0) Debug.Log("UpdateLODTextureIfNeeded() - Setting Common Progressive Texture Step: " + textureProgressiveStep);

                    if (_lodCommon != PlanetManager.TextureProgressiveCommon[textureProgressiveStep] || _force)
                    {
                        _lodCommon = PlanetManager.TextureProgressiveCommon[textureProgressiveStep];
                        _lodComposition = _lodCommon;
                        _lodBiome = _lodCommon;
                        _lodCities = _lodCommon;
                        _lodClouds = _lodCommon;
                        _lodLava = _lodCommon;
                        _lodPolarIce = _lodCommon;
                        RebuildTextures(true);
                    }

                    break;

                case PlanetManager.TextureDetailModes.Progressive_Separate:

                    if (textureProgressiveStep >= PlanetManager.TextureProgressiveSteps -1)
                        return;

                    if (IsBuildingTextures)
                        return;

                    textureProgressiveStep++;                    

                    if (textureProgressiveStep < 0)
                        return;

                    if ((int) PlanetManager.DebugLevel > 0) Debug.Log("UpdateLODTextureIfNeeded() - Setting Separate Progressive Texture Step: " + textureProgressiveStep);

                    if (_lodComposition != PlanetManager.TextureProgressiveComposition[textureProgressiveStep] || _force)
                        _lodComposition = PlanetManager.TextureProgressiveComposition[textureProgressiveStep];                    

                    if (_lodBiome != PlanetManager.TextureProgressiveBiome[textureProgressiveStep] || _force)
                        _lodBiome = PlanetManager.TextureProgressiveBiome[textureProgressiveStep];

                    if (_lodClouds != PlanetManager.TextureProgressiveClouds[textureProgressiveStep] || _force)
                        _lodClouds = PlanetManager.TextureProgressiveClouds[textureProgressiveStep];

                    if (_lodCities != PlanetManager.TextureProgressiveCities[textureProgressiveStep] || _force)
                        _lodCities = PlanetManager.TextureProgressiveCities[textureProgressiveStep];

                    if (_lodLava != PlanetManager.TextureProgressiveLava[textureProgressiveStep] || _force)
                        _lodLava = PlanetManager.TextureProgressiveLava[textureProgressiveStep];

                    if (_lodPolarIce != PlanetManager.TextureProgressivePolarIce[textureProgressiveStep] || _force)
                        _lodPolarIce = PlanetManager.TextureProgressivePolarIce[textureProgressiveStep];

                    RebuildTextures(true);

                    break;

                case PlanetManager.TextureDetailModes.LOD:
                    {
                        int _appropriateTextureLODLevel = PlanetManager.Instance.GetAppropriateTextureLODLevel(GetLODPercent());
                        if (textureLODLevel != _appropriateTextureLODLevel)
                        {                            
                            textureLODLevel = _appropriateTextureLODLevel;
                            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("UpdateLODTextureIfNeeded() - Setting Common Texture LOD Level: " + textureLODLevel);
                            if (_lodCommon != textureLODLevel || _force)
                            {
                                _lodCommon = PlanetManager.TextureLODCommon[textureLODLevel];
                                _lodComposition = _lodCommon;
                                _lodBiome = _lodCommon;
                                _lodCities = _lodCommon;
                                _lodClouds = _lodCommon;
                                _lodLava = _lodCommon;
                                _lodPolarIce = _lodCommon;
                                RebuildTextures(true);
                            }
                        }
                    }
                    break;

                case PlanetManager.TextureDetailModes.LOD_Separate:
                    {
                        int _appropriateTextureLODLevel = PlanetManager.Instance.GetAppropriateTextureLODLevel(GetLODPercent());
                        if (textureLODLevel != _appropriateTextureLODLevel)
                        {
                            textureLODLevel = _appropriateTextureLODLevel;
                            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("UpdateLODTextureIfNeeded() - Setting Separate Texture LOD Level: " + textureLODLevel);

                            if (_lodComposition != PlanetManager.TextureLODComposition[textureLODLevel] || _force)
                                _lodComposition = PlanetManager.TextureLODComposition[textureLODLevel];

                            if (_lodBiome != PlanetManager.TextureLODBiome[textureLODLevel] || _force)
                                _lodBiome = PlanetManager.TextureLODBiome[textureLODLevel];

                            if (_lodClouds != PlanetManager.TextureLODClouds[textureLODLevel] || _force)
                                _lodClouds = PlanetManager.TextureLODClouds[textureLODLevel];

                            if (_lodCities != PlanetManager.TextureLODCities[textureLODLevel] || _force)
                                _lodCities = PlanetManager.TextureLODCities[textureLODLevel];

                            if (_lodLava != PlanetManager.TextureLODLava[textureLODLevel] || _force)
                                _lodLava = PlanetManager.TextureLODLava[textureLODLevel];

                            if (_lodPolarIce != PlanetManager.TextureLODPolarIce[textureLODLevel] || _force)
                                _lodPolarIce = PlanetManager.TextureLODPolarIce[textureLODLevel];

                            RebuildTextures(true);
                        }
                    }
                    break;

            }
            
        }

        void SetDefaultProgressiveTextureLOD()
        {            
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("SolidPlanet.cs: SetDefaultProgressiveTextureLOD(): Progressive Step 0");

            textureProgressiveStep = 0;
            if (PlanetManager.TextureDetailMode == PlanetManager.TextureDetailModes.Progressive)
            {
                _lodCommon = PlanetManager.TextureProgressiveCommon[textureProgressiveStep];
                _lodComposition = _lodCommon;
                _lodBiome = _lodCommon;
                _lodCities = _lodCommon;
                _lodClouds = _lodCommon;
                _lodLava = _lodCommon;
                _lodPolarIce = _lodCommon;
            }


            if (PlanetManager.TextureDetailMode == PlanetManager.TextureDetailModes.Progressive_Separate)
            {                
                _lodComposition = PlanetManager.TextureProgressiveComposition[textureProgressiveStep];
                _lodBiome = PlanetManager.TextureProgressiveBiome[textureProgressiveStep];
                _lodCities = PlanetManager.TextureProgressiveCities[textureProgressiveStep];
                _lodClouds = PlanetManager.TextureProgressiveClouds[textureProgressiveStep];
                _lodLava = PlanetManager.TextureProgressiveLava[textureProgressiveStep];
                _lodPolarIce = PlanetManager.TextureProgressivePolarIce[textureProgressiveStep];
            }
            
        }
       

        /// <summary>
        /// Determines if any procedural texture needs to be rebuilt based on flags. 
        /// Public because the editor script calls this as well.
        /// </summary>
        /// <returns></returns>
        public bool RebuildTexturesNeeded()
        {
            if (_rebuildBiome1Needed || _rebuildBiome2Needed || _rebuildCitiesNeeded || _rebuildCloudsNeeded || _rebuildMapsNeeded || _rebuildLavaNeeded || _rebuildPolarIceNeeded || _rebuildLookupsNeeded) return true;
            return false;
        }

        /// <summary>
        /// Rebuilds textures for procedural materials where properties have been changed (or alternatively all texture if force parameter is set to true).
        /// Public because the editor script calls this as well.
        /// </summary>
        /// <param name="_force"></param>
        public void RebuildTextures(bool _force = false)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("SolidPlanet.cs: RebuildTextures(" + _force + ")");

            // Ensure that the dictionaries are updated (force flag is set to true for assurance)
            UpdateDictionariesIfNeeded(true);

            // If force flag was set, update all procedural textures for the planet.
            if (_force)
            {
                UpdateProceduralTexture("All");
                UpdateLookupTextures();
                _rebuildMapsNeeded = false;
                _rebuildBiome1Needed = false;
                _rebuildBiome2Needed = false;
                _rebuildCitiesNeeded = false;
                _rebuildCloudsNeeded = false;
                _rebuildLavaNeeded = false;
                _rebuildPolarIceNeeded = false;
                _rebuildLookupsNeeded = false;
            }

            // Update individual procedural textures if needed
            if (_rebuildMapsNeeded)
            {
                UpdateProceduralTexture("Maps");
                _rebuildMapsNeeded = false;
            }
            if (_rebuildBiome1Needed)
            {
                UpdateProceduralTexture("Biome1");
                _rebuildBiome1Needed = false;
            }
            if (_rebuildBiome2Needed)
            {
                UpdateProceduralTexture("Biome2");
                _rebuildBiome2Needed = false;
            }
            if (_rebuildCitiesNeeded)
            {
                UpdateProceduralTexture("Cities");
                _rebuildCitiesNeeded = false;
            }
            if (_rebuildCloudsNeeded)
            {
                UpdateProceduralTexture("Clouds");
                _rebuildCloudsNeeded = false;
            }
            if (_rebuildLavaNeeded)
            {
                UpdateProceduralTexture("Lava");
                _rebuildLavaNeeded = false;
            }

            if (_rebuildPolarIceNeeded)
            {
                UpdateProceduralTexture("PolarIce");
                _rebuildLavaNeeded = false;
            }

            if (_rebuildLookupsNeeded)
            {
                UpdateLookupTextures();
                _rebuildLookupsNeeded = false;
            }

            if (IsBuildingTextures && _timerStartBuildingTextures < 0)
                _timerStartBuildingTextures = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Updates procedural textures. 
        /// Array of float values for the texture is included as well as reference to the (this) object that the texture should be sent to.
        /// Public becasue PlanetManagerEditor needs to access this method to force updating of procedural textures.
        /// </summary>
        /// <param name="_textureName">Name of texture to be rebuilt (or "All")</param>
        public override void UpdateProceduralTexture(string _textureName)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("SolidPlanet.cs: UpdateProceduralTexture(" + _textureName + ")");

            if (PlanetManager.TextureDetailMode == PlanetManager.TextureDetailModes.Progressive ||
                PlanetManager.TextureDetailMode == PlanetManager.TextureDetailModes.Progressive_Separate)
            {
                if (textureProgressiveStep == -1)
                    SetDefaultProgressiveTextureLOD();
            }

            
            if (_textureName == "All" || _textureName == "Maps")
            {
                _proceduralMaterialMaps = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["composition"].GetPropertyMaterial(), gameObject, "composition");
                SetProceduralMaterialFloats("Maps", _proceduralMaterialMaps);
                _proceduralMaterialMaps.SetInputVector2Int("$outputsize", _lodComposition + 4, _lodComposition + 4);                
                _proceduralMaterialMaps.QueueForRender();
                _proceduralMaterialMaps.RenderAsync();
                _textureMaps = _proceduralMaterialMaps.GetGeneratedTextures()[0];
                _material.SetTexture("_TexMaps", _textureMaps);

            }
            if (_textureName == "All" || _textureName == "Biome1")
            {
                _proceduralMaterialBiome1 = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["biome1Type"].GetPropertyMaterial(), gameObject, "biome1Type");
                SetProceduralMaterialFloats("Biome1", _proceduralMaterialBiome1);
                _proceduralMaterialBiome1.SetInputVector2Int("$outputsize", _lodBiome + 4, _lodBiome + 4);
                _proceduralMaterialBiome1.QueueForRender();
                _proceduralMaterialBiome1.RenderAsync();
                //_proceduralMaterialBiome1.GetGeneratedTextures().ForEach(t => Debug.Log("B1: " + t.name));
                _textureBiome1DiffSpec = _proceduralMaterialBiome1.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("diffuse")).FirstOrDefault();
                _textureBiome1Normal = _proceduralMaterialBiome1.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("any")).FirstOrDefault();
                _material.SetTexture("_TexBiome1DiffSpec", _textureBiome1DiffSpec);
                _material.SetTexture("_TexBiome1Normal", _textureBiome1Normal);

            }
            if (_textureName == "All" || _textureName == "Biome2")
            {
                _proceduralMaterialBiome2 = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["biome2Type"].GetPropertyMaterial(), gameObject, "biome2Type");
                SetProceduralMaterialFloats("Biome2", _proceduralMaterialBiome2);                
                _proceduralMaterialBiome2.SetInputVector2Int("$outputsize",_lodBiome + 4, _lodBiome + 4);
                _proceduralMaterialBiome2.QueueForRender();
                _proceduralMaterialBiome2.RenderAsync();
                //_proceduralMaterialBiome2.GetGeneratedTextures().ForEach(t => Debug.Log("B2: " + t.name));
                _textureBiome2DiffSpec = _proceduralMaterialBiome2.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("diffuse")).FirstOrDefault();
                _textureBiome2Normal = _proceduralMaterialBiome2.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("any")).FirstOrDefault();
                _material.SetTexture("_TexBiome2DiffSpec", _textureBiome2DiffSpec);
                _material.SetTexture("_TexBiome2Normal", _textureBiome2Normal);

            }
            if (_textureName == "All" || _textureName == "Cities")
            {
                _proceduralMaterialCities = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["cities"].GetPropertyMaterial(), gameObject, "cities");                
                SetProceduralMaterialFloats("Cities", _proceduralMaterialCities);
                _proceduralMaterialCities.SetInputVector2Int("$outputsize", _lodCities + 4, _lodCities + 4);
                _proceduralMaterialCities.QueueForRender();
                _proceduralMaterialCities.RenderAsync();
                _textureCities = _proceduralMaterialCities.GetGeneratedTextures()[0];
                _material.SetTexture("_TexCities", _textureCities);

            }
            if (_textureName == "All" || _textureName == "Lava")
            {
                _proceduralMaterialLava = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["lava"].GetPropertyMaterial(), gameObject, "lava");
                SetProceduralMaterialFloats("Lava", _proceduralMaterialLava);
                _proceduralMaterialLava.SetInputVector2Int("$outputsize", _lodLava + 4, _lodLava + 4);
                _proceduralMaterialLava.QueueForRender();
                _proceduralMaterialLava.RenderAsync();
                _textureLavaDiffuse = _proceduralMaterialLava.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("diffuse")).FirstOrDefault();
                _textureLavaFlow = _proceduralMaterialLava.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("emissive")).FirstOrDefault();
                _material.SetTexture("_TexLavaDiffuse", _textureLavaDiffuse);
                _material.SetTexture("_TexLavaFlow", _textureLavaFlow);

            }
            if (_textureName == "All" || _textureName == "Clouds")
            {
                _proceduralMaterialClouds = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["clouds"].GetPropertyMaterial(), gameObject, "clouds");                
                SetProceduralMaterialFloats("Clouds", _proceduralMaterialClouds);
                _proceduralMaterialClouds.SetInputVector2Int("$outputsize", _lodClouds + 4, _lodClouds + 4);
                _proceduralMaterialClouds.QueueForRender();
                _proceduralMaterialClouds.RenderAsync();
                _textureClouds = _proceduralMaterialClouds.GetGeneratedTextures()[0];
                _material.SetTexture("_TexClouds", _textureClouds);

            }
            if (_textureName == "All" || _textureName == "PolarIce")
            {
                _proceduralMaterialPolarIce = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["polarIce"].GetPropertyMaterial(), gameObject, "polarIce");                
                SetProceduralMaterialFloats("PolarIce", _proceduralMaterialPolarIce);
                _proceduralMaterialPolarIce.SetInputVector2Int("$outputsize",_lodPolarIce + 4, _lodPolarIce + 4);
                _proceduralMaterialPolarIce.QueueForRender();
                _proceduralMaterialPolarIce.RenderAsync();
                _textureIceDiffuse = _proceduralMaterialPolarIce.GetGeneratedTextures()[0];
                _material.SetTexture("_TexIceDiffuse", _textureIceDiffuse);
            }

            // Start timer when rebuild of textures started
            _timerStartBuildingTextures = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Updates the lookup texture that shader uses to determine waterlevel, polar cap transition, lava coverage, and lava glow.
        /// </summary>
        void UpdateLookupTextures()
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("SolidPlanet.cs: UpdateLookupTextures()");

            _textureLookupLiquid = GenerateLookupSmoothTexture(_dictionaryFloats["liquidLevel"].GetPropertyLerp(), 0f, _dictionaryFloats["liquidShallow"].GetPropertyLerp());
            _material.SetTexture("_TexLookupLiquid", _textureLookupLiquid);
            _textureLookupPolar = GenerateLookupTexture(_dictionaryFloats["polarCapAmount"].GetPropertyLerp());
            _material.SetTexture("_TexLookupPolar", _textureLookupPolar);
            _textureLookupLava = GenerateLookupTexture(_dictionaryFloats["lavaAmount"].GetPropertyLerp());
            _material.SetTexture("_TexLookupLava", _textureLookupLava);
            _textureLookupLavaGlow = GenerateLookupSmoothTexture(_dictionaryFloats["lavaAmount"].GetPropertyLerp(), 0f, _dictionaryFloats["lavaGlowAmount"].GetPropertyLerp());
            _material.SetTexture("_TexLookupLavaGlow", _textureLookupLavaGlow);
        }

        /// <summary>
        /// Updates the shader properties for planet and atmosphere materials.
        /// </summary>
        void UpdateShader()
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("SolidPlanet.cs: UpdateShader()");

            // Update the dictionaries if needed
            UpdateDictionariesIfNeeded();

            // Update atmosphere shader properties
            if (_externalAtmosphere != null)
            {
                _externalAtmosphereMaterial.SetFloat("_Size", _dictionaryFloats["atmosphereExternalSize"].GetPropertyLerp());
                float _aedMin = _dictionaryFloats["atmosphereExternalDensity"].minValue;
                float _aedMax = _dictionaryFloats["atmosphereExternalDensity"].maxValue;
                float _aedVal = _dictionaryFloats["atmosphereExternalDensity"].value;
                float _aesVal = _dictionaryFloats["atmosphereExternalSize"].value;
                _externalAtmosphereMaterial.SetFloat("_Falloff", Mathf.Lerp(_aedMin * (1 + _aesVal), _aedMax * (1 + _aesVal), _aedVal));
            }

            // Update planet shader properties
            foreach (KeyValuePair<string, PropertyFloat> _pmm in _dictionaryFloats)
            {
                bool _isNormalShader = false;
                if (_pmm.Value.proceduralTextures != null)
                {
                    if (_pmm.Value.proceduralTextures.Length == 0)
                        _isNormalShader = true;
                }
                if (_pmm.Value.shaderProperty != null && _pmm.Value.materials != null && _isNormalShader) UpdatePropertyFloatShader(_pmm.Value);
            }
            foreach (KeyValuePair<string, PropertyColor> _pmm in _dictionaryColors)
            {
                if (_pmm.Value.shaderProperty != null && _pmm.Value.materials != null) UpdatePropertyColorShader(_pmm.Value);
            }

            // Create a list of keywords to be sent to the shader for enabling/disabling features for performance reasons
            List<string> _shaderKeywords = new List<string>();

            // Enable/disable code to render clouds in shader (shader section only exists if cloud opacity is not zero)
            if (_dictionaryFloats["cloudsOpacity"].GetPropertyLerp() > 0.00001f)
                _shaderKeywords.Add("CLOUDS_ON");
            else
                _shaderKeywords.Add("CLOUDS_OFF");

            // Enable/disable code to render lava in shader (shader section only exists if there is lava on the planet)
            if (_dictionaryFloats["lavaAmount"].GetPropertyLerp() > 0.00001f)
                _shaderKeywords.Add("LAVA_ON");
            else
                _shaderKeywords.Add("LAVA_OFF");

            // Set the shader keywords in the shader
            _material.shaderKeywords = _shaderKeywords.ToArray();

            // Clear shader update needed flag
            _updateShaderNeeded = false;
        }        

        /// <summary>
        /// Gets the planet blueprint index (i.e. the order of a planet blueprint in the blueprint hierarchy of solid planets under the Manager game object
        /// based on the random seed.
        /// </summary>
        /// <returns></returns>
        protected override int GetPlanetBlueprintSeededIndex()
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("SolidPlanet.cs: GetPlanetBlueprintSeededIndex()");

            // Save the current random state
            Random.State _oldState = Random.state;
            // Initialize the random state with the planetSeed value
            Random.InitState(planetSeed);
            // Get the random planet blueprint index (based on the seed) from the Manager
            planetBlueprintIndex = Random.Range(0, PlanetManager.Instance.listSolidPlanetBlueprints.Count);
            // Restore the previous random state
            Random.state = _oldState;
            // Return the new planet blueprint index.
            return planetBlueprintIndex;
        }

        /// <summary>
        /// Handles any modified textures by examining the string array to see if the array contains reference to one or more procedural texture(s).
        /// If there is a reference to a texture it means that it has been modified and the rebuild flag for the texture is set to true.
        /// </summary>
        /// <param name="_proceduralTextures"></param>
        protected override void HandleModifiedTextures(string[] _proceduralTextures)
        {
            if ((int) PlanetManager.DebugLevel > 1) Debug.Log("SolidPlanet.cs: HandleModifiedTextures(" + _proceduralTextures + ")");

            foreach (string _s in _proceduralTextures)
            {
                switch (_s)
                {
                    case "Maps":
                        _rebuildMapsNeeded = true;
                        break;
                    case "Cities":
                        _rebuildCitiesNeeded = true;
                        break;
                    case "Clouds":
                        _rebuildCloudsNeeded = true;
                        break;
                    case "Lava":
                        _rebuildLavaNeeded = true;
                        break;
                    case "Biome1":
                        _rebuildBiome1Needed = true;
                        break;
                    case "Biome2":
                        _rebuildBiome2Needed = true;
                        break;
                    case "Lookups":
                        _rebuildLookupsNeeded = true;
                        break;
                }
            }
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
                    if (Vector3.Distance(_ls.transform.position, transform.position) < Vector3.Distance(_localStarNearestInstance.transform.position, transform.position))
                        _localStarNearestInstance = _ls;
                }
            }

            // If there are no local stars in the scene, return
            if (_localStarNearestInstance == null) return;

            // Detect if if local star position is different from the cache - if so, update the shader with new settings and update the cache
            if (Vector3.Distance(_localStarShaderCacheSettings.position, _localStarNearestInstance.transform.position) > 0.0001f || _forceUpdate || Vector3.Distance(transform.position, _lastPosition) > 0.0001f)
            {
                _lastPosition = transform.position;
                _localStarShaderCacheSettings.position = _localStarNearestInstance.transform.position;
                _meshRenderer.sharedMaterial.SetVector(_shaderID_LocalStarPosition, _localStarNearestInstance.transform.position);
                _externalAtmosphereRenderer.sharedMaterial.SetVector(_shaderID_LocalStarPosition, _localStarNearestInstance.transform.position);
            }

            // Detect if if local star color is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.color.r - _localStarNearestInstance.color.r) > 0.0001f ||
                Mathf.Abs(_localStarShaderCacheSettings.color.g - _localStarNearestInstance.color.g) > 0.0001f ||
                Mathf.Abs(_localStarShaderCacheSettings.color.b - _localStarNearestInstance.color.b) > 0.0001f ||
                _forceUpdate)
            {
                _localStarShaderCacheSettings.color = _localStarNearestInstance.color;
                _meshRenderer.sharedMaterial.SetColor(_shaderID_LocalStarColor, _localStarNearestInstance.color);
                _externalAtmosphereRenderer.sharedMaterial.SetColor(_shaderID_LocalStarColor, _localStarNearestInstance.color);
            }

            // Detect if if local star intensity is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.intensity - _localStarNearestInstance.intensity) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.intensity = _localStarNearestInstance.intensity;
                _meshRenderer.sharedMaterial.SetFloat(_shaderID_LocalStarIntensity, _localStarNearestInstance.intensity);
                _externalAtmosphereRenderer.sharedMaterial.SetFloat(_shaderID_LocalStarIntensity, _localStarNearestInstance.intensity);
            }

            // Detect if if local star ambient intensity is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.ambientIntensity - _localStarNearestInstance.ambientIntensity) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.ambientIntensity = _localStarNearestInstance.ambientIntensity;
                _meshRenderer.sharedMaterial.SetFloat(_shaderID_LocalStarAmbientIntensity, _localStarNearestInstance.ambientIntensity);
            }
            

        }

        public override void CacheProceduralProperty(string _property, bool _value)
        {
            // nothing here yet
        }

        /// <summary>
        /// Gets baked textures - used by editor script to bake planet to prefab with PNG textures
        /// </summary>
        /// <returns></returns>
        public Texture2D[] GetBakedTextures()
        {
            Texture2D[] _tex2DArray = new Texture2D[14];
            RenderTexture _currentActiveRT = RenderTexture.active;
            RenderTexture _rt = new RenderTexture(_textureMaps.width, _textureMaps.height, 32);
            Graphics.Blit(_textureMaps, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[0] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[0].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[0].Apply();

            _rt = new RenderTexture(_textureBiome1DiffSpec.width, _textureBiome1DiffSpec.height, 32);
            Graphics.Blit(_textureBiome1DiffSpec, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[1] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[1].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[1].Apply();

            _rt = new RenderTexture(_textureBiome1Normal.width, _textureBiome1Normal.height, 32);
            Graphics.Blit(_textureBiome1Normal, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[2] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[2].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[2].Apply();

            _rt = new RenderTexture(_textureBiome2DiffSpec.width, _textureBiome2DiffSpec.height, 32);
            Graphics.Blit(_textureBiome2DiffSpec, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[3] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[3].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[3].Apply();

            _rt = new RenderTexture(_textureBiome2Normal.width, _textureBiome2Normal.height, 32);
            Graphics.Blit(_textureBiome2Normal, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[4] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[4].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[4].Apply();

            _rt = new RenderTexture(_textureIceDiffuse.width, _textureIceDiffuse.height, 32);
            Graphics.Blit(_textureIceDiffuse, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[5] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[5].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[5].Apply();

            _rt = new RenderTexture(_textureCities.width, _textureCities.height, 32);
            Graphics.Blit(_textureCities, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[6] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[6].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[6].Apply();

            _rt = new RenderTexture(_textureClouds.width, _textureClouds.height, 32);
            Graphics.Blit(_textureClouds, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[7] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[7].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[7].Apply();

            _rt = new RenderTexture(_textureLavaDiffuse.width, _textureLavaDiffuse.height, 32);
            Graphics.Blit(_textureLavaDiffuse, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[8] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[8].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[8].Apply();

            _rt = new RenderTexture(_textureLavaFlow.width, _textureLavaFlow.height, 32);
            Graphics.Blit(_textureLavaFlow, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[9] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[9].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[9].Apply();

            _rt = new RenderTexture(_textureLookupLiquid.width, _textureLookupLiquid.height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Graphics.Blit(_textureLookupLiquid, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[10] = new Texture2D(_rt.width, _rt.height, TextureFormat.ARGB32, false, false);
            _tex2DArray[10].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[10].Apply();

            _rt = new RenderTexture(_textureLookupLava.width, _textureLookupLava.height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Graphics.Blit(_textureLookupLava, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[11] = new Texture2D(_rt.width, _rt.height, TextureFormat.ARGB32, false, false);
            _tex2DArray[11].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[11].Apply();

            _rt = new RenderTexture(_textureLookupLavaGlow.width, _textureLookupLavaGlow.height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Graphics.Blit(_textureLookupLavaGlow, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[12] = new Texture2D(_rt.width, _rt.height, TextureFormat.ARGB32, false, false);
            _tex2DArray[12].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[12].Apply();

            _rt = new RenderTexture(_textureLookupPolar.width, _textureLookupPolar.height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Graphics.Blit(_textureLookupPolar, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[13] = new Texture2D(_rt.width, _rt.height, TextureFormat.ARGB32, false, false);
            _tex2DArray[13].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[13].Apply();

            RenderTexture.active = _currentActiveRT;
            DestroyImmediate(_rt);
            return _tex2DArray;
        }
    }
}
