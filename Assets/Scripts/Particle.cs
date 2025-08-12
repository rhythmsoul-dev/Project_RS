using UnityEngine;

public class Particle : PoolObject
{
    private ParticleSystem ps;

    [SerializeField] private float time;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public override void OnGet()
    {
        ps.Play(true);
        Invoke(nameof(End), time);
    }

    private void End()
    {
        ps.Stop(true);
        Release();
    }
}
