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
    
    public partial class XtraReport1 : DevExpress.XtraReports.UI.XtraReport {
        private void InitializeComponent() {
            DevExpress.XtraReports.ReportInitializer reportInitializer = new DevExpress.XtraReports.ReportInitializer(this, "HonjiMES.Reports.process.repx");

            // Controls
            this.Detail = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail");
            this.TopMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.TopMarginBand>("TopMargin");
            this.BottomMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.BottomMarginBand>("BottomMargin");
            this.GroupHeader1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupHeaderBand>("GroupHeader1");
            this.detailTable = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("detailTable");
            this.detailTableRow = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("detailTableRow");
            this.tableRow5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow5");
            this.SerialNumber = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("SerialNumber");
            this.ProcessName = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("ProcessName");
            this.ProducingMachine = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("ProducingMachine");
            this.Status = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("Status");
            this.Type = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("Type");
            this.ReCount = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("ReCount");
            this.ActualStartTime = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("ActualStartTime");
            this.ActualEndTime = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("ActualEndTime");
            this.tableCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell2");
            this.pictureBox1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPictureBox>("pictureBox1");
            this.label1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label1");
            this.pageInfo1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo1");
            this.table1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table1");
            this.Qrcode = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPictureBox>("Qrcode");
            this.tableRow2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow2");
            this.tableRow4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow4");
            this.tableRow3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow3");
            this.tableCell7 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell7");
            this.tableCell8 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell8");
            this.tableCell11 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell11");
            this.tableCell12 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell12");
            this.tableCell9 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell9");
            this.tableCell10 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell10");
            this.headerTable = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("headerTable");
            this.headerTableRow = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("headerTableRow");
            this.productNameCaption = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("productNameCaption");
            this.quantityCaption = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("quantityCaption");
            this.unitPriceCaption = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("unitPriceCaption");
            this.lineTotalCaptionCell = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("lineTotalCaptionCell");
            this.tableCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell3");
            this.tableCell4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell4");
            this.tableCell5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell5");
            this.tableCell6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell6");
            this.PageFooter = reportInitializer.GetControl<DevExpress.XtraReports.UI.PageFooterBand>("PageFooter");

            // Parameters
            this.WorkOrderNo = reportInitializer.GetParameter("WorkOrderNo");
            this.DataNo = reportInitializer.GetParameter("DataNo");
            this.DataName = reportInitializer.GetParameter("DataName");

            // Data Sources
            this.jsonDataSource1 = reportInitializer.GetDataSource<DevExpress.DataAccess.Json.JsonDataSource>("jsonDataSource1");

            // Styles
            this.baseControlStyle = reportInitializer.GetStyle("baseControlStyle");
        }
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow detailTableRow;
        private DevExpress.XtraReports.UI.XRTableRow tableRow5;
        private DevExpress.XtraReports.UI.XRTableCell SerialNumber;
        private DevExpress.XtraReports.UI.XRTableCell ProcessName;
        private DevExpress.XtraReports.UI.XRTableCell ProducingMachine;
        private DevExpress.XtraReports.UI.XRTableCell Status;
        private DevExpress.XtraReports.UI.XRTableCell Type;
        private DevExpress.XtraReports.UI.XRTableCell ReCount;
        private DevExpress.XtraReports.UI.XRTableCell ActualStartTime;
        private DevExpress.XtraReports.UI.XRTableCell ActualEndTime;
        private DevExpress.XtraReports.UI.XRTableCell tableCell2;
        private DevExpress.XtraReports.UI.XRPictureBox pictureBox1;
        private DevExpress.XtraReports.UI.XRLabel label1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRTable table1;
        private DevExpress.XtraReports.UI.XRPictureBox Qrcode;
        private DevExpress.XtraReports.UI.XRTableRow tableRow2;
        private DevExpress.XtraReports.UI.XRTableRow tableRow4;
        private DevExpress.XtraReports.UI.XRTableRow tableRow3;
        private DevExpress.XtraReports.UI.XRTableCell tableCell7;
        private DevExpress.XtraReports.UI.XRTableCell tableCell8;
        private DevExpress.XtraReports.UI.XRTableCell tableCell11;
        private DevExpress.XtraReports.UI.XRTableCell tableCell12;
        private DevExpress.XtraReports.UI.XRTableCell tableCell9;
        private DevExpress.XtraReports.UI.XRTableCell tableCell10;
        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow headerTableRow;
        private DevExpress.XtraReports.UI.XRTableCell productNameCaption;
        private DevExpress.XtraReports.UI.XRTableCell quantityCaption;
        private DevExpress.XtraReports.UI.XRTableCell unitPriceCaption;
        private DevExpress.XtraReports.UI.XRTableCell lineTotalCaptionCell;
        private DevExpress.XtraReports.UI.XRTableCell tableCell3;
        private DevExpress.XtraReports.UI.XRTableCell tableCell4;
        private DevExpress.XtraReports.UI.XRTableCell tableCell5;
        private DevExpress.XtraReports.UI.XRTableCell tableCell6;
        private DevExpress.DataAccess.Json.JsonDataSource jsonDataSource1;
        private DevExpress.XtraReports.UI.XRControlStyle baseControlStyle;
        private DevExpress.XtraReports.Parameters.Parameter WorkOrderNo;
        private DevExpress.XtraReports.Parameters.Parameter DataNo;
        private DevExpress.XtraReports.Parameters.Parameter DataName;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
    }
}
