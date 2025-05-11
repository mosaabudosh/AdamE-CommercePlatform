using Adam.Generic.Enum;
using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.IHttpServices;
using Generic.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using PdfSharp.Pdf.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection.PortableExecutable;

namespace E_Commerce.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        private IExceptionService _exceptionService;
        private IInformationService _informationService;
        private IUserAccessor _userAccessor;
        private IHttpAccessor _httpAccessor;

        protected IExceptionService ExceptionService => _exceptionService ?? (_exceptionService = HttpContext.RequestServices.GetService<IExceptionService>());
        protected IInformationService InformationService => _informationService ?? (_informationService = HttpContext.RequestServices.GetService<IInformationService>());
        protected IUserAccessor UserAccessor => _userAccessor ?? (_userAccessor = HttpContext.RequestServices.GetService<IUserAccessor>());
        protected IHttpAccessor HttpAccessor => _httpAccessor ?? (_httpAccessor = HttpContext.RequestServices.GetService<IHttpAccessor>());


        protected async Task<IActionResult> ExportReportByType(ExportReportParameters exportReportParameters)
        {
            try
            {
                switch (exportReportParameters.ReportType)
                {
                    case (int)ReportType.Pdf:
                        FileContentResult fileContentResultPDF = ToPDF(exportReportParameters.ReportPath, exportReportParameters.DataSources, exportReportParameters.Parameters);
                        return fileContentResultPDF;
                    case (int)ReportType.Excel:
                        FileContentResult fileContentResultExcel = ToExcel(exportReportParameters.ReportPath, exportReportParameters.DataSources, exportReportParameters.Parameters);
                        return fileContentResultExcel;
                    case (int)ReportType.Word:
                        FileContentResult fileContentResultWord = ToWord(exportReportParameters.ReportPath, exportReportParameters.DataSources, exportReportParameters.Parameters);
                        return fileContentResultWord;
                    default:
                        return NoContent();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await ExceptionService.LogErrorAsync(ex, nameof(ExportReportByType), UserAccessor.GetCurrentUserId(), exportReportParameters);
                return BadRequest(ex.Message);
            }
        }

        protected FileContentResult ToPDF(string path, Dictionary<string, object> datasources = null, Dictionary<string, string> parameters = null)
        {
            try
            {
                byte[] pdf = null;
                using (Microsoft.Reporting.NETCore.LocalReport report = new Microsoft.Reporting.NETCore.LocalReport())
                {
                    report.ReportPath = $"{Directory.GetCurrentDirectory()}" + path;
                    if (parameters is not null)
                    {
                        foreach (KeyValuePair<string, string> reportParameter in parameters)
                        {
                            report.SetParameters(new Microsoft.Reporting.NETCore.ReportParameter(reportParameter.Key, reportParameter.Value));
                        }
                    }
                    if (datasources is not null)
                    {
                        foreach (KeyValuePair<string, object> datasource in datasources)
                        {
                            report.DataSources.Add(new Microsoft.Reporting.NETCore.ReportDataSource(datasource.Key, datasource.Value));
                        }
                    }
                    report.Refresh();
                    report.EnableExternalImages = true;
                    pdf = report.Render("PDF");
                }
                return new FileContentResult(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                ExceptionService.LogErrorAsync(ex, nameof(ToPDF), UserAccessor.GetCurrentUserId(), path, datasources, parameters);
                throw new Exception(ex.Message);
            }
        }
   
        protected byte[] ToPDFAsByte(string path, Dictionary<string, object> datasources = null, Dictionary<string, string> parameters = null)
        {
            try
            {
                byte[] pdf;
                using (Microsoft.Reporting.NETCore.LocalReport report = new Microsoft.Reporting.NETCore.LocalReport())
                {
                    report.ReportPath = $"{Directory.GetCurrentDirectory()}{path}";

                    if (parameters is not null)
                    {
                        foreach (KeyValuePair<string, string> reportParameter in parameters)
                        {
                            report.SetParameters(new Microsoft.Reporting.NETCore.ReportParameter(reportParameter.Key, reportParameter.Value));
                        }
                    }
                    if (datasources is not null)
                    {
                        foreach (KeyValuePair<string, object> datasource in datasources)
                        {
                            report.DataSources.Add(new Microsoft.Reporting.NETCore.ReportDataSource(datasource.Key, datasource.Value));
                        }
                    }
                    report.Refresh();
                    report.EnableExternalImages = true;
                    pdf = report.Render("PDF");
                }
                return pdf;
            }
            catch(Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                ExceptionService.LogErrorAsync(ex, nameof(ToPDFAsByte), UserAccessor.GetCurrentUserId(), path, datasources, parameters);
                throw new Exception(ex.Message);
            }
        }

        private FileContentResult ToWord(string path, Dictionary<string, object> datasources = null, Dictionary<string, string> parameters = null)
        {
            try
            {
                byte[] word = null;
                using (Microsoft.Reporting.NETCore.LocalReport report = new Microsoft.Reporting.NETCore.LocalReport())
                {
                    report.ReportPath = $"{Directory.GetCurrentDirectory()}{path}";
                    if (parameters is not null)
                    {
                        foreach (KeyValuePair<string, string> reportParameter in parameters)
                        {
                            report.SetParameters(new Microsoft.Reporting.NETCore.ReportParameter(reportParameter.Key, reportParameter.Value));
                        }
                    }
                    if (datasources is not null)
                    {
                        foreach (KeyValuePair<string, object> datasource in datasources)
                        {
                            report.DataSources.Add(new Microsoft.Reporting.NETCore.ReportDataSource(datasource.Key, datasource.Value));
                        }
                    }
                    report.Refresh();
                    report.EnableExternalImages = true;
                    word = report.Render("WORD");
                }
                return new FileContentResult(word, "application/msword");
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                ExceptionService.LogErrorAsync(ex, nameof(ToWord), UserAccessor.GetCurrentUserId(), path, datasources, parameters);
                throw new Exception(ex.Message);
            }
        }

        private byte[] ToWordAsByte(string path, Dictionary<string, object> datasources = null, Dictionary<string, string> parameters = null)
        {
            try
            {
                byte[] word = null;
                using (Microsoft.Reporting.NETCore.LocalReport report = new Microsoft.Reporting.NETCore.LocalReport())
                {
                    report.ReportPath = $"{Directory.GetCurrentDirectory()}{path}";
                    if (parameters is not null)
                    {
                        foreach (KeyValuePair<string, string> reportParameter in parameters)
                        {
                            report.SetParameters(new Microsoft.Reporting.NETCore.ReportParameter(reportParameter.Key, reportParameter.Value));
                        }
                    }
                    if (datasources is not null)
                    {
                        foreach (KeyValuePair<string, object> datasource in datasources)
                        {
                            report.DataSources.Add(new Microsoft.Reporting.NETCore.ReportDataSource(datasource.Key, datasource.Value));
                        }
                    }
                    report.Refresh();
                    report.EnableExternalImages = true;
                    word = report.Render("WORD");
                }
                return word;
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                ExceptionService.LogErrorAsync(ex, nameof(ToWordAsByte), UserAccessor.GetCurrentUserId(), path, datasources, parameters);
                throw new Exception(ex.Message);
            }
        }
        private FileContentResult ToExcel(string path, Dictionary<string, object> datasources = null, Dictionary<string, string> parameters = null)
        {
            try
            {
                byte[] excel = null;
                using (Microsoft.Reporting.NETCore.LocalReport report = new Microsoft.Reporting.NETCore.LocalReport())
                {
                    report.ReportPath = $"{Directory.GetCurrentDirectory()}{path}";
                    if (parameters is not null)
                    {
                        foreach (KeyValuePair<string, string> reportParameter in parameters)
                        {
                            report.SetParameters(new Microsoft.Reporting.NETCore.ReportParameter(reportParameter.Key, reportParameter.Value));
                        }
                    }
                    if (datasources is not null)
                    {
                        foreach (KeyValuePair<string, object> datasource in datasources)
                        {
                            report.DataSources.Add(new Microsoft.Reporting.NETCore.ReportDataSource(datasource.Key, datasource.Value));
                        }
                    }
                    report.Refresh();
                    report.EnableExternalImages = true;
                    excel = report.Render("EXCEL");
                }
                return new FileContentResult(excel, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                ExceptionService.LogErrorAsync(ex, nameof(ToExcel), UserAccessor.GetCurrentUserId(), path, datasources, parameters);
                throw new Exception(ex.Message);
            }
        }

        private byte[] ToExcelAsByte(string path, Dictionary<string, object> datasources = null, Dictionary<string, string> parameters = null)
        {
            try
            {
                byte[] excel = null;
                using (Microsoft.Reporting.NETCore.LocalReport report = new Microsoft.Reporting.NETCore.LocalReport())
                {
                    report.ReportPath = $"{Directory.GetCurrentDirectory()}{path}";
                    if (parameters is not null)
                    {
                        foreach (KeyValuePair<string, string> reportParameter in parameters)
                        {
                            report.SetParameters(new Microsoft.Reporting.NETCore.ReportParameter(reportParameter.Key, reportParameter.Value));
                        }
                    }
                    if (datasources is not null)
                    {
                        foreach (KeyValuePair<string, object> datasource in datasources)
                        {
                            report.DataSources.Add(new Microsoft.Reporting.NETCore.ReportDataSource(datasource.Key, datasource.Value));
                        }
                    }
                    report.Refresh();
                    report.EnableExternalImages = true;
                    excel = report.Render("EXCEL");
                }
                return excel;
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                ExceptionService.LogErrorAsync(ex, nameof(ToExcelAsByte), UserAccessor.GetCurrentUserId(), path, datasources, parameters);
                throw new Exception(ex.Message);
            }
        }

        protected async Task<BaseResponse<byte[]>> ExportReportByTypeAsByte(ExportReportParameters exportReportParameters)
        {
            try
            {
                switch (exportReportParameters.ReportType)
                {
                    case (int)ReportType.Pdf:
                        byte[] filePDFAsByte = ToPDFAsByte(exportReportParameters.ReportPath, exportReportParameters.DataSources, exportReportParameters.Parameters);
                        return new BaseResponse<byte[]>(true, string.Empty, filePDFAsByte);
                    case (int)ReportType.Excel:
                        byte[] fileExcelAsByte = ToExcelAsByte(exportReportParameters.ReportPath, exportReportParameters.DataSources, exportReportParameters.Parameters);
                        return new BaseResponse<byte[]>(true, string.Empty, fileExcelAsByte);
                    case (int)ReportType.Word:
                        byte[] fileWordAsByte = ToWordAsByte(exportReportParameters.ReportPath, exportReportParameters.DataSources, exportReportParameters.Parameters);
                        return new BaseResponse<byte[]>(true, string.Empty, fileWordAsByte);
                    default:
                        return new BaseResponse<byte[]>(false, "cant export report as byte");
                }

            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await ExceptionService.LogErrorAsync(ex, nameof(ExportReportByType), UserAccessor.GetCurrentUserId(), exportReportParameters);
                return new BaseResponse<byte[]>(false, ex.Message);
            }
        }

        protected byte[] QRCodeGeneratorToByte(string qrCodeModel)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeInfo = qrGenerator.CreateQrCode(qrCodeModel, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeInfo);
                Bitmap qrBitmap = qrCode.GetGraphic(60);
                byte[] bitmapArray = BitmapToByteArray(qrBitmap);
                return bitmapArray;
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                ExceptionService.LogErrorAsync(ex, nameof(QRCodeGeneratorToByte), UserAccessor.GetCurrentUserId(), qrCodeModel);
                throw new Exception(ex.Message);
            }
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                ExceptionService.LogErrorAsync(ex, nameof(BitmapToByteArray), UserAccessor.GetCurrentUserId(), bitmap);
                throw new Exception(ex.Message);
            }
        }

        //public byte[] CombinePDFs(List<byte[]> srcPDFs)
        //{
        //    try
        //    {
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (PdfSharp.Pdf.PdfDocument resultPDF = new PdfSharp.Pdf.PdfDocument(ms))
        //            {
        //                foreach (byte[] pdf in srcPDFs)
        //                {
        //                    using (MemoryStream src = new MemoryStream(pdf))
        //                    {
        //                        using (PdfSharp.Pdf.PdfDocument srcPDF = PdfReader.Open(src, PdfDocumentOpenMode.Import))
        //                        {
        //                            for (int i = 0; i < srcPDF.PageCount; i++)
        //                            {
        //                                resultPDF.AddPage(srcPDF.Pages[i]);
        //                            }
        //                        }
        //                    }
        //                }
        //                resultPDF.Save(ms);
        //                return ms.ToArray();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        while (ex.InnerException is not null)
        //        {
        //            ex = ex.InnerException;
        //        }
        //        ExceptionService.LogErrorAsync(ex, nameof(CombinePDFs), UserAccessor.GetCurrentUserId(), srcPDFs);
        //        throw new Exception(ex.Message);
        //    }
        //}

    }
}
