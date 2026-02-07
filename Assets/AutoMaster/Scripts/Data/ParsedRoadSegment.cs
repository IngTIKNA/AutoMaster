namespace AutoMaster.Data
{
    //======================================//
    //              Road Segment            //
    //======================================//
    public class ParsedRoadSegment
    {
        public string Type { get; set; }
        public double S { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Hdg { get; set; }
        public double Length { get; set; }
        public double Curvature { get; set; } // Only for arcs
        public int Junction { get; set; }     // Junction identifier
    }
}
