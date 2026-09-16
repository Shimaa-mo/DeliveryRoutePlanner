using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eT3_assignment
{
    public class PackageWeightExceededException : Exception
    {
        public PackageWeightExceededException()
            : base("Package weight exceeds the allowed maximum.") { }

        public PackageWeightExceededException(string message)
            : base(message) { }

        public PackageWeightExceededException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
