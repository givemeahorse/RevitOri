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
    public class HideManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            HideWindow hidewindow = new HideWindow();
            hidewindow.Show();
            
            return Result.Succeeded;
        }

    }
}
