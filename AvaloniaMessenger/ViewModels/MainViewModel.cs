using Avalonia.Controls;
using AvaloniaMessenger.Views;
using ReactiveUI;
using AvaloniaMessenger.Models;
using System.Reactive.Linq;
using System.Diagnostics;
using System;

namespace AvaloniaMessenger.ViewModels;

public class MainViewModel : ViewModelBase
{       
    private UserControl _mainView;
    public UserControl MainView 
    {   
        get 
        { 
            return _mainView; 
        }
        private set
        {
            this.RaiseAndSetIfChanged(ref _mainView, value);
        }
    }   
    public MainViewModel()
    {   
        SetSignIn();
    }   
    public void SetMessenger(User user)
    {
        Debug.Print(user.Name);
    }
    public void SetSignIn()
    {   
        var viewModel = new SignInViewModel();

        viewModel.SignInCommand.Subscribe(user => SetMessenger(user));
        viewModel.SignUpCommand = ReactiveCommand.Create(() => { SetSignUp(); });

        MainView = new SignInView { DataContext = viewModel };
    }   
    public void SetSignUp()
    {
        var viewModel = new SignUpViewModel();

        viewModel.ReturnCommand = ReactiveCommand.Create(() => { SetSignIn(); });

        MainView = new SignUpView { DataContext = viewModel };
    }
}
