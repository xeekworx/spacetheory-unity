using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralPlanets.SimpleJSON;
using System.Linq;

namespace ProceduralPlanets
{
    /// <summary>
    /// Component used by gas planets. Planets are created by using the Inspector on the PlanetManager or via the static public method
    /// <seealso cref="PlanetManager.CreatePlanet(Vector3, int, string, string)"/> in PlanetManager.
    /// 
    /// GasPlanet is derived from the base class Planet. GasPlanet are all planets that are not solid planets.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>

    // Execute in edit mode because we want to be able to change planet parameter and rebuild textures in editor
    [ExecuteInEditMode]

    // Require MeshFilter and MeshRenderer for planet
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class GasPlanet : Planet
    {
        // Flags to indicate if maps need to be rebuilt and if shader needs to be updated with new properties
        [SerializeField] bool _rebuildMapsNeeded;
        [SerializeField] bool _rebuildStormMaskNeeded;

        [SerializeField] bool _updateShaderNeeded;

        int _lodCommon = 0;
        int _lodGas = 5;

        // Procedural materials for planet textures
        Substance.Game.SubstanceGraph _proceduralMaterialMaps;

        // Textures used by the planet
        Texture2D _textureMaps;
        Texture2D _texturePaletteLookup;
        Texture2D _textureBodyNormal;
        Texture2D _textureCapNormal;
        Texture2D _textureStormMask;


        // Materials
        Material _material;

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
            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("PlanetGas.cs: SetPlanetBlueprint(" + _index + "," + _leaveOverride + "," + _setProperties + ")");

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
            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("GasPlanet.cs: Awake()");
            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("- PlanetVersion: " + PLANET_VERSION);

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
                    _material = new Material(Shader.Find("ProceduralPlanets/GasPlanetLinear"));
                else
                    _material = new Material(Shader.Find("ProceduralPlanets/GasPlanetGamma"));

                _meshRenderer.material = _material;
            }

            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                _material.shader = Shader.Find("ProceduralPlanets/GasPlanetLinear");
            }
            else
            {
                _material.shader = Shader.Find("ProceduralPlanets/GasPlanetGamma");
            }

            // Clear properties lists
            propertyFloats.Clear();
            propertyMaterials.Clear();
            propertyColors.Clear();

            // Add property materials
            AddPropertyMaterial("gas", "Gas*", PlanetManager.Instance.gasMaterials.ToArray(), 1, new string[] { "Maps" });

            // Update dictionaries (for materials a this stage)
            UpdateDictionariesIfNeeded(true);

            // Set default properties (for materials at this stage)
            SetDefaultProperties();

            // Get references to newly created property materials
            _proceduralMaterialMaps = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["gas"].GetPropertyMaterial(), gameObject, "gas");

            // Add Float (within a range of min/max) and color properties
            AddPropertyFloat("horizontalTiling", "HorizontalTiling", 1, 10, false, true, 5, false, new string[] { "StormMask" }, _material, "_HTiling", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("verticalTiling", "Vertical Tiling", 1, 10, false, true, 8, false, new string[] { "StormMask" }, _material, "_VTiling", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("gasSeed", "Gas Seed*", 0, 255, false, true, 10, true, new string[] { "Maps" }, _proceduralMaterialMaps, "Composition_Random_Seed", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("turbulenceSeed", "Turbulence Seed*", 0, 255, false, true, 20, true, new string[] { "Maps" }, _proceduralMaterialMaps, "Turbulence_Random_Seed", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("turbulence", "Turbulence*", 0.0f, 0.2f, true, false, 20, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Turbulence", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("turbulenceScale", "Turbulence Scale*", 1, 10, false, false, 30, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Turbulence_Scale", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("turbulenceDisorder", "Turbulence Disorder*", 0.0f, 1.0f, true, false, 40, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Turbulence_Disorder", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("separation", "Separation*", 0.0f, 1.0f, true, false, 50, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Separation", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("smoothness", "Smoothness*", 0.0f, 100.0f, true, false, 60, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Smoothness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("blurriness", "Blurriness*", 0.0f, 16.0f, true, false, 70, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Blurriness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("palette", "Palette*", 1, 8, false, true, 80, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Palette", PropertyFloat.Method.VALUE, PropertyFloat.DataType.INT);
            AddPropertyFloat("detail", "Detail*", 0.0f, 1.0f, true, false, 90, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Detail", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("detailOffset", "Detail Offset*", 0.0f, 1.0f, true, false, 100, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Detail_Offset", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("contrast", "Contrast*", -0.6f, 0.6f, true, false, 110, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Contrast", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("hue", "Hue*", 0.0f, 1.0f, true, false, 120, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Hue", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("saturation", "Saturation*", 0.0f, 1.0f, true, false, 130, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Saturation", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("brightness", "Brightness*", 0.0f, 1.0f, true, false, 150, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Brightness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("banding", "Banding", 0.0f, 1.0f, true, false, 160, false, null, _material, "_Banding", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("solidness", "Solidness", 0.0f, 1.0f, true, false, 170, false, null, _material, "_Solidness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("faintness", "Faintness", 0.0f, 1.0f, true, false, 180, false, null, _material, "_Faintness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);                        
            AddPropertyColor("faintnessColor", "Faintness Color", new Color(0.5f, 0.5f, 0.5f, 1.0f), 0.2f, 0.2f, 0.2f, 190, true, _material, "_FaintnessColor");
            AddPropertyFloat("roughness", "Roughness*", 0.0f, 1.0f, true, false, 200, false, null, _material, "_Roughness", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyColor("twilightColor", "Twilight Color", new Color(0.15f, 0.0f, 0.15f, 1.0f), 0.2f, 0.2f, 0.2f, 210, true, _material, "_ColorTwilight");
            AddPropertyFloat("stormMaskIndex", "Storm Mask Index", 0.0f, 1.0f, true, false, 220, true, new string[] { "StormMask" });
            AddPropertyFloat("stormSquash", "Storm Squash*", 0.0f, 1.0f, true, false, 230, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Storm_Squash", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyColor("stormColor", "Storm Color", new Color(0.78f, 0.13f, 0.28f, 1.0f), 0.2f, 0.2f, 0.2f, 240, true, _material, "_StormColor");
            AddPropertyFloat("stormTint", "Storm Tint", 0.0f, 1.0f, true, false, 250, false, null, _material, "_StormTint", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("stormScale", "Storm Scale*", 0.0f, 1.0f, true, false, 260, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Storm_Scale", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyFloat("stormNoise", "Storm Noise*", 0.0f, 1.0f, true, false, 270, false, new string[] { "Maps" }, new Substance.Game.SubstanceGraph[] { _proceduralMaterialMaps }, "Storm_Noise", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);
            AddPropertyColor("atmosphereColor", "Atmosphere Color", new Color(0.48f, 0.48f, 0.3f, 1.0f), 0.2f, 0.2f, 0.2f, 280, true, _material, "_AtmosphereColor");
            AddPropertyFloat("atmosphereFalloff", "Atmospehre Falloff", 1.0f, 20.0f, true, false, 290, false, null, _material, "_AtmosphereFalloff", PropertyFloat.Method.LERP, PropertyFloat.DataType.FLOAT);

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

            // Update shader properties if any shader related properties have been changed
            if (_updateShaderNeeded) UpdateShader();

            // Rebuild any planet textures where procedural properties have been changed. This only executes if autoUpdateTextures is set in ProceduralPlanetManager.
            if (RebuildTexturesNeeded() && !IsBuildingTextures)
                RebuildTextures();            

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

            for (int _i = _propertyFloatAnimations.Count -1; _i >= 0; _i--)
            {
                OverridePropertyFloat(_propertyFloatAnimations[_i].key, _propertyFloatAnimations[_i].GetAnimatedValue(),false);
                if (_propertyFloatAnimations[_i].HasExpired())
                    _propertyFloatAnimations.RemoveAt(_i);
            }

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
                if ((int)PlanetManager.DebugLevel > 0) Debug.Log("GasPlannet.cs: UpdateLODMeshIfNeeded() Setting mesh LOD level to: " + meshLODLevel);
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
                        if ((int)PlanetManager.DebugLevel > 0) Debug.Log("GasPlanet.cs: UpdateLODTextureIfNeeded() Setting texture static LOD level to: " + _lodCommon);
                        _lodGas = _lodCommon;
                        RebuildTextures(true);
                    }
                    break;

                case PlanetManager.TextureDetailModes.Static_Separate:

                    bool _flag = false;

                    if (_lodGas != PlanetManager.TextureStaticGas || _force)
                    {
                        _lodGas = PlanetManager.TextureStaticGas;
                        _flag = true;
                    }

                    if (_flag && (int)PlanetManager.DebugLevel > 0) Debug.Log("GasPlanet.cs: UpdateLODTextureIfNeeded() Setting texture static separate LOD levels to: Gas: " + _lodGas);

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

                    if ((int)PlanetManager.DebugLevel > 0) Debug.Log("UpdateLODTextureIfNeeded() - Setting Common Progressive Texture Step: " + textureProgressiveStep);

                    if (_lodCommon != PlanetManager.TextureProgressiveCommon[textureProgressiveStep] || _force)
                    {
                        _lodCommon = PlanetManager.TextureProgressiveCommon[textureProgressiveStep];
                        _lodGas = _lodCommon;
                        RebuildTextures(true);
                    }

                    break;

                case PlanetManager.TextureDetailModes.Progressive_Separate:

                    if (textureProgressiveStep >= PlanetManager.TextureProgressiveSteps - 1)
                        return;

                    if (IsBuildingTextures)
                        return;

                    textureProgressiveStep++;

                    if (textureProgressiveStep < 0)
                        return;

                    if ((int)PlanetManager.DebugLevel > 0) Debug.Log("UpdateLODTextureIfNeeded() - Setting Separate Progressive Texture Step: " + textureProgressiveStep);

                    if (_lodGas != PlanetManager.TextureProgressiveGas[textureProgressiveStep] || _force)
                        _lodGas = PlanetManager.TextureProgressiveGas[textureProgressiveStep];

                    RebuildTextures(true);

                    break;

                case PlanetManager.TextureDetailModes.LOD:
                    {
                        int _appropriateTextureLODLevel = PlanetManager.Instance.GetAppropriateTextureLODLevel(GetLODPercent());
                        if (textureLODLevel != _appropriateTextureLODLevel)
                        {
                            textureLODLevel = _appropriateTextureLODLevel;
                            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("UpdateLODTextureIfNeeded() - Setting Common Texture LOD Level: " + textureLODLevel);
                            if (_lodCommon != textureLODLevel || _force)
                            {
                                _lodCommon = PlanetManager.TextureLODCommon[textureLODLevel];
                                _lodGas = _lodCommon;
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
                            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("UpdateLODTextureIfNeeded() - Setting Separate Texture LOD Level: " + textureLODLevel);

                            if (_lodGas != PlanetManager.TextureLODGas[textureLODLevel] || _force)
                                _lodGas = PlanetManager.TextureLODGas[textureLODLevel];

                            RebuildTextures(true);
                        }
                    }
                    break;

            }

        }

        void SetDefaultProgressiveTextureLOD()
        {
            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("GasPlanet.cs: SetDefaultProgressiveTextureLOD(): Progressive Step 0");

            textureProgressiveStep = 0;
            if (PlanetManager.TextureDetailMode == PlanetManager.TextureDetailModes.Progressive)
            {
                _lodCommon = PlanetManager.TextureProgressiveCommon[textureProgressiveStep];
                _lodGas = _lodCommon;
            }

            if (PlanetManager.TextureDetailMode == PlanetManager.TextureDetailModes.Progressive_Separate)
            {
                _lodGas = PlanetManager.TextureProgressiveGas[textureProgressiveStep];
            }

        }


        /// <summary>
        /// Determines if any procedural texture needs to be rebuilt based on flags. 
        /// Public because the editor script calls this as well.
        /// </summary>
        /// <returns></returns>
        public bool RebuildTexturesNeeded()
        {
            if (_rebuildMapsNeeded || _rebuildStormMaskNeeded) return true;
            return false;
        }

        /// <summary>
        /// Rebuilds textures for procedural materials where properties have been changed (or alternatively all texture if force parameter is set to true).
        /// Public because the editor script calls this as well.
        /// </summary>
        /// <param name="_force"></param>
        public void RebuildTextures(bool _force = false)
        {
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("GasPlannet.cs: RebuildTextures(" + _force + ")");

            // Ensure that the dictionaries are updated (force flag is set to true for assurance)
            UpdateDictionariesIfNeeded(true);

            // If force flag was set, update all procedural textures for the planet.
            if (_force)
            {
                UpdateProceduralTexture("All");
                UpdateLookupTextures();
                _rebuildMapsNeeded = false;
                _rebuildStormMaskNeeded = false;
            }

            // Update individual procedural textures if needed
            if (_rebuildMapsNeeded)
            {
                UpdateProceduralTexture("Maps");
                _rebuildMapsNeeded = false;
            }

            if (_rebuildStormMaskNeeded)
            {
                UpdateLookupTextures();
                _rebuildStormMaskNeeded = false;
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
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("GasPlanet.cs: UpdateProceduralTexture(" + _textureName + ")");

            if (PlanetManager.TextureDetailMode == PlanetManager.TextureDetailModes.Progressive ||
                PlanetManager.TextureDetailMode == PlanetManager.TextureDetailModes.Progressive_Separate)
            {
                if (textureProgressiveStep == -1)
                    SetDefaultProgressiveTextureLOD();
            }

            if (_textureName == "All" || _textureName == "Maps")
            {
                _proceduralMaterialMaps = PlanetManager.Instance.GetUniqueProceduralMaterial(_dictionaryMaterials["gas"].GetPropertyMaterial(), gameObject, "gas");
                SetProceduralMaterialFloats("Maps", _proceduralMaterialMaps);
                _proceduralMaterialMaps.SetInputVector2Int("$outputsize", _lodGas + 4, _lodGas + 4);
                _proceduralMaterialMaps.RenderSync();
                _textureMaps = _proceduralMaterialMaps.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("any")).FirstOrDefault();
                _textureCapNormal = _proceduralMaterialMaps.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("CapNormal")).FirstOrDefault();
                _textureBodyNormal = _proceduralMaterialMaps.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("BodyNormal")).FirstOrDefault();
                _texturePaletteLookup = _proceduralMaterialMaps.GetGeneratedTextures().ToList().Where(texture => texture.name.Contains("Palette_Diffuse")).FirstOrDefault();
                _material.SetTexture("_BodyTexture", _textureMaps);
                _material.SetTexture("_CapTexture", _textureMaps);
                _material.SetTexture("_BodyNormal", _textureBodyNormal);
                _material.SetTexture("_CapNormal", _textureCapNormal);
                _material.SetTexture("_PaletteLookup", _texturePaletteLookup);

            }

            // Start timer when rebuild of textures started
            _timerStartBuildingTextures = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Updates the lookup texture that shader uses, if any.
        /// </summary>
        void UpdateLookupTextures()
        {
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("GasPlanet.cs: UpdateLookupTextures()");
            // None used for gas planets
            float _tilesWidth = (int) GetPropertyFloat("horizontalTiling");
            float _tilesHeight = (int) GetPropertyFloat("verticalTiling");

            float _index = (int) (GetPropertyFloat("stormMaskIndex") * (float) (_tilesHeight * _tilesWidth));
            float _texRes = 32f;

            _textureStormMask = new Texture2D((int) _texRes, (int)_texRes);
            Color32 _black = new Color(0, 0, 0, 255);
            Color32[] _array = _textureStormMask.GetPixels32();
            for (int _i = 0; _i < _array.Length; _i++)
            {
                _array[_i] = _black;
            }
            _textureStormMask.SetPixels32(_array);
            _textureStormMask.Apply();

            for (int _y = 0; _y < _tilesHeight; _y++)
            {
                for (int _x = 0; _x < _tilesWidth; _x++)
                {
                    if (_y * _tilesWidth + _x == _index)
                        FillTextureQuad(_textureStormMask, (int)((float)(_x / _tilesWidth) * _texRes), (int)((float)(_y / _tilesHeight) * _texRes), (int)((1f / _tilesWidth) * _texRes), (int)((1f / _tilesHeight) * _texRes), Color.white);
                }
            }
            _textureStormMask.Apply();
            _material.SetTexture("_StormMask", _textureStormMask);
        }

        void FillTextureQuad(Texture2D _tex2D, int _x, int _y, int _width, int _height, Color _color)
        {
            for (int _yy = _y; _yy < _y + _height; _yy++)
            {
                for (int _xx = _x; _xx < _x + _width; _xx++)
                {
                    _tex2D.SetPixel(_xx, _yy, _color);
                }
            }
            return;
        }

        /// <summary>
        /// Updates the shader properties for planet materials.
        /// </summary>
        void UpdateShader()
        {
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("GasPlanet.cs: UpdateShader()");

            // Update the dictionaries if needed
            UpdateDictionariesIfNeeded();

            // Update planet shader properties
            foreach (KeyValuePair<string, PropertyFloat> _pmm in _dictionaryFloats)
            {
                /*
                bool _isNormalShader = false;
                if (_pmm.Value.proceduralTextures != null)
                {
                    if (_pmm.Value.proceduralTextures.Length == 0)
                        _isNormalShader = true;
                }
                */
                //if (_pmm.Value.shaderProperty != null && _pmm.Value.materials != null && _isNormalShader) UpdatePropertyFloatShader(_pmm.Value);
                if (_pmm.Value.shaderProperty != null && _pmm.Value.materials != null) UpdatePropertyFloatShader(_pmm.Value);
            }
            foreach (KeyValuePair<string, PropertyColor> _pmm in _dictionaryColors)
            {
                if (_pmm.Value.shaderProperty != null && _pmm.Value.materials != null) UpdatePropertyColorShader(_pmm.Value);
            }

            // Clear shader update needed flag
            _updateShaderNeeded = false;
        }

        /// <summary>
        /// Gets the planet blueprint index (i.e. the order of a planet blueprint in the blueprint hierarchy of gas
        /// planets under the Manager game object based on the random seed.
        /// </summary>
        /// <returns></returns>
        protected override int GetPlanetBlueprintSeededIndex()
        {
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("GasPlanet.cs: GetPlanetBlueprintSeededIndex()");

            // Save the current random state
            Random.State _oldState = Random.state;
            // Initialize the random state with the planetSeed value
            Random.InitState(planetSeed);
            // Get the random planet blueprint index (based on the seed) from the Manager
            planetBlueprintIndex = Random.Range(0, PlanetManager.Instance.listGasPlanetBlueprints.Count);
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
            if ((int)PlanetManager.DebugLevel > 1) Debug.Log("GasPlanet.cs: HandleModifiedTextures(" + _proceduralTextures + ")");

            foreach (string _s in _proceduralTextures)
            {
                switch (_s)
                {
                    case "Maps":
                        _rebuildMapsNeeded = true;
                        break;
                    case "StormMask":                        
                        _rebuildStormMaskNeeded = true;
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
            }

            // Detect if if local star color is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.color.r - _localStarNearestInstance.color.r) > 0.0001f ||
                Mathf.Abs(_localStarShaderCacheSettings.color.g - _localStarNearestInstance.color.g) > 0.0001f ||
                Mathf.Abs(_localStarShaderCacheSettings.color.b - _localStarNearestInstance.color.b) > 0.0001f ||
                _forceUpdate)
            {
                _localStarShaderCacheSettings.color = _localStarNearestInstance.color;
                _meshRenderer.sharedMaterial.SetColor(_shaderID_LocalStarColor, _localStarNearestInstance.color);
            }

            // Detect if if local star intensity is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.intensity - _localStarNearestInstance.intensity) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.intensity = _localStarNearestInstance.intensity;
                _meshRenderer.sharedMaterial.SetFloat(_shaderID_LocalStarIntensity, _localStarNearestInstance.intensity);
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
            //_proceduralMaterialMaps.CacheProceduralProperty(_property, _value);
        }

        /// <summary>
        /// Gets baked textures - used by editor script to bake planet to prefab with PNG textures
        /// </summary>
        /// <returns></returns>
        public Texture2D[] GetBakedTextures()
        {
            Texture2D[] _tex2DArray = new Texture2D[5];
            RenderTexture _currentActiveRT = RenderTexture.active;
            RenderTexture _rt = new RenderTexture(_textureMaps.width, _textureMaps.height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            Graphics.Blit(_textureMaps, _rt);            
            RenderTexture.active = _rt;
            _tex2DArray[0] = new Texture2D(_rt.width, _rt.height, TextureFormat.ARGB32, false, true);
            _tex2DArray[0].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[0].Apply();

            _rt = new RenderTexture(_texturePaletteLookup.width, _texturePaletteLookup.height, 24);
            Graphics.Blit(_texturePaletteLookup, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[1] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[1].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[1].Apply();

            _rt = new RenderTexture(_textureBodyNormal.width, _textureBodyNormal.height, 32);
            Graphics.Blit(_textureBodyNormal, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[2] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[2].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[2].Apply();

            _rt = new RenderTexture(_textureCapNormal.width, _textureCapNormal.height, 32);
            Graphics.Blit(_textureCapNormal, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[3] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[3].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[3].Apply();

            _rt = new RenderTexture(_textureStormMask.width, _textureStormMask.height, 24);
            Graphics.Blit(_textureStormMask, _rt);
            RenderTexture.active = _rt;
            _tex2DArray[4] = new Texture2D(_rt.width, _rt.height);
            _tex2DArray[4].ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            _tex2DArray[4].Apply();
            RenderTexture.active = _currentActiveRT;            
            DestroyImmediate(_rt);
            return _tex2DArray;
        }

    }

}
