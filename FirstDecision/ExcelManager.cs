using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using ExcelLibrary.SpreadSheet;
using Models;

namespace FirstDecision {
    public static class ExcelManager {
        public static List<FinalProductData> ReadExcelFromMessage(MailMessage message) {
            List<FinalProductData> finalProductData = ExtractProductData(message.Attachments[0].ContentStream);
            return finalProductData;
        }

        private static List<FinalProductData> ExtractProductData(Stream stream) {
            List<FinalProductData> finalProductData = new List<FinalProductData>();
            MemoryStream clone = new MemoryStream();
            clone.SetLength(stream.Length);
            stream.CopyTo(clone);
            stream.Position = 0;
            clone.Position = 0;


            Workbook workbook = Workbook.Load(clone);
            Worksheet worksheet = workbook.Worksheets[0];
            int index = 1;

            while (!worksheet.Cells[index, 0].IsEmpty) {
                string name = worksheet.Cells[index, 1].StringValue;
                int quantity = int.Parse(worksheet.Cells[index, 2].StringValue);
                decimal price = decimal.Parse(worksheet.Cells[index, 3].StringValue);

                finalProductData.Add(new FinalProductData() { Price = price, Product = name, Quantity = quantity });
                index++;
            }

            stream.Position = 0;
            return finalProductData;
        }

        public static Workbook CreateWorkbookFromAssignment(WorkerAssignmentData assignment) {
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("First Sheet");
            worksheet.Cells[0, 0] = new Cell("id");
            worksheet.Cells[0, 1] = new Cell("Product name");
            worksheet.Cells[0, 2] = new Cell("Quantity");
            worksheet.Cells[0, 3] = new Cell("Price");

            for (int i = 0; i < assignment.orders.Count; i++) {
                worksheet.Cells[i + 1, 0] = new Cell(i);
                worksheet.Cells[i + 1, 1] = new Cell(assignment.orders[i].Product);
                worksheet.Cells[i + 1, 2] = new Cell(assignment.orders[i].Quantity);
            }

            workbook.Worksheets.Add(worksheet);

            return workbook;
        }
    }
}
