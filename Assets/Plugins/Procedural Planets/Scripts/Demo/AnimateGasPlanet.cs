using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralPlanets;

/// <summary>
/// Attach this script to a Gas planet and it will animate some properties over 60 seconds with a 2 second delayed start.
/// IMPORTANT: It is strongly recommended to set PlanetManager Texture Detail to anything else than progressive in the inspector.
/// </summary>
namespace ProceduralPlanets
{
    public class AnimateGasPlanet : MonoBehaviour
    {
        Planet _planet;

        void Start()
        {
            // Get a reference to the Gas planet
            _planet = GetComponent<GasPlanet>();

            if (_planet == null)
            {
                Debug.LogError("Not a gas planet. Aborting!");
                return;
            }

            // Cache the procedural properties - should increase performance
            _planet.CacheProceduralProperty("tubulenceDisorder", true);
            _planet.CacheProceduralProperty("stormScale", true);
            _planet.CacheProceduralProperty("stormNoise", true);
            _planet.CacheProceduralProperty("turbulence", true);

            // Initiate animation of properties: <propertyKey>, <from_value>, <to_value>, <duration>, <delay>
            _planet.Animate("turbulenceDisorder", 0.0f, 1.0f, 60f, 2.0f);
            _planet.Animate("stormScale", 0.1f, 0.6f, 60f, 2.0f);
            _planet.Animate("stormNoise", 0.05f, 0.35f, 60f, 2.0f);
            _planet.Animate("turbulence", 0.2f, 0.8f, 60f, 2.0f);
        }
    }

}
