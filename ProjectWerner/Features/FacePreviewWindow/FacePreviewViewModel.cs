using Ninject;
using ProjectWerner.Contracts.API;
using ProjectWerner.ServiceLocator;
using PropertyChanged;

namespace ProjectWerner.Features.FacePreviewWindow
{
    [ImplementPropertyChanged]
    public class FacePreviewViewModel
    {
        public byte[] ImageStream { get; set; }

        public FacePreviewViewModel()
        {
            var camera3D = MicroKernel.Kernel.Get<ICamera3D>();
            camera3D.NewImageAvailable += UpdateImage;
        }

        private void UpdateImage(byte[] imageToUpdate)
        {
            ImageStream = imageToUpdate;
        }
    }
}
