using System;

namespace ApplicationCore.Exceptions
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message)
        {
        }
    }
}