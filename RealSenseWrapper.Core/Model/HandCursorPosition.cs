using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealSenseWrapper.Core.Model
{
    public class HandCursorPosition
    {
        public Double positionX;
        public Double positionY;

        public HandCursorPosition(Double posX, Double posY)
        {
            this.positionX = posX;
            this.positionY = posY;
        }


    }
}
