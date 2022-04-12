using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace Samples
{
    public class 动态绘制
    {
        [CommandMethod("tsDrawJig")]
        public void tsDrawJig()
        {
            TextDrawJig.Jig();
        }
    }

    public class MyDragJig : DrawJig
    {
        private Point3d _centerPoint;
        private double _Radius;
        private Circle _circle;
        public double Radius { get => _Radius; }
        public MyDragJig(Point3d centerPoint, Circle circle)
        {
            _centerPoint = centerPoint;
            _circle = circle;
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jppo = new JigPromptPointOptions("\n指定半径：");
            jppo.UseBasePoint = true;
            jppo.BasePoint = _centerPoint;
            

            PromptPointResult ppr = prompts.AcquirePoint(jppo);
            if(ppr.Status == PromptStatus.OK && ppr.Value.DistanceTo(_centerPoint) > 10e-10)
            {
                _Radius = ppr.Value.DistanceTo(_centerPoint);
                return SamplerStatus.OK;
            }
            else
            {
                return SamplerStatus.Cancel;
            }

        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            _circle.Radius = _Radius;
            _circle.WorldDraw(draw);
            return true;
        }

        public static void Jig()
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            //PromptPointResult ppr = ed.GetPoint("选择圆心");
            //if (ppr.Status != PromptStatus.OK)
            //    return;

            PromptEntityResult per = ed.GetEntity("选择圆");
            if (per.Status != PromptStatus.OK)
                return;

            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Circle c = tr.GetObject(per.ObjectId, OpenMode.ForWrite) as Circle;
                MyDragJig dragJig = new MyDragJig(c.Center, c);
                PromptResult pr = ed.Drag(dragJig);
                if(pr.Status == PromptStatus.OK)
                {
                    c.Radius = dragJig.Radius;
                    tr.Commit();
                }
                else
                {
                    tr.Abort();
                }

            }

        }
    }
    public class TextDrawJig : DrawJig
    {
        private DBText _Text;
        public Point3d InsertPoint;
        private List<Entity> entities = new List<Entity>();
        public TextDrawJig(DBText dBText)
        {
            _Text = dBText;
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jppo = new JigPromptPointOptions("\n选择插入点");
            PromptPointResult ppr = prompts.AcquirePoint(jppo);
            if(ppr.Status == PromptStatus.OK)
            {
                InsertPoint = ppr.Value;
                return SamplerStatus.OK;
            }
            else
            {
                return SamplerStatus.Cancel;
            }
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            
            Matrix3d mat = Matrix3d.Displacement(_Text.Position.GetVectorTo(InsertPoint));
            WorldGeometry geo = draw.Geometry;
            if (geo != null)
            {
                geo.PushModelTransform(mat);
                geo.Draw(_Text);
                foreach (Entity ent in entities)
                    geo.Draw(ent);
                geo.PopModelTransform();
                return true;
            }
            return false;
        }

        public void AddEnt(Entity ent)
        {
            entities.Add(ent);
        }
        public void TransformEntities()
        {
            Matrix3d mat = Matrix3d.Displacement(_Text.Position.GetVectorTo(InsertPoint));
            foreach (Entity ent in entities)
            {
                ent.TransformBy(mat);
            }

        }
        public static void Jig()
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                DBText dBText = new DBText();
                dBText.TextString = "hello word";
                dBText.Height = 10;
                dBText.Position = new Point3d(0, 0, 0);
                btr.AppendEntity(dBText);
                tr.AddNewlyCreatedDBObject(dBText, true);
                Polyline pl = new Autodesk.AutoCAD.DatabaseServices.Polyline();
                pl.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                pl.AddVertexAt(1, new Point2d(0, 20), 0, 0, 0);
                btr.AppendEntity(pl);
                tr.AddNewlyCreatedDBObject(pl, true);

                TextDrawJig dragJig = new TextDrawJig(dBText);
                dragJig.AddEnt(pl);

                for(int i = 0; i < 10; ++i)
                {
                    for(int j = 0; j < 10; ++j)
                    {
                        DBText dBTextTmp = new DBText();
                        dBTextTmp.TextString = "hello word";
                        dBTextTmp.Height = 1;
                        dBTextTmp.Position = new Point3d(10 * i + 10 * j, 10 * i, 0);
                        btr.AppendEntity(dBTextTmp);
                        tr.AddNewlyCreatedDBObject(dBTextTmp, true);
                        dragJig.AddEnt(dBTextTmp);

                    }
                   
                }
                PromptResult pr = ed.Drag(dragJig);
                if (pr.Status == PromptStatus.OK)
                {
                    dragJig.TransformEntities();
                    dBText.Position = dragJig.InsertPoint;
                    tr.Commit();
                }
                else
                {
                    tr.Abort();
                }

            }

        }
    }
}
