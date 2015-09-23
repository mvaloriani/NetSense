using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealSenseWrapper.Core
{
    public class RealSenseGestureEventArgs: System.EventArgs
    {

        private PXCMHandData.GestureData gestureData;
        private PXCMHandData.BodySideType bodySide;

        public PXCMHandData.BodySideType BodySide
        {
            get { return bodySide; }
            set { bodySide = value; }
        }

        public PXCMHandData.GestureData GestureData
        {
            get { return gestureData; }
        }


        public RealSenseGestureEventArgs() : base() { }
        public RealSenseGestureEventArgs(PXCMHandData.GestureData gestureData)
            : this()
        {
            this.gestureData = gestureData;
            
        }

        public RealSenseGestureEventArgs(PXCMHandData.GestureData gestureData, PXCMHandData.BodySideType bodySide)
            : this()
        {           
            this.gestureData = gestureData;
            this.bodySide = bodySide;
        }

    }
}
