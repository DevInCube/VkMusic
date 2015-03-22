using My.VKMusic.ViewModels;
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
        private WaveOut waveOutDevice;
        private AudioFileReader audioFileReader;

        public int Position
        {
            get
            {
                return (waveOutDevice != null) ? (int)audioFileReader.CurrentTime.TotalSeconds : 0;
            }
            set
            {
                if (waveOutDevice != null)
                {
                    audioFileReader.CurrentTime = new TimeSpan(0, 0, value);
                    OnPropertyChanged("Position");
                }
            }
        }
     
        public int TotalPosition
        {
            get { return (waveOutDevice != null) ? (int)audioFileReader.TotalTime.TotalSeconds : 1; }           
        }

        public float Volume
        {
            get { return (waveOutDevice == null) ? 1 : waveOutDevice.Volume; }
            set { if (waveOutDevice != null) { waveOutDevice.Volume = value; OnPropertyChanged("Volume"); } }
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
                    OnPropertyChanged("Position");
                    OnPropertyChanged("TotalPosition");
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
