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
                var existingPromotion = await GetPromotionByIdAsync(id);
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
                var promotion = await GetPromotionByIdAsync(id);
                if (promotion == null) return false;

                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
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

        public async Task<(decimal finalPrice, decimal discountAmount)> CalculateDiscountedPrice(int serviceId, decimal basePrice)
        {
            var activePromotions = await GetPromotionsForService(serviceId);
            if (!activePromotions.Any()) return (basePrice, 0);

            // Get the highest discount percentage from active promotions and ensure it's not null
            var maxDiscount = activePromotions.Max(p => p.DiscountPercent) ?? 0;
            if (maxDiscount == 0) return (basePrice, 0);

            var discountAmount = basePrice * (maxDiscount / 100m);
            var finalPrice = basePrice - discountAmount;
            
            return (finalPrice, discountAmount);
        }

        public async Task<List<Promotion>> GetPromotionsForService(int serviceId)
        {
            var currentDate = DateTime.Now; // Use current date
            
            // Debug: Print the current date and service ID
            Console.WriteLine($"Checking promotions for service {serviceId} at {currentDate}");
            
            var promotions = await _context.Promotions
                .Include(p => p.ServicePromotions)
                .Where(p => p.ServicePromotions.Any(sp => sp.ServiceId == serviceId)
                    && p.StartDate <= currentDate 
                    && p.EndDate >= currentDate)
                .ToListAsync();
            
            // Debug: Print how many promotions were found
            Console.WriteLine($"Found {promotions.Count} active promotions");
            
            return promotions;
        }

        public async Task<bool> IsPromotionValidForService(int promotionId, int serviceId)
        {
            return await _context.ServicePromotions
                .AnyAsync(sp => sp.PromotionId == promotionId && sp.ServiceId == serviceId);
        }
    }
} 