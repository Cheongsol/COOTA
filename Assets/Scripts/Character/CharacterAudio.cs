using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    [SerializeField] AudioSource footstepsAudioSource = null;
    [SerializeField] AudioSource jumpingAudioSource = null;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] dirtSteps = null;  // �߰��� ����
    [SerializeField] AudioClip dirtLanding = null;  // ���� ����
    [SerializeField] AudioClip jump = null;         // ���� ����

    [Header("Steps")]
    [SerializeField] float stepsTimeGap = 1f;

    private float stepsTimer;

    public void PlaySteps(GroundType groundType, float speedNormalized)
    {
        if (groundType == GroundType.None) return;

        stepsTimer += Time.fixedDeltaTime * speedNormalized;

        if (stepsTimer >= stepsTimeGap)
        {
            var steps = dirtSteps;
            int index = Random.Range(0, steps.Length);
            footstepsAudioSource?.PlayOneShot(steps[index]);

            stepsTimer = 0;
        }
    }

    public void PlayJump()
    {
        jumpingAudioSource?.PlayOneShot(jump);
    }

    public void PlayLanding(GroundType groundType)
    {
        jumpingAudioSource?.PlayOneShot(dirtLanding);
    }
}
