using UnityEngine;
using System.Collections;

public class ProceduralField : MonoBehaviour
{

    private ParticleSystem.Particle[] Particles;

    public float StartColor_UpdateRate;
    public Color StartColor;
    public Color[] StartColor_Colors;

    public int Particles_Max;
    public float Particles_Size;
    public float Particles_Distance;

    private float Particles_DistanceSqr;

    private ParticleSystem _ParticleSystem;

    private void Start ()
    {
        _ParticleSystem = GetComponent<ParticleSystem>();

        Particles_DistanceSqr = Particles_Distance * Particles_Distance;

        if (StartColor_Colors.Length > 1)
            InvokeRepeating("UpdateColor", 0, StartColor_UpdateRate);
    }

    private void UpdateColor ()
    {

        StartColor = StartColor_Colors[Random.Range(0, StartColor_Colors.Length)];

    }

    

    private void UpdateParticles ()
    {
        Particles = new ParticleSystem.Particle[Particles_Max];

        for (int i = 0; i < Particles_Max; i++)
        {
            Particles[i].position = Random.insideUnitSphere * Particles_Distance + transform.position;
            Particles[i].startColor = StartColor;
            Particles[i].startSize = Particles_Size;
        }
    }


    private void FixedUpdate ()
    {

        if (Particles == null)
            UpdateParticles();

        for (int particleId = 0; particleId < Particles_Max; particleId++)
        {

            float _SqrPositionMagnitude = (Particles[particleId].position - transform.position).sqrMagnitude;

            if (_SqrPositionMagnitude > Particles_DistanceSqr)
            {
                Particles[particleId].position = Random.insideUnitSphere.normalized * Particles_Distance + transform.position;

                Particles[particleId].startColor = StartColor;
            }


        }




        _ParticleSystem.SetParticles(Particles, Particles.Length);

    }
}