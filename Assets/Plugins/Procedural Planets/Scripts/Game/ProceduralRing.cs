using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPlanets
{
    /// <summary>
    /// This component generates a procedural ring.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    public static class ProceduralRing
    {

        /// <summary>
        /// Creates a procedural full or partial ring with inner and outer radius with specified number of segments and degrees of coverage.
        /// </summary>
        /// <param name="_radiusIn"></param>
        /// <param name="_radiusOut"></param>
        /// <param name="_segments"></param>
        /// <param name="_degrees"></param>
        /// <returns>Mesh (procedural full or partial ring)</returns>
        public static Mesh Create(float _radiusIn, float _radiusOut, int _segments, float _degrees = 360.0f)
        {

            float _step = 0;
            _step = _degrees / _segments;

            List<Vector3> _vertices = new List<Vector3>();
            List<int> _triangles = new List<int>();
            List<Vector2> _uv1 = new List<Vector2>();
            List<Vector2> _uv2 = new List<Vector2>();
            List<Vector3> _normals = new List<Vector3>();

            Quaternion _quaternion = Quaternion.Euler(0.0f, _step, 0.0f);


            _vertices.Add(new Vector3(0.0f, 0.0f, _radiusIn));
            _vertices.Add(new Vector3(0.0f, 0.0f, _radiusOut));
            _vertices.Add(_quaternion * _vertices[0]);
            _vertices.Add(_quaternion * _vertices[1]);
            _uv1.Add(new Vector2(0.0f, 0.0f));
            _uv1.Add(new Vector2(1.0f, 1.0f));
            _uv1.Add(new Vector2(0.0f, 0.0f));
            _uv1.Add(new Vector2(1.0f, 1.0f));
            _normals.Add(new Vector3(0, 1.0f, 0));
            _normals.Add(new Vector3(0, 1.0f, 0));
            _normals.Add(new Vector3(0, 1.0f, 0));
            _normals.Add(new Vector3(0, 1.0f, 0));
            _triangles.Add(0);
            _triangles.Add(1);
            _triangles.Add(2);
            _triangles.Add(3);
            _triangles.Add(2);
            _triangles.Add(1);

            for (int _i = 0; _i < _segments - 1; _i++)
            {
                _vertices.Add(_quaternion * _vertices[_vertices.Count - 2]);
                _vertices.Add(_quaternion * _vertices[_vertices.Count - 2]);
                _triangles.Add(_vertices.Count - 4);
                _triangles.Add(_vertices.Count - 3);
                _triangles.Add(_vertices.Count - 2);
                _triangles.Add(_vertices.Count - 1);
                _triangles.Add(_vertices.Count - 2);
                _triangles.Add(_vertices.Count - 3);
                _uv1.Add(new Vector2(0.0f, 0.0f));
                _uv1.Add(new Vector2(1.0f, 1.0f));
                _normals.Add(new Vector3(0, 1.0f, 0));
                _normals.Add(new Vector3(0, 1.0f, 0));
            }

            for (int _i = 0; _i < _vertices.Count; _i++)
            {
                _uv2.Add(new Vector2((_vertices[_i].x / _radiusOut) + 0.5f, (_vertices[_i].z / _radiusOut) + 0.5f));
            }

            Mesh _mesh = new Mesh();
            _mesh.name = "Ring";
            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
            _mesh.normals = _normals.ToArray();
            _mesh.uv = _uv1.ToArray();
            _mesh.uv2 = _uv2.ToArray();
            return _mesh;
        }
    }
}
