using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralPlanets.SimpleJSON;
using System.Text.RegularExpressions;
using Substance.Game;

namespace ProceduralPlanets
{
    /// <summary>
    /// PlanetManager is a required component that must exist as a single instance in all scenes
    /// where ProceduralPlanets are used.
    /// 
    /// The PlanetManager is a Singleton class and can only exist as one instance. It also uses 
    /// DontDestroyOnLoad so once it is created it will persist in all scenes and survive scene
    /// switching until you manually destroy the PlanetManager.
    /// 
    /// The PlanetManager has the following main purposes:
    /// 
    /// <b>Configuration</b><br>
    /// * Keeps track of Planet Blueprints (a blueprint controls ranges for random values for planet types)<br>
    /// * Keeps references to Procedural Materials that are used to generate planet textures<br>
    /// * Keeps track of probability of blueprints being used when creating random planets<br>
    /// * Keeps track of Level of Detail(LOD) for planet mesh and texture details<br>
    /// * Has one(or more) procedurally generated spherical meshes that planets share<br>
    /// * Enables creation of planets from scripts using public static methods<br>
    /// 
    /// <b>Public Static Methods</b><br>
    /// Main methods used to create planets from scripts:<br>
    /// <seealso cref="CreatePlanet(Vector3, string)"/>
    /// <seealso cref="CreatePlanet(Vector3, int, string, string)"/>
    /// Utilities:<br>
    /// <seealso cref="ExportAllBlueprints(StringFormat)"/>
    /// <seealso cref="ImportBlueprints(string)"/>
    /// 
    /// The PlanetManager also has a range of public properties that can be used to control
    /// level of detail for planet meshes annd texture resolitions.
    /// <seealso cref="MeshDetailMode"/>
    /// <seealso cref="TextureDetailMode"/>
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    [ExecuteInEditMode]
    public class PlanetManager : Singleton<PlanetManager>
    {
        // The planet shader is dependent on this being 5.0 fo render properly - change scale of planet object to get the size you want
        public const float CONST_MESH_RADIUS = 5.0f;
        // Number of texture resolutions
        public const int TEXTURE_RESOLUTION_COUNT = 8;
        // Name for generic planet blueprint
        public const string GENERIC_PLANET_BLUEPRINT_NAME = "Generic Planet Blueprint";

        // Enums
        public enum DebugLevels { Off = 0, Basic = 1, Detailed = 2, Full = 3 }
        public enum MeshDetailModes { Static, LOD }
        public enum TextureDetailModes { Static, Static_Separate, Progressive, Progressive_Separate, LOD, LOD_Separate }

        // PUBLIC PROPERTIES


        /// <summary>
        /// Gets or sets the camera used for Level of Detail (LOD) determination. Default is Camera.main.
        /// </summary>
        public static Camera cameraLOD
        {
            get
            {
                if (Instance._cameraLOD == null)
                    Instance._cameraLOD = Camera.main;
                return Instance._cameraLOD;
            }
            set { Instance._cameraLOD = value; }
        }
        [SerializeField] private Camera _cameraLOD;

        /// <summary>
        /// Sets or gets if a static (MeshDetailModes.Static) sized mesh or 2 or more Level Of Detail (MeshDetailModes.LOD) meshes should be used for planets.
        /// 
        /// When set to a new value, the procedural meshes are recreated based on the new mode and the properties MeshStaticSubdivisions or MeshLODSubdivisions.
        /// </summary>
         /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set the Mesh Detail Mode to Level of Detail (LOD)
                PlanetManager.MeshDetailMode = PlanetManager.MeshDetailModes.LOD;

                // Set number of LOD levels to 5
                PlanetManager.MeshLODSteps = 5;

