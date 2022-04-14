using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.实体选择
{
    public class Selection
    {
        private Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

        [CommandMethod("SelectOneEntity")]
        public void SelectOneEntity()
        {
            PromptEntityOptions peo = new PromptEntityOptions("\n选择实体");
            peo.SetRejectMessage("选择的不是多段线");
            peo.AddAllowedClass(typeof(Polyline), true);//只能选择多段线,SetRejectMessage必须在该函数前
            PromptEntityResult per = ed.GetEntity(peo);
            if(per.Status != PromptStatus.OK)
            {
                return;
            }
            using(Transaction tr = ed.Document.TransactionManager.StartTransaction())
            {
                Polyline pl = per.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                ed.WriteMessage($"\n多段线长{pl.Length}");
            }
        }

        [CommandMethod("GetSelection")]
        //选择多个实体
        public void GetSelection()
        {
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.RejectObjectsOnLockedLayers = false;//为FALSE时，锁定的图层也可以选中
            PromptSelectionResult psr = ed.GetSelection(pso, GetFilter());
            if (psr.Status != PromptStatus.OK)
                return;
            ed.WriteMessage($"\n共选中{psr.Value.Count}个图元");
        }

        [CommandMethod("SelectAll")]
        public void SelectAll()//选择图面所有实体
        {
            PromptSelectionResult psr = ed.SelectAll(GetFilter());
            if (psr.Status != PromptStatus.OK)
                return;
            ed.WriteMessage($"\n共选中{psr.Value.Count}个图元");
        }

        [CommandMethod("SelectImplied")]
        public void SelectImplied()//选择图面上已经被选中的实体
        {
            PromptSelectionResult psr = ed.SelectImplied();
            if (psr.Status != PromptStatus.OK)
                return;
            ed.WriteMessage($"\n共选中{psr.Value.Count}个图元");
        }

        [CommandMethod("SelectCrossingPolygon")]
        public void SelectCrossingPolygon()//在给定点组成的面相交的实体都会被选中
        {
            //只要你选择的点集不在当前视图上就无法选中，所以需要缩放视图
            Point3dCollection p3ds = new Point3dCollection();
            p3ds.Add(new Point3d(0, 0, 0));
            p3ds.Add(new Point3d(0, 100, 0));
            p3ds.Add(new Point3d(100, 100, 0));
            p3ds.Add(new Point3d(100, 0, 0));
            PromptSelectionResult psr = ed.SelectCrossingPolygon(p3ds, GetFilter());
            if (psr.Status != PromptStatus.OK)
                return;
            ed.WriteMessage($"\n共选中{psr.Value.Count}个图元");
        }

        [CommandMethod("SelectCrossingWindow")]
        public void SelectCrossingWindow()//在给定点构成的矩形相交的实体都会被选中
        {
            PromptSelectionResult psr = ed.SelectCrossingWindow(new Point3d(0,0,0), new Point3d(100,100,0), GetFilter());
            if (psr.Status != PromptStatus.OK)
                return;
            ed.WriteMessage($"\n共选中{psr.Value.Count}个图元");
        }

        [CommandMethod("SelectFence")]
        public void SelectFence()//在给定点构成的路径相交的实体都会被选中
        {
            Point3dCollection p3ds = new Point3dCollection();
            p3ds.Add(new Point3d(0, 0, 0));
            p3ds.Add(new Point3d(0, 100, 0));
            PromptSelectionResult psr = ed.SelectFence(p3ds, GetFilter());
            if (psr.Status != PromptStatus.OK)
                return;
            ed.WriteMessage($"\n共选中{psr.Value.Count}个图元");
        }

        public SelectionFilter GetFilter()//过滤器
        {
            //过滤0图层中的多段线和圆
            TypedValue[] typedValues = new TypedValue[]
            {
                /*DxfCode介绍可查考acad_dxf.chm文档*/
                new TypedValue((int)DxfCode.Operator, "<AND"),
                new TypedValue((int)DxfCode.Operator, "<OR"),
                new TypedValue((int)DxfCode.Start, "LWPolyline"),//类型名称指定是dxfname,通过RXClass类获得
                new TypedValue((int)DxfCode.Start, "Circle"),
                new TypedValue((int)DxfCode.Operator, "OR>"),
                new TypedValue((int)DxfCode.LayerName, "0"),
                new TypedValue((int)DxfCode.Operator, "AND>")
            };
            SelectionFilter filter = new SelectionFilter(typedValues);
            return filter;
        }
    }
}
