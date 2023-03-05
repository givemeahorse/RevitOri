using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hide
{

    public class IdSave 
    {

        // public static List<ICollection<ElementId>> colid = new List<ICollection<ElementId>>();
        public IdSave()
        {
        }

        private static List<ICollection<ElementId>> _IDS;

        public static List<ICollection<ElementId>> IDS
        {
            get
            {
                if (_IDS == null)
                {
                    _IDS = new List<ICollection<ElementId>>();
                }
                return _IDS;
            }


        }

        private static List<List<string>> _instance;

        public static List<List<string>> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance=new List<List<string>>();
                }
                return _instance;
            }
        }

        private static List<String> _timerec;

        public static List<String> Timerec
        {
            get
            {
                if (_timerec == null)
                {
                    _timerec = new List<String>();
                }
                return _timerec;
            }
        }

        private static List<string>  _viewlist;

        public static List<string>  ViewList
        {
            get
            {
                if (_viewlist == null)
                {
                    _viewlist = new List<String>();
                }
                return _viewlist;
            }
        }

        private static List<View> _viewget;

        public static List<View> ViewGet
        {
            get
            {
                if (_viewget == null)
                {
                    _viewget = new List<View>();
                }
                return _viewget;
            }

        }










    }
}
