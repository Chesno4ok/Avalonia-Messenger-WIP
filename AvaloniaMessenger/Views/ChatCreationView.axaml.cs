using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaMessenger.ViewModels;
using System;

namespace AvaloniaMessenger.Views;

public partial class ChatCreationView : UserControl
{
    public ChatCreationView()
    {
        InitializeComponent();
        Grid grid = new();
    }

    private void Grid_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var vm = DataContext as ChatCreationViewModel;
        vm.CloseViewCommand.Execute().Subscribe();
    }
}