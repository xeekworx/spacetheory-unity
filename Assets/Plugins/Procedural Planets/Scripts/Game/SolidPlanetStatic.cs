using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralPlanets.SimpleJSON;

namespace ProceduralPlanets
{

    /// <summary>
    /// This component is used by solid planets that have been baked from being a procedural planet to a static baked planet.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>


    // Execute in edit mode because we want to be able to change planet parameter and rebuild textures in editor
    [ExecuteInEditMode]

    // Require MeshFilter and MeshRenderer for planet
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class SolidPlanetStatic : PlanetStatic
    {
        // External Atmosphere
        protected GameObject _externalAtmosphere;
        protected MeshFilter _externalAtmosphereMeshFilter;
        protected MeshRenderer _externalAtmosphereRenderer;
        
        public int atmosphereSubdivisions;        
        
        protected override void Awake()
        {
            if ((int)PlanetManager.DebugLevel > 0) Debug.Log("SolidPlanetStatic.cs: Awake()");
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
            
            _externalAtmosphere = transform.Find("ExternalAtmosphere").gameObject;
            if (_externalAtmosphere == null)
                Debug.LogError("Planet has no atmosphere game object. You could try to rebake this planet from the original procedural planet or the JSON file in the asset folder for this static planet.");
            _externalAtmosphereMeshFilter = _externalAtmosphere.GetComponent<MeshFilter>();
            // Use the planet's procedural octahedron sphere mesh as the atmosphere mesh as well
            _externalAtmosphereMeshFilter.sharedMesh = meshFilter.sharedMesh;
            _externalAtmosphereRenderer = _externalAtmosphere.GetComponent<MeshRenderer>();

            // Get reference to MeshRenderer Component
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();

            // Update shader for planet lighting
            UpdateShaderLocalStar(true);
        }

        /// <summary>
        /// Updates the shader to take into account the properties of a local star in the scene (e.g. position, intensity, color of star).
        /// Used for lighting and shadows.
        /// </summary>
        /// <param name="_forceUpdate"></param>
        protected override void UpdateShaderLocalStar(bool _forceUpdate)
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
    }

}
