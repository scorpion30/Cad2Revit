using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Cad2Revit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cad2Revit
{
    /// <summary>
    /// Interaction logic for myWindow.xaml
    /// </summary>
    public partial class myWindow : Window
    {
        Autodesk.Revit.DB.Document doc = null;

        List<ColumnData> columns = new List<ColumnData>();
        MyExternalEventHandler myHandler = null;
        ExternalEvent externalEvent = null;

        public myWindow(Document doc)
        {
            this.doc = doc;
            InitializeComponent();

            myHandler = new MyExternalEventHandler();
            externalEvent = ExternalEvent.Create(myHandler);
        }

        private void Collect_Click(object sender, RoutedEventArgs e)
        {
            var dwgsImport = new FilteredElementCollector(this.doc)
                .OfClass(typeof(ImportInstance)).WhereElementIsNotElementType()
                .ToElementIds();

            foreach (var dwgId in dwgsImport)
            {
                var dwg = doc.GetElement(dwgId) as ImportInstance;
                var dwgName = dwg.Category.Name;
                this.DWG_Box.Items.Add(dwgName.ToString());
            }

            // collect all column types

            var columnTypes = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsElementType().ToElementIds();

            foreach (var colTypeId in columnTypes)
            {
                var colType = doc.GetElement(colTypeId) as FamilySymbol;
                var colTypeName = colType.Name;
                this.ColumnType_Box.Items.Add(colTypeName.ToString());
            }

            //Collect Levels

            var levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level)).WhereElementIsNotElementType()
                .ToElementIds();

            foreach (var lvlId in levels)
            {
                var lvl = doc.GetElement(lvlId) as Level;
                var lvlName = lvl.Name;
                this.TopLevel_Box.Items.Add(lvlName.ToString());
                this.BottomLevel_Box.Items.Add(lvlName.ToString());
            }

        }


        private void Refresh_Btn_Click(object sender, RoutedEventArgs e)
        {

            columns.Clear();
            Layers_Box.Items.Clear();

            var selectedDwg = this.DWG_Box.SelectedItem.ToString();
            var targetDwg = new FilteredElementCollector(this.doc)
                .OfClass(typeof(ImportInstance)).WhereElementIsNotElementType()
                .Cast<ImportInstance>()
                .FirstOrDefault(x => x.Category.Name == selectedDwg);

            var dwgGeometry = targetDwg.get_Geometry(new Options());
            foreach (var geoEle in dwgGeometry)
            {
                GeometryElement ele = null;
                if (geoEle is GeometryInstance)
                {
                    var instance = geoEle as GeometryInstance;
                    ele = instance.GetInstanceGeometry();
                }
                if (ele != null && ele.Count() > 0)
                {
                    foreach (var inst in ele)
                    {
                        //var geometryInst = inst as GeometryInstance;
                        //var geoInstance = geometryInst.GetInstanceGeometry();
                        //foreach (var curve in geoInstance)
                        //{
                        if (inst is Autodesk.Revit.DB.PolyLine)
                        {
                            var poly = inst as Autodesk.Revit.DB.PolyLine;
                            var layerId = poly.GraphicsStyleId;
                            var layer = doc.GetElement(layerId) as GraphicsStyle;
                            var layerName = layer.GraphicsStyleCategory.Name;
                            if (!(this.Layers_Box.Items.Contains(layerName.ToString())))
                            {
                                this.Layers_Box.Items.Add(layerName.ToString());
                            }

                            var minP = poly.GetOutline().MinimumPoint;
                            var maxP = poly.GetOutline().MaximumPoint;
                            var midP = GetMidPoint(minP, maxP);

                            ColumnData myColumn = new ColumnData(layerName, midP);
                            columns.Add(myColumn);
                        }
                    }
                }
            }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            string selected_LayerName = this.Layers_Box.SelectedItem.ToString();
            string selected_ColumnTypeName = this.ColumnType_Box.SelectedItem.ToString();
            string selected_TopLevel = this.TopLevel_Box.SelectedItem.ToString();
            string selected_BottomLevel = this.BottomLevel_Box.SelectedItem.ToString();

            List<ColumnData> selected_Columns = new List<ColumnData>();
            foreach (var col in columns)
            {
                if (col.LayerName == selected_LayerName)
                {
                    selected_Columns.Add(col);
                }
            }

            // collect all column types

            var columnType = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsElementType().Cast<FamilySymbol>()
                .FirstOrDefault(x => x.Name == selected_ColumnTypeName);

            //Collect Levels

            var topLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level)).WhereElementIsNotElementType()
                .Cast<Level>()
                .FirstOrDefault(x => x.Name == selected_TopLevel);
            var topLevel_Elevation = topLevel.Elevation;

            var bottomLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level)).WhereElementIsNotElementType()
                .Cast<Level>()
                .FirstOrDefault(x => x.Name == selected_BottomLevel);
            var bottomLevel_Elevation = bottomLevel.Elevation;

            Helper.doc = doc;
            Helper.bottomLevel = bottomLevel;
            Helper.bottomLevel_Elevation = bottomLevel_Elevation;
            Helper.columnType = columnType;
            Helper.topLevel_Elevation = topLevel_Elevation;
            Helper.SelectedColumns = selected_Columns;

            externalEvent.Raise();
        }

        private XYZ GetMidPoint(XYZ MinPoint, XYZ MaxPoint)
        {
            double Mid_x = (MinPoint.X + MaxPoint.X) / 2;
            double Mid_y = (MinPoint.Y + MaxPoint.Y) / 2;
            double Mid_z = (MinPoint.Z + MaxPoint.Z) / 2;

            return new XYZ(Mid_x, Mid_y, Mid_z);
        }

        private void Read_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.ColumnNumber_Btn.Text = Helper.noOfColumns.ToString();
        }
    }
} // Closing brace for the myWindow class
