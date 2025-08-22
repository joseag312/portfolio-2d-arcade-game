using Godot;
using System;
using System.Collections.Generic;

public partial class AutoMusic : Node
{
    public static AutoMusic Instance { get; private set; }

    private const float TrackLengthSeconds = 19.0f;

    private AudioStreamPlayer _musicPlayer;
    private Dictionary<string, TrackData> _trackRegistry = new();
    private string _defaultTrackPath = "res://assets/music/soundtrack.ogg";
    private float _defaultTrackLength = 19.0f;
    private bool _isPlaying = false;
    private Timer _loopTimer;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;

            if (_musicPlayer == null) _musicPlayer = new AudioStreamPlayer();
            _musicPlayer.Name = "MusicPlayer";
            _musicPlayer.Bus = "Music";
            _musicPlayer.VolumeDb = LinearToDb(G.CF.MasterVolume);
            AddChild(_musicPlayer);

            var musicIdx = AudioServer.GetBusIndex("Music");
            _musicPlayer.Bus = musicIdx >= 0 ? "Music" : "Master";

            _trackRegistry[Music.MAIN] = new TrackData("res://assets/music/soundtrack.ogg", 19.0f, 0.75f);
            _trackRegistry[Music.AMAZING] = new TrackData("res://assets/music/level_amazing.ogg", 137.0f, 0.75f);
            _trackRegistry[Music.SNEAKY] = new TrackData("res://assets/music/level_amazing.ogg", 118.0f, 0.75f);
            _trackRegistry[Music.ANGRY] = new TrackData("res://assets/music/level_angry.ogg", 125.0f, 0.75f);
            _trackRegistry[Music.FINAL] = new TrackData("res://assets/music/level_final.ogg", 134.0f, 0.75f);
            _trackRegistry[Music.FADE] = new TrackData("res://assets/music/fade.ogg", 157.0f, 0.75f);

            SetupLoopTimer();
            SetVolumeDb(LinearToDb(G.CF.MasterVolume));
            AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), G.CF.IsMuted);
        }
        else
        {
            QueueFree();
        }
    }

    public void Stop()
    {
        if (!_isPlaying)
            return;

        _musicPlayer.Stop();
        _loopTimer.Stop();
        _isPlaying = false;
    }

    public void PlayTrack(string trackKey)
    {
        if (!_trackRegistry.TryGetValue(trackKey, out var track))
        {
            Log.Err($"Unknown Music Key '{trackKey}'");
            return;
        }

        PlayTrack(track.Path, track.Length, track.FadeDuration);
    }

    public void PlayTrack(string resourcePath, float trackLength, float fadeDuration = 0.5f)
    {
        if (_isPlaying && _musicPlayer.Stream.ResourcePath == resourcePath)
            return;

        Stop();

        var newStream = GD.Load<AudioStream>(resourcePath);
        if (newStream == null)
        {
            Log.Err($"Could not load Music track at '{resourcePath}'");
            return;
        }

        _musicPlayer.Stream = newStream;
        _musicPlayer.VolumeDb = -80;
        _musicPlayer.Bus = "Music";
        _musicPlayer.Play();

        _loopTimer.WaitTime = trackLength;
        _loopTimer.Start();

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_musicPlayer, "volume_db", LinearToDb(G.CF.MasterVolume), fadeDuration);

        _isPlaying = true;
    }

    private static float LinearToDb(float linear)
    {
        if (linear <= 0.001f)
            return -80f;
        return 20f * (float)Math.Log10(linear);
    }

    private void SetupLoopTimer()
    {
        _loopTimer = new Timer
        {
            WaitTime = TrackLengthSeconds,
            OneShot = false,
            Autostart = false,
        };
        _loopTimer.ProcessMode = ProcessModeEnum.Always;
        AddChild(_loopTimer);

        _loopTimer.Timeout += () =>
        {
            if (!_musicPlayer.Playing)
                _musicPlayer.Play();
        };
    }

    public void SetVolumeDb(float volumeDb)
    {
        _musicPlayer.VolumeDb = volumeDb;
    }

    public void FadeOut(float duration = 1.0f)
    {
        if (!_isPlaying)
            return;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_musicPlayer, "volume_db", -80, duration);
        tween.TweenCallback(Callable.From(() =>
        {
            _isPlaying = false;
            _loopTimer.Stop();
        }));
    }

    public void FadeIn(float duration = 1.0f)
    {
        if (_isPlaying)
            return;

        _isPlaying = true;
        _musicPlayer.VolumeDb = -80;
        _musicPlayer.Play();
        _loopTimer.Start();

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_musicPlayer, "volume_db", LinearToDb(G.CF.MasterVolume), duration);
    }

    private class TrackData
    {
        public string Path;
        public float Length;
        public float FadeDuration;

        public TrackData(string path, float length, float fade = 0.5f)
        {
            Path = path;
            Length = length;
            FadeDuration = fade;
        }
    }
}
