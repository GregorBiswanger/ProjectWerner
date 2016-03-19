using System;

namespace ProjectWerner.MvvmHelper.Commands
{
	public interface ICommandUpdater : IDisposable
    {
        event EventHandler UpdateOfCanExecuteChangedRequired;
    }
}