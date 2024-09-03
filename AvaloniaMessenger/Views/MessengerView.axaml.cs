using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using AvaloniaMessenger.ViewModels;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaMessenger.Views
{
    public partial class MessengerView : UserControl
    {
        private ListBox? _listBox;
        private Vector? _scrollHeight;
        private bool _saveScroll;
        private bool _addingNewMessages;

        public MessengerView()
        {
            InitializeComponent();
            
            _listBox = this.FindControl<ListBox>("MessagePanel");

            _saveScroll = false;

            MessageTextBox.KeyDown += MessageTextBox_KeyDown;

        }

        public async void InitialiseEvents()
        {
            var vm = DataContext as MessengerViewModel;
            if (vm == null)
                throw new Exception("ViewModel hasn't been initialised");

            // Incoming messages event
            vm.LoadNewMessagesScrollCommand = ReactiveCommand.Create(ScrollOnIncomingMessages);

            var scrollViewer = await _listBox.GetObservable(ListBox.ScrollProperty).OfType<ScrollViewer>().FirstAsync();
            scrollViewer.ScrollChanged += OnScrollChanged;


            scrollViewer.GetObservable(ScrollViewer.ScrollBarMaximumProperty).Subscribe(i => SaveScrollPosition(i));

        }
        public void SaveScrollPosition(Vector maxHeight)
        {
            if (_saveScroll == false)
                return;
            

            _listBox.Scroll.Offset = (Vector)(maxHeight - _scrollHeight);

            _saveScroll = false;
        }
        private async void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() => LoadPreviousMessagesAsync(sender));
        }
        private async Task LoadPreviousMessagesAsync(object? sender)
        {
            var sv = sender as ScrollViewer;
            var vm = DataContext as MessengerViewModel;

            if (sv.Offset.Y != 0 || vm.SelectedChat == null || vm.Messages.Count == 0)
                return;

            _addingNewMessages = false;
            var maxScrollHeight = await sv.GetObservable(ScrollViewer.ScrollBarMaximumProperty).FirstAsync();
            _scrollHeight = maxScrollHeight;

            
            vm.LoadPreviousMessagesCommand.Execute().Subscribe();

            _saveScroll = true;
            sv.Offset = new Vector(0, 1);
        }
        private async void ScrollOnIncomingMessages()
        {
            Dispatcher.UIThread.Post(() => ScrollToBottom());
        }
        private async void ScrollToBottom()
        {
            if (_listBox == null)
                throw new Exception("No MessagePanel found");

            _addingNewMessages = true;

            var scrollViewer = await _listBox.GetObservable(ListBox.ScrollProperty).OfType<ScrollViewer>().FirstAsync();

            scrollViewer.GetObservable(ScrollViewer.ScrollBarMaximumProperty).Subscribe(i =>
            {
                if (_addingNewMessages)
                    _listBox.Scroll.Offset = i;
            });
        }
        private void MessageTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            (DataContext as MessengerViewModel).SendMessage();
        }

    }
}
