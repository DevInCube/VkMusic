using My.VKMusic.ViewModels;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.VKMusic.Models
{
    public class AudioPlayer : IDisposable
    {

        private AudioFile audio;

        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;

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
            waveOutDevice.Play();
            audio.IsPlaying = true;
        }

        public void Stop()
        {
            waveOutDevice.Stop();
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
