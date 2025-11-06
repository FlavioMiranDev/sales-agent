namespace chatbot.Models;

public class Sale
{
    public int Id { get; set; }
    public DateOnly Data { get; set; }
    public string Cliente { get; set; }
    public string Produto { get; set; }
    public string Categoria { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Total { get; set; }
    public string Regiao { get; set; }
    public string Observacoes { get; set; }

    public string ToSemanticString()
    {
        return $"No dia {Data:dd/MM/yyyy}, foi realizada uma venda de {Quantidade} unidade(s) de {Produto} " +
           $"para o cliente {Cliente}, na região de {Regiao}. " +
           $"O produto pertence à categoria {Categoria}, com preço unitário de {PrecoUnitario:C}, " +
           $"totalizando {Total:C}. Observações adicionais: {Observacoes}.";
    }
}
