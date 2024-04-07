using Avalonia.Controls;
using AvaloniaMessenger.Views;
using ReactiveUI;

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
    public void SetSignIn()
    {   
        var viewModel = new SignInViewModel();

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
