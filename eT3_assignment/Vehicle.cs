using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eT3_assignment
{
    internal class Vehicle
    {
        private Delivery _delivery;

        public Delivery Delivery
        {
            get => _delivery;
            set
            {
                if (value is null)
                {
                    _delivery = null;
                    return;
                }

                if (value.packageWeight <= 10)
                {
                    _delivery = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Delivery package weight cannot exceed 10 kg.");
                }

            }
        }
    }
}
