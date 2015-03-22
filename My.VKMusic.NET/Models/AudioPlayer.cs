﻿using My.VKMusic.ViewModels;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace My.VKMusic.Models
{
    public class AudioPlayer : ObservableObject, IDisposable
    {

        private AudioFile audio;
        private int _Position, _LoadingPosition, _TotalPosition;
        private WaveOut waveOutDevice;
        private AudioFileReader audioFileReader;

        public int Position
        {
            get { return _Position; }
            set { _Position = value; OnPropertyChanged("Position"); }
        }

        public int LoadingPosition
        {
            get { return _LoadingPosition; }
            set { _LoadingPosition = value; OnPropertyChanged("LoadingPosition"); }
        }
        public int TotalPosition
        {
            get { return _TotalPosition; }
            set { _TotalPosition = value; OnPropertyChanged("TotalPosition"); }
        }

        public AudioPlayer()
        {
            //
        }

        public void Init(AudioFile info)
        {
            PlaybackState state = waveOutDevice == null ? PlaybackState.Stopped : waveOutDevice.PlaybackState;
            Dispose();
            this.audio = info;
            audioFileReader = audio.GetReader();
            waveOutDevice = new WaveOut();
            waveOutDevice.Init(audioFileReader);
            if (state == PlaybackState.Playing) this.Play();
        }
        

        public void Play()
        {
            System.Timers.Timer t = new System.Timers.Timer(500);
            t.AutoReset = true;
            t.Elapsed += (s, e) => {
                try
                {
                    Position = (int)(audioFileReader.CurrentTime.TotalSeconds);
                    TotalPosition = (int)audioFileReader.TotalTime.TotalSeconds;
                }
                catch { }
            };
            t.Start();
            Task.Factory.StartNew(() =>
            {
                audioFileReader = audio.GetReader();                
                waveOutDevice = new WaveOut();                
                waveOutDevice.Init(audioFileReader);                
                waveOutDevice.Play();
                audio.IsPlaying = true;              
            });
        }


        public void Stop()
        {
            if (waveOutDevice != null)
                waveOutDevice.Stop();
            if (audio != null)
                audio.IsPlaying = false;
        }

        public void Pause()
        {
            waveOutDevice.Pause();
            audio.IsPlaying = false;
        }

        public void Dispose()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
                audio.IsPlaying = false;
            }
            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
            if (waveOutDevice != null)
            {
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
        }
    }
}
