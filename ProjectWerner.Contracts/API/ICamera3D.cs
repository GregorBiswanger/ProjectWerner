using System;
using System.ComponentModel.Composition;
using SharpSenses;

namespace ProjectWerner.Contracts.API
{
    [InheritedExport]
    public interface ICamera3D
    {
        event Action FaceVisible;
        event Action FaceLost;
        event Action MouthVisible;
        event Action MouthLost;
        event Action MouthOpened;
        event Action MouthClosed;
        event Action<PositionEventArgs> MouthMoved;
        event Action<byte[]> NewImageAvailable;
        bool IsFaceMouthOpen { get; set; }

        /// <summary>
        /// Only works with intel real sense
        /// how much the mouth has to be open to fire open event
        /// 0 closed - 100 open
        /// </summary>
        int MouthOpenValue { get; set; }

        void OnMouthOpened(object sender, EventArgs e);
        void OnMouthClosed(object sender, EventArgs e);
        void OnFaceLost(object sender, EventArgs e);
        void OnFaceVisible(object sender, EventArgs e);
        void Speech(string message);

        /// <summary>
        /// called when camera is connected
        /// camera can be connected after application start
        /// </summary>
        event Action Connected;
    }
}