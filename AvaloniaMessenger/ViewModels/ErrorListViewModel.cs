using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaMessenger.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Kernel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace AvaloniaMessenger.ViewModels
{
    class ErrorListViewModel : ViewModelBase
    {
        private AvaloniaList<Error> _errorList = new AvaloniaList<Error>();
        public AvaloniaList<Error> ErrorList 
        { 
            get { return _errorList; }
            set { this.RaiseAndSetIfChanged(ref _errorList, value); } 
        }
        public SourceList<Error> ErrorSourceList = new();
        private List<Error> DeleteQueue = new();
        DispatcherTimer deleteTimer = new DispatcherTimer();
        public ErrorListViewModel()
        {
            ErrorSourceList.Connect().OnItemAdded(i => onAdded(i)).OnItemRemoved(i => onRemoved(i)).Subscribe();

            deleteTimer.Interval = TimeSpan.FromSeconds(0.5);
            deleteTimer.Tick += ClearQueue;
        }

        private void ClearQueue(object? sender, EventArgs e)
        {
            foreach(var i in DeleteQueue)
            {
                ErrorList.Remove(i);
            }
            deleteTimer.IsEnabled = false;
        }
        private void onAdded(Error error)
        {
            ErrorList.Add(error);
        }
        private void onRemoved(Error error)
        {
            error.ErrorPopup.HideAnimation();

            DeleteQueue.Add(error);
            deleteTimer.IsEnabled = true;
        }
        public void AddError(string error)
        {
            if(_errorList.FirstOrDefault(i => i.Text == error) == null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    ErrorSourceList.Add(new Error(error));
                });
            }
        }
        public void RemoveError(string error)
        {
            if (_errorList.FirstOrDefault(i => i.Text == error) != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    ErrorSourceList.Remove(_errorList.FirstOrDefault(i => i.Text == error));
                });
            }
        }
    }

    partial class Error : ObservableObject
    {
        public Error(string errorText)
        {
            ErrorPopupViewModel = new() { ErrorText = errorText };
            ErrorPopup = new ErrorPopupView() { DataContext = ErrorPopupViewModel };
            Text = errorText;

        }

        [ObservableProperty]
        private ErrorPopupView? _errorPopup;

        [ObservableProperty]
        private ErrorPopupViewModel? _errorPopupViewModel;

        public string Text;
    }
}
