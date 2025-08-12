using UnityEngine;

[RequireComponent(typeof(PlayerDetector))]
public class HealObject : InteractableObject
{
    private PlayerDetector detector;
    private Vector3 portalPosition;
    private ParticleSystem[] particles;
    
    protected override void Init()
    {
        ObjectType = ObjectType.Heal;
        outLine = transform.FindComponent<SpriteRenderer>("OutLine");
        detector = GetComponent<PlayerDetector>();
        detector.Detected.Changed = DetectPlayer;
        particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            particle.Stop();
        }
    }

    public override void OnInteract()
    {
        SoundManager.Instance().Play(GameConstants.Sound.USE_PORTAL);
        
        PlayerManager.Instance().LocalContext.Stats.RefillPotion();
        
        PlayerManager.Instance().FullHeal();
    }
    
    private void DetectPlayer(bool detect)
    {
        switch (detect)
        {
            case true:
                foreach (var particle in particles)
                {
                    particle.Play();
                }
                break;
            case false:
                foreach (var particle in particles)
                {
                    particle.Stop();
                }
                break;
        }
    }
}
