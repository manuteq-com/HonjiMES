//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HonjiMES.Reports {
    
    public partial class XtraReporttest : DevExpress.XtraReports.UI.XtraReport {
        private void InitializeComponent() {
            DevExpress.XtraReports.ReportInitializer reportInitializer = new DevExpress.XtraReports.ReportInitializer(this, "HonjiMES.Reports.XtraReport1.repx");

            // Controls
            this.Detail = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail");
            this.TopMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.TopMarginBand>("TopMargin");
            this.BottomMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.BottomMarginBand>("BottomMargin");
            this.GroupHeader2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupHeaderBand>("GroupHeader2");
            this.GroupFooter1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupFooterBand>("GroupFooter1");
            this.GroupHeader1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupHeaderBand>("GroupHeader1");
            this.vendorLogo = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPictureBox>("vendorLogo");
            this.xrLine3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLine>("xrLine3");
            this.thankYouLabel = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("thankYouLabel");
            this.xrLine2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLine>("xrLine2");
            this.headerTable = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("headerTable");
            this.headerTableRow = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("headerTableRow");
            this.productNameCaption = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("productNameCaption");
            this.quantityCaption = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("quantityCaption");
            this.unitPriceCaption = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("unitPriceCaption");
            this.lineTotalCaption = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("lineTotalCaption");
            this.table1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table1");
            this.tableRow1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow1");
            this.tableCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell1");
            this.tableCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell3");
            this.tableCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell2");
            this.label1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label1");

            // Parameters
            this.testa = reportInitializer.GetParameter("testa");

            // Data Sources
            this.jsonDataSource1 = reportInitializer.GetDataSource<DevExpress.DataAccess.Json.JsonDataSource>("jsonDataSource1");

            // Styles
            this.baseControlStyle = reportInitializer.GetStyle("baseControlStyle");
            this.simpleTextStyle = reportInitializer.GetStyle("simpleTextStyle");
            this.captionsStyle = reportInitializer.GetStyle("captionsStyle");
        }
        private DevExpress.DataAccess.Json.JsonDataSource jsonDataSource1;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader2;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter1;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRPictureBox vendorLogo;
        private DevExpress.XtraReports.UI.XRLine xrLine3;
        private DevExpress.XtraReports.UI.XRLabel thankYouLabel;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow headerTableRow;
        private DevExpress.XtraReports.UI.XRTableCell productNameCaption;
        private DevExpress.XtraReports.UI.XRTableCell quantityCaption;
        private DevExpress.XtraReports.UI.XRTableCell unitPriceCaption;
        private DevExpress.XtraReports.UI.XRTableCell lineTotalCaption;
        private DevExpress.XtraReports.UI.XRControlStyle baseControlStyle;
        private DevExpress.XtraReports.UI.XRControlStyle simpleTextStyle;
        private DevExpress.XtraReports.UI.XRControlStyle captionsStyle;
        private DevExpress.XtraReports.UI.XRTable table1;
        private DevExpress.XtraReports.UI.XRTableRow tableRow1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell3;
        private DevExpress.XtraReports.Parameters.Parameter testa;
        private DevExpress.XtraReports.UI.XRTableCell tableCell2;
        private DevExpress.XtraReports.UI.XRLabel label1;
    }
}