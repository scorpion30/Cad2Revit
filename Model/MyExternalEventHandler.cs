using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Cad2Revit.Model
{
    internal class MyExternalEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                using (Transaction trans = new Transaction(Helper.doc, "Create New Columns"))
                {
                    trans.Start();
                    foreach (var col in Helper.SelectedColumns)
                    {

                        XYZ bottom_Point = new XYZ(col.coordinates.X, col.coordinates.Y, Helper.bottomLevel_Elevation);
                        XYZ top_Point = new XYZ(col.coordinates.X, col.coordinates.Y, Helper.topLevel_Elevation);
                        Curve column_Line = Autodesk.Revit.DB.Line.CreateBound(bottom_Point, top_Point);

                        Helper.doc.Create.NewFamilyInstance(column_Line, Helper.columnType, Helper.bottomLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);

                        Helper.noOfColumns++;

                    }
                    trans.Commit();
                }

            }
            catch (System.Exception)
            {

                MessageBox.Show("There is as Error", "Error", MessageBoxButton.OK
                    , MessageBoxImage.Hand);
            }
        }

        public string GetName()
        {
            return "MyExternalEventHandler";
        }
    }
}
