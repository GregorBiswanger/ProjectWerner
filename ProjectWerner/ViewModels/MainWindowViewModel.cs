using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ProjectWerner.Dto;
using ProjectWerner.Services;

#pragma warning disable 0067

namespace ProjectWerner.ViewModels
{
	internal class MainWindowViewModel
	{
        public ObservableCollection<ExtensionDataSet> Extensions { get; set; }

        private readonly DispatcherTimer _selectionTimer;
		private int _selectedItemIndex;

		public MainWindowViewModel()
		{
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Extensions = ExtensionDataSetSampledata.LoadSampledata();
            }
            else
            {
                var extensionLoader = new ExtensionLoader();
                Extensions = new ObservableCollection<ExtensionDataSet>(extensionLoader.GetExtensions());
                Extensions.First().IsSelected = true;

                _selectionTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(2)
                };

                _selectionTimer.Tick += OnSeletionTimerTick;
                _selectionTimer.Start();
            }
        }

        ~MainWindowViewModel()
        {
            _selectionTimer.Stop();
            _selectionTimer.Tick -= OnSeletionTimerTick;
        }

        private void OnSeletionTimerTick(object sender, EventArgs eventArgs)
		{
			Extensions[_selectedItemIndex].IsSelected = false;

			_selectedItemIndex++;

		    if (_selectedItemIndex == Extensions.Count)
		    {
		        _selectedItemIndex = 0;
		    }

		    Extensions[_selectedItemIndex].IsSelected = true;
		}

	    public void StartSelectedExtension()
	    {
            ExtensionStarter.StartExtension(Extensions[_selectedItemIndex].Extension.AppUserControl);
	    }
	}
}