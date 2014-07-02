using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreAudioApi;
using NUnit.Framework;

namespace TrayMonkey.Tests
{
    public class TestVolume
    {
        [Test]
        [Ignore("Run manually")]
        public void SimpleTest()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var enumerator = new MMDeviceEnumerator();
            var defaultDeviceEndpoint = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume;
            var leftChannel =  defaultDeviceEndpoint.Channels[0];
            var rightChannel = defaultDeviceEndpoint.Channels[1];
            var startedWith = leftChannel.VolumeLevelScalar;
            Debug.WriteLine(startedWith);
            leftChannel.VolumeLevelScalar = 0.5f;
            rightChannel.VolumeLevelScalar = 0.5f;

            //---------------Test Result -----------------------
        }
        [Test]
        [Ignore("Run manually")]
        public void GNARLY()
        {
                //---------------Set up test pack-------------------

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var enumerator = new MMDeviceEnumerator();
                var defaultDeviceEndpoint =
                    enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume;
                var leftChannel = defaultDeviceEndpoint.Channels[0];
                var rightChannel = defaultDeviceEndpoint.Channels[1];
                var startedWith = leftChannel.VolumeLevelScalar;
                Debug.WriteLine(startedWith);
                Action<float, float> setVolTo = (l, r) =>
                {
                    leftChannel.VolumeLevelScalar = l;
                    rightChannel.VolumeLevelScalar = r;
                };
                setVolTo(0, 1);
            try
            {

                var left = 0f;
                var right = 1f;
                var delta = 0.1f;
                var interval = 50;
                var totalTime = 10000;
                for (var i = 0; i < (totalTime/interval); i++)
                {
                    Debug.WriteLine("left/right/delta: {0} / {1} / {2}", left, right, delta);
                    left += delta;
                    if (left < 0.05 || left > 0.95)
                    {
                        delta = -1*delta;
                        left += 2 * delta;
                    }
                    right -= delta;
                    setVolTo(left, right);
                    Thread.Sleep(interval);
                }
            }
            catch (Exception)
            {
                setVolTo(0.6f, 0.6f);
                throw;
            }

            //---------------Test Result -----------------------
        }

    }
}
