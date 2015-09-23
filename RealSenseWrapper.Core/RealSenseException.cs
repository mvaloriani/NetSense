using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealSenseWrapper.Core
{
    public class RealSenseException : Exception
    {
        private pxcmStatus status;

        public pxcmStatus Status
        {
            get { return status; }            
        }

        public RealSenseException() : base() { }

        public RealSenseException(string message) : base(message) { }

        public RealSenseException(string message, pxcmStatus status) : this(message) {
            this.status = status;
        }
    }
}
