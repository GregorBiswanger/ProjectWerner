using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ProjectWerner.MvvmHelper.Utils;

namespace ProjectWerner.MvvmHelper.Commands
{
	public class PropertyChangedCommandUpdater : DisposingObject, ICommandUpdater
	{
		public event EventHandler UpdateOfCanExecuteChangedRequired;

	    private readonly INotifyPropertyChanged notifyingObject;
	    private readonly IReadOnlyList<string> properties;  

       		
		public PropertyChangedCommandUpdater (INotifyPropertyChanged notifyingObject, params string[] properties)
		{
		    this.notifyingObject = notifyingObject;
		    this.properties = properties;
            
            notifyingObject.PropertyChanged += OnPropertyChanged;
        }

		
		private void OnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{			
			if (properties.Contains(propertyChangedEventArgs.PropertyName))
			{
				UpdateOfCanExecuteChangedRequired?.Invoke(this, new EventArgs());
			}
		}
        
	    protected override void CleanUp()
	    {
            notifyingObject.PropertyChanged -= OnPropertyChanged;
        }	    
	}
}