using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace Hide
{
    [Transaction(TransactionMode.Manual)]
    class UITab: IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // 标题栏
            application.CreateRibbonTab("PinkSecret_V1.0");//Tab模块
            // 【一】创建视图类panel
            RibbonPanel rp = application.CreateRibbonPanel("PinkSecret_V1.0", "视图类");//Panel模块
            //获得当前程序路径
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            //1、隐藏模块
            string classNamehide = "Hide.Hidenow";
            //创建pushbutton
            PushButtonData pbhide = new PushButtonData("1", "隐藏", assemblyPath, classNamehide);
            PushButton pushButton1 = rp.AddItem(pbhide) as PushButton;
            //添加图标
            pushButton1.LargeImage= new BitmapImage(new Uri("pack://application:,,,/Hide;component/pic/隐藏.png", UriKind.Absolute));
            //图标提示信息
            pushButton1.ToolTip = "隐藏所选构件";

            //2、视图管理器模块
            string classNamehidemanager = "Hide.HideManager";
            //创建pushbutton
            PushButtonData pbhidemana = new PushButtonData("2", "隐藏管理器", assemblyPath, classNamehidemanager);
            PushButton pushButton2 = rp.AddItem(pbhidemana) as PushButton;
            //添加图标
            pushButton2.LargeImage = new BitmapImage(new Uri("pack://application:,,,/Hide;component/pic/隐藏管理器.png", UriKind.Absolute));
            //图标提示信息
            pushButton2.ToolTip = "打开隐藏管理器";

            //【三】轻量化导出
            RibbonPanel rp3 = application.CreateRibbonPanel("PinkSecret_V1.0", "轻量化");//Panel模块
            string assemblyPath3 = Assembly.GetExecutingAssembly().Location;
            //5、轻量化导出模块
            string classGltf = "Hide.RevitToGltf";
            PushButtonData pbGltf = new PushButtonData("5", "GLTF导出", assemblyPath3, classGltf);
            PushButton pushButton5 = rp3.AddItem(pbGltf) as PushButton;
            pushButton5.LargeImage = new BitmapImage(new Uri("pack://application:,,,/Hide;component/pic/轻量化.png", UriKind.Absolute));
            pushButton5.ToolTip = "导出Gltf模型";

            // 【二】创建标注类panel
            RibbonPanel rp2 = application.CreateRibbonPanel("PinkSecret_V1.0", "标注类");//Panel模块

            string assemblyPath2 = Assembly.GetExecutingAssembly().Location;

            //3、净高快速显示模块
            string classNameheight = "Hide.ClearHeightShow";
            //创建pushbutton
            PushButtonData pbheight = new PushButtonData("3", "净高快显", assemblyPath2, classNameheight);
            PushButton pushButton3 = rp2.AddItem(pbheight) as PushButton;
            //添加图标
            pushButton3.LargeImage = new BitmapImage(new Uri("pack://application:,,,/Hide;component/pic/净高显示.png", UriKind.Absolute));
            //图标提示信息
            pushButton3.ToolTip = "快速显示构件净高";

            //4、净高快速标注
            string classNameheightmark = "Hide.ClearHeightShow";
            PushButtonData pbheightmark = new PushButtonData("4", "净高快标", assemblyPath2, classNameheightmark);
            PushButton pushButton4 = rp2.AddItem(pbheightmark) as PushButton;
            pushButton4.LargeImage = new BitmapImage(new Uri("pack://application:,,,/Hide;component/pic/净高标注.png", UriKind.Absolute));
            pushButton4.ToolTip = "快速标注构件净高";

            




            return Result.Succeeded;
        }
    }
}

