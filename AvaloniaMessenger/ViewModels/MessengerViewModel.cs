﻿using AvaloniaMessenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.ViewModels
{
    class MessengerViewModel : ViewModelBase
    {
        public User user { get; set; }
        public MessengerViewModel(User user)
        {
            this.user = user;
        }
    }
}