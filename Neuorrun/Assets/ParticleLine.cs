using UnityEngine;

public class ParticleLineFollow : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private int numberOfParticles = 100;

    private ParticleSystem.Particle[] particles;

    private void Start()
    {
        InitializeParticles();
    }

    private void Update()
    {
        UpdateParticlePositions();
    }

    private void InitializeParticles()
    {
        particles = new ParticleSystem.Particle[numberOfParticles];
        particleSystem.Emit(numberOfParticles);
        particleSystem.GetParticles(particles);
    }

    private void UpdateParticlePositions()
    {
        for (int i = 0; i < numberOfParticles; i++)
        {
            float t = i / (float)(numberOfParticles - 1);
            particles[i].position = Vector3.Lerp(startPoint.position, endPoint.position, t);
        }

        particleSystem.SetParticles(particles, numberOfParticles);
    }
}