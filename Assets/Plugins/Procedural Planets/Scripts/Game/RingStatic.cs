using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ProceduralPlanets.SimpleJSON;

namespace ProceduralPlanets
{
    /// <summary>
    /// This is the class for static ring.
    /// 
    /// Static rings have been baked from procedural rings and can no longer be changed.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    [ExecuteInEditMode]
    public class RingStatic : MonoBehaviour
    {
        const string RING_VERSION = "1.0";
        
        // Textures used by the ring
        private Texture2D _textureRing;
        
        // Private Variables
        // There are two of each ring object, mesh, renderer, and materials because the ring is split in two sections to
        // allow proper sorting when infront and behind semitransparent atmosphere.
        private GameObject[] _ring = new GameObject[2];
        private Mesh[] _mesh = new Mesh[2];
        private MeshFilter[] _meshFilter = new MeshFilter[2];
        private MeshRenderer[] _meshRenderer = new MeshRenderer[2];
        public Material[] materials = new Material[2];

        public float ringInnerRadius;
        public float ringOuterRadius;

        public int ringMeshSubdivisions;

        // Planet variables
        private Transform  _planetTransform;
        private float _planetRadius;


        // Integer IDs of shader properties for performance
        private int _shaderID_LocalStarPosition;
        private int _shaderID_LocalStarColor;
        private int _shaderID_LocalStarIntensity;
        private int _shaderID_LocalStarAmbientIntensity;
        private int _shaderID_PlanetRadius;
        private int _shaderID_PlanetPosition;

        // Local Star
        private LocalStar.ShaderCacheSettings _localStarShaderCacheSettings;
        private LocalStar _localStarNearestInstance;

        void Reset()
        {
            if (gameObject.GetComponent<Planet>() != null)
            {
                Debug.LogError("You can't add this ring component directly to a planet. It must be a child object. Aborting and removing component.");
                DestroyImmediate(this);
                return;
            }
        }

        /// <summary>
        /// Creates the ring and adds all necessary property materials and floats
        /// </summary>
        void Awake()
        {
            if (gameObject.GetComponent<Planet>() != null)
            {
                Debug.LogError("You can't add this ring component directly to a planet. It must be a child object. Aborting and removing component.");
                DestroyImmediate(this);
                return;
            }

            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("RingStatic.cs: Awake()");
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("- RingStatic Version: " + RING_VERSION);

            // Set Shader property int IDs for increased performance when updating property parameters
            _shaderID_LocalStarPosition = Shader.PropertyToID("_LocalStarPosition");
            _shaderID_LocalStarColor = Shader.PropertyToID("_LocalStarColor");
            _shaderID_LocalStarIntensity = Shader.PropertyToID("_LocalStarIntensity");
            _shaderID_LocalStarAmbientIntensity = Shader.PropertyToID("_LocalStarAmbientIntensity");
            _shaderID_PlanetRadius = Shader.PropertyToID("_PlanetRadius");
            _shaderID_PlanetPosition = Shader.PropertyToID("_PlanetPosition");

            // Ensure that there is a LocalStar in the scene.
            if (FindObjectOfType<LocalStar>() == null)
                Debug.LogWarning("There is no LocalStar in the scene. Planet and ring will not be lit. Create a game object and add the LocalStar component. The position of the local star game object will be the light source.");

            // Ensure that this ring has a parent planet transform
            if (transform.parent == null)
            {
                Debug.LogWarning("There is no parent planet transform to this ring. Aborting ring creation.");
                return;
            }

            // Set the planet radius (used for shadow size) - this is static at the moment and it'll scale with planet scale
            _planetTransform = transform.parent;
            _planetRadius = PlanetManager.CONST_MESH_RADIUS + 1.0f;

            // Create meshes
            CreateMeshes();

            // Update shader for planet lighting
            UpdateShaderLocalStar(true);

            
        }

        /// <summary>
        /// Update is called every frame and it rotates the ring in relation to the camera to ensure atmosphere transparency sorting works.
        /// </summary>
        void Update()
        {
            // Find the main camera transform acting as target
            Transform _target = Camera.main.transform;

            // Get the target position Vector3
            Vector3 _targetPostition = new Vector3(_target.position.x, transform.position.y, _target.position.z);

            // Make the ring transform look at the camera along and rotate around the up axis
            transform.LookAt(_targetPostition, Vector3.up);

            // Get the main camera transform forward direction vector3
            Vector3 _cameraForward = Camera.main.transform.forward;

            // Get the sign of the cross product of the negative transform and camera forward vector
            int _sign = Vector3.Cross(-transform.forward, _cameraForward).y < 0 ? -1 : 1;

            // Get the angle between the negative transform foward and camera forward
            float _angle = Vector3.Angle(-transform.forward, _cameraForward);

            // Multiply angle by the sign 
            _angle *= _sign;

            // Set the new forward of the ring transform
            Vector3 _newForward = Vector3.Lerp(transform.forward, new Vector3(-Camera.main.transform.forward.x, 0, -Camera.main.transform.forward.z), 0.5f);            
            if (_newForward != Vector3.zero)
                transform.forward = _newForward;

            // Rotate the ring negative 80 degrees, making it most probable that the ring and atmosphere sorts correctly
            // It's 80 degrees and not 90 because the close part of the ring mesh covers 160 degrees (80 is half of that) and the far side of the ring covers 200 degrees
            transform.Rotate(new Vector3(0.0f, -80.0f, 0.0f));
            
            // Update shader based on local star parameters (only if changed, hence force is set to false)
            UpdateShaderLocalStar(false);

        }

