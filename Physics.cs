using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameLogic
{
    public class Physics
    {
        // For calculating laser new position
        public int Movelaser(int currentY,int speed)
        {
            return currentY - speed;// Laser will go up
        }

        //To check weather the laser is out of the screen
        public bool IsOutOfBounds(int Y)
        {
            return Y < 0;
        }
    }
}
