using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using AvaloniaMessenger.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;

namespace AvaloniaMessenger.Controls;

public class SideBarMenu : TemplatedControl
{
    private TemplateAppliedEventArgs _templateControls;

    public static readonly StyledProperty<List<SideBarButton>> CommandListProperty =
          AvaloniaProperty.Register<MessageTemplate, List<SideBarButton>>(nameof(CommandList));
    public List<SideBarButton> CommandList
    {
        get => GetValue(CommandListProperty);
        set => SetValue(CommandListProperty, value);
    }

    public static readonly StyledProperty<bool> IsMenuEnbabledProperty =
           AvaloniaProperty.Register<MessageTemplate, bool>(nameof(IsMenuEnbabled));
    public bool IsMenuEnbabled
    {
        get => GetValue(IsMenuEnbabledProperty);
        set => SetValue(IsMenuEnbabledProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> ToggleSideBarCommandProperty =
           AvaloniaProperty.Register<MessageTemplate, ReactiveCommand<Unit, Unit>>(nameof(ToggleSideBarCommand));
    public ReactiveCommand<Unit, Unit> ToggleSideBarCommand
    {
        get => GetValue(ToggleSideBarCommandProperty);
        set => SetValue(ToggleSideBarCommandProperty, value);
    }

    public SideBarMenu()
    {
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _templateControls = e;

        var grid = e.NameScope.Find<Grid>("BackgroundGrid");
        grid.PointerPressed += Grid_PointerPressed;

        ToggleSideBarCommand = ReactiveCommand.Create(ToggleSideBar);
        IsMenuEnbabled = false;
        this.Width = 0;

        base.OnApplyTemplate(e);
    }

    private void ToggleSideBar()
    {
        IsMenuEnbabled = !IsMenuEnbabled;

        if (IsMenuEnbabled == true)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }
    }
    private void ShowMenu()
    {
        this.Width = double.NaN;

        var grid = _templateControls.NameScope.Find<Grid>("BackgroundGrid");
        grid.Classes.Clear();
        grid.Classes.Add("BackgroundEaseIn");

        var listBox = _templateControls.NameScope.Find<ListBox>("ItemList");
        listBox.Classes.Clear();
        listBox.Classes.Add("SlideIn");
    }
    private void HideMenu()
    {
        var grid = _templateControls.NameScope.Find<Grid>("BackgroundGrid");
        grid.Classes.Clear();
        grid.Classes.Add("BackgroundEaseOut");

        var listBox = _templateControls.NameScope.Find<ListBox>("ItemList");
        listBox.Classes.Clear();
        listBox.Classes.Add("SlideOut");


        var timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(190),
            IsEnabled = true
        };
        timer.Tick += HideMenuAfterAnimation;

    }

    private void HideMenuAfterAnimation(object? sender, EventArgs e)
    {
        var timer = sender as DispatcherTimer;
        timer.IsEnabled = false;

        this.Width = 0;
    }

    private void Grid_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        ToggleSideBarCommand.Execute().Subscribe();
    }


}
public struct SideBarButton
{
    public ReactiveCommand<Unit, Unit> ReactiveCommand { get; set; }
    public string CommandName { get; set; }
}