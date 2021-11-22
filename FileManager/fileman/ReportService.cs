using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Docx;

namespace fileman
{
    public sealed class ReportService
    {
        public bool GenerateReport(string[,] drivesInfo)
        {
            if (drivesInfo is null)
            {
                return false;
            }
            string templateFile;
            string outputFile;
            try
            {
                string dir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                dir = Path.GetDirectoryName(dir);
                string[] fileNames = File.ReadAllLines(dir + "\\" + FMConstants.reportConfigFileName);
                templateFile = fileNames[0];
                outputFile = fileNames[1];
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                File.Copy(templateFile, outputFile);
            }
            catch (Exception)
            {
                return false;
            }

            List<TableRowContent> rows = new List<TableRowContent>();

            int n = drivesInfo.GetUpperBound(1) + 1;
            for (int i = 0; i < n; i++)
            {
                rows.Add(new TableRowContent(new List<FieldContent>()
                {
                    new FieldContent("Name", drivesInfo[FMConstants.nName, i]),
                    new FieldContent("Type", drivesInfo[FMConstants.nType, i]),
                    new FieldContent("Format", drivesInfo[FMConstants.nFormat, i]),
                    new FieldContent("Label", drivesInfo[FMConstants.nLabel, i]),
                    new FieldContent("Total", drivesInfo[FMConstants.nTotal, i]),
                    new FieldContent("Free", drivesInfo[FMConstants.nFree, i]),
                    new FieldContent("Available", drivesInfo[FMConstants.nAvailable, i]),
                }));
            }

            var valuesToFill = new Content(
                TableContent.Create("Drives", rows),
                new FieldContent("Student Name", "Evgenii Molchkov"));
            try
            {
                using (var outputDocument = new TemplateProcessor(outputFile).SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();
                }
            }
            catch (Exception)
            {
                   return false;
            }
            return true;
        }
    }
}
