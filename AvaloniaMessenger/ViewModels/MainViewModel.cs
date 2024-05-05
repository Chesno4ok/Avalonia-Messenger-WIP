using Avalonia.Controls;
using AvaloniaMessenger.Controllers;
using AvaloniaMessenger.Models;
using AvaloniaMessenger.Views;
using ReactiveUI;
using System;
using AvaloniaMessenger.Controllers;
using System.IO;
using System.Security.Authentication;
using System.Diagnostics;
using System.Reactive.Linq;

namespace AvaloniaMessenger.ViewModels;

class MainViewModel : ViewModelBase
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

    MessengerController Messenger = new MessengerController(new Uri("https://localhost:7284"));

       
    public MainViewModel()
    {   
        SetSignIn();
    }   
    public void SetMessenger(User? user)
    {
        User? userToken = null;
        try
        {
            userToken = Messenger.SignIn(user.Login, user.Password);
        }
        catch
        {

        }
        
        if(userToken != null)
            MainView = new MessengerView();
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
        var viewModel = new SignUpViewModel(Messenger);

        viewModel.ReturnCommand = ReactiveCommand.Create(() => { SetSignIn(); });

        MainView = new SignUpView { DataContext = viewModel };
    }
}
