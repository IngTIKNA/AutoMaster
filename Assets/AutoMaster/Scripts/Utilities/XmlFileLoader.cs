using System.Xml;

namespace AutoMaster.Utilities
{
    public class XmlFileLoader
    {
        //______________________//
        XmlDocument doc;        //
        //______________________//

        public XmlFileLoader(string filePath)
        {
            doc = new XmlDocument();
            doc.Load(filePath);
        }

        public XmlNodeList getRoadList()
        {
            XmlNodeList roadList = doc.GetElementsByTagName("road");
            //Debug.Log("Number of roadList      => " + roadList.Count);
            return roadList;
        }

        public XmlNodeList getGeoList()
        {
            XmlNodeList geoList = doc.GetElementsByTagName("geometry");
            //Debug.Log("Num of geometry tags" + geoList.Count);
            return geoList;
        }

        public XmlNodeList getSignalsList()
        {
            XmlNodeList signalsList = doc.GetElementsByTagName("signals");
            //Debug.Log("Number of Signals in first element => " + signalList[0].ChildNodes.Count);
            return signalsList;
        }

        public XmlNodeList getPlanViewList()
        {
            XmlNodeList planViewList = doc.GetElementsByTagName("planView");
            //Debug.Log("Number of planViewList      => " + planViewList.Count);
            return planViewList;
        }
    }
}
