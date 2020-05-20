using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownShooter.Exceptions
{
    public class DuplicateComponentException : Exception
    {
        public DuplicateComponentException() : base() { }
        public DuplicateComponentException(string message) : base(message) { }
    }
}
