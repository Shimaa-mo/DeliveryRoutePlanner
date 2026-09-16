using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eT3_assignment
{
    public class TripFullException : Exception
    {
        public TripFullException() 
            : base("Trip is full capacity") { }

        public TripFullException(string message)
            : base(message) { }

        public TripFullException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
