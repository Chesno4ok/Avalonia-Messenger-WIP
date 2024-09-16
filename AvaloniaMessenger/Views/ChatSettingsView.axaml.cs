using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaMessenger.ViewModels;
using System;

namespace AvaloniaMessenger.Views;

public partial class ChatSettingsView : UserControl
{
    public ChatSettingsView()
    {
        InitializeComponent();
    }

    private void Grid_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var vm = DataContext as ChatSettingsViewModel;
        vm.CloseChatSettings.Execute().Subscribe();
    }
}