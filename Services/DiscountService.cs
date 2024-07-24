using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtlobLap.Services
{
    public interface IDiscountService
    {
        Task AddDiscount(Discount discount);
        Task UpdateDiscount(Discount discount);
        Task DeleteDiscount(int id);
        Task<List<Discount>> GetAllDiscounts();
        Task<Discount> GetDiscountAsync(int id);
        Task ToggleDiscountStatus(int discountId);

    }

    public class DiscountService : IDiscountService
    {
        private readonly AppDbContext _context;

        public DiscountService(AppDbContext context)
        {
            _context = context;
        }

        // Private method to ignore query filters
        private IQueryable<Discount> IgnoreQueryFilters(IQueryable<Discount> query)
        {
            return query.IgnoreQueryFilters();
        }

        public async Task AddDiscount(Discount discount)
        {
            var existingDiscount = await IgnoreQueryFilters(_context.Discounts).FirstOrDefaultAsync(d => d.ProductId == discount.ProductId);
            if (existingDiscount != null)
            {
                _context.Entry(existingDiscount).State = EntityState.Detached;
                discount.Id = existingDiscount.Id;
                await UpdateDiscount(discount);
            }
            else
            {
                await _context.Discounts.AddAsync(discount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateDiscount(Discount discount)
        {
            if (discount != null)
            {
                _context.Discounts.Update(discount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteDiscount(int id)
        {
            var discount = await IgnoreQueryFilters(_context.Discounts).FirstOrDefaultAsync(d=>d.Id == id);

            if (discount != null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Discount>> GetAllDiscounts()
        {
            return await IgnoreQueryFilters(_context.Discounts.Include(d => d.Product)).ToListAsync();
        }

        public async Task<Discount> GetDiscountAsync(int id)
        {
            return await IgnoreQueryFilters(_context.Discounts).FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task ToggleDiscountStatus(int discountId)
        {
            var discount = await GetDiscountAsync(discountId);

            if (discount != null)
            {
                discount.IsActive = !discount.IsActive;

                _context.Discounts.Update(discount);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Discount not found.");
            }
        }
    }
}
