using chatbot.Models;

namespace chatbot.Interfaces;

public interface IDataService
{
    List<Sale> ReadSpreadsheetAsync();
    List<string> SemanticTextConvert(List<Sale> sales);
}
