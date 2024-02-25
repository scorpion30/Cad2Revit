using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2Revit
{
    internal class ColumnData
    {
       public string LayerName = null;
       public XYZ coordinates = null;

        public ColumnData(string layerName, XYZ coordinates)
        {
            LayerName = layerName;
            this.coordinates = coordinates;
        }
    }
}
