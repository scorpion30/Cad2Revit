using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2Revit.Model
{
    internal class Helper
    {
        public static Autodesk.Revit.DB.Document doc = null;
        public static List<ColumnData> SelectedColumns = new List<ColumnData>();
        public static double bottomLevel_Elevation = 0;
        public static double topLevel_Elevation = 0;
        public static FamilySymbol columnType = null;
        public static Level bottomLevel = null;

        public static int noOfColumns = 0;

    }
}
