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
using AvaloniaMessenger.Exceptions;
using System.Threading.Tasks;

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

    MessengerController Messenger { get; set; }

    public MainViewModel()
    {

        Messenger = new MessengerController(new Uri("https://localhost:7284"));
        SetSignIn();
    }   
    public void SetMessenger(User user)
    {
        Messenger.apiCaller.Token = "Bearer " + user.Token;

        var viewModel = new MessengerViewModel(user, Messenger, ReactiveCommand.Create(SetSignIn));

        MainView = new MessengerView() { DataContext = viewModel };
        viewModel.MessengerView = MainView as MessengerView;

        (MainView as MessengerView).InitialiseEvents();
    }
    public void SetSignIn()
    {   
        var viewModel = new SignInViewModel() { Messenger = Messenger };

        viewModel.SetMessengerCommand = ReactiveCommand.Create<User>(user => SetMessenger(user));
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
