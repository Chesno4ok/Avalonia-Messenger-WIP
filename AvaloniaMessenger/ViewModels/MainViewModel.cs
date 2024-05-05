using Avalonia.Controls;
using AvaloniaMessenger.Views;
using ReactiveUI;
using AvaloniaMessenger.Models;
using System.Reactive.Linq;
using System.Diagnostics;
using System;
using AvaloniaMessenger.Controllers;
using System.IO;
using System.Security.Authentication;

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

    MessengerController Messenger = new MessengerController(new Uri("https://localhost:7284"));

       
    public MainViewModel()
    {   
        SetSignIn();
    }   
    public void SetMessenger(User? user)
    {
        if (user == null)
        {
            throw new InvalidCredentialException();
        }

        User? regUser = Messenger.SignIn(user.Login, user.Password);

        if (regUser == null)
            return;

        var viewModel = new MessengerViewModel(regUser);
        MainView = new MessengerView { DataContext = viewModel };
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
