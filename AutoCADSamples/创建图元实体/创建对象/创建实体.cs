using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    public class CreateEntity
    {
        [CommandMethod("AddPolyline")]
        public void Polyline_多段线()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Polyline pl = new Polyline();
                pl.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                pl.AddVertexAt(1, new Point2d(10, 10), 0, 0, 0);
                pl.AddVertexAt(2, new Point2d(100, 50), 0, 0, 0);

                btr.AppendEntity(pl);
                tr.AddNewlyCreatedDBObject(pl, true);
                tr.Commit();
            }
        }

        [CommandMethod("AddLine")]
        public void Line_直线()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Line line = new Line();
                line.StartPoint = new Point3d(0, 0, 0);
                line.EndPoint = new Point3d(10, 10, 0);

                btr.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                tr.Commit();
            }
        }

        [CommandMethod("AddText")]
        public void DBText_文字注记()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                DBText dbtext = new DBText();
                dbtext.TextString = "hello world";
                dbtext.Height = 10;
                dbtext.HorizontalMode = TextHorizontalMode.TextCenter;//水平方向居中
                dbtext.VerticalMode = TextVerticalMode.TextVerticalMid;//水平方向居中
                dbtext.Position = new Point3d(100, 100, 0);
                //当手动设置了文字的居中方式时,一定要设置AlignmentPoint属性，否则文字显示在原点
                dbtext.AlignmentPoint = dbtext.Position;

                btr.AppendEntity(dbtext);
                tr.AddNewlyCreatedDBObject(dbtext, true);
                tr.Commit();
            }
        }

        [CommandMethod("AddCircle")]
        public void Circle_圆()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Circle c = new Circle();
                c.Center = new Point3d(0, 0, 0);
                c.Radius = 100;

                btr.AppendEntity(c);
                tr.AddNewlyCreatedDBObject(c, true);
                tr.Commit();
            }
        }
        [CommandMethod("AddArc")]
        public void Arc_圆弧()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Arc arc = new Arc();
                arc.Center = new Point3d(0, 0, 0);
                arc.StartAngle = 0;
                arc.EndAngle = Math.PI;
                arc.Radius = 10;
                btr.AppendEntity(arc);
                tr.AddNewlyCreatedDBObject(arc, true);
                tr.Commit();
            }
        }

        [CommandMethod("AddXline")]
        public void Xline_构造线()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Xline xline = new Xline();
                xline.BasePoint = new Point3d(0, 0, 0);
                xline.SecondPoint = new Point3d(10, 10, 0);

                btr.AppendEntity(xline);
                tr.AddNewlyCreatedDBObject(xline, true);
                tr.Commit();
            }
        }

        [CommandMethod("AddSpline")]
        public void Spline_构造线()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //Spline spline = new Spline();

                //btr.AppendEntity(xline);
                //tr.AddNewlyCreatedDBObject(xline, true);
                //tr.Commit();
            }
        }
        [CommandMethod("AddEllipse")]
        public void Ellipse_椭圆()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //RadiusRatio属性必须小于1
                Ellipse e = new Ellipse(new Point3d(0, 0, 0), Vector3d.ZAxis, Vector3d.XAxis, 0.5, 0, 2 * Math.PI);
               

                btr.AppendEntity(e);
                tr.AddNewlyCreatedDBObject(e, true);
                tr.Commit();
            }
        }
        [CommandMethod("AddRegion")]
        public void Region_面域()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PromptEntityResult per = ed.GetEntity("选择实体");
                if (per.Status != PromptStatus.OK)
                    return;

                Entity ent = tr.GetObject(per.ObjectId, OpenMode.ForWrite) as Entity;

                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                DBObjectCollection dBObjectCollection = new DBObjectCollection();
                dBObjectCollection.Add(ent);
                Region region = Region.CreateFromCurves(dBObjectCollection)[0] as Region;

                btr.AppendEntity(region);
                tr.AddNewlyCreatedDBObject(region, true);
                tr.Commit();
            }
        }

        [CommandMethod("entityType")]
        public void entityType()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PromptEntityResult per = ed.GetEntity("选择实体");
                if (per.Status != PromptStatus.OK)
                    return;

                Entity ent = tr.GetObject(per.ObjectId, OpenMode.ForWrite) as Entity;
                ed.WriteMessage($"\nTypeName:{ent.GetType().Name}");
                ed.WriteMessage($"\nRXClassName:{ent.GetRXClass().Name}");
                ed.WriteMessage($"\nDxfName:{ent.GetRXClass().DxfName}");
            }
        }
    }
}
