using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.Exceptions
{
    class NoConnectionException : Exception
    { 
        string? message { get; set; }

        public NoConnectionException() { }
        public NoConnectionException(string? message)
        {
            this.message = message;
        }
    }
    class InvalidCredentialsException : Exception
    {
        string? message { get; set; }

        public InvalidCredentialsException() { }
        public InvalidCredentialsException(string? message)
        {
            this.message = message;
        }
    }
}
