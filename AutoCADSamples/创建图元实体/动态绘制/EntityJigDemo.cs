using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.动态绘制
{
    public class EntityJigDemo : EntityJig
    {
        private Circle _Circle;
        public double R;
        public EntityJigDemo(Circle c) : base(c)
        {
            _Circle = c;
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jppo = new JigPromptPointOptions();
            jppo.UseBasePoint = true;
            jppo.BasePoint = _Circle.Center;

            PromptPointResult ppr = prompts.AcquirePoint(jppo);
            if (ppr.Status == PromptStatus.OK && ppr.Value.DistanceTo(_Circle.Center) > 10e-10)
            {
                R = ppr.Value.DistanceTo(_Circle.Center);
                return SamplerStatus.OK;
            }
            else if (ppr.Value.DistanceTo(_Circle.Center) < 10e-10)
                return SamplerStatus.NoChange;
            else
                return SamplerStatus.Cancel;
        }

        protected override bool Update()
        {
            try
            {
                _Circle.Radius = R;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Jig()
        {
            //模拟动态绘制圆
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            PromptPointResult ppr = ed.GetPoint("\n选择圆心");
            if(ppr.Status != PromptStatus.OK)
            {
                return;
            }

            using(Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Circle c = new Circle();
                c.Center = ppr.Value;
                c.Radius = 1.0;
                btr.AppendEntity(c);
                tr.AddNewlyCreatedDBObject(c, true);
                EntityJigDemo jig = new EntityJigDemo(c);
                PromptResult pr = ed.Drag(jig);
                if(pr.Status == PromptStatus.OK)
                {
                    c.Radius = jig.R;
                    tr.Commit();
                }
                else
                {
                    c.Dispose();
                    tr.Abort();
                }

            }
            
        }
        [CommandMethod("tsEntityJig")]
        public static void tsEntityJig()
        {
            EntityJigDemo.Jig();
        }
    }
}
