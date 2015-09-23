using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealSenseWrapper.Core
{


   public class RealSenseEventArgs : System.EventArgs
    {

       private PXCMImage.ImageData source;

       public PXCMImage.ImageData Source
        {
            get { return source; }
        }

       private PXCMImage.ImageInfo info;
       public PXCMImage.ImageInfo Info
       {
           get { return info; }       
       }
        

        public RealSenseEventArgs() : base() { }
        public RealSenseEventArgs(PXCMImage.ImageData source, PXCMImage.ImageInfo info)
            : this()
        {
            this.source = source;
            this.info = info;
        }

    }
}
