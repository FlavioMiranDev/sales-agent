using chatbot.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace chatbot.Tools;

public class RunSQLTool
{
    private readonly AppDbContext _context;

    public RunSQLTool(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> ExecuteAsync(string sql)
    {
        // Segurança básica — evita instruções de modificação
        if (!sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            return "";
        }

        try
        {
            // Executa a query dinâmica — exemplo usando Dapper-like approach
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();

            var result = new List<Dictionary<string, object?>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                result.Add(row);
            }

            await _context.Database.CloseConnectionAsync();

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"Erro ao executar SQL: {ex.Message}";
        }
    }
}
