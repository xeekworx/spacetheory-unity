using UnityEngine;

namespace ProceduralPlanets
{
    /// <summary>
    /// This component contains properties for a local star and it's used by planets to locate a star in the scene (by finding this component)
    /// 
    /// Note: This is not a traditional light of any sort - the planet and ring shaders will use location and these parameters to apply lighting to themselves. It has no other lighting effect in Unity on other objects.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    public class LocalStar : MonoBehaviour
    {
        // Color of the local star (default is white) - this will affect the color tinting of planets and rings
        public Color color = Color.white;

        // Light intensity of the local star - this will affect the brightness of planets and rings
        [Range(0f, 4.0f)]
        public float intensity = 1.0f;

        // Ambient intensity of the local star - this will affect the areas of planets and rings that are not directly hit by the light
        [Range(-0.1f, 1.0f)]
        public float ambientIntensity = 0.01f;

        /// <summary>
        /// Class used by planets and rings to store cached settings for increased performance to see if the local star has changed.
        /// </summary>
        public class ShaderCacheSettings
        {
            public Vector3 position;
            public float intensity;
            public Color color;
            public float ambientIntensity;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 5);
        }
    }
}
