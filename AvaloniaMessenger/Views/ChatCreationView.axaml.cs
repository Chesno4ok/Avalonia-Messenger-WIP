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
    }

    private void Grid_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var vm = DataContext as ChatCreationViewModel;
        vm.CloseViewCommand.Execute().Subscribe();
    }

    private void Grid_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        
    }

    private void Grid_PointerPressed2(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
    }
}