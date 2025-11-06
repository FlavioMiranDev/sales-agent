using chatbot.Data;
using chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Repositories;

public class SalesRepository
{
    private readonly AppDbContext _context;

    public SalesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(List<Sale> sales)
    {
        var allEntities = await _context.Sales.ToListAsync();

        _context.RemoveRange(allEntities);

        await _context.SaveChangesAsync();

        await _context.Sales.AddRangeAsync(sales);

        await _context.SaveChangesAsync();
    }
}
