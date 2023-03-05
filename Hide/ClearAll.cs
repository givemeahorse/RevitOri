using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hide
{
    class ClearAll : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;
            UIDocument uiDoc = app.ActiveUIDocument;


            //执行
            Transaction trans = new Transaction(doc, "清空");
            trans.Start();

            for (int i = 0; i < IdSave.IDS.Count(); i++)
            {
                //判断是否为当前视图，不是则跳转
                View view = uiDoc.ActiveView;
                if (uiDoc.ActiveView != IdSave.ViewGet[i])
                {
                    view = IdSave.ViewGet[i];
                }
                view.UnhideElements(IdSave.IDS[i]);
            }

            trans.Commit();

            IdSave.IDS.Clear();
            IdSave.Instance.Clear();
            IdSave.Timerec.Clear();
            IdSave.ViewList.Clear();
            IdSave.ViewGet.Clear();
        }

        public string GetName()
        {
            return "ClearAll";
        }
    }
}
