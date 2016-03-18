using System;
using System.ComponentModel.Composition;

namespace ProjectWerner.Contracts.API
{
    [InheritedExport]
    public interface ICamera3D
    {
        event Action FaceVisible;
        event Action FaceLost;
        event Action MouthOpened;
        event Action MouthClosed;
        bool IsFaceMouthOpen { get; set; }
        void Speech(string message);
    }
}