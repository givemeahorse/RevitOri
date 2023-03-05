using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hide
{
    [Transaction(TransactionMode.Manual)]

    public class Hidenow : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            
            //获取当前文档

            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;

            //执行
            Autodesk.Revit.DB.View view = uiDoc.ActiveView;
            Selection selection = uiDoc.Selection;
            ICollection<ElementId> set = selection.GetElementIds();

            List<string> names = new List<string>();

            Hashtable viewdic = new Hashtable();
            viewdic.Add("FloorPlan", "楼层平面");
            viewdic.Add("EngineeringPlan","结构平面");
            viewdic.Add("CeilingPlan", "天花板平面");
            viewdic.Add("Elevation", "立面");
            viewdic.Add("Section", "剖面");
            viewdic.Add("AreaPlan", "面积平面");
            viewdic.Add("ThreeD", "三维");


            foreach (var id in set)
            {
                names.Add(doc.GetElement(id).Name);
            }

            Transaction trans = new Transaction(doc, "隐藏元素");
            //事件
            trans.Start();
            if (set.Count() == 0)
            {
                TaskDialog.Show("pinksecret提示", "你没选择任何构件！");
            }

            else
            {

                view.HideElements(set);
                
                IdSave.IDS.Add(set);
                IdSave.Instance.Add(names);                             
                IdSave.Timerec.Add(DateTime.Now.ToString());
                IdSave.ViewList.Add(viewdic[view.ViewType.ToString()].ToString()+":"+ view.Name.ToString());
                IdSave.ViewGet.Add(view);
            }
            trans.Commit();
            return Result.Succeeded;
        }
    }

}
