using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    public void PlayMoveSound()
    {
        if (!PlayerManager.Instance().LocalPlayer.IsLocked)
        {
            SoundManager.Instance().Play(GameConstants.Sound.MOVE);
        }
    }
}
