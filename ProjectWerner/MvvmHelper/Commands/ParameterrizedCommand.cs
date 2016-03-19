using System;
using System.Collections.Generic;
using System.Windows.Input;
using ProjectWerner.MvvmHelper.Utils;

namespace ProjectWerner.MvvmHelper.Commands
{
	public class ParameterrizedCommand<T> : DisposingObject, ICommand
    {
        private readonly Predicate<T> canExecute;
        private readonly Action<T> execute;
        private readonly IReadOnlyList<ICommandUpdater> commandUpdaterList;

        public ParameterrizedCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.commandUpdaterList = null;
        }

        public ParameterrizedCommand(Action<T> execute, Predicate<T> canExecute, params ICommandUpdater[] commandUpdaterList)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.commandUpdaterList = commandUpdaterList;

            foreach (var commandUpdater in commandUpdaterList)
            {
                commandUpdater.UpdateOfCanExecuteChangedRequired += CanExecuteChangedRequired;
            }
        }

        private void CanExecuteChangedRequired(object sender, EventArgs eventArgs)
        {
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (!(parameter is T))
                return false;

            if (canExecute == null)
                return true;

            return canExecute((T)parameter);
        }

        public bool CanExecute(T parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute((parameter == null) ? default(T) : (T)parameter);
        }

        public void Execute(T parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        protected override void CleanUp()
        {
            if (commandUpdaterList != null)
            {
                foreach (var commandUpdater in commandUpdaterList)
                {
                    commandUpdater.UpdateOfCanExecuteChangedRequired -= CanExecuteChangedRequired;
                    commandUpdater.Dispose();
                }
            }
        }
    }
}

