using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SFX_Controller : MonoBehaviour
{
    // Enumeración para representar los diferentes tipos de sonido
    public enum SoundType
    {
        ButtonClick,
        ButtonHover,
        Shoot,
        Spawn,
        Pem,
        Bomb,
        Coins,
        Shield,
        Healing,
        ReceiveDamage,
        Death,
        OST1,
    }

    // Clase que representa un sonido
    [System.Serializable]
    public class Sound
    {
        public SoundType soundType;
        public AudioSource audioSource;
        public float volume;
        public bool mute;
    }

    // Lista de sonidos y controles de volumen y silencio
    [Header("Sound Settings")]
    public List<Sound> sounds;

    public Slider masterVolumeSlider;
    public Toggle masterVolumeMuteToggle;

    public Slider musicVolumeSlider;
    public Toggle musicVolumeMuteToggle;

    public Slider sfxVolumeSlider;
    public Toggle sfxVolumeMuteToggle;

    // Diccionario para almacenar los sonidos por tipo
    private Dictionary<SoundType, Sound> soundDictionary;

    // Singleton para la instancia de este controlador
    public static SFX_Controller instance;

    // Destruir la nueva instancia que se está intentando crear. 
    //Esto asegurará que siempre haya una sola instancia del controlador FX.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        soundDictionary = new Dictionary<SoundType, Sound>();
        foreach (var sound in sounds)
        {
            soundDictionary.Add(sound.soundType, sound);
        }
    }


    private void Start()
    {
        // Inicializa el diccionario y agrega todos los sonidos a él
        soundDictionary = new Dictionary<SoundType, Sound>();
        foreach (var sound in sounds)
        {
            soundDictionary.Add(sound.soundType, sound);
        }

        // Agrega los listeners a los controles de volumen y silencio
        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
        masterVolumeMuteToggle.onValueChanged.AddListener(UpdateMasterMute);

        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        musicVolumeMuteToggle.onValueChanged.AddListener(UpdateMusicMute);

        sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
        sfxVolumeMuteToggle.onValueChanged.AddListener(UpdateSFXMute);

        // Si hay una instancia de Manager_Controller, inicia la música
        if (Manager_Controller.instance != null)
        {
            PlaySound(SoundType.OST1);
        }

        // Carga las configuraciones de sonido guardadas
        LoadSoundSettings();
    }

    // Actualiza el volumen maestro y guarda el valor
    private void UpdateMasterVolume(float value)
    {
        foreach (var sound in soundDictionary.Values)
        {
            // Verifica si la música o los efectos de sonido están muteados antes de ajustar el volumen
            if ((sound.soundType != SoundType.OST1 || !musicVolumeMuteToggle.isOn) &&
                (sound.soundType == SoundType.OST1 || !sfxVolumeMuteToggle.isOn))
            {
                sound.audioSource.volume = value * sound.volume;
            }
        }

        // Actualiza los sliders de volumen de música y SFX
        musicVolumeSlider.value = value;
        sfxVolumeSlider.value = value;

        // Guarda el valor del volumen maestro
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    // Actualiza el estado de mute maestro y guarda el valor
    private void UpdateMasterMute(bool value)
    {
        foreach (var sound in soundDictionary.Values)
        {
            // Verifica si la música o los efectos de sonido están muteados antes de ajustar el estado de mute
            if ((sound.soundType != SoundType.OST1 || !musicVolumeMuteToggle.isOn) &&
                (sound.soundType == SoundType.OST1 || !sfxVolumeMuteToggle.isOn))
            {
                sound.audioSource.mute = value || sound.mute;
            }
        }

        // Actualiza los estados de mute de música y SFX
        musicVolumeMuteToggle.isOn = value;
        sfxVolumeMuteToggle.isOn = value;

        // Guarda el estado de mute maestro
        PlayerPrefs.SetInt("MasterMute", value ? 1 : 0);
    }

    // Actualiza el volumen de la música y guarda el valor
    private void UpdateMusicVolume(float value)
    {
        var music = soundDictionary[SoundType.OST1];
        music.audioSource.volume = value * music.volume;

        // Guarda el valor del volumen de la música
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    // Actualiza el estado de mute de la música y guarda el valor
    private void UpdateMusicMute(bool value)
    {
        var music = soundDictionary[SoundType.OST1];
        music.audioSource.mute = value || music.mute;

        // Guarda el estado de mute de la música
        PlayerPrefs.SetInt("MusicMute", value ? 1 : 0);
    }

    // Actualiza el volumen de los efectos de sonido (SFX) y guarda el valor
    private void UpdateSFXVolume(float value)
    {
        foreach (var sound in soundDictionary.Values)
        {
            if (sound.soundType != SoundType.OST1)
            {
                sound.audioSource.volume = value * sound.volume;
            }
        }

        // Guarda el valor del volumen de los efectos de sonido (SFX)
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    // Actualiza el estado de mute de los efectos de sonido (SFX) y guarda el valor
    private void UpdateSFXMute(bool value)
    {
        foreach (var sound in soundDictionary.Values)
        {
            if (sound.soundType != SoundType.OST1)
            {
                sound.audioSource.mute = value || sound.mute;
            }
        }

        // Guarda el estado de mute de los efectos de sonido
        PlayerPrefs.SetInt("SFXMute", value ? 1 : 0);
    }

    // Reproduce un sonido
    public void PlaySound(SoundType soundType)
    {
        soundDictionary[soundType].audioSource.Play();
    }

    // Detiene un sonido
    public void StopSound(SoundType soundType)
    {
        soundDictionary[soundType].audioSource.Stop();
    }

    // Carga las configuraciones de sonido guardadas
    private void LoadSoundSettings()
    {
        // Carga los valores guardados y actualiza los controles
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            masterVolumeSlider.value = masterVolume;
            UpdateMasterVolume(masterVolume);
        }

        if (PlayerPrefs.HasKey("MasterMute"))
        {
            int masterMute = PlayerPrefs.GetInt("MasterMute");
            masterVolumeMuteToggle.isOn = masterMute == 1;
            UpdateMasterMute(masterMute == 1);
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicVolumeSlider.value = musicVolume;
            UpdateMusicVolume(musicVolume);
        }

        if (PlayerPrefs.HasKey("MusicMute"))
        {
            int musicMute = PlayerPrefs.GetInt("MusicMute");
            musicVolumeMuteToggle.isOn = musicMute == 1;
            UpdateMusicMute(musicMute == 1);
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            sfxVolumeSlider.value = sfxVolume;
            UpdateSFXVolume(sfxVolume);
        }

        if (PlayerPrefs.HasKey("SFXMute"))
        {
            int sfxMute = PlayerPrefs.GetInt("SFXMute");
            sfxVolumeMuteToggle.isOn = sfxMute == 1;
            UpdateSFXMute(sfxMute == 1);
        }
    }

}
