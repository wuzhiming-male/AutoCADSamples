using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
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
            ed.GetSelection();
        }

        public SelectionFilter GetFilter()
        {
            TypedValue[] typedValues = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Operator, "<ADN"),
                new TypedValue((int)DxfCode.Operator, "<ADN")
            };
            SelectionFilter filter = new SelectionFilter();

        }
    }
}
