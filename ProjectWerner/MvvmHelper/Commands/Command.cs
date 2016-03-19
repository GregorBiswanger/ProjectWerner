using System;
using System.Collections.Generic;
using System.Windows.Input;
using ProjectWerner.MvvmHelper.Utils;

namespace ProjectWerner.MvvmHelper.Commands
{
	public class Command : DisposingObject, ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Func<bool> canExecute;
        private readonly Action execute;
        private readonly IReadOnlyList<ICommandUpdater> commandUpdaterList;

        
        public Command(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.commandUpdaterList = null;
        }

        public Command(Action execute, Func<bool> canExecute, params ICommandUpdater[] commandUpdaterList)
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

        public bool CanExecute(object parameter = null)
        {
            if (canExecute == null)
                return true;

            return canExecute();
        }

        public void Execute(object parameter = null)
        {
            execute();
        }        

        public void RaiseCanExecuteChanged()
        {           
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

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
