
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralPlanets.SimpleJSON;

namespace ProceduralPlanets
{
    /// <summary>
    /// This component is used by gas planets that have been baked from being a procedural planet to a static baked planet.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    
    // Execute in edit mode because we want to be able to change planet parameter and rebuild textures in editor
    [ExecuteInEditMode]

    // Require MeshFilter and MeshRenderer for planet
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class GasPlanetStatic : PlanetStatic
    {
        protected override void Awake()
        {
            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("GasPlanetStatic.cs: Awake()");
            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("- PlanetVersion: " + PLANET_VERSION);

            // Set Shader property int IDs for increased performance when updating property parameters
            _shaderID_LocalStarPosition = Shader.PropertyToID("_LocalStarPosition");
            _shaderID_LocalStarColor = Shader.PropertyToID("_LocalStarColor");
            _shaderID_LocalStarIntensity = Shader.PropertyToID("_LocalStarIntensity");
            _shaderID_LocalStarAmbientIntensity = Shader.PropertyToID("_LocalStarAmbientIntensity");

            // Ensure that there is a LocalStar in the scene.
            if (FindObjectOfType<LocalStar>() == null)
                Debug.LogWarning("There is no LocalStar in the scene. Planet will not be lit. Create a game object and add the LocalStar component. The position of the game object will be the light source.");

            // Get reference to the MeshFilter component
            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = ProceduralOctahedron.Create(meshSubdivisions, 5.0f);

            // Get reference to MeshRenderer Component
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();

            // Update shader for planet lighting
            UpdateShaderLocalStar(true);
        }

        
    }

}
