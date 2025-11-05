using chatbot.Interfaces;
using chatbot.Models;
using Pgvector;

namespace chatbot.Services;

public class EmbeddingDataService : IEmbeddingDataService
{
    private readonly IEmbeddingRepository _repository;
    private readonly IDataService _dataService;
    private readonly IEmbeddingService _embeddingService;

    public EmbeddingDataService(IEmbeddingRepository repository, IDataService dataService, IEmbeddingService embeddingService)
    {
        _repository = repository;
        _dataService = dataService;
        _embeddingService = embeddingService;
    }

    public async Task UpdateDatabase()
    {
        var datas = _dataService.ReadSpreadsheetAsync();
        var datasSemanticStrings = datas.Select(data => data.ToSemanticString()).ToList();
        var embeddingsResults = await _embeddingService.GetManyEmbeddingsAsync(datasSemanticStrings);
        var embeddingsList = new List<EmbeddingData>();

        for (int i = 0; i < datas.Count; i++)
        {
            embeddingsList.Add(new EmbeddingData
            {
                Id = datas[i].Id,
                OriginalText = datasSemanticStrings[i],
                Vector = new Vector(embeddingsResults[i])
            });
        }

        await _repository.DeleteAllAsync();

        await _repository.CommitAsync();

        await _repository.AddRange(embeddingsList);

        await _repository.CommitAsync();
    }
}


// var _ = _dataService.SemanticTextConvert(_dataService.ReadSpreadsheetAsync());