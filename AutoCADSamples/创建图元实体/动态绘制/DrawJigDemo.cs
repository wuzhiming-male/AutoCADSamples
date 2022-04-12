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

namespace Samples.动态绘制
{
    public class DrawJigDemo : DrawJig
    {
        public List<Entity> _Entities = new List<Entity>();
        private Point3d _BasePoint;
        private Point3d _MovePoint;
        public DrawJigDemo(Point3d basePoint)
        {
            _BasePoint = basePoint;
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jppo = new JigPromptPointOptions("\n指定移动点：");
            jppo.UseBasePoint = true;
            jppo.BasePoint = _BasePoint;


            PromptPointResult ppr = prompts.AcquirePoint(jppo);
            if (ppr.Status == PromptStatus.OK && ppr.Value.DistanceTo(_BasePoint) > 10e-10)
            {
                _MovePoint = ppr.Value;
                return SamplerStatus.OK;
            }
            else
            {
                return SamplerStatus.Cancel;
            }
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            Matrix3d mat = Matrix3d.Displacement(_BasePoint.GetVectorTo(_MovePoint));
            WorldGeometry geo = draw.Geometry;
            if(geo != null)
            {
                geo.PushModelTransform(mat);
                foreach (Entity ent in _Entities)
                    geo.Draw(ent);
                geo.PopModelTransform();
                return true;
            }
            return false;
        }

        public void AddEntity(Entity ent)
        {
            _Entities.Add(ent);
        }

        public void TransformEntities()
        {
            Matrix3d mat = Matrix3d.Displacement(_BasePoint.GetVectorTo(_MovePoint));
            foreach(Entity ent in _Entities)
            {
                ent.TransformBy(mat);
            }
        }
        public static void Jig()
        {
            //平移实体动态显示
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status != PromptStatus.OK)
                return;

            PromptPointResult ppr = ed.GetPoint("\n选择基点");
            if (ppr.Status != PromptStatus.OK)
                return;

            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DrawJigDemo dJig = new DrawJigDemo(ppr.Value);
                foreach (ObjectId oid in psr.Value.GetObjectIds())
                {
                    Entity ent = tr.GetObject(oid, OpenMode.ForWrite) as Entity;
                    dJig.AddEntity(ent);
                }
                PromptResult pr = ed.Drag(dJig);
                if (pr.Status == PromptStatus.OK)
                {
                    dJig.TransformEntities();
                    tr.Commit();
                }
                else
                {
                    tr.Abort();
                }

            }

        }
        [CommandMethod("tsDrawJig")]
        public static void tsDrawJig()
        {
            DrawJigDemo.Jig();
        }
    }
}
