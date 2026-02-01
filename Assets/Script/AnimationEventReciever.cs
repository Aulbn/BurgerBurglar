using UnityEngine;

public class AnimationEventReciever : MonoBehaviour
{
    public AudioClip[] FootstepList;
    public Vector2 FootstepRandomPitch = new Vector2(0.9f, 1.1f);
    public AudioSource FootstepSound;

    public void Footstep()
    {
        FootstepSound.clip = FootstepList[Random.Range(0, FootstepList.Length)];
        FootstepSound.pitch = Random.Range(FootstepRandomPitch.x, FootstepRandomPitch.y);
        FootstepSound.Play();
    }
}