                // Set the mesh detail levels by using an array of subdivision levels
                // The first value in the array is the highest level of detail and the last value in the array is the lowest quality
                PlanetManager.MeshLODSubdivisions = new int[5] { 6, 5, 4, 3, 2 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.MeshLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="MeshLODSteps"/>
        /// <seealso cref="MeshLODSubdivisions"/>       
        /// <seealso cref="MeshLODPlanetSizes"/>
        public static MeshDetailModes MeshDetailMode {
            get { return Instance._meshDetailMode; }
            set {
                if (Instance._meshDetailMode != value)
                {
                    Instance._meshDetailMode = value;
                    Instance.RecreateProceduralMeshes();
                }                    
            } }
        [SerializeField] private MeshDetailModes _meshDetailMode = MeshDetailModes.Static;

        /// <summary>
        /// Specifies the static mesh subdivisions level (Valid values: 0 - 6).
        /// 
        /// Subdivisions: <br>
        /// 0 = 8 triangles, <br>
        /// 1 = 32 triangles, <br>
        /// 2 = 128 triangles, <br>
        /// 3 = 512 triangles, <br>
        /// 4 = 2048 triangles, <br>
        /// 5 = 8192 triangles, <br>
        /// 6 = 32768 triangles
        /// 
        /// Default value is 6 (32768 triangles).
        /// 
        /// When changed to a new value the static mesh is automatically recreated with the new number of subdivisions.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set the Mesh Detail Mode to Level of Detail (LOD)
                PlanetManager.MeshDetailMode = PlanetManager.MeshDetailModes.LOD;

                // Set number of LOD levels to 5
                PlanetManager.MeshLODSteps = 5;

                // Set the mesh detail levels by using an array of subdivision levels
                // The first value in the array is the highest level of detail and the last value in the array is the lowest quality
                PlanetManager.MeshLODSubdivisions = new int[5] { 6, 5, 4, 3, 2 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.MeshLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="MeshDetailMode"/>        
        /// <seealso cref="MeshLODSteps"/>
        /// <seealso cref="MeshLODPlanetSizes"/> 
        public static int MeshStaticSubdivisions {
            get { return Instance._meshStaticSubdivisions; }
            set {
                if (value < 0 || value > 6)
                    Debug.LogWarning("Cannot set subdivisions to specified value, must be between 0-6. Setting value: " + Mathf.Clamp(value, 0, 6));

                if (Instance._meshStaticSubdivisions != Mathf.Clamp(value, 0, 6))
                {
                    Instance._meshStaticSubdivisions = Mathf.Clamp(value, 0, 6);
                    Instance.RecreateProceduralMeshes();
                }
            } }
        [SerializeField] private int _meshStaticSubdivisions = 6;

        /// <summary>
        /// Specifies the number of LOD steps that should be used if MeshDetailMode.LOD is used. Valid values = 2-5.
        /// 
        /// Default value is 5 (max).
        /// 
        /// When changed to a new value the LOD meshes are automatically recreated with the new number of steps. Note that you may need to update MeshLODSubdivisions and MeshLODSizes arrays.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set the Mesh Detail Mode to Level of Detail (LOD)
                PlanetManager.MeshDetailMode = PlanetManager.MeshDetailModes.LOD;

                // Set number of LOD levels to 5
                PlanetManager.MeshLODSteps = 5;

                // Set the mesh detail levels by using an array of subdivision levels
                // The first value in the array is the highest level of detail and the last value in the array is the lowest quality
                PlanetManager.MeshLODSubdivisions = new int[5] { 6, 5, 4, 3, 2 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.MeshLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="MeshDetailMode"/>        
        /// <seealso cref="MeshLODSubdivisions"/>
        /// <seealso cref="MeshLODPlanetSizes"/>
        public static int MeshLODSteps {
            get { return Instance._meshLODSteps; }
            set {
                if (value < 2 || value > 5)
                    Debug.LogWarning("Cannot set steps to specified value, must be between 2-5. Setting value: " + Mathf.Clamp(value, 2, 5));

                if (Instance._meshLODSteps != Mathf.Clamp(value, 2,5))
                {
                    Instance._meshLODSteps = Mathf.Clamp(value, 2, 5);

                    // If MeshLODPlanetSizes array is too small, resize and pad it with extrapolated values
                    if (MeshLODPlanetSizes.Length < Instance._meshLODSteps - 1)
                        MeshLODPlanetSizes = Instance.PadArray(MeshLODPlanetSizes, Instance._meshLODSteps - 1);

                    // If MeshLODSubdivisions array is too small, resize and pad it with last value of original array
                    if (MeshLODSubdivisions.Length < Instance._meshLODSteps)
                        MeshLODSubdivisions = Instance.PadArray(MeshLODSubdivisions, Instance._meshLODSteps);

                    // Recreate the LOD meshes
                    Instance.RecreateProceduralMeshes();
                }                
            } }
        [SerializeField] private int _meshLODSteps = 5;

        /// <summary>
        /// Helper method  to resize float array and pad it with extrapolated values 
        /// </summary>
        /// <param name="_array">Original array</param>
        /// <param name="_length">New length of array</param>
        /// <returns>Padded float array</returns>
        float[] PadArray(float[] _array, int _length)
        {
            float[] _returnArray = new float[_length];
            if (_array.Length < _length)
            {
                for (int _i = 0; _i < _length; _i++)
                {
                    if (_i < _array.Length)
                        _returnArray[_i] = _array[_i];
                    else
                        _returnArray[_i] = Mathf.Lerp(_array[_array.Length - 1], 0.0f, (_i - _array.Length + 1) / (_length - _array.Length));
                }
            }
            return _returnArray;
        }

        /// <summary>
        /// Helper method to resize int array and pad it with the last value of the original array
        /// </summary>
        /// <param name="_array">Original array</param>
        /// <param name="_length">New length of array</param>
        /// <returns>Padded int array</returns>
        int[] PadArray(int[] _array, int _length)
        {
            int[] _returnArray = new int[_length];
            if (_array.Length < _length)
            {
                for (int _i = 0; _i < _length; _i++)
                {
                    if (_i < _array.Length)
                        _returnArray[_i] = _array[_i];
                    else
                        _returnArray[_i] = _array[_array.Length - 1];
                }
            }
            return _returnArray;
        }


        /// <summary>
        /// Array of Level of Detail (LOD) subdivisions levels. Valid values in array: 0 - 6.       
        /// The first entry in array is highest level of detail, last entry in array is lowest level of detail.
        /// 
        /// Subdivision levels: <br>
        /// 0 = 8 triangles <br>
        /// 1 = 32 triangles <br>
        /// 2 = 128 triangles <br>
        /// 3 = 512 triangles <br>
        /// 4 = 2048 triangles <br>
        /// 5 = 8192 triangles <br>
        /// 6 = 32768 triangles <br>
        /// 
        /// Default array is 6,5,4,3,2 (32768 (highest LOD), 8192, 2048, 512, 128 (lowest LOD) triangles)
        /// 
        /// Important: Size of array must be same as MeshLODSteps is set to.
        /// 
        /// When changed, the LOD meshes are automatically recreated with the new number of subdivisions.
        /// </summary>
         /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set the Mesh Detail Mode to Level of Detail (LOD)
                PlanetManager.MeshDetailMode = PlanetManager.MeshDetailModes.LOD;

                // Set number of LOD levels to 5
                PlanetManager.MeshLODSteps = 5;

                // Set the mesh detail levels by using an array of subdivision levels
                // The first value in the array is the highest level of detail and the last value in the array is the lowest quality
                PlanetManager.MeshLODSubdivisions = new int[5] { 6, 5, 4, 3, 2 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.MeshLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="MeshDetailMode"/>
        /// <seealso cref="MeshLODSteps"/>
        /// <seealso cref="MeshLODPlanetSizes"/>
        public static int[] MeshLODSubdivisions { get { return Instance._meshLODSubdivisions; }
            set {
                if (value.Length != MeshLODSteps)
                {
                    Debug.LogError("Array must be the same length as MeshLODSteps property (currently set to " + MeshLODSteps + ") - Aborting.");
                    return;
                }
                
                Instance._meshLODSubdivisions = value;                
            } }
        [SerializeField] public int[] _meshLODSubdivisions = new int[5] { 6, 5, 4, 3, 2 };

        /// <summary>
        /// Array of Level of Detail (LOD) planet size levels. Valid values in array = 0.0f and up
        /// The first entry in array is highest level of detail, last entry in array is lowest level of detail.
        /// 
        /// The sizes are specified from 0.0f (0%) - 1.0f (100%) but can also be higher than 1.0 (e.g. 1.5 = 150%)
        /// The size of the planet is calculated by comparing the planet diameter as seen by a specified camera (main camera by default) and comparing it to the height of screen.
        /// 
        /// Default array is { 0.8f, 0.3f, 0.1f, 0.05f }
        /// 
        /// Important: Size of array must be MeshLODSteps - 1 
        /// 
        /// The reason it's - 1 is because the sizes specifies between each step and if there are 5 steps there are only 4 transitions between the 5 steps, e.g.:
        ///   (Step0) Size0 (Step1) Size1 (Step2) Size2 (Step3) Size3 (Step4)
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set the Mesh Detail Mode to Level of Detail (LOD)
                PlanetManager.MeshDetailMode = PlanetManager.MeshDetailModes.LOD;

                // Set number of LOD levels to 5
                PlanetManager.MeshLODSteps = 5;

                // Set the mesh detail levels by using an array of subdivision levels
                // The first value in the array is the highest level of detail and the last value in the array is the lowest quality
                PlanetManager.MeshLODSubdivisions = new int[5] { 6, 5, 4, 3, 2 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.MeshLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="MeshDetailMode"/>
        /// <seealso cref="MeshLODSteps"/>
        /// <seealso cref="MeshLODSubdivisions"/>
        public static float[] MeshLODPlanetSizes {
            get { return Instance._meshLODPlanetSizes; }
            set {
                if (value.Length != MeshLODSteps -1)
                {
                    Debug.LogError("Array must be the same length as MeshLODSteps - 1 so this array must contain " + (MeshLODSteps - 1) + " entries - Aborting.");
                    return;
                }
                Instance._meshLODPlanetSizes = value;                
            } }
        [SerializeField] public float[] _meshLODPlanetSizes = new float[4] { 0.8f, 0.3f, 0.01f, 0.05f };

        /// <summary>
        /// (Read-only) Array of meshes for Level of Detail (LOD) levels. This is automatically created when specifying 
        /// </summary>
        public static Mesh[] MeshLODMeshes { get { return Instance._meshLODMeshes; } }
        [SerializeField] private Mesh[] _meshLODMeshes;


        /// <summary>
        /// Texture detail mode specifies how textures should be generated. Planets consists of multiple textures (e.g. Composition Texture, 2 x Biome Textures, Clouds Texture, etc.).
        /// 
        /// Static: All textures for planet have a fixed resolution.        
        /// Static_Separate: Different textures for planets have individually fixed resolutions.
        /// Progressive: All textures for a planet have the same resolution but iteratively improves when planet is created to give fast load time.
        /// Progressive_Separate: Textures iteratively improves with independently controlled progression for different planet textures.
        /// LOD: All textures have the same resolution but varies based on Level of Detail (LOD) which determines and updates textures based on how large the planet is visibly on the screen.
        /// LOD_Separate: Textures have independently controlled Level of Detail (LOD) resolutions to determine and update textures based on how large the planet is visibly on the screen.
        /// 
        /// Note: Changing the TextureDetailMode forces rebuild of all planet textures.
        /// </summary>        
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureStaticCommon"/>>
        /// <seealso cref="TextureStaticComposition"/>>
        /// <seealso cref="TextureStaticBiome"/>>
        /// <seealso cref="TextureStaticClouds"/>>
        /// <seealso cref="TextureStaticCities"/>>
        /// <seealso cref="TextureStaticLava"/>>
        /// <seealso cref="TextureStaticPolarIce"/>>
        /// <seealso cref="TextureStaticGas"/>>
        public static TextureDetailModes TextureDetailMode {
            get { return Instance._textureDetailMode; }
            set {
                if (Instance._textureDetailMode != value)
                {
                    Instance._textureDetailMode = value;
                    Instance.RebuildAllPlanetTextures(true);
                }
            } }
        [SerializeField] private TextureDetailModes _textureDetailMode = TextureDetailModes.Static_Separate;

        /// <summary>
        /// Gets or sets texture resolution for all textures when TextureDetailMode is set to TextureDetailModes.Static
        /// 
        /// All planets and planet textures share this resolution.
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// 
        /// Note: Changing this value forces a rebuild of textures of existing planets in the scene.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static;

                // Set all planet textures to level 7 (which is 2048 x 2048 pixels)
                // This automatically rebuilds any textures displayed by any planet
                PlanetManager.TextureStaticCommon = 7;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureStaticComposition"/>>
        /// <seealso cref="TextureStaticBiome"/>>
        /// <seealso cref="TextureStaticClouds"/>>
        /// <seealso cref="TextureStaticCities"/>>
        /// <seealso cref="TextureStaticLava"/>>
        /// <seealso cref="TextureStaticPolarIce"/>>
        /// <seealso cref="TextureStaticGas"/>>
        public static int TextureStaticCommon
        {
            get { return Instance._textureStaticCommon; }
            set { Instance.SetTextureResolution(value, 0, 7, ref Instance._textureStaticCommon);}
        }
        [SerializeField] private int _textureStaticCommon = 5;

        /// <summary>
        /// Gets or sets texture resolution for Composition texture when TextureDetailMode is set to TextureDetailModes.Static_Separate.
        /// 
        /// All planets share this resolution for the Composition texture.
        /// 
        /// The Composition texture controls the layout of continents, mixture of biome materials, 
        /// water coverage, shorelines, polar coverage and more. It is generally more important to
        /// have this texture at a higher resolution compared to Biome textures for example. 
        /// If you can only have a few textures at a higher resolution, make it the Compositionn 
        /// texture and the Clouds texture.
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// 
        /// Note: Changing this value forces a rebuild of textures of existing planets in the scene.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static_Separate;

                // Set all planet Composition textures to level 7 (which is 2048 x 2048 pixels)
                // This automatically rebuilds any textures displayed by any planet
                PlanetManager.TextureStaticComposition = 7;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureStaticCommon"/>>
        /// <seealso cref="TextureStaticBiome"/>>
        /// <seealso cref="TextureStaticClouds"/>>
        /// <seealso cref="TextureStaticCities"/>>
        /// <seealso cref="TextureStaticLava"/>>
        /// <seealso cref="TextureStaticPolarIce"/>>
        /// <seealso cref="TextureStaticGas"/>>
        public static int TextureStaticComposition
        {
            get { return Instance._textureStaticComposition; }
            set { Instance.SetTextureResolution(value, 0, 7, ref Instance._textureStaticComposition); }
        }
        [SerializeField] private int _textureStaticComposition = 7;

        /// <summary>
        /// Gets or sets texture resolution for Biome texture when TextureDetailMode is set to TextureDetailModes.Static_Separate.
        /// 
        /// The Biome texture dictates the surface of a planet, e.g. desert, forest, ice, tundra, etc.
        /// The Composition texture controls the coverage and combination of two Biome textures. 
        /// It is not very important that the Biome texture is high resolution since it is tiled much more
        /// compared to the Compositionn, Clouds, and Cities textures.

        /// 
        /// All planets share this resolution for the Biome texture.
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// 
        /// Note: Changing this value forces a rebuild of textures of existing planets in the scene.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static_Separate;

                // Set all planet Biome textures to level 7 (which is 2048 x 2048 pixels)
                // This automatically rebuilds any textures displayed by any planet
                PlanetManager.TextureStaticBiome = 7;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureStaticCommon"/>>
        /// <seealso cref="TextureStaticComposition"/>>
        /// <seealso cref="TextureStaticClouds"/>>
        /// <seealso cref="TextureStaticCities"/>>
        /// <seealso cref="TextureStaticLava"/>>
        /// <seealso cref="TextureStaticPolarIce"/>>
        /// <seealso cref="TextureStaticGas"/>>
        public static int TextureStaticBiome
        {
            get { return Instance._textureStaticBiome; }
            set { Instance.SetTextureResolution(value, 0, 7, ref Instance._textureStaticBiome); }
        }
        [SerializeField] private int _textureStaticBiome = 7;

        /// <summary>
        /// Gets or sets texture resolution for Clouds texture when TextureDetailMode is set to TextureDetailModes.Static_Separate.
        /// 
        /// The Clouds texture defines the clouds around a planet. It is generally important
        /// to keep the Clouds texture as high of a resolution as you can since it is not
        /// tiled as frequently as, for example, the Biome textures.
        /// 
        /// All planets share this resolution for the Clouds texture.
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// 
        /// Note: Changing this value forces a rebuild of textures of existing planets in the scene.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static_Separate;

                // Set all planet Clouds textures to level 7 (which is 2048 x 2048 pixels)
                // This automatically rebuilds any textures displayed by any planet
                PlanetManager.TextureStaticClouds = 7;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureStaticCommon"/>>
        /// <seealso cref="TextureStaticComposition"/>>
        /// <seealso cref="TextureStaticBiome"/>>
        /// <seealso cref="TextureStaticCities"/>>
        /// <seealso cref="TextureStaticLava"/>>
        /// <seealso cref="TextureStaticPolarIce"/>>
        // <seealso cref="TextureStaticGas"/>>
        public static int TextureStaticClouds
        {
            get { return Instance._textureStaticClouds; }
            set { Instance.SetTextureResolution(value, 0, 7, ref Instance._textureStaticClouds); }
        }
        [SerializeField] private int _textureStaticClouds = 7;

        /// <summary>
        /// Gets or sets texture resolution for Cities texture when TextureDetailMode is set to TextureDetailModes.Static_Separate.
        /// 
        /// The Cities texture controls the night lights from cities on the dark side of the planet.
        /// 
        /// All planets share this resolution for the Cities texture.
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// 
        /// Note: Changing this value forces a rebuild of textures of existing planets in the scene.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static_Separate;

                // Set all planet Cities textures to level 7 (which is 2048 x 2048 pixels)
                // This automatically rebuilds any textures displayed by any planet
                PlanetManager.TextureStaticCities = 7;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureStaticCommon"/>>
        /// <seealso cref="TextureStaticComposition"/>>
        /// <seealso cref="TextureStaticBiome"/>>
        /// <seealso cref="TextureStaticClouds"/>>
        /// <seealso cref="TextureStaticLava"/>>
        /// <seealso cref="TextureStaticPolarIce"/>>
        /// <seealso cref="TextureStaticGas"/>>
        public static int TextureStaticCities
        {
            get { return Instance._textureStaticCities; }
            set { Instance.SetTextureResolution(value, 0, 7, ref Instance._textureStaticCities); }
        }
        [SerializeField] private int _textureStaticCities = 7;

        /// <summary>
        /// Gets or sets texture resolution for Lava texture when TextureDetailMode is set to TextureDetailModes.Static_Separate.
        /// 
        /// The Lava texture contains the animated lava on a planet if used. The lava is animated
        /// using a flowmap which is generated at the same resolution.
        ///
        /// All planets share this resolution for the Lava texture.
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// 
        /// Note: Changing this value forces a rebuild of textures of existing planets in the scene.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static_Separate;

                // Set all planet Lava textures to level 7 (which is 2048 x 2048 pixels)
                // This automatically rebuilds any textures displayed by any planet
                PlanetManager.TextureStaticLava = 7;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureStaticCommon"/>>
        /// <seealso cref="TextureStaticComposition"/>>
        /// <seealso cref="TextureStaticBiome"/>>
        /// <seealso cref="TextureStaticClouds"/>>
        /// <seealso cref="TextureStaticCities"/>>
        /// <seealso cref="TextureStaticPolarIce"/>>
        /// <seealso cref="TextureStaticGas"/>>
        public static int TextureStaticLava
        {
            get { return Instance._textureStaticLava; }
            set { Instance.SetTextureResolution(value, 0, 7, ref Instance._textureStaticLava); }
        }
        [SerializeField] private int _textureStaticLava = 7;

        /// <summary>
        /// Gets or sets texture resolution for PolarIce texture when TextureDetailMode is set to TextureDetailModes.Static_Separate.
        /// 
        /// The PolarIce texture is used at the polar caps of planets - it replaces the
        /// biome texture where the polar caps reaches. 
        ///
        /// All planets share this resolution for the PolarIce texture.
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// 
        /// Note: Changing this value forces a rebuild of textures of existing planets in the scene.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static_Separate;

                // Set all planet PolarIce textures to level 7 (which is 2048 x 2048 pixels)
                // This automatically rebuilds any textures displayed by any planet
                PlanetManager.TextureStaticPolarIce = 7;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureStaticCommon"/>>
        /// <seealso cref="TextureStaticComposition"/>>
        /// <seealso cref="TextureStaticBiome"/>>
        /// <seealso cref="TextureStaticClouds"/>>
        /// <seealso cref="TextureStaticCities"/>>
        /// <seealso cref="TextureStaticLava"/>>
        /// <seealso cref="TextureStaticGas"/>>
        public static int TextureStaticPolarIce
        {
            get { return Instance._textureStaticPolarIce; }
            set { Instance.SetTextureResolution(value, 0, 7, ref Instance._textureStaticPolarIce); }
        }
        [SerializeField] private int _textureStaticPolarIce = 7;

        /// <summary>
        /// Gets or sets texture resolution for Gas texture when TextureDetailMode is set to TextureDetailModes.Static_Separate.
        /// 
        /// The Gas texture is used for all texture maps for Gas Planets.
        ///
        /// All planets share this resolution for the Gas texture.
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// 
        /// Note: Changing this value forces a rebuild of textures of existing planets in the scene.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Static (same texture resolution for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static_Separate;

                // Set all planet Gas textures to level 7 (which is 2048 x 2048 pixels)
                // This automatically rebuilds any textures displayed by any planet
                PlanetManager.TextureStaticGas = 7;
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureStaticCommon"/>>
        /// <seealso cref="TextureStaticComposition"/>>
        /// <seealso cref="TextureStaticBiome"/>>
        /// <seealso cref="TextureStaticClouds"/>>
        /// <seealso cref="TextureStaticCities"/>>
        /// <seealso cref="TextureStaticLava"/>>
        /// <seealso cref="TextureStaticGas"/>>
        public static int TextureStaticGas
        {
            get { return Instance._textureStaticGas; }
            set { Instance.SetTextureResolution(value, 0, 7, ref Instance._textureStaticGas); }
        }
        [SerializeField] private int _textureStaticGas = 7;

        /// <summary>
        /// Helper method for Texture Properties.
        /// </summary>
        /// <param name="_value">Value to set</param>
        /// <param name="_min">Minimum value permitted</param>
        /// <param name="_max">Maximum value permitted</param>
        /// <param name="_textureRef">Reference to the property</param>
        void SetTextureResolution(int _value, int _min, int _max, ref int _textureRef)
        {
            if (_value < _min || _value > _max)
                Debug.LogWarning("Cannot set texture resolution to specified value, must be between " + _min + "-" + _max + ". Setting value: " + Mathf.Clamp(_value, _min, _max));

            if (_textureRef != Mathf.Clamp(_value, _min, _max))
            {
                _textureRef = Mathf.Clamp(_value, _min, _max);
                Instance.RebuildAllPlanetTextures(true);
            }
        }

        /// <summary>
        /// Gets or sets the number of steps / levels for progressive texture resolutions..
        /// 
        /// Valid values are 2-5. (1 is not allowed, set static texture resolution instead)        
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive (same texture resolution progression for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressiveCommon = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveCommon"/>>
        /// <seealso cref="TextureProgressiveComposition"/>>
        /// <seealso cref="TextureProgressiveBiome"/>>
        /// <seealso cref="TextureProgressiveClouds"/>>
        /// <seealso cref="TextureProgressiveCities"/>>
        /// <seealso cref="TextureProgressiveLava"/>>
        /// <seealso cref="TextureProgressivePolarIce"/>>
        /// <seealso cref="TextureProgressiveGas"/>>
        public static int TextureProgressiveSteps {
            get { return Instance._textureProgressiveSteps; }
            set
            {
                if (value < 2 || value > 5)
                    Debug.LogWarning("Cannot set steps to specified value, must be between 2-5. Setting value: " + Mathf.Clamp(value, 2, 5));

                if (Instance._textureProgressiveSteps != Mathf.Clamp(value, 2, 5))
                {
                    Instance._textureProgressiveSteps = Mathf.Clamp(value, 2, 5);

                    // If TextureProgressive arrays are too small, resize and pad them with last value of original array
                    if (TextureProgressiveCommon.Length < Instance._textureProgressiveSteps)
                        TextureProgressiveCommon = Instance.PadArray(TextureProgressiveCommon, Instance._textureProgressiveSteps);
                    if (TextureProgressiveComposition.Length < Instance._textureProgressiveSteps)
                        TextureProgressiveComposition = Instance.PadArray(TextureProgressiveComposition, Instance._textureProgressiveSteps);
                    if (TextureProgressiveBiome.Length < Instance._textureProgressiveSteps)
                        TextureProgressiveBiome = Instance.PadArray(TextureProgressiveBiome, Instance._textureProgressiveSteps);
                    if (TextureProgressiveClouds.Length < Instance._textureProgressiveSteps)
                        TextureProgressiveClouds = Instance.PadArray(TextureProgressiveClouds, Instance._textureProgressiveSteps);
                    if (TextureProgressiveCities.Length < Instance._textureProgressiveSteps)
                        TextureProgressiveCities = Instance.PadArray(TextureProgressiveCities, Instance._textureProgressiveSteps);
                    if (TextureProgressiveLava.Length < Instance._textureProgressiveSteps)
                        TextureProgressiveLava = Instance.PadArray(TextureProgressiveLava, Instance._textureProgressiveSteps);
                    if (TextureProgressivePolarIce.Length < Instance._textureProgressiveSteps)
                        TextureProgressivePolarIce = Instance.PadArray(TextureProgressivePolarIce, Instance._textureProgressiveSteps);
                }
            }
        }
        [SerializeField] private int _textureProgressiveSteps = 5;

        /// <summary>
        /// Helper to set a texture resolution array.
        /// </summary>
        /// <param name="_newArray">New array property is trying to set.</param>
        /// <param name="_expectedArrayLength">Expected array length.</param>
        /// <param name="_stepsPropertyName">Steps property.</param>
        /// <param name="_array">Original array.</param>
        /// <returns>Int array.</returns>
        int[] SetTextureResolutionArray(int[] _newArray, int _expectedArrayLength, string _stepsPropertyName, ref int[] _array)
        {
            if (_newArray.Length != _expectedArrayLength)
            {
                Debug.LogError("Array must be the same size as " + _stepsPropertyName + " ( " + _expectedArrayLength + ") - Aborting.");
                return null;
            }
            for (int _i = 0; _i < _newArray.Length; _i++)
            {
                if (_newArray[_i] < 0 || _newArray[_i] >= TEXTURE_RESOLUTION_COUNT)
                {
                    Debug.LogError("Array contains invalid element at index " + _i + " (value must be between 0 and " + (TEXTURE_RESOLUTION_COUNT - 1) + ") - Aborting.");
                    return null;
                }
            }
            return _array;
        }

        /// <summary>
        /// Gets or sets the array of texture resolution levels for progressive textures when TextureDetailMode = TextureDetailModes.Progressive
        /// 
        /// Array size must be the same as TextureProgressiveSteps.
        /// 
        /// Valid values in the arary are 0 (16 x 16 pixels) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive (same texture resolution progression for all planets and textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressiveCommon = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveSteps"/>>
        /// <seealso cref="TextureProgressiveComposition"/>>
        /// <seealso cref="TextureProgressiveBiome"/>>
        /// <seealso cref="TextureProgressiveClouds"/>>
        /// <seealso cref="TextureProgressiveCities"/>>
        /// <seealso cref="TextureProgressiveLava"/>>
        /// <seealso cref="TextureProgressivePolarIce"/>>
        /// <seealso cref="TextureProgressiveGas"/>>
        public static int[] TextureProgressiveCommon {
            get { return Instance._textureProgressiveCommon; }
            set 
            { 
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureProgressiveSteps, "TextureProgressiveSteps", ref Instance._textureProgressiveCommon);
                if (_newArray != null) Instance._textureProgressiveCommon = _newArray;
            }
        }
        [SerializeField] int[] _textureProgressiveCommon = new int[5] { 3, 4, 5, 6, 7 };

        /// <summary>
        /// Gets or sets the array of texture resolution levels for Composition progressive textures when TextureDetailMode = TextureDetailModes.Progressive_Separate
        /// 
        /// The Composition texture controls the layout of continents, mixture of biome materials, 
        /// water coverage, shorelines, polar coverage and more. It is generally more important to
        /// have this texture at a higher resolution compared to Biome textures for example. 
        /// If you can only have a few textures at a higher resolution, make it the Compositionn 
        /// texture and the Clouds texture.
        /// 
        /// Array size must be the same as TextureProgressiveSteps.
        /// 
        /// Valid values in the arary are 0 (16 x 16 pixels) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive_Separate (separate texture resolutions for different types of planet textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive_Separate;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressiveComposition = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveSteps"/>>
        /// <seealso cref="TextureProgressiveCommon"/>>
        /// <seealso cref="TextureProgressiveBiome"/>>
        /// <seealso cref="TextureProgressiveClouds"/>>
        /// <seealso cref="TextureProgressiveCities"/>>
        /// <seealso cref="TextureProgressiveLava"/>>
        /// <seealso cref="TextureProgressivePolarIce"/>>
        /// <seealso cref="TextureProgressiveGas"/>>
        public static int[] TextureProgressiveComposition
        {
            get { return Instance._textureProgressiveComposition; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureProgressiveSteps, "TextureProgressiveSteps", ref Instance._textureProgressiveComposition);
                if (_newArray != null) Instance._textureProgressiveComposition = _newArray;
            }
        }
        [SerializeField] private int[] _textureProgressiveComposition = new int[5] { 3, 4, 5, 6, 7 };

        /// <summary>
        /// Gets or sets the array of texture resolution levels for Biome progressive textures when TextureDetailMode = TextureDetailModes.Progressive_Separate
        /// 
        /// The Biome texture dictates the surface of a planet, e.g. desert, forest, ice, tundra, etc.
        /// The Composition texture controls the coverage and combination of two Biome textures. 
        /// It is not very important that the Biome texture is high resolution since it is tiled much more
        /// compared to the Compositionn, Clouds, and Cities textures.
        /// 
        /// Array size must be the same as TextureProgressiveSteps.
        /// 
        /// Valid values in the arary are 0 (16 x 16 pixels) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive_Separate (separate texture resolutions for different types of planet textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive_Separate;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressiveBiome = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveSteps"/>>
        /// <seealso cref="TextureProgressiveCommon"/>>
        /// <seealso cref="TextureProgressiveComposition"/>>
        /// <seealso cref="TextureProgressiveClouds"/>>
        /// <seealso cref="TextureProgressiveCities"/>>
        /// <seealso cref="TextureProgressiveLava"/>>
        /// <seealso cref="TextureProgressivePolarIce"/>>
        /// <seealso cref="TextureProgressiveGas"/>>
        public static int[] TextureProgressiveBiome
        {
            get { return Instance._textureProgressiveBiome; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureProgressiveSteps, "TextureProgressiveSteps", ref Instance._textureProgressiveBiome);
                if (_newArray != null) Instance._textureProgressiveBiome = _newArray;
            }
        }
        [SerializeField] private int[] _textureProgressiveClouds = new int[5] { 3, 4, 5, 6, 7 };

        /// <summary>
        /// Gets or sets the array of texture resolution levels for Clouds progressive textures when TextureDetailMode = TextureDetailModes.Progressive_Separate
        /// 
        /// The Clouds texture defines the clouds around a planet. It is generally important
        /// to keep the Clouds texture as high of a resolution as you can since it is not
        /// tiled as frequently as, for example, the Biome textures.
        /// 
        /// Array size must be the same as TextureProgressiveSteps.
        /// 
        /// Valid values in the arary are 0 (16 x 16 pixels) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive_Separate (separate texture resolutions for different types of planet textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive_Separate;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressiveClouds = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveSteps"/>>
        /// <seealso cref="TextureProgressiveCommon"/>>
        /// <seealso cref="TextureProgressiveComposition"/>>
        /// <seealso cref="TextureProgressiveBiome"/>>
        /// <seealso cref="TextureProgressiveCities"/>>
        /// <seealso cref="TextureProgressiveLava"/>>
        /// <seealso cref="TextureProgressivePolarIce"/>>
        /// <seealso cref="TextureProgressiveGas"/>>
        public static int[] TextureProgressiveClouds
        {
            get { return Instance._textureProgressiveClouds; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureProgressiveSteps, "TextureProgressiveSteps", ref Instance._textureProgressiveClouds);
                if (_newArray != null) Instance._textureProgressiveClouds = _newArray;
            }
        }
        [SerializeField] private int[] _textureProgressiveBiome = new int[5] { 3, 4, 5, 6, 7 };

        /// <summary>
        /// Gets or sets the array of texture resolution levels for Cities progressive textures when TextureDetailMode = TextureDetailModes.Progressive_Separate
        /// 
        /// The Cities texture controls the night lights from cities on the dark side of the planet.
        /// 
        /// Array size must be the same as TextureProgressiveSteps.
        /// 
        /// Valid values in the arary are 0 (16 x 16 pixels) - 7 (2048 x 2048)
        ///
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive_Separate (separate texture resolutions for different types of planet textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive_Separate;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressiveCities = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveSteps"/>>
        /// <seealso cref="TextureProgressiveCommon"/>>
        /// <seealso cref="TextureProgressiveComposition"/>>
        /// <seealso cref="TextureProgressiveBiome"/>>
        /// <seealso cref="TextureProgressiveClouds"/>>
        /// <seealso cref="TextureProgressiveLava"/>>
        /// <seealso cref="TextureProgressivePolarIce"/>>
        /// <seealso cref="TextureProgressiveGas"/>>
        public static int[] TextureProgressiveCities
        {
            get { return Instance._textureProgressiveCities; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureProgressiveSteps, "TextureProgressiveSteps", ref Instance._textureProgressiveCities);
                if (_newArray != null) Instance._textureProgressiveCities = _newArray;
            }
        }
        [SerializeField] private int[] _textureProgressiveCities = new int[5] { 3, 4, 5, 6, 7 };

        /// <summary>
        /// Gets or sets the array of texture resolution levels for Lava progressive textures when TextureDetailMode = TextureDetailModes.Progressive_Separate
        /// 
        /// The Lava texture contains the animated lava on a planet if used. The lava is animated
        /// using a flowmap which is generated at the same resolution.
        /// 
        /// Array size must be the same as TextureProgressiveSteps.
        /// 
        /// Valid values in the arary are 0 (16 x 16 pixels) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive_Separate (separate texture resolutions for different types of planet textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive_Separate;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressiveLava = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveSteps"/>>
        /// <seealso cref="TextureProgressiveCommon"/>>
        /// <seealso cref="TextureProgressiveBiome"/>>
        /// <seealso cref="TextureProgressiveComposition"/>>
        /// <seealso cref="TextureProgressiveClouds"/>>
        /// <seealso cref="TextureProgressiveCities"/>>
        /// <seealso cref="TextureProgressivePolarIce"/>>
        /// <seealso cref="TextureProgressiveGas"/>>
        public static int[] TextureProgressiveLava
        {
            get { return Instance._textureProgressiveLava; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureProgressiveSteps, "TextureProgressiveSteps", ref Instance._textureProgressiveLava);
                if (_newArray != null) Instance._textureProgressiveLava = _newArray;
            }
        }
        [SerializeField] private int[] _textureProgressiveLava = new int[5] { 3, 4, 5, 6, 7 };

        /// <summary>
        /// Gets or sets the array of texture resolution levels for PolarIce progressive textures when TextureDetailMode = TextureDetailModes.Progressive_Separate
        /// 
        /// The PolarIce texture is used at the polar caps of planets - it replaces the
        /// biome texture where the polar caps reaches.
        /// 
        /// Array size must be the same as TextureProgressiveSteps.
        /// 
        /// Valid values in the arary are 0 (16 x 16 pixels) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive_Separate (separate texture resolutions for different types of planet textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive_Separate;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressivePolarIce = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveSteps"/>>
        /// <seealso cref="TextureProgressiveCommon"/>>
        /// <seealso cref="TextureProgressiveComposition"/>>
        /// <seealso cref="TextureProgressiveBiome"/>>
        /// <seealso cref="TextureProgressiveClouds"/>>
        /// <seealso cref="TextureProgressiveCities"/>>
        /// <seealso cref="TextureProgressiveLava"/>>
        /// <seealso cref="TextureProgressiveGas"/>>
        public static int[] TextureProgressivePolarIce
        {
            get { return Instance._textureProgressivePolarIce; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureProgressiveSteps, "TextureProgressiveSteps", ref Instance._textureProgressivePolarIce);
                if (_newArray != null) Instance._textureProgressivePolarIce = _newArray;
            }
        }
        [SerializeField] private int[] _textureProgressivePolarIce = new int[5] { 3, 4, 5, 6, 7 };

