
 // GameAudioManager.cs — Complete Superhero Audio System
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameAudioManager : MonoBehaviour
{
    // Background music
    [Header("Background Music")]
    public AudioClip togetherWeStand;   // Music during intro
    public AudioClip heroMusic;         // Music after palm opens

    // Narrator dialogues
    [Header("Narrator Dialogues")]
    public AudioClip intro;             // Welcome dialogue
    public AudioClip[] hitDialogues;    // Three successful-hit dialogues
    public AudioClip[] missDialogues;   // Three missed-shot dialogues
    public AudioClip trainingComplete;  // Played after 5 successful hits
    public AudioClip finale;            // Goodbye dialogue
    public AudioClip afterGoodbye;      // One final remark after Goodbye

    // Game settings
    [Header("Game Settings")]
    public int requiredHits = 5;

    // Audio players
    private AudioSource musicSource;
    private AudioSource voiceSource;
    private Coroutine dialogueRoutine;

    // Game state
    private int hitCount = 0;
    private int missCount = 0;
    private bool heroMusicStarted = false;
    private bool trainingFinished = false;
    private bool goodbyeFinished = false;
    private bool afterGoodbyePlayed = false;

    // Teammates can read these values for the billboard
    public bool TrainingFinished => trainingFinished;
    public int HitCount => hitCount;

    void Awake()
    {
        // Create background music player
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;

        // Create separate dialogue player
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;
        voiceSource.loop = false;
        voiceSource.spatialBlend = 0f;
        voiceSource.volume = 1f;
    }

    void Start()
    {
        // Start Together We Stand quietly
        if (togetherWeStand != null)
        {
            musicSource.clip = togetherWeStand;
            musicSource.volume = 0.2f;
            musicSource.Play();
        }

        // Play welcome dialogue
        // Step 11 — Delay welcome dialogue by 3 seconds
        StartCoroutine(DelayedIntro());
        // Step 11 — Play welcome dialogue after music intro
        IEnumerator DelayedIntro()
        {
            yield return new WaitForSeconds(2f);
            PlayDialogue(intro);
        }
    }

    // Called by teammate when player opens palm
    public void OnPalmOpened()
    {
        if (heroMusicStarted || trainingFinished || heroMusic == null)
            return;

        heroMusicStarted = true;

        musicSource.Stop();
        musicSource.clip = heroMusic;

        // Keep music quiet if dialogue is playing
        musicSource.volume = voiceSource.isPlaying ? 0.07f : 0.4f;
        musicSource.Play();
    }

    // Called when snowball hits the target
    public void OnTargetHit()
    {
        // After training, allow one final bonus dialogue
        if (trainingFinished)
        {
            TryPlayAfterGoodbye();
            return;
        }

        hitCount++;
        Debug.Log("Target Hits: " + hitCount);

        // Player reaches winning score
        if (hitCount >= requiredHits)
        {
            trainingFinished = true;
            StartDialogueSequence();
            return;
        }

        // Rotate through successful-hit dialogues
        if (hitDialogues != null && hitDialogues.Length > 0)
        {
            int index = (hitCount - 1) % hitDialogues.Length;
            PlayDialogue(hitDialogues[index]);
        }
    }

    // Called when snowball misses the target
    public void OnTargetMissed()
    {
        // After training, allow one final bonus dialogue
        if (trainingFinished)
        {
            TryPlayAfterGoodbye();
            return;
        }

        // Rotate through missed-shot dialogues
        if (missDialogues == null || missDialogues.Length == 0)
            return;

        int index = missCount % missDialogues.Length;
        missCount++;

        PlayDialogue(missDialogues[index]);
    }

    // Play bonus dialogue once, only after Goodbye ends
    void TryPlayAfterGoodbye()
    {
        if (!goodbyeFinished || afterGoodbyePlayed)
            return;

        afterGoodbyePlayed = true;
        PlayDialogue(afterGoodbye);
    }

    // Starts a regular dialogue
    void PlayDialogue(AudioClip clip)
    {
        if (clip == null) return;

        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        dialogueRoutine = StartCoroutine(PlayVoice(clip));
    }

    // Lower music while narrator speaks
    IEnumerator PlayVoice(AudioClip clip)
    {
        voiceSource.Stop();
        voiceSource.clip = clip;

        if (musicSource.isPlaying)
            musicSource.volume = 0.07f;

        voiceSource.Play();

        yield return new WaitForSeconds(clip.length);

        // Restore music volume, unless training has ended
        if (musicSource.isPlaying)
            musicSource.volume = heroMusicStarted ? 0.4f : 0.2f;

        dialogueRoutine = null;
    }

    // Start training-complete and Goodbye dialogues
    void StartDialogueSequence()
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        dialogueRoutine = StartCoroutine(CompleteTraining());
    }

    IEnumerator CompleteTraining()
    {
        voiceSource.Stop();
        musicSource.volume = 0.07f;

        // First: Training Complete dialogue
        if (trainingComplete != null)
        {
            voiceSource.clip = trainingComplete;
            voiceSource.Play();

            yield return new WaitForSeconds(trainingComplete.length);
        }

        // Second: Goodbye dialogue
        if (finale != null)
        {
            voiceSource.clip = finale;
            voiceSource.Play();

            yield return new WaitForSeconds(finale.length);
        }

        // Mark Goodbye as completed
        goodbyeFinished = true;

        // Fade out music smoothly
        float startVolume = musicSource.volume;
        float fadeDuration = 2f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(
                startVolume, 0f,
                Mathf.Clamp01(elapsed / fadeDuration)
            );

            yield return null;
        }

        musicSource.Stop();
        dialogueRoutine = null;
    }

    // Temporary keyboard controls for AudioTestScene
    void Update()
    {
        if (Keyboard.current == null) return;

        // P = Simulate opening palm
        if (Keyboard.current.pKey.wasPressedThisFrame)
            OnPalmOpened();

        // H = Simulate successful hit
        if (Keyboard.current.hKey.wasPressedThisFrame)
            OnTargetHit();

        // M = Simulate missed shot
        if (Keyboard.current.mKey.wasPressedThisFrame)
            OnTargetMissed();
    }
}
