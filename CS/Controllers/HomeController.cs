﻿using System.Drawing;
using System.IO;
using System.Web.Mvc;
using CS.Model;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;

namespace CS.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return View(new MyViewModel { Categories = MyModel.GetCategories() });
        }
        public ActionResult GridViewPartialCategories() {
            return PartialView(MyModel.GetCategories());
        }

        public ActionResult ExportTo() {
            var ps = new PrintingSystemBase();

            var headerImageLink = new LinkBase(ps);

            headerImageLink.CreateDetailArea += headerImageLink_CreateDetailArea;

            var link1 = new PrintableComponentLinkBase(ps);
            GridViewSettings categoriesGridSettings = new GridViewSettings();
            categoriesGridSettings.Name = "gvCategories";
            categoriesGridSettings.KeyFieldName = "CategoryID";
            categoriesGridSettings.Columns.Add("CategoryID");
            categoriesGridSettings.Columns.Add("CategoryName");
            categoriesGridSettings.Columns.Add("Description");
            link1.Component = GridViewExtension.CreatePrintableObject(categoriesGridSettings, MyModel.GetCategories());

            var compositeLink = new CompositeLinkBase(ps);
            compositeLink.Links.AddRange(new object[] { headerImageLink, link1 });
            compositeLink.CreateDocument();

            FileStreamResult result = CreateExcelExportResult(compositeLink);
            ps.Dispose();

            return result;
        }

        protected void headerImageLink_CreateDetailArea(object sender, CreateAreaEventArgs e) {
            e.Graph.DrawImage(SystemIcons.Application.ToBitmap(), new RectangleF(0, 0, 100, 50));
        }

        protected FileStreamResult CreateExcelExportResult(CompositeLinkBase link) {
            MemoryStream stream = new MemoryStream();
            link.PrintingSystemBase.ExportToXls(stream);
            stream.Position = 0;
            FileStreamResult result = new FileStreamResult(stream, "application/xls");
            result.FileDownloadName = "MyData.xls";
            return result;
        }
    }
}