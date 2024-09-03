using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.ViewModels
{
    class ErrorPopupViewModel : ViewModelBase
    {
        private string? _errorText;
        public string? ErrorText { get => _errorText; set => this.RaiseAndSetIfChanged(ref _errorText, value); }
    }
}
