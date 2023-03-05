using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hide
{
    class ShowSelected : IExternalEventHandler
    {
        public int IdIndex { get; set; }


        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;
            UIDocument uiDoc = app.ActiveUIDocument;

            
            //判断是否为当前视图，不是则跳转
            View view = uiDoc.ActiveView;
            if (uiDoc.ActiveView != IdSave.ViewGet[IdIndex])
            {
                view = IdSave.ViewGet[IdIndex];
                uiDoc.ActiveView = view;
            }

            //执行
            Transaction trans = new Transaction(doc, "选中行恢复显示");
            trans.Start();
            
            //判断有元素是否已经被恢复显示
                foreach (var item in IdSave.IDS[IdIndex])
            {
                if (!doc.GetElement(item).IsHidden(view))
                {
                    TaskDialog.Show("PinkSecret提示", "选中列表中有构件已恢复显示");
                    break;
                }
            }
            
            view.UnhideElements(IdSave.IDS[IdIndex]);

            trans.Commit();
        }

        public string GetName()
        {
            return "ShowSelected";
        }
    }
}
