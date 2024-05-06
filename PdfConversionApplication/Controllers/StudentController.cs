using Microsoft.AspNetCore.Mvc;
using HtmlToPdfGenerator;
using PdfConversionApplication.Models;

namespace PdfConversionApplication.Controllers
{
    public class StudentController : Controller
    {
        private readonly IHtmlToPdfGenerator _pdfGenerator; // Remove nullable indicator

        /// <summary> 
        /// Inject IHtmlToPdfGenerator dependency through constructor
        /// </summary>
        /// <param name="pdfGenerator"></param>
        public StudentController(IHtmlToPdfGenerator pdfGenerator)
        {
            _pdfGenerator = pdfGenerator;
        }

        /// <summary>
        /// Index action to dispaly the home page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {            
            return View();
        }

        /// <summary>
        /// Download PDF action method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DownloadPdf()
        {
            var models = new StudentInfo
            {
                Id = 101,
                Student_Name = "Amisha",
                Email_Id = "amisha.gupta@gmail.com",
                Standard = 12
            };

            string partialViewPath = "Views/Student/StudentPartialView.cshtml";

            // Generate PDF synchronously
            byte[] pdfBytes = GeneratePdf(models, partialViewPath);

            // Return the generated PDF as a file download
            return File(pdfBytes, "application/pdf", "download.pdf");
        }

        /// <summary> 
        ///Generate PDF method by using the nuget package
        /// </summary>
        /// <param name="studentInfo"></param>
        /// <param name="partialViewPath"></param>
        /// <returns></returns>
        public byte[] GeneratePdf(StudentInfo studentInfo, string partialViewPath)
        {
            // Load the HTML content of the partial view
            string htmlContent = LoadPartialView(partialViewPath);

            // Inject the student information into the HTML content
            htmlContent = InjectStudentInfoIntoHtml(htmlContent, studentInfo);

            // Convert the modified HTML content to PDF
            byte[] pdfBytes = _pdfGenerator.ConvertHtmlToPdf(htmlContent, true, "Student Data");

            return pdfBytes;
        }

        /// <summary>
        /// Load partial view method 
        /// </summary>
        /// <param name="partialViewPath"></param>
        /// <returns></returns>
        private string LoadPartialView(string partialViewPath)
        {
            // Load the HTML content of the partial view
            string htmlContent;
            using (StreamReader reader = new StreamReader(partialViewPath))
            {
                htmlContent = reader.ReadToEnd();
            }
            return htmlContent;
        }

        /// <summary> 
        /// Inject student info into HTML method
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <param name="studentInfo"></param>
        /// <returns></returns>
        private string InjectStudentInfoIntoHtml(string htmlContent, StudentInfo studentInfo)
        {
            // Inject the student information into the HTML content            
            htmlContent = htmlContent.Replace("{{StudentName}}", studentInfo.Student_Name);
            htmlContent = htmlContent.Replace("{{StudentId}}", studentInfo.Id.ToString());
            htmlContent = htmlContent.Replace("{{StudentEmail}}", studentInfo.Email_Id);
            htmlContent = htmlContent.Replace("{{StudentStandard}}", studentInfo.Standard.ToString());
            return htmlContent;
        }
    }
}