        /// <summary>
        /// Create ring meshes. There are two because ring is split into two sections with different sort orders to render in front and behind semitransparent planet atmosphere.
        /// </summary>
        public void CreateMeshes()
        {
            if ((int) PlanetManager.DebugLevel > 0) Debug.Log("RingStatic.cs: CreateMeshes()");

            // If ring meshes exist, destroy them because we'll recreate them
            if (transform.Find("MeshClose") != null) DestroyImmediate(transform.Find("MeshClose").gameObject);
            if (transform.Find("MeshFar") != null) DestroyImmediate(transform.Find("MeshFar").gameObject);

            // Create two meshes
            for (int _i = 0; _i < 2; _i++)
            {
                _ring[_i] = new GameObject();
                _ring[_i].transform.parent = transform;
                _ring[_i].transform.localPosition = Vector3.zero;
                _ring[_i].transform.localRotation = Quaternion.identity;                    

                if (_i == 0)
                {
                    // The first mesh is the close mesh and it covers 160 degrees closest to the camera
                    _mesh[_i] = ProceduralRing.Create(ringInnerRadius, ringOuterRadius, 200, 160f);
                    _ring[_i].name = "MeshClose";
                }
                else
                {
                    // The second mesh is the far side mesh and it covers 200 degrees on the far side of the planet
                    _mesh[_i] = ProceduralRing.Create(ringInnerRadius, ringOuterRadius, 200, 200f);
                    _ring[_i].name = "MeshFar";
                    _ring[_i].transform.Rotate(0.0f, 160.0f, 0.0f);
                }
                _meshFilter[_i] = _ring[_i].AddComponent<MeshFilter>();
                _meshFilter[_i].sharedMesh = _mesh[_i];
                _meshRenderer[_i] = _ring[_i].AddComponent<MeshRenderer>();
                _meshRenderer[_i].sharedMaterial = materials[_i];                
            }
        }

        /// <summary>
        /// Updates the shader to take into account the properties of a local star in the scene (e.g. position, intensity, color of star).
        /// Used for lighting and shadows.
        /// </summary>
        /// <param name="_forceUpdate"></param>
        void UpdateShaderLocalStar(bool _forceUpdate)
        {
            if (materials.Length != 2)
                return;
            if (materials[0] == null)
                return;
            if (materials[1] == null)
                return;

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
                    if (Vector3.Distance(_localStarNearestInstance.transform.position, transform.position) < Vector3.Distance(_localStarNearestInstance.transform.position, transform.position))
                        _localStarNearestInstance = _ls;
                }
            }

            // If there are no local stars in the scene, return
            if (_localStarNearestInstance == null) return;

            // Optimize
            for (int _i = 0; _i < 2; _i++)
            {
                materials[_i].SetFloat(_shaderID_PlanetRadius, _planetRadius);
                materials[_i].SetVector(_shaderID_PlanetPosition, _planetTransform.position);
            }
                
            // Detect if if local star position is different from the cache - if so, update the shader with new settings and update the cache
            if (Vector3.Distance(_localStarShaderCacheSettings.position, _localStarNearestInstance.transform.position) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.position = _localStarNearestInstance.transform.position;                
                for (int _i = 0; _i < 2; _i++)
                    _meshRenderer[_i].sharedMaterial.SetVector(_shaderID_LocalStarPosition, _localStarNearestInstance.transform.position);                

            }

            // Detect if if local star color is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.color.r - _localStarNearestInstance.color.r) > 0.0001f ||
                Mathf.Abs(_localStarShaderCacheSettings.color.g - _localStarNearestInstance.color.g) > 0.0001f ||
                Mathf.Abs(_localStarShaderCacheSettings.color.b - _localStarNearestInstance.color.b) > 0.0001f ||
                _forceUpdate)
            {
                _localStarShaderCacheSettings.color = _localStarNearestInstance.color;
                for (int _i = 0; _i < 2; _i++)
                    _meshRenderer[_i].sharedMaterial.SetColor(_shaderID_LocalStarColor, _localStarNearestInstance.color);
            }

            // Detect if if local star intensity is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.intensity - _localStarNearestInstance.intensity) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.intensity = _localStarNearestInstance.intensity;
                for (int _i = 0; _i < 2; _i++)
                    _meshRenderer[_i].sharedMaterial.SetFloat(_shaderID_LocalStarIntensity, _localStarNearestInstance.intensity);
            }

            // Detect if if local star ambient intensity is different from the cache - if so, update the shader with new settings and update the cache
            if (Mathf.Abs(_localStarShaderCacheSettings.ambientIntensity - _localStarNearestInstance.ambientIntensity) > 0.0001f || _forceUpdate)
            {
                _localStarShaderCacheSettings.ambientIntensity = _localStarNearestInstance.ambientIntensity;
                for (int _i = 0; _i < 2; _i++)
                    _meshRenderer[_i].sharedMaterial.SetFloat(_shaderID_LocalStarAmbientIntensity, _localStarNearestInstance.ambientIntensity);
            }
        }

    }
}

