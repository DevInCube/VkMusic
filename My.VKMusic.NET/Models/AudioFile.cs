using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.VKMusic.Models
{
    public class AudioFile : IDisposable
    {

        private AudioFileInfo audioInfo;

        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;

        public AudioFile(AudioFileInfo info)
        {
            this.audioInfo = info;
        }

        public void Play()
        {
            waveOutDevice = new WaveOut();
            audioFileReader = new AudioFileReader(audioInfo.URL);
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
        }

        public void Dispose()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
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
