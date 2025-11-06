using chatbot.Interfaces;
using chatbot.Models;
using chatbot.Repositories;
using Pgvector;

namespace chatbot.Services;

public class EmbeddingDataService : IEmbeddingDataService
{
    private readonly IEmbeddingRepository _embRepository;
    private readonly SalesRepository _salesRepository;
    private readonly IDataService _dataService;
    private readonly IEmbeddingService _embeddingService;

    public EmbeddingDataService(
        IEmbeddingRepository repository,
        IDataService dataService,
        IEmbeddingService embeddingService,
        SalesRepository repo)
    {
        _embRepository = repository;
        _dataService = dataService;
        _embeddingService = embeddingService;
        _salesRepository = repo;
    }

    public async Task UpdateDatabase()
    {
        var datas = _dataService.ReadSpreadsheetAsync();

        await _salesRepository.AddRangeAsync(datas);

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

        await _embRepository.DeleteAllAsync();

        await _embRepository.CommitAsync();

        await _embRepository.AddRange(embeddingsList);

        await _embRepository.CommitAsync();
    }
}


// var _ = _dataService.SemanticTextConvert(_dataService.ReadSpreadsheetAsync());