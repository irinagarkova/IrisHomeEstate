using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Exceptions
{
    public class CustomValidationException : Exception
    {
        public CustomValidationException(string message, Dictionary<string, string> errors) :base(message)
        {
            Errors = errors;
        }

        public Dictionary<string, string> Errors { get; }
    }
}
