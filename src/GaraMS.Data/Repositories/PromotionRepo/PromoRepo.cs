using GaraMS.Data.Models;
using GaraMS.Data.Repositories.PromotionRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GaraMS.Data.Repository
{
    public class PromoRepo : IPromoRepo
    {
        private readonly GaraManagementSystemContext _context;

        public PromoRepo(GaraManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<List<Promotion>> GetAllPromotionsAsync()
        {
            return await _context.Promotions
                .Include(p => p.ServicePromotions)
                .ToListAsync();
        }

        public async Task<Promotion> GetPromotionByIdAsync(int id)
        {
            return await _context.Promotions
                .Include(p => p.ServicePromotions)
                .FirstOrDefaultAsync(p => p.PromotionId == id);
        }

        public async Task<bool> CreatePromotionAsync(Promotion promotion)
        {
            try
            {
                await _context.Promotions.AddAsync(promotion);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePromotionAsync(int id, Promotion promotion)
        {
            try
            {
                var existingPromotion = await _context.Promotions.FindAsync(id);
                if (existingPromotion == null)
                    return false;

                // Update only specific fields
                existingPromotion.PromotionName = promotion.PromotionName;
                existingPromotion.StartDate = promotion.StartDate;
                existingPromotion.EndDate = promotion.EndDate;
                existingPromotion.DiscountPercent = promotion.DiscountPercent;

                _context.Promotions.Update(existingPromotion);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
    try
    {
        var promotion = await _context.Promotions
            .Include(p => p.ServicePromotions)  // Include related ServicePromotions
            .FirstOrDefaultAsync(p => p.PromotionId == id);

        if (promotion == null)
            return false;

        // Remove all related ServicePromotion records first
        if (promotion.ServicePromotions != null)
        {
            _context.ServicePromotions.RemoveRange(promotion.ServicePromotions);
        }

        // Then remove the promotion
        _context.Promotions.Remove(promotion);
        
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception)
    {
        return false;
    }
        }

        public async Task<List<Promotion>> GetActivePromotionsAsync()
        {
            var currentDate = DateTime.Now;
            return await _context.Promotions
                .Include(p => p.ServicePromotions)
                .Where(p => p.StartDate <= currentDate && p.EndDate >= currentDate)
                .ToListAsync();
        }
    }
} 