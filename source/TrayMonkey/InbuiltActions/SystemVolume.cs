using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreAudioApi;

namespace TrayMonkey.InbuiltActions
{
    public interface ISystemVolume
    {
        float SetVolume(float volume);
    }
    
    public class SystemVolume : ISystemVolume
    {
        public float SetVolume(float volume)
        {
            var enumerator = new MMDeviceEnumerator();
            var defaultDeviceEndpoint = enumerator
                .GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume;
            var channels = new List<AudioEndpointVolumeChannel>();
            for (var i = 0; i < defaultDeviceEndpoint.Channels.Length; i++)
                channels.Add(defaultDeviceEndpoint.Channels[i]);
            var originalVolume = channels.Select(c => c.VolumeLevelScalar).Max();
            Debug.WriteLine($"Setting system volume to: {volume}");
            Debug.WriteLine($"Saving restore volume of: {originalVolume}");
            foreach (var channel in channels)
                channel.VolumeLevelScalar = volume;
            return originalVolume;
        }
    }
}