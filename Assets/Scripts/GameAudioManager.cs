// GameAudioManager.cs — Complete Superhero Audio System
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameAudioManager : MonoBehaviour
{
    // Background music
    [Header("Background Music")]
    public AudioClip togetherWeStand;
    public AudioClip heroMusic;

    // Narrator dialogues
    [Header("Narrator Dialogues")]
    public AudioClip intro;
    public AudioClip[] hitDialogues;
    public AudioClip[] missDialogues;
    public AudioClip trainingComplete;
    public AudioClip finale;
    public AudioClip afterGoodbye;

    // Game settings
    [Header("Game Settings")]
    public int requiredHits = 5;

    // Reference existing trial stopwatch
    public TrialHandler trialHandler;

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
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;

        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;
        voiceSource.loop = false;
        voiceSource.spatialBlend = 0f;
        voiceSource.volume = 1f;
    }

    void Start()
    {
        // Start superhero music immediately
        if (heroMusic != null)
        {
            musicSource.clip = heroMusic;
            musicSource.volume = 0.2f;
            musicSource.Play();
        }

        // Play welcome dialogue after 2 seconds
        StartCoroutine(DelayedIntro());

        // Stopwatch integration remains disabled for now
        // if (trialHandler != null && !trialHandler.trialActive)
        //     trialHandler.StartTrial();
    }

    IEnumerator DelayedIntro()
    {
        yield return new WaitForSeconds(2f);
        PlayDialogue(intro);
    }

    // Optional gesture-triggered music switching
    public void OnPalmOpened()
    {
        if (heroMusicStarted || trainingFinished || heroMusic == null)
            return;

        heroMusicStarted = true;

        musicSource.Stop();
        musicSource.clip = heroMusic;
        musicSource.volume = voiceSource.isPlaying ? 0.07f : 0.4f;
        musicSource.Play();
    }

    // Called when a target is successfully hit
    public void OnTargetHit()
    {
        if (trainingFinished)
        {
            TryPlayAfterGoodbye();
            return;
        }

        hitCount++;
        Debug.Log("Target Hits: " + hitCount);

        if (hitDialogues != null && hitDialogues.Length > 0)
        {
            int index = (hitCount - 1) % hitDialogues.Length;
            PlayDialogue(hitDialogues[index]);
        }
    }

    // Called when a snowball misses its target
    public void OnTargetMissed()
    {
        if (trainingFinished)
        {
            TryPlayAfterGoodbye();
            return;
        }

        if (missDialogues == null || missDialogues.Length == 0)
            return;

        int index = missCount % missDialogues.Length;
        missCount++;

        PlayDialogue(missDialogues[index]);
    }

    // Step 2 — Detect missed snowball throws
    // Called by SnowballSpawnable when a ball is released
    public void OnSnowballThrown()
    {
        StartCoroutine(CheckForMiss());
    }

    private IEnumerator CheckForMiss()
    {
        int hitsBeforeThrow = hitCount;

        // Allow time for the snowball to reach a target
        yield return new WaitForSeconds(3f);

        // No successful hit registered during this period
        if (!trainingFinished && hitCount == hitsBeforeThrow)
        {
            OnTargetMissed();
        }
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
        if (clip == null)
            return;

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

        if (musicSource.isPlaying)
            musicSource.volume = heroMusicStarted ? 0.4f : 0.2f;

        dialogueRoutine = null;
    }

    // Trigger victory after all four layouts
    public void OnAllLayoutsCompleted()
    {
        if (trainingFinished)
            return;

        trainingFinished = true;
        StartDialogueSequence();
    }

    // Start Training Complete and Goodbye dialogues
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

        // Training Complete dialogue
        if (trainingComplete != null)
        {
            voiceSource.clip = trainingComplete;
            voiceSource.Play();

            yield return new WaitForSeconds(trainingComplete.length);
        }

        // Goodbye dialogue
        if (finale != null)
        {
            voiceSource.clip = finale;
            voiceSource.Play();

            yield return new WaitForSeconds(finale.length);
        }

        goodbyeFinished = true;

        // Fade out music
        float startVolume = musicSource.volume;
        float fadeDuration = 2f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            musicSource.volume = Mathf.Lerp(
                startVolume,
                0f,
                Mathf.Clamp01(elapsed / fadeDuration)
            );

            yield return null;
        }

        musicSource.Stop();
        dialogueRoutine = null;
    }

    // Temporary keyboard controls for testing
    void Update()
    {
        if (Keyboard.current == null)
            return;

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