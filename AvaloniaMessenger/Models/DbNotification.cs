using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.Models
{
    public class DbNotification<T>
    {
        public DbNotification()
        {

        }
        public string table { get; set; }
        public string action { get; set; }
        public T data { get; set; }
    }
}
