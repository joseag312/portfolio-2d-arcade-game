using Godot;
using System;
using System.Collections.Generic;

public partial class AutoSFX : Node
{
    public static AutoSFX Instance { get; private set; }

    [Export] public int PrewarmVoices { get; set; } = 6;
    [Export] public int MaxVoices { get; set; } = 16;

    private readonly Dictionary<string, AudioStream> _sounds = new();
    private readonly List<AudioStreamPlayer> _pool = new();
    private float _baseVolumeDb = 0f;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;

            _sounds[SFX.OIIA_SLOW] = GD.Load<AudioStream>("res://assets/sounds/oiia_slow.ogg");
            _sounds[SFX.OIIA_FAST] = GD.Load<AudioStream>("res://assets/sounds/oiia_fast.ogg");
            _sounds[SFX.OIIA_DEATH] = GD.Load<AudioStream>("res://assets/sounds/oiia_death.ogg");
            _sounds[SFX.MEOW] = GD.Load<AudioStream>("res://assets/sounds/meow.ogg");
            _sounds[SFX.CLICK] = GD.Load<AudioStream>("res://assets/sounds/click.ogg");
            _sounds[SFX.COIN] = GD.Load<AudioStream>("res://assets/sounds/coin.ogg");
            _sounds[SFX.LASER] = GD.Load<AudioStream>("res://assets/sounds/laser.ogg");
            _sounds[SFX.RAY] = GD.Load<AudioStream>("res://assets/sounds/ray.ogg");
            _sounds[SFX.HEAL] = GD.Load<AudioStream>("res://assets/sounds/heal.ogg");
            _sounds[SFX.EXPLOSION] = GD.Load<AudioStream>("res://assets/sounds/explosion.ogg");
            _sounds[SFX.MISSILE] = GD.Load<AudioStream>("res://assets/sounds/missile.ogg");
            _sounds[SFX.SHIELD] = GD.Load<AudioStream>("res://assets/sounds/shield.ogg");
            _sounds[SFX.TELEPORT] = GD.Load<AudioStream>("res://assets/sounds/teleport.ogg");
            _sounds[SFX.THUD] = GD.Load<AudioStream>("res://assets/sounds/thud.ogg");

            _baseVolumeDb = LinearToDb(G.CF.SfxVolume);

            int sfxBus = AudioServer.GetBusIndex("SFX");
            AudioServer.SetBusMute(sfxBus, G.CF.IsMuted);

            for (int i = 0; i < PrewarmVoices; i++)
                _pool.Add(CreatePlayer());
        }
        else
        {
            QueueFree();
        }
    }

    public void Play(string name, float pitch = 1f, float volumeDbOffset = 0f)
    {
        if (!_sounds.TryGetValue(name, out var stream))
        {
            Log.Err($"Unknown Sound Key '{name}'");
            return;
        }

        var p = GetFreePlayer();
        p.Stream = stream;
        p.PitchScale = pitch;
        p.VolumeDb = _baseVolumeDb + volumeDbOffset;
        p.Play();
    }

    public void PlayExclusive(string name, float pitch = 1f, float volumeDbOffset = 0f)
    {
        StopAll();
        Play(name, pitch, volumeDbOffset);
    }

    public void StopAll()
    {
        foreach (var p in _pool)
            if (IsInstanceValid(p) && p.Playing)
                p.Stop();
    }

    public void SetVolumeDb(float db)
    {
        _baseVolumeDb = db;
        foreach (var p in _pool)
            if (IsInstanceValid(p))
                p.VolumeDb = _baseVolumeDb;
    }

    public void ApplySettingsFromConfig()
    {
        _baseVolumeDb = LinearToDb(G.CF.SfxVolume);
        SetVolumeDb(_baseVolumeDb);
        int sfxBus = AudioServer.GetBusIndex("SFX");
        AudioServer.SetBusMute(sfxBus, G.CF.IsMuted);
    }

    private AudioStreamPlayer CreatePlayer()
    {
        var p = new AudioStreamPlayer
        {
            Bus = "SFX",
            VolumeDb = _baseVolumeDb,
            Autoplay = false
        };

        AddChild(p);
        return p;
    }

    private AudioStreamPlayer GetFreePlayer()
    {
        foreach (var p in _pool)
            if (IsInstanceValid(p) && !p.Playing)
                return p;

        if (_pool.Count < MaxVoices)
        {
            var np = CreatePlayer();
            _pool.Add(np);
            return np;
        }

        return _pool[0];
    }

    private static float LinearToDb(float linear)
    {
        if (linear <= 0.001f) return -80f;
        return 20f * (float)Math.Log10(linear);
    }
}
