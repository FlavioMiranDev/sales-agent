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
        return $"Venda de ID:{Id} - {Quantidade} {Produto} para cliente \"{Cliente}\", no valor de {PrecoUnitario} por unidade, dando o valor {Total} no total, executada na data {Data}, para a região {Regiao}, com a observação: {Observacoes}";
    }
}
