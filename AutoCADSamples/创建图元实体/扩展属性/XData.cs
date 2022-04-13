using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.扩展属性
{
    public class XData
    {
        public  bool SetXData(ObjectId objId, string regAppName, string value)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                RegAppTable rat = db.RegAppTableId.GetObject(OpenMode.ForWrite) as RegAppTable;
                if (!rat.Has(regAppName))//所有的扩展数据都存放在指定的应用程序中，所以必须在应用程序表中添加该应用程序记录
                {
                    RegAppTableRecord ratr = new RegAppTableRecord();
                    ratr.Name = regAppName;
                    rat.Add(ratr);
                    tr.AddNewlyCreatedDBObject(ratr, true);
                }

                DBObject dbObj = objId.GetObject(OpenMode.ForWrite) as DBObject;
                if (dbObj == null)
                {
                    tr.Abort();
                    return false;
                }
                ResultBuffer rb = new ResultBuffer();
                rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, regAppName));
                rb.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, value));
                dbObj.XData = rb;
                tr.Commit();
                return true;
            }
        }

        public List<object> GetXdata(ObjectId objId, string regAppName)
        {
            List<object> xdatas = new List<object>();
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                RegAppTable rat = db.RegAppTableId.GetObject(OpenMode.ForRead) as RegAppTable;
                if (!rat.Has(regAppName))
                    return xdatas;
                DBObject dBObject = objId.GetObject(OpenMode.ForRead) as DBObject;
                if (dBObject == null)
                    return xdatas;

                ResultBuffer rb = dBObject.GetXDataForApplication(regAppName);
                if (rb == null)
                    return xdatas;
                for(int i = 1; i < rb.AsArray().Count(); ++i)
                {
                    TypedValue tv = rb.AsArray()[i];
                    xdatas.Add(tv.Value);
                }
                return xdatas;
            }
        }

        public void DeleteXData(string regAppName)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                RegAppTable rat = db.RegAppTableId.GetObject(OpenMode.ForWrite) as RegAppTable;
                if (!rat.Has(regAppName))
                    return;
                //删除应用程序名称后所有使用该名称的扩展属性都会被删除
                RegAppTableRecord ratr = rat[regAppName].GetObject(OpenMode.ForWrite) as RegAppTableRecord;
                ratr.Erase();
                tr.Commit();
            }
        }
        [CommandMethod("SetXData")]
        public void SetXData()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityResult per = ed.GetEntity("");
            if (per.Status != PromptStatus.OK)
                return;
            SetXData(per.ObjectId, "regAppName", "hello");
            SetXData(per.ObjectId, "regAppName", "world");//会覆盖上一次的数据
            SetXData(per.ObjectId, "regAppName", "123");
        }

        [CommandMethod("GetXData")]
        public void GetXData()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityResult per = ed.GetEntity("");
            if (per.Status != PromptStatus.OK)
                return;
            List<object> list = GetXdata(per.ObjectId, "regAppName");
            foreach(object obj in list)
            {
                ed.WriteMessage($"\n{obj.ToString()}");
            }
        }

        [CommandMethod("DeleteXData")]
        public void DeleteXData()
        {
            DeleteXData("regAppName");
        }
    }
}
