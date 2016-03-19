using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace ProjectWerner.Contracts.API
{
    [InheritedExport]
    public interface IEyeTracking
    {
        event Action<Point> EyePositionUpdate;
        event Action LeftEyeBlinked;
        event Action RightEyeBlinked;
    }
}
