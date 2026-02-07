using System;
using UnityEngine;

namespace AutoMaster.Geometry
{
    //==================================================//
    //                  ARC GEOMETRY                    //
    //==================================================//
    public class ArcPath : PathBase
    {
        public float xCentral;
        public float xEnd;
        public float xTemp;
        public float yCentral;
        public float yEnd;
        public float yTemp;
        public float z;
        public float angle;
        public float length;
        public float radius;
        public float centralAngle;
        public int arrSize;
        public int sign;
        public Vector3[] tmp_markers;
        public Vector3[] markers;

        public ArcPath(int road_index, float x_Start, float y_Start, float z, float length, float hdg, float curvature)
        {
            this.xStart = x_Start;
            this.yStart = y_Start;
            this.z = z;
            this.length = length;
            this.pathIndex = road_index;
            this.radius = (Math.Abs(1 / curvature));
            this.centralAngle = this.length / this.radius;

            this.arrSize = Mathf.FloorToInt(this.length / 0.05f) + 2;                       // Num of increments | must be at least 2
            this.sign = Math.Sign(curvature);                                         //
            this.markers = new Vector3[this.arrSize];                                       //
            //this.tmp_markers = new Vector3[this.arrSize];                                 //

            //==========================================//
            //              CENTRAL POINT               //
            //==========================================//
            this.xCentral = this.xStart + Convert.ToSingle(Math.Cos(hdg + (Math.PI / 2) * this.sign * (-1) - Math.PI)) * this.radius;
            this.yCentral = this.yStart + Convert.ToSingle(Math.Sin(hdg + (Math.PI / 2) * this.sign * (-1) - Math.PI)) * this.radius;
            //
            this.xEnd = this.xCentral + Convert.ToSingle(Math.Cos((hdg + (this.centralAngle) * this.sign) - (Math.PI / 2) * this.sign)) * this.radius;
            this.yEnd = this.yCentral + Convert.ToSingle(Math.Sin((hdg + (this.centralAngle) * this.sign) - (Math.PI / 2) * this.sign)) * this.radius;
            //
            this.markers[this.arrSize - 1] = new Vector3(this.xEnd, 0f, this.yEnd);           //  END
            this.markers[0] = new Vector3((this.xStart), (this.z), (this.yStart));        //  START

            //==============================================================//
            //              POINT BETWEEN START and END POINTS              //
            //==============================================================//
            for (int i = 1; i < (this.arrSize - 1); i++)
            {
                this.xTemp = this.xCentral + Convert.ToSingle(Math.Cos((hdg + ((i) * (this.centralAngle / (this.arrSize - 1))) * this.sign) - (Math.PI / 2) * this.sign)) * this.radius;
                this.yTemp = this.yCentral + Convert.ToSingle(Math.Sin((hdg + ((i) * (this.centralAngle / (this.arrSize - 1))) * this.sign) - (Math.PI / 2) * this.sign)) * this.radius;
                this.markers[i] = new Vector3(this.xTemp, this.z, this.yTemp);
            }
        }
    }
}
