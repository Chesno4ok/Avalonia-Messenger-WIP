using Avalonia.Controls;
using AvaloniaMessenger.Controllers;
using AvaloniaMessenger.Models;
using AvaloniaMessenger.Views;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace AvaloniaMessenger.ViewModels;

class MainViewModel : ViewModelBase
{       
    private UserControl _mainView;
    public MessengerController Messenger { get; set; } = new MessengerController(new Uri("https://localhost:7284"));
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
        var viewModel = new SignUpViewModel();

        viewModel.ReturnCommand = ReactiveCommand.Create(() => { SetSignIn(); });

        MainView = new SignUpView { DataContext = viewModel };
    }
}
