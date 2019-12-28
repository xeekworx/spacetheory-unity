/*  
    Class: CreatePlanetFromScript
    Version: 0.1.1 (alpha release)
    Date: 2018-01-10
    Author: Stefan Persson
    (C) Imphenzia AB

    This script demonstrates:
    * Simple random planet creation from script
    * Planet creation from a Base64 encoded JSON string from script
    * UI slider to override one "liquidLevel" shader property (fast) and one "cloudsCoverage" procedural texture property (slow)
    * Subscribing to OnTextureBuildComplete messages
    * Changing texture resolution
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// If you don't use this directive you have to use the fully qualified name for each method call, e.g. ProceduralPlanets.Manager.Instance.CreatePlanet() instead of Manager.Instance.CreatePlanet();
using ProceduralPlanets;

public class CreatePlanetFromScript : MonoBehaviour {
    // UI elements
    public Slider uiSliderLiquidLevel;
    public Slider uiSliderCloudsCoverage;
    public Dropdown uiDropdownResolution;
    public Text uiTextMessage;

    // Camera parent gameObject, used for easy orbit
    public Transform cameraParent;

    // Private variables
    private Planet _planet;
    private float _dragSpeed = 0.1f;
    private float _mouseSpeed = 10f;
    private bool _ignoreOverrideFlag = false;

    /// <summary>
    /// The Start() method is a default monobehavior method that runs every time a game/scene is started.
    /// </summary>
    void Start()
    {
        // Force use of static resolution
        PlanetManager.TextureDetailMode = PlanetManager.TextureDetailModes.Static;

        // Force resolution to 512x512
        /* 
        Valid resolution values are:
        0 = 16 x 16 (lowest)
        1 = 32 x 32
        2 = 64 x 64
        3 = 128 x 128
        4 = 256 x 256
        5 = 512 x 512
        6 = 1024 x 1024
        7 = 2048 x 2048
        */
        PlanetManager.TextureStaticCommon = 5;

        // Create a planet from predefined Base64 JSON string
        CreatePlanetUsingBase64EncodedJSON();

        // Add listeners and delegate methods to sliders and dropdowns
        uiSliderLiquidLevel.onValueChanged.AddListener(delegate { OverrideLiquidLevel(); });
        uiSliderCloudsCoverage.onValueChanged.AddListener(delegate { OverrideCloudsCoverage(); });
        uiDropdownResolution.onValueChanged.AddListener(delegate { SetTextureResolution(); });

        // The UI buttons to create planets are configured in the UI component inspector where it calls methods in this script on the OnClicked evenets.
        
        // Clear the UI text message.
        uiTextMessage.text = "";

    }



    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {

        // If the pointer (or finger) is over any UI object, return without executing the code below
        if (IsPointerOverUIObject()) return;

        // COMPUTERS (or devices without touch support) - Camera Rotate and Zoom
        if (!Input.touchSupported)
        {
            // If left mouse button is held down...
            if (Input.GetMouseButton(0))
            {
                // The main camera has been paranted to a gameobject that is in the center of a scene to make it easy to orbit by rotating the parent gameobject.
                cameraParent.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * _mouseSpeed, 0));

                // Move the camera closer/away based on Mouse Y position
                Camera.main.transform.localPosition = new Vector3(0, 5, Camera.main.transform.localPosition.z + Input.GetAxis("Mouse Y") * _mouseSpeed * 0.01f);
            }
        }

        // TOUCH DEVICES - Camera Rotate and Zoom
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 _touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // The main camera has been paranted to a gameobject that is in the center of a scene to make it easy to orbit by rotating the parent gameobject.
            // Rotation is performed based on touch device dragging left/right
            cameraParent.Rotate(new Vector3(0, _touchDeltaPosition.x * _dragSpeed, 0));

            // Move the camera closer/away based on touch device dragging up/down
            Camera.main.transform.localPosition = new Vector3(0, 5, Camera.main.transform.localPosition.z + Input.GetAxis("Mouse Y") * _dragSpeed);
        }
    }

    /// <summary>
    /// Creates a simple random planet.
    /// </summary>
    public void CreateRandomPlanet()
    {
        // If a planet already exists in the scene, destroy it
        if (_planet != null) DestroyImmediate(_planet.gameObject);

        // Use PlanetManager.CreatePlanet method to create a new planet - when only a position is included as an argument it will create a totally random planet
        // and select a planet blueprint based on the blueprint's probability value.
        _planet = PlanetManager.CreatePlanet(Vector3.zero);
        _planet.AddListener(gameObject);

        // Set the UI text message to indicate that textures are being rebuilt.
        uiTextMessage.text = "Rebuilding textures...";
    }

    /// <summary>
    /// Creates a planet based on a previously exported Base64 encoded JSON string (exported via the inspector of a planet in the Unity editor).
    /// </summary>
    public void CreatePlanetUsingBase64EncodedJSON()
    {
        // If a planet already exists in the scene, destroy it
        if (_planet != null) DestroyImmediate(_planet.gameObject);

        // Use PlanetManager.CreatePlanet method to create a new planet and include the Base64 JSON string as a second argument to force all properties.
        _planet = PlanetManager.CreatePlanet(Vector3.zero, "eyJjYXRlZ29yeSI6InBsYW5ldCIsInR5cGUiOiJTb2xpZFBsYW5ldCIsInZlcnNpb24iOiIwLjEuMiIsInBsYW5ldCI6eyJwbGFuZXRTZWVkIjo3NTQzODA2OTcsInZhcmlhdGlvblNlZWQiOjAsImJsdWVwcmludEluZGV4IjowLCJibHVlcHJpbnROYW1lIjoiVGVycmVzdHJpYWwiLCJwcm9wZXJ0eUZsb2F0cyI6eyJhbGllbml6YXRpb24iOjAsImNvbnRpbmVudFNlZWQiOjk0LCJjb250aW5lbnRTaXplIjowLjc4NTMyODEsImNvbnRpbmVudENvbXBsZXhpdHkiOjAuMzExNjc2MiwiY29hc3RhbERldGFpbCI6MC4yNDYwMzg3LCJjb2FzdGFsUmVhY2giOjAuNDkyNTEyNCwibGlxdWlkTGV2ZWwiOjAuNTYyLCJsaXF1aWRPcGFjaXR5IjoxLCJsaXF1aWRTaGFsbG93IjowLCJsaXF1aWRTcGVjdWxhclBvd2VyIjowLjUxOTMzNjYsInBvbGFyQ2FwQW1vdW50IjowLjQ2NDE5NjIsImF0bW9zcGhlcmVFeHRlcm5hbFNpemUiOjAuNTU2MjQxMywiYXRtb3NwaGVyZUV4dGVybmFsRGVuc2l0eSI6MC43MTIzMTMzLCJhdG1vc3BoZXJlSW50ZXJuYWxEZW5zaXR5IjowLjk3NTE4NzMsImNsb3Vkc09wYWNpdHkiOjEsImNsb3Vkc1NlZWQiOjExNSwiY2xvdWRzQ292ZXJhZ2UiOjAuNDc5LCJjbG91ZHNMYXllcjEiOjAuNDU1LCJjbG91ZHNMYXllcjIiOjAuODEyLCJjbG91ZHNMYXllcjMiOjAuODc1LCJjbG91ZHNTaGFycG5lc3MiOjAuNDU2MjE2NSwiY2xvdWRzUm91Z2huZXNzIjowLjM1MDg3MjksImNsb3Vkc1RpbGluZyI6MiwiY2xvdWRzU3BlZWQiOjAuMDY4MjgxNDYsImNsb3Vkc0hlaWdodCI6MC4zOTg2OTg2LCJjbG91ZHNTaGFkb3ciOjAuMzA2NDQxMywibGF2YUFtb3VudCI6MCwibGF2YUNvbXBsZXhpdHkiOjAsImxhdmFGcmVxdWVuY3kiOjAuMDM3NjY1MzcsImxhdmFEZXRhaWwiOjAuNDIyNjc4OCwibGF2YVJlYWNoIjowLjI3NzcxNDMsImxhdmFDb2xvclZhcmlhdGlvbiI6MC43MjExNzI4LCJsYXZhRmxvd1NwZWVkIjowLjU1MzIyNjUsImxhdmFHbG93QW1vdW50IjowLjkyMTM1NjEsInN1cmZhY2VUaWxpbmciOjgsInN1cmZhY2VSb3VnaG5lc3MiOjAuMTI4MjE2MywiY29tcG9zaXRpb25TZWVkIjo5NCwiY29tcG9zaXRpb25UaWxpbmciOjIsImNvbXBvc2l0aW9uQ2hhb3MiOjAuNjYxNDE0NSwiY29tcG9zaXRpb25CYWxhbmNlIjowLjE3NSwiY29tcG9zaXRpb25Db250cmFzdCI6MC44NTY3MjY0LCJiaW9tZTFTZWVkIjozMywiYmlvbWUxQ2hhb3MiOjAuMDE3MzIyNTQsImJpb21lMUJhbGFuY2UiOjAuNDQzNjcxOCwiYmlvbWUxQ29udHJhc3QiOjAuNjg3NTcxLCJiaW9tZTFDb2xvclZhcmlhdGlvbiI6MC41MTE1NzQ2LCJiaW9tZTFTYXR1cmF0aW9uIjowLjUzNzA4NzUsImJpb21lMUJyaWdodG5lc3MiOjAuNTE4NzE5MywiYmlvbWUxU3VyZmFjZUJ1bXAiOjAuMTQ1MzM5MywiYmlvbWUxQ3JhdGVyc1NtYWxsIjowLjQ4NTg1MjIsImJpb21lMUNyYXRlcnNNZWRpdW0iOjAuMzQyNzY0NiwiYmlvbWUxQ3JhdGVyc0xhcmdlIjowLjY5MzI3OTcsImJpb21lMUNyYXRlcnNFcm9zaW9uIjowLjU1MDc2OTksImJpb21lMUNyYXRlcnNEaWZmdXNlIjowLjg4ODQ4NjcsImJpb21lMUNyYXRlcnNCdW1wIjowLjIyNTY2MjYsImJpb21lMUNhbnlvbnNEaWZmdXNlIjowLjExNjA1NTYsImJpb21lMUNhbnlvbnNCdW1wIjowLjQ2NDUxNzUsImJpb21lMlNlZWQiOjcxLCJiaW9tZTJDaGFvcyI6MC42MTcwNTEzLCJiaW9tZTJCYWxhbmNlIjowLjU3NDM3NjksImJpb21lMkNvbnRyYXN0IjowLjg3NzY1NCwiYmlvbWUyQ29sb3JWYXJpYXRpb24iOjAuNDU2NzE3OSwiYmlvbWUyU2F0dXJhdGlvbiI6MC40NjcxMjE4LCJiaW9tZTJCcmlnaHRuZXNzIjowLjQ4MDQ4NzMsImJpb21lMlN1cmZhY2VCdW1wIjowLjQ5NDY2MzIsImJpb21lMkNyYXRlcnNTbWFsbCI6MC42NDQwNTk1LCJiaW9tZTJDcmF0ZXJzTWVkaXVtIjowLjc5NDUxMzMsImJpb21lMkNyYXRlcnNMYXJnZSI6MC45NDUyMjk1LCJiaW9tZTJDcmF0ZXJzRXJvc2lvbiI6MC4wODM2NTkyOSwiYmlvbWUyQ3JhdGVyc0RpZmZ1c2UiOjAuMjQ1NTc3MiwiYmlvbWUyQ3JhdGVyc0J1bXAiOjAuMzc2NzM2NSwiYmlvbWUyQ2FueW9uc0RpZmZ1c2UiOjAuNTIwNDg1LCJiaW9tZTJDYW55b25zQnVtcCI6MC42MzQxOTc2LCJjaXRpZXNTZWVkIjo2MCwiY2l0aWVzUG9wdWxhdGlvbiI6MC45NDc0NDI4LCJjaXRpZXNBZHZhbmNlbWVudCI6MC4wNTYzMTk2MSwiY2l0aWVzR2xvdyI6MCwiY2l0aWVzVGlsaW5nIjozfSwicHJvcGVydHlNYXRlcmlhbHMiOnsiY29tcG9zaXRpb24iOnsiaW5kZXgiOjYsIm5hbWUiOiJTb2xpZF9UZXJyZXN0cmlhbCJ9LCJwb2xhckljZSI6eyJpbmRleCI6MCwibmFtZSI6IlBvbGFyX0ljZSJ9LCJjbG91ZHMiOnsiaW5kZXgiOjMsIm5hbWUiOiJDbG91ZHNfMDQifSwibGF2YSI6eyJpbmRleCI6MCwibmFtZSI6IkxhdmFfMDEifSwiYmlvbWUxVHlwZSI6eyJpbmRleCI6NSwibmFtZSI6IkJpb21lX1JhaW5fRm9yZXN0In0sImJpb21lMlR5cGUiOnsiaW5kZXgiOjcsIm5hbWUiOiJCaW9tZV9UdW5kcmEifSwiY2l0aWVzIjp7ImluZGV4IjowLCJuYW1lIjoiQ2l0aWVzIn19LCJwcm9wZXJ0eUNvbG9ycyI6eyJzcGVjdWxhckNvbG9yIjp7InIiOjAuMzk4NDk2LCJnIjowLjMyMjIzMDgsImIiOjAuMDQ3MDEwMjF9LCJsaXF1aWRDb2xvciI6eyJyIjowLjAwMzU5MzQ1OCwiZyI6MC4wMTU1NDM1MSwiYiI6MC4wMzQxMTN9LCJpY2VDb2xvciI6eyJyIjowLjk1NDkxNjUsImciOjAuOTI1NTA1LCJiIjowLjkyNTUwNX0sImF0bW9zcGhlcmVDb2xvciI6eyJyIjowLjE1NTMxNzUsImciOjAuNzQ0Njc4MywiYiI6MX0sInR3aWxpZ2h0Q29sb3IiOnsiciI6MC4zODU0NjQsImciOjAuMzE5NjA4NCwiYiI6MC4wODcwNTUzOX0sImNsb3Vkc0NvbG9yIjp7InIiOjEsImciOjEsImIiOjF9LCJsYXZhR2xvd0NvbG9yIjp7InIiOjAuOTY3ODU2OCwiZyI6MC4wNDA3OTc4OSwiYiI6MC4wMTY5NDQxMX0sImNpdGllc0NvbG9yIjp7InIiOjAuOTMzMDU1OCwiZyI6MC45MjQxNzkxLCJiIjowLjYzNTQzM319fX0=");
        _planet.AddListener(gameObject);

        // Set the UI text message to indicate that textures are being rebuilt.
        uiTextMessage.text = "Rebuilding textures...";
    }

    /// <summary>
    /// This method is automatically called by a planet once it has finished rebuilding any textures.
    /// You must call the SubscribeMessageOnTextureBuildComplete(GameObject _gameObject) method of the planet to add a gameobject to the message/event subscription.
    /// </summary>
    /// <param name="_time">The time argument contains time it took for the last rebuild process to finish in seconds.</param>
    void OnTextureBuildComplete(float _time)
    {
        // Display a log message that the method was called and how long it took to rebuild the textures.
        Debug.Log("Planet done regenerating textures (" + _time + " seconds)");

        // Set a temporary ignore flag while updating the slider values because the slider onValueChanged
        // delegate methods are executed when a value is set which would call the override method of the planet again.
        _ignoreOverrideFlag = true;

        // When a planet has finished building the textures, update the slider positions
        if (_planet is SolidPlanet)
        {
            uiSliderLiquidLevel.value = _planet.GetPropertyFloat("liquidLevel");
            uiSliderCloudsCoverage.value = _planet.GetPropertyFloat("cloudsCoverage");
        }

        // Update the ui text message to dispaly texture rebuild time
        uiTextMessage.text = "Build time: " + _time.ToString("F2") + " seconds.";

        // Remove the temporary ignore flag
        _ignoreOverrideFlag = false;
    }

    /// <summary>
    /// Overrides Liquid Level of the planet with a value from the UI slider.
    /// This method is called every time the slider is moved because an onValueChange delegate was added in the Start() method.
    /// </summary>
    public void OverrideLiquidLevel()
    {
        // Override the "liquidLevel" property to the value set by the UI slider
        // The liquidLevel property is fairly cheap to call performance-wise because it only creates a lookup texture for the shader.
        if (_planet is SolidPlanet)
        {
            _planet.OverridePropertyFloat("liquidLevel", uiSliderLiquidLevel.value);
        }
        
    }

    /// <summary>
    /// Overrides Cloud Coverage of the planet with a value from the UI slider.
    /// This method is called every time the slider is moved because an onValueChange delegate was added in the Start() method.
    /// </summary>
    public void OverrideCloudsCoverage()
    {
        // A temporary flag was set in the OnTextureBuildComplete() method to avoid this from being called twice when the UI value is updated from the script.
        if (_ignoreOverrideFlag) return;

        // Override the "cloudsCoverage" property to the value set by the UI slider
        // The cloudsCoverage property is fairly expensive to call performance-wise because it needs to rebuild the cloud procedural texture.
        if (_planet is SolidPlanet)
        {
            _planet.OverridePropertyFloat("cloudsCoverage", uiSliderCloudsCoverage.value);
        }
    }

    /// <summary>
    /// Sets the texture resolutions. They can be set independently of one another and usually you want to keep the Composition and Clouds
    /// textures higher than the other textures because they tile less.
    /// </summary>
    public void SetTextureResolution()
    {
        /* 
            Valid resolution values are:
                0 = 16 x 16 (lowest)
                1 = 32 x 32
                2 = 64 x 64
                3 = 128 x 128
                4 = 256 x 256
                5 = 512 x 512
                6 = 1024 x 1024
                7 = 2048 x 2048
        */

        // Set texture static resolution
        PlanetManager.TextureStaticCommon = uiDropdownResolution.value;

        uiTextMessage.text = "Rebuilding textures...";
    }

    /// <summary>
    /// Helper method that detects if mouse or a touch finger is over a Raycast target UI element or not.
    /// This is to avoid camera rotation/zoom when interacting with the UI sliders.
    /// </summary>
    /// <returns>True/False</returns>
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
