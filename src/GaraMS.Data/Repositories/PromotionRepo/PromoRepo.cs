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
            var promotions = await _context.Promotions
                .Include(p => p.ServicePromotions)
                    .ThenInclude(sp => sp.Service)
                .ToListAsync();

            foreach (var promotion in promotions)
            {
                foreach (var servicePromotion in promotion.ServicePromotions)
                {
                    if (servicePromotion.Service != null)
                    {
                        servicePromotion.Service.ApplyPromotionDiscount((decimal)promotion.DiscountPercent);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return promotions;
        }

        public async Task<Promotion> GetPromotionByIdAsync(int id)
        {
            return await _context.Promotions
                .Include(p => p.ServicePromotions)
                .FirstOrDefaultAsync(p => p.PromotionId == id);
        }

        public async Task<Promotion> CreatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            return promotion;
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

        public async Task<bool> DeletePromotionAsync(int promotionId)
        {
            try
            {
                // Find the promotion
                var promotion = await _context.Promotions
                    .Include(p => p.ServicePromotions)
                    .FirstOrDefaultAsync(p => p.PromotionId == promotionId);
                    
                if (promotion == null)
                    return false;
                    
                // Get all services associated with this promotion
                var serviceIds = promotion.ServicePromotions.Select(sp => sp.ServiceId).ToList();
                
                // Reset the promotion amount and total price for each service
                foreach (var serviceId in serviceIds)
                {
                    var service = await _context.Services.FindAsync(serviceId);
                    if (service != null)
                    {
                        // Reset promotion to 0
                        service.Promotion = 0;
                        
                        // Recalculate total price without promotion
                        service.TotalPrice = (service.ServicePrice ?? 0) + (service.InventoryPrice ?? 0);
                        service.UpdatedAt = DateTime.Now;
                        
                        _context.Entry(service).State = EntityState.Modified;
                    }
                }
                
                // Remove all service-promotion relationships
                _context.ServicePromotions.RemoveRange(promotion.ServicePromotions);
                
                // Remove the promotion itself
                _context.Promotions.Remove(promotion);
                
                // Save all changes
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting promotion: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Promotion>> GetActivePromotionsAsync()
        {
            var currentDate = DateTime.Now;
            return await _context.Promotions
                .Include(p => p.ServicePromotions)
                    .ThenInclude(sp => sp.Service)
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

        public async Task<bool> ApplyPromotionToServiceAsync(int serviceId, decimal discountPercent)
        {
            try
            {
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null)
                    return false;

                decimal originalPrice = (service.ServicePrice ?? 0) + (service.InventoryPrice ?? 0);
                service.Promotion = (originalPrice * discountPercent) / 100;
                service.TotalPrice = originalPrice - service.Promotion;
                service.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateServicePromotionAsync(int serviceId, int promotionId)
        {
            try
            {
                // Check if service exists
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null)
                    return false;
                    
                // Check if promotion exists
                var promotion = await _context.Promotions.FindAsync(promotionId);
                if (promotion == null)
                    return false;
                    
                // Check if the relationship already exists
                var existingRelation = await _context.ServicePromotions
                    .FirstOrDefaultAsync(sp => sp.ServiceId == serviceId && sp.PromotionId == promotionId);
                    
                if (existingRelation == null)
                {
                    // Create new relationship
                    var servicePromotion = new ServicePromotion
                    {
                        ServiceId = serviceId,
                        PromotionId = promotionId
                    };
                    
                    _context.ServicePromotions.Add(servicePromotion);
                }
                
                // Calculate and apply the promotion
                decimal originalPrice = (service.ServicePrice ?? 0) + (service.InventoryPrice ?? 0);
                decimal discountAmount = (decimal)((originalPrice * promotion.DiscountPercent) / 100);
                
                // Update the service with the promotion amount
                service.Promotion = discountAmount;
                service.TotalPrice = originalPrice - discountAmount;
                service.UpdatedAt = DateTime.Now;
                
                // Save all changes
                await _context.SaveChangesAsync();
                
                // Verify the changes were saved
                var updatedService = await _context.Services.FindAsync(serviceId);
                if (updatedService.Promotion != discountAmount)
                {
                    // If changes weren't saved, try again with explicit update
                    _context.Entry(service).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error creating service promotion: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveServicePromotionAsync(int serviceId)
        {
            try
            {
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null)
                    return false;

                // Reset promotion and recalculate total price
                service.Promotion = 0;
                service.TotalPrice = (service.ServicePrice ?? 0) + (service.InventoryPrice ?? 0);
                service.UpdatedAt = DateTime.Now;

                // Remove the service-promotion relationship
                var servicePromotion = await _context.ServicePromotions
                    .FirstOrDefaultAsync(sp => sp.ServiceId == serviceId);
                if (servicePromotion != null)
                {
                    _context.ServicePromotions.Remove(servicePromotion);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 