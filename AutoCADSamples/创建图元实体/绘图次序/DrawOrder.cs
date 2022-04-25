using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Samples.绘图次序
{
    public class DrawOrder
    {
        [CommandMethod("MoveToBottom")]
        public void MoveToBottom()
        {
            //实体后置
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            PromptEntityResult per = ed.GetEntity("");
            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                Entity ent = per.ObjectId.GetObject(OpenMode.ForRead) as Entity;
                BlockTableRecord btr = ent.BlockId.GetObject(OpenMode.ForRead) as BlockTableRecord;
                DrawOrderTable drawOrderTable = btr.DrawOrderTableId.GetObject(OpenMode.ForWrite) as DrawOrderTable;
                drawOrderTable.MoveToBottom(new ObjectIdCollection() { ent.ObjectId });
                tr.Commit();
            }
        }

        [CommandMethod("MoveToTop")]
        public void MoveToTop()
        {
            //实体前置
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            PromptEntityResult per = ed.GetEntity("");
            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                Entity ent = per.ObjectId.GetObject(OpenMode.ForRead) as Entity;
                BlockTableRecord btr = ent.BlockId.GetObject(OpenMode.ForRead) as BlockTableRecord;
                DrawOrderTable drawOrderTable = btr.DrawOrderTableId.GetObject(OpenMode.ForWrite) as DrawOrderTable;
                drawOrderTable.MoveToTop(new ObjectIdCollection() { ent.ObjectId });
                tr.Commit();
            }
        }
    }
}
