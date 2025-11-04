using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] public AudioSource sourceBgm;
    [SerializeField] public AudioSource sourceSfx;

    private readonly Dictionary<string, AudioClip> audioClipCache = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- MAIN PLAYBACK METHODS --- //

    public async void PlaySfx(string address)
    {
        AudioClip clip = await LoadClipAsync(address);
        if (clip == null)
        {
            LogManager.Error($"[AudioManager] Failed to load clip at address '{address}'");
            return;
        }

        var src = sourceSfx;
        src.spatialBlend = 0f;
        src.PlayOneShot(clip);
    }

    public async void PlayBgm(string address)
    {
        AudioClip clip = await LoadClipAsync(address);
        if (clip == null)
        {
            LogManager.Error($"[AudioManager] Failed to load clip at address '{address}'");
            return;
        }

        var src = sourceBgm;
        src.spatialBlend = 0f;
        src.clip = clip;
        src.Play();
    }

    // --- ADDRESSABLE LOADING --- //

    private async Task<AudioClip> LoadClipAsync(string address)
    {
        if (audioClipCache.TryGetValue(address, out var cached))
            return cached;

        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(address);
        AudioClip clip = await handle.Task;

        if (clip != null)
        {
            audioClipCache[address] = clip;
        }

        return clip;
    }

    public void UnloadClip(string address)
    {
        if (audioClipCache.TryGetValue(address, out var clip))
        {
            Addressables.Release(clip);
            audioClipCache.Remove(address);
        }
    }

    public void ClearAllCachedClips()
    {
        foreach (var kvp in audioClipCache)
            Addressables.Release(kvp.Value);

        audioClipCache.Clear();
    }
}
