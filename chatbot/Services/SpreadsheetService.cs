using chatbot.Interfaces;
using chatbot.Models;
using OfficeOpenXml;

namespace chatbot.Services;

public class SpreadsheetService : IDataService
{
    public List<Sale> ReadSpreadsheetAsync()
    {
        string pathFile = "Vendas.xlsx";
        var sales = new List<Sale>();

        ExcelPackage.License.SetNonCommercialPersonal("Flavio");

        if (!File.Exists(pathFile))
        {
            throw new FileNotFoundException($"\n=================================================================\nCaminho: {Directory.GetCurrentDirectory()}/{pathFile}\n=================================================================");
        }

        using var package = new ExcelPackage(new FileInfo(pathFile));

        var worksheet = package.Workbook.Worksheets[0];
        int lastRow = worksheet.Dimension.End.Row;

        for (int row = 2; row <= lastRow; row++)
        {
            if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text)) continue;

            var cells = worksheet.Cells;

            sales.Add(new Sale
            {
                Id = cells[row, 1].GetCellValue<int>(),
                Data = cells[row, 2].GetCellValue<DateOnly>(),
                Cliente = cells[row, 3].GetCellValue<string>(),
                Produto = cells[row, 4].GetCellValue<string>(),
                Categoria = cells[row, 5].GetCellValue<string>(),
                Quantidade = cells[row, 6].GetCellValue<int>(),
                PrecoUnitario = cells[row, 7].GetCellValue<decimal>(),
                Total = cells[row, 8].GetCellValue<decimal>(),
                Regiao = cells[row, 9].GetCellValue<string>(),
                Observacoes = cells[row, 10].GetCellValue<string>()
            });
        }

        return sales;
    }

    public List<string> SemanticTextConvert(List<Sale> sales)
    {
        var text = new List<string>();

        foreach (Sale sale in sales)
        {
            text.Add(sale.ToSemanticString());
        }

        return text;
    }
}
