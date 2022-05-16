using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Windows.Media.Capture;

using FluentSpeaker.Helpers;
using FluentSpeaker.UWP.Classes;

[assembly: Dependency(typeof(UWPMicrophoneService))]
namespace FluentSpeaker.UWP.Classes
{
    public class UWPMicrophoneService : IMicrophoneService
    {
        public UWPMicrophoneService()
        {
        }

        public async Task<bool> GetPermissionAsync()
        {
            bool isMicAvailable = true;
            try
            {
                var mediaCapture = new MediaCapture();
                var settings = new MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                await mediaCapture.InitializeAsync(settings);
            }
            catch (Exception ex)
            {
                isMicAvailable = false;
            }

            if (!isMicAvailable)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone"));
            }

            return isMicAvailable;
        }

        public void OnRequestPermissionResult(bool isGranted)
        {
            // intentionally does nothing
        }
    }
}