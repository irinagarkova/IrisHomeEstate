using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object id) :
            base($"{entityName} with ID {id} was not found.")
        {
       
        }

    }
}
