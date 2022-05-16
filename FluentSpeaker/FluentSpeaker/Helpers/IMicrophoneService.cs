using System.Threading.Tasks;

namespace FluentSpeaker.Helpers
{
    public interface IMicrophoneService
    {
        Task<bool> GetPermissionAsync();
        void OnRequestPermissionResult(bool isGranted);
    }
}
