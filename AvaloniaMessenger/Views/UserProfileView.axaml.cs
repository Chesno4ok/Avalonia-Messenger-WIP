using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaMessenger.ViewModels;
using ReactiveUI.Validation.Extensions;
using System;

namespace AvaloniaMessenger;

public partial class UserProfileView : UserControl
{
    public UserProfileView()
    {
        InitializeComponent();

       
    }

    private void Grid_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var vm = DataContext as UserProfileViewModel;

        if (vm is null)
            return;

        vm.ExitUserProfile.Execute(null).Subscribe();

    }
}