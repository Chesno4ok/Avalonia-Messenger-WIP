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

    // 
    public MainViewModel()
    {
        var settings = Settings.GetInstance();

        Messenger = new MessengerController(new Uri(Settings.GetInstance().ConnectionString));

        if(settings.ApiKey == null)
        {
            SetSignIn();
            return;
        }

        User? user = new User();
        Messenger.apiCaller.Token = "Bearer " + settings.ApiKey;
        user = Messenger.GetMe();

        if (user is null)
        {
            SetSignIn();
            return;
        }

        user.Token = settings.ApiKey;
        SetMessenger(user);

        
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
