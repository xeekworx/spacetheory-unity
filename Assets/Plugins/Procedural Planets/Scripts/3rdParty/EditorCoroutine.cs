/* 
    Source & Credit: https://gist.github.com/benblo/10732554

    Purpose: Allows coroutines to execute in editor.
*/

#if UNITY_EDITOR
using System.Collections;

namespace ProceduralPlanets
{
    public class EditorCoroutine
    {
        public static EditorCoroutine start(IEnumerator _routine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(_routine);
            coroutine.start();
            return coroutine;
        }

        readonly IEnumerator routine;
        EditorCoroutine(IEnumerator _routine)
        {
            routine = _routine;
        }

        void start()
        {
            UnityEditor.EditorApplication.update += update;
        }

        public void stop()
        {
            UnityEditor.EditorApplication.update -= update;
        }

        void update()
        {
            if (!routine.MoveNext())
            {
                stop();
            }
        }
    }
}
#endif