using System;
using UnityEngine;

namespace AutoMaster.Geometry
{
    //==================================================//
    //                  LINE GEOMETRY                   //
    //==================================================//
    public class LinePath : PathBase
    {
        public float xEnd;
        public float yEnd;
        public float z;
        public float angle;
        public float length;
        public Vector3[] markers = new Vector3[2];

        public LinePath(int road_index, float x_Start, float y_Start, float z, float length, float hdg)
        {
            this.xStart = x_Start;
            this.yStart = y_Start;
            this.z = z;
            this.angle = hdg;
            this.length = length;
            this.pathIndex = road_index;
            this.markers[0] = new Vector3(this.xStart, this.z, this.yStart);
            this.xEnd = this.xStart + Convert.ToSingle(Math.Cos(this.angle)) * this.length;
            this.yEnd = this.yStart + Convert.ToSingle(Math.Sin(this.angle)) * this.length;
            this.markers[1] = new Vector3(this.xEnd, this.z, this.yEnd);
        }
    }
}
