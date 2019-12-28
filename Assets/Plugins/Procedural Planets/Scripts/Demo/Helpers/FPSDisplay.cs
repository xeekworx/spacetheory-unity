// Displays Frames Per Second (FPS) in upper left corner of screen.

using UnityEngine;

namespace ProceduralPlanets
{
    public class FPSDisplay : MonoBehaviour
    {
        private float _deltaTime = 0.0f;
        private Color _textColor = new Color(1.0f, 1.0f, 0.3f, 1.0f);

        void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int _w = Screen.width;
            int _h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(10, 10, _w, _h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = _h * 2 / 100;
            style.normal.textColor = _textColor;
            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}