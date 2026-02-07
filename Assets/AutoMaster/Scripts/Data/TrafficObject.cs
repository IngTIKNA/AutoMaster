using AutoMaster.Utilities;

namespace AutoMaster.Data
{
    //==================================================//
    //                  Traffic Objects                 //
    //==================================================//
    public class TrafficObject
    {
        //____________________________
        public float xPos;
        public float yPos;
        public float sPos;
        public float tPos;
        public float sPosStart;
        public float sPosEnd;
        public float tPosEnd;
        public PathType pathType;
        public int pathIndex;
        public int place;               // determines if the object will be placed into the sim env
        public int done;
        //____________________________

        public TrafficObject()
        {
            this.place = 0;
            this.xPos = 0;
            this.yPos = 0;
            this.sPos = 0;
            this.tPos = 0;
            this.sPosStart = 0;
            this.sPosEnd = 0;
            this.tPosEnd = 0;
            this.done = 0;
        }
    }
}
