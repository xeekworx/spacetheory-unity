
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProceduralPlanets.SimpleJSON;
namespace ProceduralPlanets
{
    /// <summary>
    /// This is the base class for static planets. Static planets have been baked from a procedural planet and can no longer be changed.
    /// Solid and Gas static planets derive from this class.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    public abstract class PlanetStatic : MonoBehaviour
    {
        protected const string PLANET_VERSION = "1.0";
        
        // Meshes and renderers
        //protected Mesh _mesh;
        public int meshLODLevel;
        public MeshFilter meshFilter;
        protected MeshRenderer _meshRenderer;
        public int meshSubdivisions = 6;

        // Integer IDs of shader properties for performance
        protected int _shaderID_LocalStarPosition;
        protected int _shaderID_LocalStarColor;
        protected int _shaderID_LocalStarIntensity;
        protected int _shaderID_LocalStarAmbientIntensity;

        // Local Star
        protected LocalStar.ShaderCacheSettings _localStarShaderCacheSettings;
        protected LocalStar _localStarNearestInstance;       

        // Used to keep track of last position to update local star shader light direction if planet has moved.
        protected Vector3 _lastPosition;

        protected abstract void Awake();

        /// <summary>
        /// Updates local star position and checks if any textures need to be rebuilt. This happens every frame.
        /// </summary>
        protected virtual void Update()
        {
            // Update local star position
            UpdateShaderLocalStar(false);
        }

        /// <summary>
        /// Updates the shader to take into account the properties of a local star in the scene (e.g. position, intensity, color of star).
        /// Used for lighting and shadows.
        /// </summary>
        /// <param name="_forceUpdate"></param>
        protected virtual void UpdateShaderLocalStar(bool _forceUpdate)
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
    }
}
