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
    public class ClearHeightShow : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //获取当前文档
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //执行
            TaskDialog.Show("PinkSecret", "还没写别点了QAQ");
            return Result.Succeeded;
        }
    }
}