        /// <summary>
        /// Gets or sets the array of texture resolution levels for Gas progressive textures when TextureDetailMode = TextureDetailModes.Progressive_Separate
        /// 
        /// The Gas texture is used for all Gas planet textures.
        /// 
        /// Array size must be the same as TextureProgressiveSteps.
        /// 
        /// Valid values in the arary are 0 (16 x 16 pixels) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to Progressive_Separate (separate texture resolutions for different types of planet textures)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Progressive_Separate;

                // Set number of progressive steps to 4.
                PlanetManager.TextureProgressiveSteps = 4;

                // Set progressive resolution array to: 2 (64 x 64), 3 (128 x 128), 4 (256 x 256), 7 (2048 x 2048)
                PlanetManager.TextureProgressiveGas = new int[4] { 2,3,4,7 };
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>>
        /// <seealso cref="TextureProgressiveSteps"/>>
        /// <seealso cref="TextureProgressiveCommon"/>>
        /// <seealso cref="TextureProgressiveComposition"/>>
        /// <seealso cref="TextureProgressiveBiome"/>>
        /// <seealso cref="TextureProgressiveClouds"/>>
        /// <seealso cref="TextureProgressiveCities"/>>
        /// <seealso cref="TextureProgressiveLava"/>>
        public static int[] TextureProgressiveGas
        {
            get { return Instance._textureProgressiveGas; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureProgressiveSteps, "TextureProgressiveSteps", ref Instance._textureProgressiveGas);
                if (_newArray != null) Instance._textureProgressiveGas = _newArray;
            }
        }
        [SerializeField] private int[] _textureProgressiveGas = new int[5] { 3, 4, 5, 6, 7 };

