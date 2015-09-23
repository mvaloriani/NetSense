using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealSenseWrapper.Core
{
    public class RealSenseHandsEventArgs: System.EventArgs
    {
        Dictionary<PXCMHandData.JointType, PXCMHandData.JointData>[] hands = new Dictionary<PXCMHandData.JointType, PXCMHandData.JointData>[4];

        public Dictionary<PXCMHandData.JointType, PXCMHandData.JointData>[] Hands
        {
            get { return hands; }
        }

        PXCMHandData.IHand[] totalHands = new PXCMHandData.IHand[4];

        public PXCMHandData.IHand[] TotalHands
        {
            get { return totalHands; }
        }
        

        public RealSenseHandsEventArgs() : base() { }
        public RealSenseHandsEventArgs(Dictionary<PXCMHandData.JointType, PXCMHandData.JointData>[] hands, PXCMHandData.IHand[] totalHands)
            : this()
        {
            this.hands = hands;
            this.totalHands = totalHands;
        }
        public RealSenseHandsEventArgs(PXCMHandData.IHand[] totalHands) : this(null, totalHands){}
        public RealSenseHandsEventArgs(Dictionary<PXCMHandData.JointType, PXCMHandData.JointData>[] hands) : this(hands, null){}

    }

}
