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
        PlanetManager.TextureLODComposition = new int[5] { 7, 6, 5, 3, 1 };
        // Set the planet sizes used to transition between mesh LOD levels
        // Note: There are only 4 entries in this array because it specifies the size *between* the LOD levels.
        // The float values in the array represent the ratio between screen height and the height of a planet as seen by the camera.
        // E.g. the value 0.5f = when the planet takes up half the screen height.                
        PlanetManager.TextureLODPlanetSizes = new float[4] { 0.6f, 0.4f, 0.2f, 0.05f };
    }
}