        /// <summary>
        /// Specifies the number of Level Of Detail (LOD) steps that should be used if TextureDetailMode.LOD or TextureDetailMode.LOD_Separate is used. Valid values = 2-5.
        /// 
        /// Default value is 5 (max).
        /// 
        /// When changed to a new value the LOD textures for planets existing in the scene are automatically recreated 
        /// with the new number of steps. Note that you may need to update TextureLOD* arrays.
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set the Texture Detail Mode to Level of Detail (LOD)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD;

                // Set number of texture LOD levels to 5
                PlanetManager.TextureLODSteps = 5;

                // Set the texture LOD detail levels by using an array of texture resolution levels
                // The first value in the array is the highest level of detail and the last value in the array is the lowest quality
                PlanetManager.TextureLODCommon = new int[5] { 7, 6, 5, 3, 1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODPlanetSizes"/>    
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODComposition"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODLava"/>
        /// <seealso cref="TextureLODPolarIce"/>
        /// <seealso cref="TextureLODGas"/>
        public static int TextureLODSteps
        {
            get { return Instance._textureLODSteps; }
            set
            {
                if (value < 2 || value > 5)
                    Debug.LogWarning("Cannot set steps to specified value, must be between 2-5. Setting value: " + Mathf.Clamp(value, 2, 5));

                if (Instance._textureLODSteps != Mathf.Clamp(value, 2, 5))
                {
                    Instance._textureLODSteps = Mathf.Clamp(value, 2, 5);
                    Instance.PadTextureLODArraysIfNecessary(Instance._textureLODSteps);
                }
            }
        }
        [SerializeField] private int _textureLODSteps = 5;

        /// <summary>
        /// Helper methid to pads the texture LOD arrays if necessary.
        /// </summary>
        /// <param name="_steps">Number of LOD steps.</param>
        void PadTextureLODArraysIfNecessary(int _steps) 
        {
            // If TextureLOD arrays are too small, resize and pad them with last value of original array
            if (TextureLODCommon.Length < _steps)
                TextureLODCommon = Instance.PadArray(TextureLODCommon, _steps);
            if (TextureLODComposition.Length < _steps)
                TextureLODComposition = Instance.PadArray(TextureLODComposition, _steps);
            if (TextureLODBiome.Length < _steps)
                TextureLODBiome = Instance.PadArray(TextureLODBiome, _steps);
            if (TextureLODClouds.Length < _steps)
                TextureLODClouds = Instance.PadArray(TextureLODClouds, _steps);
            if (TextureLODCities.Length < _steps)
                TextureLODCities = Instance.PadArray(TextureLODCities, _steps);
            if (TextureLODLava.Length < _steps)
                TextureLODLava = Instance.PadArray(TextureLODLava, _steps);
            if (TextureLODPolarIce.Length < _steps)
                TextureLODPolarIce = Instance.PadArray(TextureLODPolarIce, _steps);            
        }

        /// <summary>
        /// Gets or sets the texture LOD Common texture resolutions array.
        /// 
        /// Array size must be same as TextureLODSteps property.
        /// 
        /// Valid values in the array are 0 (16 x 16) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /// <value>The texture LOD Common array.</value>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to LOD (texture resolution for all planet textures are generated at
                // a level of detail specified for a given size of the planet on the screen.)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD;

                // Set number of LOD steps to 5.
                PlanetManager.TextureLODSteps = 5;

                // Set LOD resolution array to: 7 (2048 x 2048), 6 (1024x1024), 5 (512x512), 3 (128x128), 1 (64x64) 
                PlanetManager.TextureLODCommon = new int[5] { 7,6,5,3,1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                             
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODPlanetSizes"/>
        /// <seealso cref="TextureLODComposition"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODLava"/>
        /// <seealso cref="TextureLODPolarIce"/>
        /// <seealso cref="TextureLODGas"/>
        public static int[] TextureLODCommon
        {
            get { return Instance._textureLODCommon; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureLODSteps, "TextureLODSteps", ref Instance._textureLODCommon);
                if (_newArray != null) Instance._textureLODCommon = _newArray;
            }
        }
        [SerializeField] private int[] _textureLODCommon = new int[5] { 7, 6, 5, 4, 3 };

        /// <summary>
        /// Gets or sets the texture LOD Composition texture resolutions array. 
        /// 
        /// The Composition texture controls the layout of continents, mixture of biome materials, 
        /// water coverage, shorelines, polar coverage and more. It is generally more important to
        /// have this texture at a higher resolution compared to Biome textures for example. 
        /// If you can only have a few textures at a higher resolution, make it the Compositionn 
        /// texture and the Clouds texture.
        /// 
        /// Array size must be same as TextureLODSteps property.
        /// 
        /// Valid values in the array are 0 (16 x 16) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /// <value>The texture LOD Composition array.</value>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to LOD (texture resolution for all planet textures are independently 
                // generated at a level of detail specified for a given size of the planet on the screen.
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD_Separate;

                // Set number of LOD steps to 5.
                PlanetManager.TextureLODSteps = 5;

                // Set LOD resolution array to: 7 (2048 x 2048), 6 (1024x1024), 5 (512x512), 3 (128x128), 1 (64x64) 
                PlanetManager.TextureLODComposition = new int[5] { 7,6,5,3,1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODPlanetSizes"/>
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODLava"/>
        /// <seealso cref="TextureLODPolarIce"/>
        /// <seealso cref="TextureLODGas"/>
        public static int[] TextureLODComposition
        {
            get { return Instance._textureLODComposition; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureLODSteps, "TextureLODSteps", ref Instance._textureLODComposition);
                if (_newArray != null) Instance._textureLODComposition = _newArray;
            }
        }
        [SerializeField] private int[] _textureLODComposition = new int[5] { 7, 6, 5, 4, 3 };

        /// <summary>
        /// Gets or sets the texture LOD Biome texture resolutions array. 
        /// 
        /// The Biome texture dictates the surface of a planet, e.g. desert, forest, ice, tundra, etc.
        /// The Composition texture controls the coverage and combination of two Biome textures. 
        /// It is not very important that the Biome texture is high resolution since it is tiled much more
        /// compared to the Compositionn, Clouds, and Cities textures.
        /// 
        /// Array size must be same as TextureLODSteps property.
        /// 
        /// Valid values in the array are 0 (16 x 16) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /// <value>The texture LOD Biome array.</value>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to LOD (texture resolution for all planet textures are independently 
                // generated at a level of detail specified for a given size of the planet on the screen.
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD_Separate;

                // Set number of LOD steps to 5.
                PlanetManager.TextureLODSteps = 5;

                // Set LOD resolution array to: 7 (2048 x 2048), 6 (1024x1024), 5 (512x512), 3 (128x128), 1 (64x64) 
                PlanetManager.TextureLODBiome = new int[5] { 7,6,5,3,1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                

             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODPlanetSizes"/>
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODComposition"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODLava"/>
        /// <seealso cref="TextureLODPolarIce"/>
        /// <seealso cref="TextureLODGas"/>
        public static int[] TextureLODBiome
        {
            get { return Instance._textureLODBiome; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureLODSteps, "TextureLODSteps", ref Instance._textureLODBiome);
                if (_newArray != null) Instance._textureLODBiome = _newArray;
            }
        }
        [SerializeField] private int[] _textureLODBiome = new int[5] { 7, 6, 5, 4, 3 };

        /// <summary>
        /// Gets or sets the texture LOD Clouds texture resolutions array. 
        /// 
        /// The Clouds texture defines the clouds around a planet. It is generally important
        /// to keep the Clouds texture as high of a resolution as you can since it is not
        /// tiled as frequently as, for example, the Biome textures.
        /// 
        /// If you can only have a few textures at a higher resolution, make it the Clouds texture 
        /// and the Compositionns texture.
        /// 
        /// Array size must be same as TextureLODSteps property.
        /// 
        /// Valid values in the array are 0 (16 x 16) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels

        /// </summary>
        /// <value>The texture LOD Composition array.</value>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to LOD (texture resolution for all planet textures are independently 
                // generated at a level of detail specified for a given size of the planet on the screen.
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD_Separate;

                // Set number of LOD steps to 5.
                PlanetManager.TextureLODSteps = 5;

                // Set LOD resolution array to: 7 (2048 x 2048), 6 (1024x1024), 5 (512x512), 3 (128x128), 1 (64x64) 
                PlanetManager.TextureLODComposition = new int[5] { 7,6,5,3,1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                

             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODPlanetSizes"/>
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODComposition"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODLava"/>
        /// <seealso cref="TextureLODPolarIce"/>
        /// <seealso cref="TextureLODGas"/>
        public static int[] TextureLODClouds
        {
            get { return Instance._textureLODClouds; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureLODSteps, "TextureLODSteps", ref Instance._textureLODClouds);
                if (_newArray != null) Instance._textureLODClouds = _newArray;
            }
        }
        [SerializeField] private int[] _textureLODClouds = new int[5] { 7, 6, 5, 4, 3 };

        /// <summary>
        /// Gets or sets the texture LOD Composition texture resolutions array. 
        /// 
        /// The Cities texture controls the night lights from cities on the dark side of the planet.
        /// 
        /// Array size must be same as TextureLODSteps property.
        /// 
        /// Valid values in the array are 0 (16 x 16) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /// <value>The texture LOD Cities array.</value>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to LOD (texture resolution for all planet textures are independently 
                // generated at a level of detail specified for a given size of the planet on the screen.
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD_Separate;

                // Set number of LOD steps to 5.
                PlanetManager.TextureLODSteps = 5;

                // Set LOD resolution array to: 7 (2048 x 2048), 6 (1024x1024), 5 (512x512), 3 (128x128), 1 (64x64) 
                PlanetManager.TextureLODCities = new int[5] { 7,6,5,3,1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                

             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODPlanetSizes"/>
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODLava"/>
        /// <seealso cref="TextureLODPolarIce"/>
        /// <seealso cref="TextureLODGas"/>
        public static int[] TextureLODCities
        {
            get { return Instance._textureLODCities; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureLODSteps, "TextureLODSteps", ref Instance._textureLODCities);
                if (_newArray != null) Instance._textureLODCities = _newArray;
            }
        }
        [SerializeField] private int[] _textureLODCities = new int[5] { 7, 6, 5, 4, 3 };

        /// <summary>
        /// Gets or sets the texture LOD Lava texture resolutions array. 
        /// 
        /// The Lava texture contains the animated lava on a planet if used. The lava is animated
        /// using a flowmap which is generated at the same resolution.
        /// 
        /// Array size must be same as TextureLODSteps property.
        /// 
        /// Valid values in the array are 0 (16 x 16) - 7 (2048 x 2048)
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /// <value>The texture LOD Lava array.</value>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to LOD (texture resolution for all planet textures are independently 
                // generated at a level of detail specified for a given size of the planet on the screen.
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD_Separate;

                // Set number of LOD steps to 5.
                PlanetManager.TextureLODSteps = 5;

                // Set LOD resolution array to: 7 (2048 x 2048), 6 (1024x1024), 5 (512x512), 3 (128x128), 1 (64x64) 
                PlanetManager.TextureLODLava = new int[5] { 7,6,5,3,1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                

             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODPlanetSizes"/>
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODComposition"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODPolarIce"/>
        /// <seealso cref="TextureLODGas"/>
        public static int[] TextureLODLava
        {
            get { return Instance._textureLODLava; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureLODSteps, "TextureLODSteps", ref Instance._textureLODLava);
                if (_newArray != null) Instance._textureLODLava = _newArray;
            }
        }
        [SerializeField] private int[] _textureLODLava = new int[5] { 7, 6, 5, 4, 3 };

        /// <summary>
        /// Gets or sets the texture LOD PolarIce texture resolutions array. 
        /// 
        /// The PolarIce texture is used at the polar caps of planets - it replaces the
        /// biome texture where the polar caps reaches.
        /// 
        /// Array size must be same as TextureLODSteps property.
        /// 
        /// Valid values in the array are 0 (16 x 16) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /// <value>The texture LOD PolarIce array.</value>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to LOD (texture resolution for all planet textures are independently 
                // generated at a level of detail specified for a given size of the planet on the screen.
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD_Separate;

                // Set number of LOD steps to 5.
                PlanetManager.TextureLODSteps = 5;

                // Set LOD resolution array to: 7 (2048 x 2048), 6 (1024x1024), 5 (512x512), 3 (128x128), 1 (64x64) 
                PlanetManager.TextureLODPolarIce = new int[5] { 7,6,5,3,1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODPlanetSizes"/>
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODComposition"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODLava"/>
        /// <seealso cref="TextureLODGas"/>
        public static int[] TextureLODPolarIce
        {
            get { return Instance._textureLODPolarIce; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureLODSteps, "TextureLODSteps", ref Instance._textureLODPolarIce);
                if (_newArray != null) Instance._textureLODPolarIce = _newArray;
            }
        }
        [SerializeField] private int[] _textureLODPolarIce = new int[5] { 7, 6, 5, 4, 3 };


        /// <summary>
        /// Gets or sets the texture LOD Gas texture resolutions array. 
        /// 
        /// The Gas texture is used for all Gas planet textures.
        /// 
        /// Array size must be same as TextureLODSteps property.
        /// 
        /// Valid values in the array are 0 (16 x 16) - 7 (2048 x 2048).
        /// 
        /// 0 - 16 x 16 pixels, <br>
        /// 1 - 32 x 32 pixels, <br>
        /// 2 - 64 x 64 pixels, <br>
        /// 3 - 128 x 128 pixels, <br>
        /// 4 - 256 x 256 pixels, <br>
        /// 5 - 512 x 512 pixels, <br>
        /// 6 - 1024 x 1024 pixels, <br>
        /// 7 - 2048 x 2048 pixels
        /// </summary>
        /// <value>The texture LOD Gas array.</value>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set planet texture detail mode to LOD (texture resolution for all planet textures are independently 
                // generated at a level of detail specified for a given size of the planet on the screen.
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD_Separate;

                // Set number of LOD steps to 5.
                PlanetManager.TextureLODSteps = 5;

                // Set LOD resolution array to: 7 (2048 x 2048), 6 (1024x1024), 5 (512x512), 3 (128x128), 1 (64x64) 
                PlanetManager.TextureLODGas = new int[5] { 7,6,5,3,1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODPlanetSizes"/>
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODComposition"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODLava"/>        
        public static int[] TextureLODGas
        {
            get { return Instance._textureLODGas; }
            set
            {
                int[] _newArray = Instance.SetTextureResolutionArray(value, TextureLODSteps, "TextureLODSteps", ref Instance._textureLODGas);
                if (_newArray != null) Instance._textureLODGas = _newArray;
            }
        }
        [SerializeField] private int[] _textureLODGas = new int[5] { 7, 6, 5, 4, 3 };

        /// <summary>
        /// Array of Level of Detail (LOD) planet size levels. Valid values in array = 0.0f and up
        /// The first entry in array is highest level of detail, last entry in array is lowest level of detail.
        /// 
        /// The sizes are specified from 0.0f (0%) - 1.0f (100%) but can also be higher than 1.0 (e.g. 1.5 = 150%)
        /// The size of the planet is calculated by comparing the planet diameter as seen by a specified camera (main camera by default) and comparing it to the height of screen.
        /// 
        /// Default array is { 0.65f, 0.35f, 0.15f, 0.05f }
        /// 
        /// Important: Size of array must be TextureLODSteps - 1 
        /// 
        /// The reason it's - 1 is because the sizes specifies between each step and if there are 5 steps there are only 4 transitions between the 5 steps, e.g.:
        ///   (Step0) Size0 (Step1) Size1 (Step2) Size2 (Step3) Size3 (Step4)
        /// </summary>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;

         // This example requires a PlanetManager instance to be present in the scene. 
         // Add the "Procedural Planets/Prefabs/PlanetManager" prefab to the scene first.         
         // Then create another gameobject and attach this example script to it.

         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a random planet at the center of the scene
                PlanetManager.CreatePlanet(Vector3.zero);

                // Set the Texture Detail Mode to Level of Detail (LOD)
                PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.LOD;

                // Set number of texture LOD levels to 5
                PlanetManager.TextureLODSteps = 5;

                // Set the texture LOD detail levels by using an array of texture resolution levels
                // The first value in the array is the highest level of detail and the last value in the array is the lowest quality
                PlanetManager.TextureLODCommon = new int[5] { 7, 6, 5, 3, 1 };

                // Set the planet sizes used to transition between mesh LOD levels
                // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
                // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
                // E.g. the value 0.5f = when the planet takes up half the screen height.                
                PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };                
             }
         }                  
        \endcode
        */
        /// <seealso cref="TextureDetailMode"/>    
        /// <seealso cref="TextureLODSteps"/>
        /// <seealso cref="TextureLODCommon"/>
        /// <seealso cref="TextureLODComposition"/>
        /// <seealso cref="TextureLODBiome"/>
        /// <seealso cref="TextureLODClouds"/>
        /// <seealso cref="TextureLODCities"/>
        /// <seealso cref="TextureLODLava"/>
        /// <seealso cref="TextureLODPolarIce"/>
        /// <seealso cref="TextureLODGas"/>
        public static float[] TextureLODPlanetSizes
        {
            get { return Instance._textureLODPlanetSizes; }
            set
            {
                if (value.Length != TextureLODSteps - 1)
                {
                    Debug.LogError("Array must be the same length as TextureLODSteps - 1 so this array must contain " + (TextureLODSteps - 1) + " entries - Aborting.");
                    return;
                }
                Instance._textureLODPlanetSizes = value;
            }
        }
        [SerializeField] float[] _textureLODPlanetSizes = new float[4] { 0.65f, 0.35f, 0.15f, 0.05f };

        /// <summary>
        /// Sets the debug level. This affects the output to console event log during runtime and for scripts executing in edit mode.
        /// </summary>
        public static DebugLevels DebugLevel { get { return Instance._debugLevel; } set { Instance._debugLevel = value; } }
        [SerializeField] private DebugLevels _debugLevel;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:ProceduralPlanets.PlanetManager"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if is initialized; otherwise, <c>false</c>.</value>
        public static bool IsInitialized { get { return Instance._isInitialized; }}
        bool _isInitialized;


        // Public Attributes

        // List of planet blueprints
        public List<BlueprintSolidPlanet> listSolidPlanetBlueprints = new List<BlueprintSolidPlanet>(0);
        public List<BlueprintGasPlanet> listGasPlanetBlueprints = new List<BlueprintGasPlanet>(0);

        // Solid planet procedural materials
        public List<SubstanceGraph> solidCompositionMaterials = new List<SubstanceGraph>(0);
        public List<SubstanceGraph> solidBiomeMaterials = new List<SubstanceGraph>(0);
        public List<SubstanceGraph> solidCloudsMaterials = new List<SubstanceGraph>(0);
        public List<SubstanceGraph> solidCitiesMaterials = new List<SubstanceGraph>(0);
        public List<SubstanceGraph> solidLavaMaterials = new List<SubstanceGraph>(0);
        public List<SubstanceGraph> solidPolarIceMaterials = new List<SubstanceGraph>(0);

        // Gas planet procedural materials
        public List<SubstanceGraph> gasMaterials = new List<SubstanceGraph>(0);

        // Ring procedural material
        public List<SubstanceGraph> ringMaterials = new List<SubstanceGraph>(0);

        // A list of duplicate materials is needed for multiple instances of planets
        public List<SubstanceGraph> substanceDuplicates = new List<SubstanceGraph>(0);

        // Substance allocation dictionary - using a list for serialization purposes since dictionary serialization is not supported by Unity
        [SerializeField] private List<SubstanceAllicationEntry> substanceAllocation = new List<SubstanceAllicationEntry>(0);
        private struct SubstanceAllicationEntry
        {
            public SubstanceGraph proceduralMaterial;
            public GameObject gameObject;
            public string materialName;

            public SubstanceAllicationEntry(SubstanceGraph _key, GameObject _gameObject, string _materialName)
            {
                proceduralMaterial = _key;
                gameObject = _gameObject;
                materialName = _materialName;
            }
        }

        public bool showEditorMeshLOD = false;
        public bool showEditorTextureLOD = false;

        [SerializeField]
        private BlueprintRing _blueprintRingGeneric;

        // Dictionary of planet bluprints and their calculate probability to be used for newly created planets
        private Dictionary<BlueprintPlanet, float> _planetBlueprintDictionary = new Dictionary<BlueprintPlanet, float>();

        /// <summary>
        /// Refreshes Blueprint Lists and Dictionary upon every start. 
        /// Also creates the procedural meshes that the planets and atmospheres use.
        /// Initializing takes place in Start() since parent Singleton class already has the Awake() method.
        /// Planets can force initialization of manager if their Awake() method runs prior to PlanetManager's
        /// since they need to access the procedurally generated meshes.
        /// </summary>
        void Start()
        {            
            if (!IsInitialized)
                Initialize();
        }

        /// <summary>
        /// Force initialization of PlanetManager if it is not already initialized.
        /// </summary>
        public static void Initialize()
        {
            if ((int)DebugLevel > 0) Debug.Log("PlanetManager.cs: Initialize()");

            if (Instance._isInitialized && MeshLODMeshes.Length == MeshLODSteps)
            {
                bool _flag = false;
                foreach (Mesh _m in MeshLODMeshes)
                    if (_m == null)
                        _flag = true;

                if (!_flag)
                    return;
            }
                
            
            if ((int)DebugLevel > 1)
            {
                Debug.Log("PlanetManager.cs: DEBUG_LEVEL: " + DebugLevel);
                Debug.Log("- deviceModel: " + SystemInfo.deviceModel);
                Debug.Log("- deviceType: " + SystemInfo.deviceModel);
                Debug.Log("- graphicsDeviceType: " + SystemInfo.graphicsDeviceType);
                Debug.Log("- graphicsDeviceVendor: " + SystemInfo.graphicsDeviceVendor);
                Debug.Log("- graphicsDeviceVersion: " + SystemInfo.graphicsDeviceVersion);
                Debug.Log("- graphicsMemorySize: " + SystemInfo.graphicsMemorySize);
                Debug.Log("- graphicsMultiThreaded: " + SystemInfo.graphicsMultiThreaded);
                Debug.Log("- graphicsShaderLevel: " + SystemInfo.graphicsShaderLevel);
                Debug.Log("- maxTextureSize: " + SystemInfo.maxTextureSize);
                Debug.Log("- graphicsShaderLevel: " + SystemInfo.graphicsShaderLevel);
                Debug.Log("- operatingSystem: " + SystemInfo.operatingSystem);
                Debug.Log("- processorType: " + SystemInfo.processorType);
                Debug.Log("- processorFrequency: " + SystemInfo.processorFrequency);
                Debug.Log("- systemMemorySize: " + SystemInfo.systemMemorySize);
                Debug.Log("- Manager.cs: Start()");
            }

            // Recreate generic blueprint
            Instance.CreateGenericBlueprint();
            
            Instance.RefreshLists();
            Instance.RefreshBlueprintDictionary();
            Instance.RecreateProceduralMeshes();
            Instance.substanceAllocation.Clear();

            Instance._isInitialized = true;
        }

        /// <summary>
        /// Reset is called when this component is initially added to a gameobject or when reset is hit in the inspector.
        /// </summary>
        void Reset()
        {
            if ((int) DebugLevel > 0) Debug.Log("PlanetManager.cs: Reset()");

            // Refresh lists containing planet blueprints
            RefreshLists();
            // Refresh the dictionary containing planet blueprints and existence probabilities
            RefreshBlueprintDictionary();
        }

        /// <summary>
        /// Creates a planet using a JSON-string. 
        /// 
        /// The JSON string contains the blueprint that the planet uses and also the values of the seed and all parameters. The reason why all parameters are included even
        /// if no parameters are overridden at the time of creation is because any changes to the PlanetManager configuration or materials (even the order) results in a different random
        /// result so parameters that were note originally overridden may require to be overridden at the time of import.
        /// 
        /// You can generate a JSON string for a planet by highlighting a planet in the Hierarchy and then in the Inspector you click "Export To Clipboard (Escaped JSON)" or "Export To Clipboard (Base64 JSON)". 
        /// Both options will create identical planets but the Base64 string is shorter but is not human readable compared to escaped string. 
        /// "Escaped" string means that charachters like quotes (") are preceded by a backslash, like this \" so they can reside within a string.
        /// </summary>
        /// <param name="_position">Vector3 position of the planet to be created.</param>
        /// <param name="_jsonString">JSON string containing the configuration of the planet (can be escaped text or Base64 encoded).</param>
        /// <returns>Planet component (a derived class, like SolidPlanet.cs)</returns>
        /*! \code{.cs}
         using UnityEngine;
         using System.Collections;
         using ProceduralPlanets;
         
         public class Example : MonoBehaviour
         {
             void Start() 
             {
                // Create a planet using a Base64 encoded configuration string. 
                // JSON strings can be generated in the Inspector for a planet by using the Export buttons.

                PlanetManager.CreatePlanet(new Vector3(0,1,2), 
                    "eyJjYXRlZ29yeSI6InBsYW5ldCIsInR5cGUiOiJTb2xpZFBsYW5ldCIsInZlcnNpb24iOiIwLjEuMiIsInBsYW5ldCI6eyJwbGFuZXRTZWVkIjo3NTQzODA2OTcsInZhcmlhdGlvblNlZWQiOjAs" +
                    "ImJsdWVwcmludEluZGV4IjowLCJibHVlcHJpbnROYW1lIjoiVGVycmVzdHJpYWwiLCJwcm9wZXJ0eUZsb2F0cyI6eyJhbGllbml6YXRpb24iOjAsImNvbnRpbmVudFNlZWQiOjk0LCJjb250aW5l" +
                    "bnRTaXplIjowLjc4NTMyODEsImNvbnRpbmVudENvbXBsZXhpdHkiOjAuMzExNjc2MiwiY29hc3RhbERldGFpbCI6MC4yNDYwMzg3LCJjb2FzdGFsUmVhY2giOjAuNDkyNTEyNCwibGlxdWlkTGV2" + 
                    "ZWwiOjAuNTYyLCJsaXF1aWRPcGFjaXR5IjoxLCJsaXF1aWRTaGFsbG93IjowLCJsaXF1aWRTcGVjdWxhclBvd2VyIjowLjUxOTMzNjYsInBvbGFyQ2FwQW1vdW50IjowLjQ2NDE5NjIsImF0bW9z" +
                    "cGhlcmVFeHRlcm5hbFNpemUiOjAuNTU2MjQxMywiYXRtb3NwaGVyZUV4dGVybmFsRGVuc2l0eSI6MC43MTIzMTMzLCJhdG1vc3BoZXJlSW50ZXJuYWxEZW5zaXR5IjowLjk3NTE4NzMsImNsb3Vk" +
                    "c09wYWNpdHkiOjEsImNsb3Vkc1NlZWQiOjExNSwiY2xvdWRzQ292ZXJhZ2UiOjAuNDc5LCJjbG91ZHNMYXllcjEiOjAuNDU1LCJjbG91ZHNMYXllcjIiOjAuODEyLCJjbG91ZHNMYXllcjMiOjAu" +
                    "ODc1LCJjbG91ZHNTaGFycG5lc3MiOjAuNDU2MjE2NSwiY2xvdWRzUm91Z2huZXNzIjowLjM1MDg3MjksImNsb3Vkc1RpbGluZyI6MiwiY2xvdWRzU3BlZWQiOjAuMDY4MjgxNDYsImNsb3Vkc0hl" + 
                    "aWdodCI6MC4zOTg2OTg2LCJjbG91ZHNTaGFkb3ciOjAuMzA2NDQxMywibGF2YUFtb3VudCI6MCwibGF2YUNvbXBsZXhpdHkiOjAsImxhdmFGcmVxdWVuY3kiOjAuMDM3NjY1MzcsImxhdmFEZXRh" + 
                    "aWwiOjAuNDIyNjc4OCwibGF2YVJlYWNoIjowLjI3NzcxNDMsImxhdmFDb2xvclZhcmlhdGlvbiI6MC43MjExNzI4LCJsYXZhRmxvd1NwZWVkIjowLjU1MzIyNjUsImxhdmFHbG93QW1vdW50Ijow" + 
                    "LjkyMTM1NjEsInN1cmZhY2VUaWxpbmciOjgsInN1cmZhY2VSb3VnaG5lc3MiOjAuMTI4MjE2MywiY29tcG9zaXRpb25TZWVkIjo5NCwiY29tcG9zaXRpb25UaWxpbmciOjIsImNvbXBvc2l0aW9u" +
                    "Q2hhb3MiOjAuNjYxNDE0NSwiY29tcG9zaXRpb25CYWxhbmNlIjowLjE3NSwiY29tcG9zaXRpb25Db250cmFzdCI6MC44NTY3MjY0LCJiaW9tZTFTZWVkIjozMywiYmlvbWUxQ2hhb3MiOjAuMDE3" + 
                    "MzIyNTQsImJpb21lMUJhbGFuY2UiOjAuNDQzNjcxOCwiYmlvbWUxQ29udHJhc3QiOjAuNjg3NTcxLCJiaW9tZTFDb2xvclZhcmlhdGlvbiI6MC41MTE1NzQ2LCJiaW9tZTFTYXR1cmF0aW9uIjow" +
                    "LjUzNzA4NzUsImJpb21lMUJyaWdodG5lc3MiOjAuNTE4NzE5MywiYmlvbWUxU3VyZmFjZUJ1bXAiOjAuMTQ1MzM5MywiYmlvbWUxQ3JhdGVyc1NtYWxsIjowLjQ4NTg1MjIsImJpb21lMUNyYXRl" +
                    "cnNNZWRpdW0iOjAuMzQyNzY0NiwiYmlvbWUxQ3JhdGVyc0xhcmdlIjowLjY5MzI3OTcsImJpb21lMUNyYXRlcnNFcm9zaW9uIjowLjU1MDc2OTksImJpb21lMUNyYXRlcnNEaWZmdXNlIjowLjg4" + 
                    "ODQ4NjcsImJpb21lMUNyYXRlcnNCdW1wIjowLjIyNTY2MjYsImJpb21lMUNhbnlvbnNEaWZmdXNlIjowLjExNjA1NTYsImJpb21lMUNhbnlvbnNCdW1wIjowLjQ2NDUxNzUsImJpb21lMlNlZWQi" + 
                    "OjcxLCJiaW9tZTJDaGFvcyI6MC42MTcwNTEzLCJiaW9tZTJCYWxhbmNlIjowLjU3NDM3NjksImJpb21lMkNvbnRyYXN0IjowLjg3NzY1NCwiYmlvbWUyQ29sb3JWYXJpYXRpb24iOjAuNDU2NzE3" +
                    "OSwiYmlvbWUyU2F0dXJhdGlvbiI6MC40NjcxMjE4LCJiaW9tZTJCcmlnaHRuZXNzIjowLjQ4MDQ4NzMsImJpb21lMlN1cmZhY2VCdW1wIjowLjQ5NDY2MzIsImJpb21lMkNyYXRlcnNTbWFsbCI6" +
                    "MC42NDQwNTk1LCJiaW9tZTJDcmF0ZXJzTWVkaXVtIjowLjc5NDUxMzMsImJpb21lMkNyYXRlcnNMYXJnZSI6MC45NDUyMjk1LCJiaW9tZTJDcmF0ZXJzRXJvc2lvbiI6MC4wODM2NTkyOSwiYmlv" +
                    "bWUyQ3JhdGVyc0RpZmZ1c2UiOjAuMjQ1NTc3MiwiYmlvbWUyQ3JhdGVyc0J1bXAiOjAuMzc2NzM2NSwiYmlvbWUyQ2FueW9uc0RpZmZ1c2UiOjAuNTIwNDg1LCJiaW9tZTJDYW55b25zQnVtcCI6" +
                    "MC42MzQxOTc2LCJjaXRpZXNTZWVkIjo2MCwiY2l0aWVzUG9wdWxhdGlvbiI6MC45NDc0NDI4LCJjaXRpZXNBZHZhbmNlbWVudCI6MC4wNTYzMTk2MSwiY2l0aWVzR2xvdyI6MCwiY2l0aWVzVGls" +
                    "aW5nIjozfSwicHJvcGVydHlNYXRlcmlhbHMiOnsiY29tcG9zaXRpb24iOnsiaW5kZXgiOjYsIm5hbWUiOiJTb2xpZF9UZXJyZXN0cmlhbCJ9LCJwb2xhckljZSI6eyJpbmRleCI6MCwibmFtZSI6" +
                    "IlBvbGFyX0ljZSJ9LCJjbG91ZHMiOnsiaW5kZXgiOjMsIm5hbWUiOiJDbG91ZHNfMDQifSwibGF2YSI6eyJpbmRleCI6MCwibmFtZSI6IkxhdmFfMDEifSwiYmlvbWUxVHlwZSI6eyJpbmRleCI6" +
                    "NSwibmFtZSI6IkJpb21lX1JhaW5fRm9yZXN0In0sImJpb21lMlR5cGUiOnsiaW5kZXgiOjcsIm5hbWUiOiJCaW9tZV9UdW5kcmEifSwiY2l0aWVzIjp7ImluZGV4IjowLCJuYW1lIjoiQ2l0aWVz" +
                    "In19LCJwcm9wZXJ0eUNvbG9ycyI6eyJzcGVjdWxhckNvbG9yIjp7InIiOjAuMzk4NDk2LCJnIjowLjMyMjIzMDgsImIiOjAuMDQ3MDEwMjF9LCJsaXF1aWRDb2xvciI6eyJyIjowLjAwMzU5MzQ1" +
                    "OCwiZyI6MC4wMTU1NDM1MSwiYiI6MC4wMzQxMTN9LCJpY2VDb2xvciI6eyJyIjowLjk1NDkxNjUsImciOjAuOTI1NTA1LCJiIjowLjkyNTUwNX0sImF0bW9zcGhlcmVDb2xvciI6eyJyIjowLjE1" +
                    "NTMxNzUsImciOjAuNzQ0Njc4MywiYiI6MX0sInR3aWxpZ2h0Q29sb3IiOnsiciI6MC4zODU0NjQsImciOjAuMzE5NjA4NCwiYiI6MC4wODcwNTUzOX0sImNsb3Vkc0NvbG9yIjp7InIiOjEsImci" +
                    "OjEsImIiOjF9LCJsYXZhR2xvd0NvbG9yIjp7InIiOjAuOTY3ODU2OCwiZyI6MC4wNDA3OTc4OSwiYiI6MC4wMTY5NDQxMX0sImNpdGllc0NvbG9yIjp7InIiOjAuOTMzMDU1OCwiZyI6MC45MjQx" +
                    "NzkxLCJiIjowLjYzNTQzM319fX0=");
             }
         }                  
        \endcode
        */
        /// <seealso cref="CreatePlanet(Vector3, int, string, string)"/>
        public static Planet CreatePlanet(Vector3 _position, string _jsonString)
        {
            if ((int) DebugLevel > 0) Debug.Log("PlanetManager.cs: CreatePlanet(" + _position + "," + _jsonString + ")");

            // Validate and (if necessary decode) JSON string
            if (_jsonString.IsBase64()) _jsonString = _jsonString.FromBase64();
            _jsonString = _jsonString.Replace("&quot;", "\"");
            var N = JSON.Parse(_jsonString);
            if (N["category"] != "planet")
            {
                Debug.LogError("Failed to build planet. No planet category in JSON string.");
                return null;
            }

            // Create the planet using the JSON string
            return CreatePlanet(_position, -1, "", _jsonString);
        }

        /// <summary>
        /// Creates a planet.
        /// 
        /// Can be totally random (if only position parameter is set) and optionally based on a specific seed, blueprint and/or JSON-string.
        ///       
        /// </summary>
        /// <param name="_position">Vector3 position of the planet to be created.</param>
        /// <param name="_planetSeed">Optional random seed used for the planet. Provided that the PlanetManager Material and probability settings are not changed the same seed will generate an identical planet each time.</param>
        /// <param name="_blueprintName">Optional forced blueprint name for the planet, e.g. force the planet to be a Terrestrial planet.</param>
        /// <param name="_jsonString">Optional JSON string containing the configuration of the planet (can be escaped text or Base64 encoded).</param>
        /// <returns>Planet component (a derived class, like SolidPlanet.cs)</returns>
        /*! \code{.cs}
        using UnityEngine;
        using System.Collections;
        using ProceduralPlanets;
         
        public class Example : MonoBehaviour
        {
            void Start()
            {

            // Create a totally random planet
            PlanetManager.CreatePlanet(new Vector3(-5, 0,0));

            // Create a planet with random seed 12345
            PlanetManager.CreatePlanet(new Vector3(0, 0, 0), 12345);

            // Create a planet with a random seed but force blueprint Terrestrial
            PlanetManager.CreatePlanet(new Vector3(5, 0, 0), -1, "Terrestrial");            
            }
        }                  
        \endcode
        */
        /// <seealso cref="CreatePlanet(Vector3, string)"/>
        public static Planet CreatePlanet(Vector3 _position, int _planetSeed = -1, string _blueprintName = "", string _jsonString = null)
        {
            if ((int) DebugLevel > 0) Debug.Log("PlanetManager.cs:CreatePlanet(" + _position + "," + _planetSeed + ", " + _blueprintName + "," + _jsonString + ")");

            // Refresh the blueprint dictionary 
            Instance.RefreshBlueprintDictionary();

            // If no seed is specified, use random seed
            if (_planetSeed < 0) _planetSeed = Random.Range(0, int.MaxValue - 1000000);

            // Get a random planet blueprint based on probability
            float _r = Random.Range(0.0f, 1.0f);

            // Select a blueprint 
            float _previousValue = 0f;
            Object _previousKey = null;
            BlueprintPlanet _newPlanetBlueprint = null;
            foreach (KeyValuePair<BlueprintPlanet, float> _k in Instance._planetBlueprintDictionary)
            {
                // Select blueprint randomly based on blueprint probability (since no blueprint name was specified)
                if (_blueprintName == "")
                {
                    // Find blueprint by comparing probability to random value
                    if (_r > _previousValue && _r < _k.Value)
                        _newPlanetBlueprint = _k.Key;
                    _previousValue = _k.Value;
                    _previousKey = _k.Key;
                }
                else
                {
                    // Set a specific blueprint
                    if (_blueprintName == _k.Key.name)
                        _newPlanetBlueprint = _k.Key;
                }
            }

            // If blueprint was not found, log error and return null
            if (_newPlanetBlueprint == null)
            {
                Instance.RefreshLists();
                Instance.RefreshBlueprintDictionary();                
                Debug.LogError("Could not find the specified blueprint. Refreshing Manager and aborting. Please try again.");
                return null;
            }
            
            // Create new planet gameobject
            GameObject _planetGameObject = new GameObject();

            // Deactivate the gameobject to prevent Awake() initialization to be called prior to planet having an assigned blueprint
            _planetGameObject.SetActive(false);

            // Set name and position of planet
            _planetGameObject.name = "New Procedural Planet";
            _planetGameObject.transform.position = _position;

            // Find the planet class (same blueprint type but without the 'Blueprint' prefix).
            System.Type _planetClass = System.Type.GetType("ProceduralPlanets." + _newPlanetBlueprint.GetType().Name.Replace("Blueprint",""));

            if (_planetClass == null)
            {
                // Planet creation was not successful, destroy gameobject
                Destroy(_planetGameObject);

                // Log error and return
                Debug.LogError("There is no planet class as specified by blueprint type (" + "ProceduralPlanets." + _newPlanetBlueprint.GetType().Name.Replace("Blueprint", "") + ").");
                return null;
            }

            // Ensure that the class is in fact a subclass of Planet and nothing else
            if (_planetClass.IsSubclassOf(typeof(Planet)))
            {
                // Add the appropriate component class for this planet
                Planet _p = (Planet)_planetGameObject.AddComponent(_planetClass);

                // Set the planet blueprint
                _p.SetPlanetBlueprint(Instance.GetPlanetBlueprintIndex(_newPlanetBlueprint), false, false);

                // Determine if this is a seed/blueprint instatiated planet or a json string importet planet
                if (_jsonString == null)
                {
                    _p.planetSeed = _planetSeed;

                    // Verify if blueprint has planetary ring blueprint
                    BlueprintRing _blueprintRing = null;
                    if ((_newPlanetBlueprint).transform.Find("Ring") != null)
                        _blueprintRing = (_newPlanetBlueprint).transform.Find("Ring").GetComponent<BlueprintRing>();

                    // Verify the probability of (and create if within probability) a ring existing for this particular planet
                    if (_blueprintRing != null)
                        if (_r < (_newPlanetBlueprint).ringProbability)
                            _planetGameObject.GetComponent<Planet>().CreateRing();
                    _p.initJSONSettings = "";
                }
                else
                {
                    // Set init JSON string - settings within string will be used to set up planet (and optional ring if present)
                    _p.initJSONSettings = _jsonString;

                    var N = JSON.Parse(_jsonString);
                    if (N != null)
                        if (N["ring"] != null)
                            _p.CreateRing(_jsonString);
                }                
                // Set the planet gameobject to active to execute Awake() method which will set configure planet according to seed/blueprint/json-string
                _planetGameObject.SetActive(true);
            }
            else
            {
                // Planet creation was not successful, destroy gameobject
                Destroy(_planetGameObject);

                // Log error and return
                Debug.LogError("The planet class is not derived from Planet class.");
                return null;
            }           

            // Return a reference to the new planet        
            return _planetGameObject.GetComponent<Planet>();        
        }

        /// <summary>
        /// Refreshes planet blueprint lists bases on the children of the Manager. It ensures there are no blueprint duplicate names.
        /// </summary>
        public void RefreshLists()
        {
            if ((int) DebugLevel > 2) Debug.Log("PlanetManager.cs:RefreshLists()");

            // Create a list to store all blueprint names to ensure there are no duplicate names
            List<string> _blueprintNames = new List<string>();

            // Clear the solid planet blueprint list
            listSolidPlanetBlueprints.Clear();

            // Find all solid planet blueprints that are children of the manager (there should be no other blueprints components in any scene other than as direct children of the manager.
            BlueprintSolidPlanet[] _ps = gameObject.GetComponentsInChildren<BlueprintSolidPlanet>();

            // Iterate through each solid planet blueprint
            foreach (BlueprintSolidPlanet _p in _ps)
            {
                // Add the solid blueprint to the list of solid planet blueprints
                listSolidPlanetBlueprints.Add(_p);
                // Add the name of the blueprint used to check for duplicates
                _blueprintNames.Add(_p.gameObject.name);
            }

            // Clear the gas planet blueprint list
            listGasPlanetBlueprints.Clear();

            // Find all gas planet blueprints that are children of the manager (there should be no other blueprints components in any scene other than as direct children of the manager.
            BlueprintGasPlanet[] _pg = gameObject.GetComponentsInChildren<BlueprintGasPlanet>();

            // Iterate through each gas planet blueprint
            foreach (BlueprintGasPlanet _p in _pg)
            {
                // Add the gas blueprint to the list of gas planet blueprints
                listGasPlanetBlueprints.Add(_p);
                // Add the name of the blueprint used to check for duplicates
                _blueprintNames.Add(_p.gameObject.name);
            }                

            // Check for duplicate blueprint names
            if (_blueprintNames.HasDuplicates())
            {
                // Throw error to the debug log if duplicate blueprint names are found.
                Debug.LogError("Blueprints cannot have the same name.");
            }
        }

        /// <summary>
        /// Refreshes the blueprint dictionary which is used to store probability of each blueprint in relation of one another (so some planet types can be more common than others)
        /// </summary>
        public void RefreshBlueprintDictionary()
        {
            if ((int) DebugLevel > 2) Debug.Log("PlanetManager.cs:RefreshBlueprintDictionary()");

            // Clear the blueprint dictionary
            _planetBlueprintDictionary.Clear();

            // Set the total probability to zero
            float _total = 0f;

            // Calculate the total probability value by iterating through all solid and gas planet blueprints
            foreach (BlueprintSolidPlanet _p in listSolidPlanetBlueprints)
                _total += _p.probability;
            foreach (BlueprintGasPlanet _p in listGasPlanetBlueprints)
                _total += _p.probability;

            // Set the temporary counter value to zero
            float _value = 0f;

            // Iterate through each solid planet blueprint
            foreach (BlueprintSolidPlanet _p in listSolidPlanetBlueprints)
            {
                // Add the blueprint as a key and the probability divided by the total amount for a unique "slot" in the lookup dictionary
                _planetBlueprintDictionary.Add(_p, _value + (_p.probability / _total));
                // Add the counter to move the "slot" forward in the lookup dictionary
                _value += _p.probability / _total;
            }

            // Iterate through each gas planet blueprint
            foreach (BlueprintGasPlanet _p in listGasPlanetBlueprints)
            {
                // Add the blueprint as a key and the probability divided by the total amount for a unique "slot" in the lookup dictionary
                _planetBlueprintDictionary.Add(_p, _value + (_p.probability / _total));
                // Add the counter to move the "slot" forward in the lookup dictionary
                _value += _p.probability / _total;
            }

        }

        /// <summary>
        /// Gets the ring blueprint by parent planet blueprint name
        /// </summary>
        /// <param name="_planetBlueprintName"></param>
        /// <returns>BlueprintRing component of a planet blueprint.</returns>
        public BlueprintRing GetRingBlueprintByPlanetBlueprintName(string _planetBlueprintName)
        {
            if ((int) DebugLevel > 0) Debug.Log("PlanetManager.cs: GetRingBlueprintByPlanetBlueprintName(" + _planetBlueprintName + ")");

            // Refresh lists and dictionary
            Instance.RefreshLists();
            Instance.RefreshBlueprintDictionary();

            // Find the planet blueprint by name (it should be a child of this manager game object)
            Transform _planetBlueprint = Instance.transform.Find(_planetBlueprintName);

            // If no planet blueprint is found, log an error and return null
            if (_planetBlueprint == null)
            {
                Debug.LogError("No planet blueprint found by the name of '" + _planetBlueprintName + "' - returning null.");
                return null;
            }

            // Find the ring blueprint child transform of the planet transform, it should always be named "Ring"
            Transform _ringBlueprint = _planetBlueprint.Find("Ring");

            // If no ring blueprint is found, log an error and return null
            if (_ringBlueprint == null)
            {
                Debug.LogWarning("No ring blueprint found under the planet blueprint by the name of '" + _planetBlueprintName+ "' - returning null.");
                return null;
            }

            // Return the BlueprintRing component of the ring blueprint gameobject            
            return _ringBlueprint.GetComponent<BlueprintRing>();
        }
       
        /// <summary>
        /// Gets the index number of a planet blueprint based on the blueprint name (could be either solid or gas planet)
        /// </summary>
        /// <param name="_name"></param>
        /// <returns>Integer number of a planet blueprint (in the list of the planet type)</returns>
        public int GetPlanetBlueprintIndexByName(string _name)
        {
            if ((int) DebugLevel > 1) Debug.Log("PlanetManager.cs: GetPlanetBlueprintIndexByName(" + _name + ")");

            // Refresh lists and dictionary
            Instance.RefreshLists();
            Instance.RefreshBlueprintDictionary();

            // Iterate through all the solid planet blueprints
            for (int _i = 0; _i < Instance.listSolidPlanetBlueprints.Count; _i++)
                // If the name exists in the solid planet blueprint list...
                if (Instance.listSolidPlanetBlueprints[_i].name == _name)
                    // Return the index number
                    return _i;

            // Iterate through all the gas planet blueprints
            for (int _i = 0; _i < Instance.listGasPlanetBlueprints.Count; _i++)
                // If the name exists in the gas planet blueprint list...
                if (Instance.listGasPlanetBlueprints[_i].name == _name)
                    // Return the index number
                    return _i;

            // No blueprint was found by that name - log error and return -1
            Debug.LogError("No blueprint name found by the name of '" + _name + "'");
            return -1;
        }

        /// <summary>
        /// Gets the planet blueprint name by index in a blueprint list
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_caller"></param>
        /// <returns>Name of the planet blueprint</returns>
        public string GetPlanetBlueprintNameByIndex(int _index, Object _caller)
        {
            if ((int) DebugLevel > 1) Debug.Log("PlanetManager.cs: GetPlanetBlueprintNameByIndex(" + _index + "," + _caller + ")");

            // Refresh lists and dictionary
            Instance.RefreshLists();
            Instance.RefreshBlueprintDictionary();

            // If the caller type is a solid planet, look in the solid planet blueprint list
            if (_caller.GetType() == typeof(BlueprintSolidPlanet) || _caller.GetType() == typeof(SolidPlanet)) 
            {
                // If index is larger than the list count - return ""
                if (_index >= Instance.listSolidPlanetBlueprints.Count) return "";
                // Return name in the list at the index position
                return Instance.listSolidPlanetBlueprints[_index].name;
            }

            // If the caller type is a gas planet, look in the gas planet blueprint list
            if (_caller.GetType() == typeof(BlueprintGasPlanet) || _caller.GetType() == typeof(GasPlanet))
            {
                // If index is larger than the list count - return ""
                if (_index >= Instance.listSolidPlanetBlueprints.Count) return "";
                // Return name in the list at the index position
                return Instance.listGasPlanetBlueprints[_index].name;
            }

            // No blueprint was found - return ""
            return "";
        }

        /// <summary>
        /// Gets the planet blueprint by index in a blueprint list.
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_caller"></param>
        /// <returns>Planet blueprint</returns>
        public BlueprintPlanet GetPlanetBlueprintByIndex(int _index, Object _caller)
        {
            if ((int) DebugLevel > 1) Debug.Log("PlanetManager.cs: GetPlanetBlueprintByIndex(" + _index + "," + _caller + ")");

            // Refresh lists and dictionary
            Instance.RefreshLists();
            Instance.RefreshBlueprintDictionary();

            // If the caller type is a solid planet, look in the solid planet blueprint list
            if (_caller.GetType() == typeof(BlueprintSolidPlanet) || _caller.GetType() == typeof(SolidPlanet))
            {
                // If index is larger than the list count - return null
                if (_index >= Instance.listSolidPlanetBlueprints.Count) return null;
                // Return name in the list at the index position
                return Instance.listSolidPlanetBlueprints[_index];
            }

            // If the caller type is a gas planet, look in the gas planet blueprint list
            if (_caller.GetType() == typeof(BlueprintGasPlanet) || _caller.GetType() == typeof(GasPlanet))
            {
                // If index is larger than the list count - return null
                if (_index >= Instance.listSolidPlanetBlueprints.Count) return null;
                // Return name in the list at the index position
                return Instance.listGasPlanetBlueprints[_index];
            }

            // No blueprint was found - return null
            return null;
        }

        /// <summary>
        /// Get planet blueprint index based on solid planet blueprint object
        /// </summary>
        /// <param name="_object"></param>
        /// <returns>Integer index of a planet blueprint in the manager list of blueprints</returns>
        private int GetPlanetBlueprintIndex(Object _object)
        {
            if ((int) DebugLevel > 1) Debug.Log("PlanetManager.cs: GetPlanetBlueprintIndex(" + _object + ")");

            // Refresh lists and dictionary
            Instance.RefreshLists();
            Instance.RefreshBlueprintDictionary();

            // If the caller type is a solid planet, look in the solid planet blueprint list
            if (_object.GetType() == typeof(BlueprintSolidPlanet) || _object.GetType() == typeof(SolidPlanet))
                // Iterate through all the solid planet blueprints
                for (int _i = 0; _i < Instance.listSolidPlanetBlueprints.Count; _i++)
                    // If the object is found in the list...
                    if (((BlueprintSolidPlanet)_object) == Instance.listSolidPlanetBlueprints[_i])
                        // Return the index number of the blueprint
                        return _i;

            // If the caller type is a gas planet, look in the solid planet blueprint list
            if (_object.GetType() == typeof(BlueprintGasPlanet) || _object.GetType() == typeof(GasPlanet))
                // Iterate through all the solid planet blueprints
                for (int _i = 0; _i < Instance.listGasPlanetBlueprints.Count; _i++)
                    // If the object is found in the list...
                    if (((BlueprintGasPlanet)_object) == Instance.listGasPlanetBlueprints[_i])
                        // Return the index number of the blueprint
                        return _i;

            // No blueprint was found - return -1
            return -1;
        }

        /// <summary>
        /// Exports all planet blueprints (and any child ring blueprints) to a JSON string. 
        /// 
        /// You can use the exported string as a method to backup all your blueprints. 
        /// Call ExportAllBlueprints() and save the resulting string to a text file which can be imported at a later stage if necessary.
        /// 
        /// Note: There is an Import from Clipboard button in the PlanetManager Inspector which may be easier than scripting import/export.
        /// </summary>
        /// <param name="_stringFormat">String format - defaults to escaped JSON but can also be compact JSON, easy-read JSON or Base64 JSON. </param>
        /// <returns>JSON string of all blueprints stored in the PlanetManager component.</returns>
        /// <seealso cref="SimpleJSON.StringFormat"/>
        /// <seealso cref="ImportBlueprints"/>

        public static string ExportAllBlueprints(StringFormat _stringFormat = StringFormat.JSON_ESCAPED)
        {
            if ((int) DebugLevel > 0) Debug.Log("PlanetManager.cs: ExportAllBlueprints(" + _stringFormat + ")");

            // Initialize the JSON string
            string _str = "{";

            // Iterate through all solid planet blueprints
            for (int _i = 0; _i < Instance.listSolidPlanetBlueprints.Count; _i++)
            {
                // Add an item with counter (to avoid duplicate keys)
                _str += "\r\n  \"item" + _i + "\" : ";
                // Add the exported solid planet blueprint (and take care of formatting)
                _str += Instance.listSolidPlanetBlueprints[_i].ExportToJSON(true).Replace("\r\n  ", "\r\n    ").Replace("\r\n}", "\r\n  }");
                // If there are more solid planet blueprints (or any gas planet blueprints), add a comma
                if (_i < Instance.listSolidPlanetBlueprints.Count - 1 || Instance.listGasPlanetBlueprints.Count > 0) _str += ",";
            }

            int _offset = Instance.listSolidPlanetBlueprints.Count;

            // Iterate through all gas planet blueprints
            for (int _i = 0; _i < Instance.listGasPlanetBlueprints.Count; _i++)
            {
                // Add an item with counter (to avoid duplicate keys)
                _str += "\r\n  \"item" + (_i + _offset) + "\" : ";
                // Add the exported gas planet blueprint (and take care of formatting)
                _str += Instance.listGasPlanetBlueprints[_i].ExportToJSON(true).Replace("\r\n  ", "\r\n    ").Replace("\r\n}", "\r\n  }");
                // If there are more gas planet blueprints, add a comma
                if (_i < Instance.listGasPlanetBlueprints.Count - 1) _str += ",";
            }

            // Close the JSON string
            _str += "\r\n}";

            return _str;
        }

        /// <summary>
        /// Exports all planet blueprints (and any child ring blueprints) to clipboard as a JSON string.
        /// </summary>
        public void ExportAllBlueprintsToClipboard()
        {
            if ((int) DebugLevel > 0) Debug.Log("PlanetManager.cs: ExportAllBlueprintsToClipboard()");

            string _str = ExportAllBlueprints(StringFormat.JSON_ESCAPED);

            // Copy JSON string to clip board
            GUIUtility.systemCopyBuffer = _str;

            // Show an editor dialog to confirm export to clipboard.
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog("Finished", "All blueprints were saved to clipboard", "Close");
#endif
        }

        /// <summary>
        /// Imports planet blueprints (and related ring blueprints) from a JSON string.
        /// 
        /// Exporting and Importing blueprints may be most useful for backing up configuration. 
        /// 
        /// Note: There is an Import from Clipboard button in the PlanetManager Inspector which may be easier than scripting import/export.       
        /// </summary>
        /// <param name="_jsonString">JSON string - can be escaped text format or Base64 Encoded.</param>
        /// <seealso cref="SimpleJSON.StringFormat"/>
        /// <seealso cref="ExportAllBlueprints"/>
        public static void ImportBlueprints(string _jsonString)
        {
            if ((int) DebugLevel > 0) Debug.Log("PlanetManager.cs: ImportBlueprintsFromClipboard()");

            // Parse the JSON string from the clipboard
            var N = JSON.Parse(_jsonString);

            // Validate JSON to ensure it's a SoliePlanet string and that it contains required properties.
            if (N == null)
            {
                Debug.LogWarning("Corrupt - could not parse JSON. Aborting.");
                return;
            }

            // Iterate through all numbered items
            int _i = 0;
            while (N["item" + _i] != null)
            {
                // If item is a blueprint...
                if (N["item" + _i]["category"] == "blueprint")
                {
                    // If blueprint already exists...
                    if (Instance.transform.Find(N["item" + _i]["name"]) != null)
                        // Destroy the blueprint
                        DestroyImmediate(Instance.transform.Find(N["item" + _i]["name"]).gameObject);

                    // Create a new gameobject for the planet blueprint
                    GameObject _go = new GameObject();

                    // Set the name of the planet blueprint
                    _go.name = N["item" + _i]["name"];

                    // Parent the new blueprint to the manager
                    _go.transform.parent = Instance.transform;

                    // Set the blueprint class to type of planet blueprint
                    System.Type _blueprintClass = System.Type.GetType("ProceduralPlanets." + N["item" + _i]["type"]);

                    if (_blueprintClass == null)
                    {
                        Debug.LogError("The specified blueprint class does not exist (" + "ProceduralPlanets." + N["item" + _i]["type"] + "). Skipping.");
                    }
                    else
                    {
                        // If the blueprint is a subclass of BlueprintPlanet...
                        if (_blueprintClass.IsSubclassOf(typeof(BlueprintPlanet)))
                        {
                            // Add the component of the blueprint planet
                            BlueprintPlanet _c = (BlueprintPlanet)_go.AddComponent(_blueprintClass);

                            // Call the import method of the planet blueprint to import the specific planet blueprint
                            _c.ImportFromJSON(N["item" + _i].ToString());
                        }
                    }
                }
                // Increment the counter
                _i++;
            }
        }

        /// <summary>
        /// Imports blueprints from clipboard.
        /// </summary>
        public void ImportBlueprintsFromClipboard()
        {
            if ((int) DebugLevel > 0) Debug.Log("PlanetManager.cs: ImportBlueprintsFromClipboard()");

            ImportBlueprints(GUIUtility.systemCopyBuffer);
        }

        /// <summary>
        /// Gets ta new unique blueprint name
        /// </summary>
        /// <returns>String containing a unique blueprint name</returns>
        public string GetUniqueBlueprintName()
        {
            if ((int) DebugLevel > 0) Debug.Log("GetUniqueBlueprintName()");

            // Initialize temporary variables
            int _i = 0;
            string _name = "";

            // Loop until a new name is found (or until a ceiling is hit to avoid infinite loop)
            while (_name == "")
            {
                // If a child transform can't be found with a name - set the name variable
                if (transform.Find("New_Blueprint_" + _i) == null)
                    _name = "New_Blueprint_" + _i;

                if (_i > 999)
                {
                    Debug.LogError("Maximum number of unnamed blueprints reached - generating random name.");
                    _name = "New_Blueprint_" + Random.Range(1000, 100000).ToString();
                }
                _i++;
            }


            // Return the unique name string
            return _name;
        }

        public SubstanceGraph GetUniqueProceduralMaterial(SubstanceGraph _proceduralMaterial, GameObject _gameObject, string _mapName)
        {
            if ((int)PlanetManager.DebugLevel > 1)
                Debug.Log("PlanetManager.cs GetUniqueProceduralMaterial( " + _proceduralMaterial + ", " + _gameObject + ", " + _mapName + ")");

            // Clear out substance allocation for planets that don't exist and for this particular requested material
            Dictionary<SubstanceGraph, SubstanceAllicationEntry> _tempDictionary = new Dictionary<SubstanceGraph, SubstanceAllicationEntry>(0);

            List<SubstanceAllicationEntry> _removals = new List<SubstanceAllicationEntry>();
            foreach (SubstanceAllicationEntry _g in substanceAllocation)
            {
                if (_g.gameObject == null)
                {
                    // Remove substance allocations to gameobjects that do not exist.
                    _removals.Add(_g);
                }

                else
                {
                    if (_g.gameObject == _gameObject && _g.materialName == _mapName)
                    {
                        // Remove substance allocations where gameobject already has the material name as an entry
                        // We will be replacing this entry with the newly allocated procedural material.
                        _removals.Add(_g);
                    }
                    else
                    {
                        //Debug.Log(_proceduralMaterial.name + "    " + _gameObject.name + "    " + _mapName + "    " + _g.proceduralMaterial.name);
                        _tempDictionary.Add(_g.proceduralMaterial, _g);
                    }
                }

            }

            foreach (SubstanceAllicationEntry _g in _removals)
            {
                substanceAllocation.Remove(_g);
            }                

            //Try to use the first / main substance instance if not already in use
            if (!_tempDictionary.ContainsKey(_proceduralMaterial))
            {
                substanceAllocation.Add(new SubstanceAllicationEntry(_proceduralMaterial, _gameObject, _mapName));
                return _proceduralMaterial;
            }

            // Try to find an available substance
            foreach (SubstanceGraph _p in substanceDuplicates)
            {
                Regex _rgx = new Regex("(_\\d+)$");
                if (_rgx.Replace(_p.name, "") == _proceduralMaterial.name)
                {
                    if (!_tempDictionary.ContainsKey(_p))
                    {
                        substanceAllocation.Add(new SubstanceAllicationEntry(_p, _gameObject, _mapName));
                        return _p;
                    }
                }
            }
            
            // Peparing for the release of Substance 2.4.2 by Adobe/Allegorithmic, it's supposed to fix the bug that means duplicating substances can't be used
            //SubstanceGraph _substanceGraph = _proceduralMaterial.Duplicate();
            //_substanceGraph.QueueForRender();
            //_substanceGraph.RenderSync();
            //return _substanceGraph;

            // No substance is available - log error
            Debug.LogError("No free substances available. You have reached the maximum number of simultaneous planets. See manual how to add support for additional simultaneous planets.\n" +
                $"Additional info: The requested graph name was {_proceduralMaterial.name} and there were {substanceDuplicates.Count} entries in the substanceDuplicates list. The substanceAllication dictionary contains {substanceAllocation.Count} entries.");


            return null;
        }

        /// <summary>
        /// Creates procedural meshes (including configured Level of Detail (LOD) versions used by planets as shared meshes.
        /// </summary>
        public void RecreateProceduralMeshes()
        {
            if (MeshDetailMode == MeshDetailModes.Static)
            {
                _meshLODMeshes = new Mesh[1];
                _meshLODMeshes[0] = ProceduralOctahedron.Create(MeshStaticSubdivisions, CONST_MESH_RADIUS);
            }

            if (MeshDetailMode == MeshDetailModes.LOD)
            {                
                _meshLODMeshes = new Mesh[MeshLODSteps];
                for (int _i = 0; _i < MeshLODSteps; _i++)
                    _meshLODMeshes[_i] = ProceduralOctahedron.Create(_meshLODSubdivisions[_i], CONST_MESH_RADIUS);
            }

            Planet[] _planets = FindObjectsOfType<Planet>();
            foreach (Planet _p in _planets)
            {
                if (_p.meshLODLevel >= _meshLODMeshes.Length)
                {
                    _p.meshLODLevel = _meshLODMeshes.Length - 1;                
                }

                _p.SetSharedMesh(_meshLODMeshes[_p.meshLODLevel]);
            }
        }

        /// <summary>
        /// Rebuilds all planet textures.
        /// </summary>
        /// <param name="_force">True to force rebuild of all textures.</param>
        public void RebuildAllPlanetTextures(bool _force = false)
        {
            SolidPlanet[] _planets = FindObjectsOfType<SolidPlanet>();
            foreach (SolidPlanet _p in _planets)
            {
                _p.UpdateLODTextureIfNeeded(_force);
                _p.RebuildTextures(true);
            }
        }

        /// <summary>
        /// Gets the appropriate Mesh LOD level based on planet size on screen (planet percent = planet diameter to screen height ratio).
        /// </summary>
        /// <param name="_percent">Planet size in percent, planet diameter to screen height ratio</param>
        /// <returns>Planet LOD level (integer) - </returns>
        /// <seealso cref="_meshLODSubdivisions"/>
        /// <seealso cref="Planet.GetLODPercent()"/>
        public int GetAppropriateMeshLODLevel(float _percent)
        {
            if (MeshDetailMode == MeshDetailModes.Static)
                return 0;

            int _appropriateLOD = -1;
            for (int _i = 0; _i < MeshLODSteps -1; _i++)
            {
                if ((_percent) >= Instance._meshLODPlanetSizes[_i])
                {
                    _appropriateLOD = _i;
                    break;
                }
            }
            if (_appropriateLOD == -1)
                _appropriateLOD = MeshLODSteps - 1;

            return _appropriateLOD;
        }

        /// <summary>
        /// Gets the appropriate Texture LOD level based on planet size on screen (planet percent = planet diameter to screen height ratio).
        /// </summary>
        /// <param name="_percent"></param>
        /// <returns></returns>
        public int GetAppropriateTextureLODLevel(float _percent)
        {
            if (TextureDetailMode == TextureDetailModes.LOD || TextureDetailMode == TextureDetailModes.LOD_Separate)
            {
                int _appropriateLODLevel = -1;
                for (int _i = 0; _i < TextureLODSteps - 1; _i++)
                {
                    if ((_percent) >= TextureLODPlanetSizes[_i])
                    {
                        _appropriateLODLevel = _i;
                        break;
                    }
                }
                if (_appropriateLODLevel == -1)
                    _appropriateLODLevel = TextureLODSteps - 1;

                return _appropriateLODLevel;
            }

            Debug.LogError("Failed to get appropriate LOD level - returning texture LOD for static setting.");
            return Instance._textureStaticCommon;
        }

        /// <summary>
        /// Creates a generic blueprint used to add rings to planet without ring blueprint. Public so editor script can call this method.
        /// </summary>
        public void CreateGenericBlueprint()
        {
            if (Instance.transform.Find(GENERIC_PLANET_BLUEPRINT_NAME) != null)
            {
                // Generic blueprint already exists, no action needed
                return;
            }
                
            GameObject _goPlanet = new GameObject();
            _goPlanet.name = GENERIC_PLANET_BLUEPRINT_NAME;
            _goPlanet.transform.SetParent(Instance.transform);
            _goPlanet.AddComponent<Blueprint>();
            _goPlanet.hideFlags = HideFlags.HideInHierarchy;
            GameObject _goRing = new GameObject();
            _goRing.name = "Ring";
            _goRing.transform.SetParent(_goPlanet.transform);
            _goRing.AddComponent<BlueprintRing>();
            _goRing.hideFlags = HideFlags.HideInHierarchy;
        }
    }   
}
