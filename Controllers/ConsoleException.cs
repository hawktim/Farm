using System;

namespace FarmConsoleApp
{
    public class ConsoleException : Exception
    {
        public ConsoleException(string message) : base(message)
        {
        }
    }
}
