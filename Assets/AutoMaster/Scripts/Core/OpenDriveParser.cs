using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;
using EasyRoads3Dv3;
using AutoMaster.Data;
using AutoMaster.Geometry;
using AutoMaster.Utilities;

namespace AutoMaster.Core
{
    public class OpenDriveParser : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Path to the OpenDRIVE (.xodr) map file")]
        public string mapFilePath = "Assets/AutoMaster/Data/Maps/Town01.xodr";

        [Header("Runtime")]
        public ERRoadNetwork roadNetwork;

        void Start()
        {
            string filePath = mapFilePath;

            List<ParsedRoadSegment> geometries = ParseOpenDrive(filePath);

            foreach (var geom in geometries)
            {
                Debug.Log($"Type: {geom.Type}, S: {geom.S}, X: {geom.X}, Y: {geom.Y}, Hdg: {geom.Hdg}, Length: {geom.Length}, Curvature: {geom.Curvature}, Junction: {geom.Junction}");
            }

            roadNetwork = new ERRoadNetwork();
            //==========================================//
            //          Road Type - Junction            //
            //==========================================//
            ERRoadType roadType_jnc = new ERRoadType();
            roadType_jnc.roadWidth = 12.0f;             //  Road Width
            roadType_jnc.layer = 1;
            roadType_jnc.tag = "Untagged";

            Material road_material_junction = Resources.Load("Materials/Roads/noBoundaries") as Material;
            // Check if material exists
            if (road_material_junction != null)
            {
                roadType_jnc.roadMaterial = road_material_junction;    //  Road Material -> Marking Type etc.
            }
            else
            {
                Debug.LogError("1 - Material 'noBoundaries' not found in Resources/Materials/Roads!");
            }

            //==========================================//
            //          Road Type - Ordinary            //
            //==========================================//
            ERRoadType roadType = new ERRoadType();
            roadType.roadWidth = 12.0f;                 //  Road Width
            roadType.layer = 1;                         //
            roadType.tag = "Untagged";                  //

            Material roadMaterial = Resources.Load("Materials/Roads/twoLaneRoadMat") as Material;
            // Check if material exists
            if (roadMaterial != null)
            {
                roadType.roadMaterial = roadMaterial;    //
            }
            else
            {
                Debug.LogError("2 - Material 'twoLaneRoadMat' not found in Resources/Materials/Roads!");
            }

            //==============================================//
            //                    Line                      //
            //==============================================//
            var LineRoads = new List<ERRoad>();             //   The container individually keeps road components
            var linePaths = new List<LinePath>();           //   The container individually keeps road components with the line geometry

            //==============================================//
            //                     Arc                      //
            //==============================================//
            var ArcRoads = new List<ERRoad>();             //   The container individually keeps road components
            var arcPaths = new List<ArcPath>();            //   The container individually keeps road components with the arc geometry

            //=======================================================//
            //                      XML PARSING                      //
            //=======================================================//
            XmlFileLoader XODR_File = new XmlFileLoader(filePath);                     //
            XmlNodeList geoList = XODR_File.getGeoList();            //
            XmlNodeList roadList = XODR_File.getRoadList();          //
            XmlNodeList signalsList = XODR_File.getSignalsList();    //
            XmlNodeList PlanViewList = XODR_File.getPlanViewList();  //

            PathType pathType;
            double tmp;
            //int numberOfSignal;

            int[] arrGeo = new int[PlanViewList.Count];
            int[] arrSignal = new int[PlanViewList.Count];

            int cntr = 0;
            for (int i = 0; i < PlanViewList.Count; i++)
            {
                //******************************************************************//
                cntr = 0;                                                       //
                foreach (XmlNode child in PlanViewList[i].ChildNodes)
                {           //
                    cntr++;                                                    //
                }                                                               //
                //Debug.Log("Num of Geo" + cntr);                               //
                arrGeo[i] = cntr;                                               //
                //**************************************************************//
                cntr = 0;                                                       //
                if (signalsList.Count != 0)
                {                                     //
                    foreach (XmlNode child in signalsList[i].ChildNodes)
                    {        //
                        cntr++;                                                //
                    }                                                           //
                }                                                               //
                //Debug.Log("Num of Signal" + cntr);                            //
                arrSignal[i] = cntr;                                            //
            }                                                                   //
            //******************************************************************//

            int generalGeoCounter = 0;
            int generalSignalCounter = 0;
            float x_refPos, y_refPos;

            for (int j = 0; j < PlanViewList.Count; j++)
            {
                float[] sValues = new float[10];
                float[] tValues = new float[10];
                TrafficObject newTrafficObject = new TrafficObject();

                if (signalsList.Count != 0)
                {
                    //Debug.Log("Signal list count : " + signalsList.Count);
                    for (int q = 0; q < arrSignal[j]; q++)
                    {
                        //Debug.Log("---------------------------------*******************");
                        foreach (XmlNode child in signalsList[generalSignalCounter].ChildNodes)
                        {
                            //Debug.Log(child.Name);
                            if (child.Name == "signal")
                            {
                                newTrafficObject.place = 1;
                                XmlElement trafficSign = (System.Xml.XmlElement)child;
                                var nameObj = trafficSign.GetAttribute("name");
                                //Debug.Log("Name of object" + nameObj);
                                //_______________________________
                                var s_Value = trafficSign.GetAttribute("s");
                                double.TryParse(s_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                                float sVal = Convert.ToSingle(tmp);
                                //_______________________________
                                var t_Value = trafficSign.GetAttribute("t");
                                double.TryParse(t_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                                float tVal = Convert.ToSingle(tmp);
                                //_______________________________
                                //sValues[0] = sVal;
                                //tValues[0] = tVal;
                                newTrafficObject.sPos = sVal;
                                newTrafficObject.tPos = tVal;
                                //Debug.Log("                 Planview"  +  j  +",   S val -> " + sVal);
                                //Debug.Log("                 t val -> " + tVal.ToString());
                            }
                            else
                            {
                                Debug.Log("Unknown Child --------------");
                            }
                        }
                    }
                }

                generalSignalCounter++;

                for (int k = 0; k < arrGeo[j]; k++)
                {
                    foreach (XmlNode child in geoList[generalGeoCounter].ChildNodes)
                    {
                        //Debug.Log(generalGeoCounter);
                        //Debug.Log(geoList.Count);
                        if (child.Name == "line")
                        {
                            //_______________________________________________________________________________________________________________________________//
                            XmlElement geoA = (System.Xml.XmlElement)geoList[generalGeoCounter];
                            XmlElement raodA = (System.Xml.XmlElement)roadList[j];
                            Debug.Log("======================================");
                            Debug.Log("************* junction **************");
                            //Debug.Log("j : " + j);
                            var junc_Value = raodA.GetAttribute("junction");
                            Debug.Log("junc_Value : " + junc_Value);
                            double.TryParse(junc_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float juncVal = Convert.ToSingle(tmp);
                            Debug.Log("roadA : " + juncVal);
                            Debug.Log("======================================");
                            //_______________________________________________________________________________________________________________________________//
                            var s_Value = geoA.GetAttribute("s");
                            double.TryParse(s_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float sVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            var x_Value = geoA.GetAttribute("x");
                            double.TryParse(x_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float xVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            var y_Value = geoA.GetAttribute("y");
                            double.TryParse(y_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float yVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            var heading = geoA.GetAttribute("hdg");
                            double.TryParse(heading, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float hdgVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            var length = geoA.GetAttribute("length");
                            double.TryParse(length, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float lenVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            pathType = PathType.Line;
                            if (juncVal < 0)
                            {                //  Ordinary Road   //
                                linePaths.Add(new LinePath(linePaths.Count, xVal, yVal, 0f, lenVal, hdgVal));
                                LineRoads.Add(new ERRoad());
                                LineRoads[linePaths.Count - 1] = roadNetwork.CreateRoad("line" + (linePaths.Count - 1).ToString(), roadType, linePaths[linePaths.Count - 1].markers);
                            }
                            else
                            {                          //  Junction        //
                                linePaths.Add(new LinePath(linePaths.Count, xVal, yVal, 0.01f, lenVal, hdgVal));
                                LineRoads.Add(new ERRoad());
                                LineRoads[linePaths.Count - 1] = roadNetwork.CreateRoad("line" + (linePaths.Count - 1).ToString(), roadType_jnc, linePaths[linePaths.Count - 1].markers);
                            }
                            if (sVal == 0f)
                            {
                                x_refPos = xVal;
                                y_refPos = yVal;
                            }
                            if (newTrafficObject.place == 1 && (newTrafficObject.sPosStart < sVal))
                            {
                                newTrafficObject.sPosStart = sVal;
                                newTrafficObject.pathIndex = linePaths.Count - 1;
                                newTrafficObject.pathType = PathType.Line;
                            }
                            else if (newTrafficObject.place == 1 && (newTrafficObject.done == 0 && (newTrafficObject.sPosStart >= sVal)))
                            {
                                newTrafficObject.sPosEnd = sVal;
                                float ratio = Math.Abs(newTrafficObject.sPos - newTrafficObject.sPosStart) / Math.Abs(newTrafficObject.sPosStart - newTrafficObject.sPosEnd);
                                Debug.Log("_____________________________________________");
                                Debug.Log("x start " + linePaths[linePaths.Count - 1].xStart + " x end" + linePaths[linePaths.Count - 1].xEnd);
                                Debug.Log("y start " + linePaths[linePaths.Count - 1].yStart + " y end" + linePaths[linePaths.Count - 1].yEnd);
                                newTrafficObject.done = 1;
                            }
                        }
                        else if (child.Name == "arc")
                        {                                   //
                            //_______________________________________________________________________________________________________________________________//
                            XmlElement geoA = (System.Xml.XmlElement)geoList[generalGeoCounter];
                            XmlElement raodA = (System.Xml.XmlElement)roadList[j];
                            Debug.Log("======================================");
                            Debug.Log("************* junction **************");
                            //Debug.Log("j : " + j);
                            var junc_Value = raodA.GetAttribute("junction");
                            Debug.Log("junc_Value : " + junc_Value);
                            double.TryParse(junc_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float juncVal = Convert.ToSingle(tmp);
                            Debug.Log("roadA : " + juncVal);
                            Debug.Log("======================================");
                            //_______________________________________________________________________________________________________________________________//
                            var s_Value = geoA.GetAttribute("s");
                            double.TryParse(s_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float sVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________
                            var x_Value = geoA.GetAttribute("x");
                            double.TryParse(x_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float xVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            var y_Value = geoA.GetAttribute("y");
                            double.TryParse(y_Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float yVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            var heading = geoA.GetAttribute("hdg");
                            double.TryParse(heading, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float hdgVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            var length = geoA.GetAttribute("length");
                            double.TryParse(length, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float lenVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            var child_curvature = child.Attributes["curvature"];
                            double.TryParse(child_curvature.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp);
                            float curveVal = Convert.ToSingle(tmp);
                            //_______________________________________________________________________________________________________________________________//
                            pathType = PathType.Arc;
                            if (juncVal < 0)
                            {                //  Ordinary Road   //
                                arcPaths.Add(new ArcPath(arcPaths.Count, xVal, yVal, 0f, lenVal, hdgVal, curveVal));
                                ArcRoads.Add(new ERRoad());
                                ArcRoads[arcPaths.Count - 1] = roadNetwork.CreateRoad("arc" + (arcPaths.Count - 1).ToString(), roadType, arcPaths[arcPaths.Count - 1].markers);
                            }
                            else
                            {                          //  Junction        //
                                arcPaths.Add(new ArcPath(arcPaths.Count, xVal, yVal, 0.01f, lenVal, hdgVal, curveVal));
                                ArcRoads.Add(new ERRoad());
                                ArcRoads[arcPaths.Count - 1] = roadNetwork.CreateRoad("arc" + (arcPaths.Count - 1).ToString(), roadType_jnc, arcPaths[arcPaths.Count - 1].markers);
                            }
                            if (sVal == 0f)
                            {
                                x_refPos = xVal;
                                y_refPos = yVal;
                            }
                        }
                        else if (child.Name == "spiral")
                        {
                            //Debug.Log("Child Node Name ==> spiral"  + i.ToString());
                            pathType = PathType.Spiral;
                        }
                        generalGeoCounter++;
                    }
                }
            }
        }

        public List<ParsedRoadSegment> ParseOpenDrive(string filePath)
        {
            List<ParsedRoadSegment> geometries = new List<ParsedRoadSegment>();

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList roadNodes = doc.SelectNodes("//OpenDRIVE/road");
            foreach (XmlNode roadNode in roadNodes)
            {
                XmlNodeList geometryNodes = roadNode.SelectNodes("planView/geometry");
                foreach (XmlNode geometryNode in geometryNodes)
                {
                    ParsedRoadSegment geometry = new ParsedRoadSegment
                    {
                        S = Convert.ToDouble(geometryNode.Attributes["s"].Value),
                        X = Convert.ToDouble(geometryNode.Attributes["x"].Value),
                        Y = Convert.ToDouble(geometryNode.Attributes["y"].Value),
                        Hdg = Convert.ToDouble(geometryNode.Attributes["hdg"].Value),
                        Length = Convert.ToDouble(geometryNode.Attributes["length"].Value),
                        Junction = Convert.ToInt32(roadNode.Attributes["junction"].Value)
                    };

                    if (geometryNode["line"] != null)
                    {
                        geometry.Type = "line";
                    }
                    else if (geometryNode["arc"] != null)
                    {
                        geometry.Type = "arc";
                        geometry.Curvature = Convert.ToDouble(geometryNode["arc"].Attributes["curvature"].Value);
                    }
                    else if (geometryNode["spiral"] != null)
                    {
                        geometry.Type = "spiral";
                    }

                    geometries.Add(geometry);
                }
            }

            return geometries;
        }
    }
}
