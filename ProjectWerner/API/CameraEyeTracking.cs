using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectWerner.Contracts.API;

namespace ProjectWerner.API
{
    public class CameraEyeTracking : ICamera3D
    {
        public bool IsFaceMouthOpen
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public event Action FaceLost;
        public event Action FaceVisible;
        public event Action MouthClosed;
        public event Action MouthOpened;

        public void Speech(string message)
        {
            throw new NotImplementedException();
        }
    }
}
