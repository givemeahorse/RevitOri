using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hide
{
    [Transaction(TransactionMode.Manual)]
    public class RevitToGltf : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //获取当前文档
            Document doc = commandData.Application.ActiveUIDocument.Document;
            var view = doc.ActiveView;
            if (view is View3D)
            {
                var config = new ExportConfig();
                config.OutputPath = @"C:\Users\20191107\Desktop\Gltfsave\test.gltf";
                config.LevelOfDetail = 10;
                config.Filter = GetFilter(doc);
                var exportContext = new GltfExporter(doc, config);
                var exporter = new CustomExporter(doc, exportContext);
                exporter.Export(view);
                TaskDialog.Show("BIMBOX", "导出成功");
                return Result.Succeeded;
            }
            else
            {
                throw new Exception("请切换到三维视图");

            }
            //执行
        }
        private List<int> GetFilter(Document doc)
        {
            var elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType();
            return elements.Select(x => x.Id.IntegerValue).ToList();
        }
    }
}
