using Microsoft.EntityFrameworkCore;
using InventorySystem.Models;
using System.Text.RegularExpressions;

namespace InventorySystem.Services
{
    public interface IAIAssistantService
    {
        Task<string> ProcessQuestionAsync(string question);
        Task<string> DetectIntentAndQueryAsync(string question);
    }

    public class AIAssistantService : IAIAssistantService
    {
        private readonly InventoryContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIAssistantService> _logger;

        public AIAssistantService(InventoryContext context, IConfiguration configuration, ILogger<AIAssistantService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> ProcessQuestionAsync(string question)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(question))
                {
                    return "Please ask me a question about your inventory, products, sales, purchases, or suppliers.";
                }

                return await DetectIntentAndQueryAsync(question);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing question: {ex.Message}");
                return "I couldn't retrieve the information right now. Please try again.";
            }
        }

        public async Task<string> DetectIntentAndQueryAsync(string question)
        {
            var normalizedQuestion = NormalizeQuestion(question);

            // Low Stock Queries
            if (IsPossibleLowStockQuery(normalizedQuestion))
            {
                return await GetLowStockProductsAsync();
            }

            // Most Sold / Best Sellers
            if (IsPossibleMostSoldQuery(normalizedQuestion))
            {
                return await GetMostSoldProductsAsync();
            }

            // Lowest Selling Products
            if (IsPossibleLowestSellingQuery(normalizedQuestion))
            {
                return await GetLowestSellingProductsAsync();
            }

            // Total Sales / Revenue
            if (IsPossibleTotalSalesQuery(normalizedQuestion))
            {
                return await GetTotalSalesAsync();
            }

            // Total Inventory
            if (IsPossibleTotalInventoryQuery(normalizedQuestion))
            {
                return await GetTotalInventoryAsync();
            }

            // Top Supplier
            if (IsPossibleTopSupplierQuery(normalizedQuestion))
            {
                return await GetTopSupplierAsync();
            }

            // Recent Sales
            if (IsPossibleRecentSalesQuery(normalizedQuestion))
            {
                return await GetRecentSalesAsync();
            }

            // Highest Revenue Products
            if (IsPossibleHighestRevenueQuery(normalizedQuestion))
            {
                return await GetHighestRevenueProductsAsync();
            }

            // Restock Recommendations
            if (IsPossibleRestockQuery(normalizedQuestion))
            {
                return await GetRestockRecommendationsAsync();
            }

            return "I can help you with questions about low-stock products, best-selling items, total sales, recent sales, top suppliers, inventory totals, revenue analysis, and restocking recommendations. Try asking something like: 'Which products are low in stock?' or 'What are the most sold products?'";
        }

        private async Task<string> GetLowStockProductsAsync()
        {
            var lowStockProducts = await _context.Products
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();

            if (!lowStockProducts.Any())
            {
                return "Great news! All products have sufficient stock levels.";
            }

            var response = $"There are currently {lowStockProducts.Count} products with low stock:\n\n";
            foreach (var product in lowStockProducts)
            {
                response += $"• {product.ProductName} - Current Stock: {product.StockQuantity} units (Threshold: {product.LowStockThreshold})\n";
            }

            return response.TrimEnd();
        }

        private async Task<string> GetMostSoldProductsAsync()
        {
            var mostSoldProducts = await _context.SaleItems
                .GroupBy(si => si.Product.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(si => si.Quantity),
                    TotalRevenue = g.Sum(si => si.Quantity * si.UnitPrice)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();

            if (!mostSoldProducts.Any())
            {
                return "No sales data is available yet.";
            }

            var response = "The most sold products are:\n\n";
            int rank = 1;
            foreach (var product in mostSoldProducts)
            {
                response += $"{rank}. {product.ProductName} - {product.TotalQuantity} units sold, ${product.TotalRevenue:F2} revenue\n";
                rank++;
            }

            return response.TrimEnd();
        }

        private async Task<string> GetLowestSellingProductsAsync()
        {
            var lowestSellingProducts = await _context.SaleItems
                .GroupBy(si => si.Product.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(si => si.Quantity)
                })
                .OrderBy(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();

            if (!lowestSellingProducts.Any())
            {
                return "No sales data is available yet.";
            }

            var response = "The lowest selling products are:\n\n";
            int rank = 1;
            foreach (var product in lowestSellingProducts)
            {
                response += $"{rank}. {product.ProductName} - {product.TotalQuantity} units sold\n";
                rank++;
            }

            return response.TrimEnd();
        }

        private async Task<string> GetTotalSalesAsync()
        {
            var totalSalesCount = await _context.Sales.CountAsync();
            var totalRevenue = await _context.Sales.SumAsync(s => (decimal?)s.TotalAmount) ?? 0;
            var totalQuantitySold = await _context.SaleItems.SumAsync(si => (int?)si.Quantity) ?? 0;

            var response = $"Sales Summary:\n\n";
            response += $"• Total Sales Transactions: {totalSalesCount}\n";
            response += $"• Total Quantity Sold: {totalQuantitySold} units\n";
            response += $"• Total Revenue: ${totalRevenue:F2}";

            return response;
        }

        private async Task<string> GetTotalInventoryAsync()
        {
            var totalQuantity = await _context.Products.SumAsync(p => (int?)p.StockQuantity) ?? 0;
            var totalProducts = await _context.Products.CountAsync();
            var averageStockPerProduct = totalProducts > 0 ? (decimal)totalQuantity / totalProducts : 0;

            var response = $"Inventory Summary:\n\n";
            response += $"• Total Stock Quantity: {totalQuantity} units\n";
            response += $"• Total Product Types: {totalProducts}\n";
            response += $"• Average Stock per Product: {averageStockPerProduct:F1} units";

            return response;
        }

        private async Task<string> GetTopSupplierAsync()
        {
            var topSupplier = await _context.PurchaseItems
                .GroupBy(pi => pi.Purchase.Supplier.SupplierName)
                .Select(g => new
                {
                    SupplierName = g.Key,
                    TotalQuantity = g.Sum(pi => pi.Quantity),
                    TotalValue = g.Sum(pi => pi.Quantity * pi.UnitCost)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .FirstOrDefaultAsync();

            if (topSupplier == null)
            {
                return "No purchase data is available yet.";
            }

            var response = $"Top Supplier:\n\n";
            response += $"• Supplier Name: {topSupplier.SupplierName}\n";
            response += $"• Total Quantity Supplied: {topSupplier.TotalQuantity} units\n";
            response += $"• Total Purchase Value: ${topSupplier.TotalValue:F2}";

            return response;
        }

        private async Task<string> GetRecentSalesAsync()
        {
            var recentSales = await _context.Sales
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .ToListAsync();

            if (!recentSales.Any())
            {
                return "No sales data is available yet.";
            }

            var response = "Recent Sales:\n\n";
            int rank = 1;
            foreach (var sale in recentSales)
            {
                response += $"{rank}. Date: {sale.SaleDate:MM/dd/yyyy HH:mm}, Amount: ${sale.TotalAmount:F2}\n";
                rank++;
            }

            return response.TrimEnd();
        }

        private async Task<string> GetHighestRevenueProductsAsync()
        {
            var highestRevenueProducts = await _context.SaleItems
                .GroupBy(si => si.Product.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalRevenue = g.Sum(si => si.Quantity * si.UnitPrice),
                    TotalQuantity = g.Sum(si => si.Quantity)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(5)
                .ToListAsync();

            if (!highestRevenueProducts.Any())
            {
                return "No sales data is available yet.";
            }

            var response = "Products with Highest Revenue:\n\n";
            int rank = 1;
            foreach (var product in highestRevenueProducts)
            {
                response += $"{rank}. {product.ProductName} - ${product.TotalRevenue:F2} revenue ({product.TotalQuantity} units)\n";
                rank++;
            }

            return response.TrimEnd();
        }

        private async Task<string> GetRestockRecommendationsAsync()
        {
            var restockCandidates = await _context.Products
                .Where(p => p.StockQuantity <= p.LowStockThreshold * 1.5M) // 1.5x threshold
                .OrderBy(p => p.StockQuantity)
                .Take(5)
                .ToListAsync();

            if (!restockCandidates.Any())
            {
                return "No immediate restocking is needed. All products have adequate stock levels.";
            }

            var response = "Restocking Recommendations:\n\n";
            foreach (var product in restockCandidates)
            {
                var reason = product.StockQuantity <= product.LowStockThreshold ? "LOW STOCK" : "APPROACHING LOW STOCK";
                response += $"• {product.ProductName} - Current: {product.StockQuantity} units ({reason})\n";
            }

            return response.TrimEnd();
        }

        private string NormalizeQuestion(string question)
        {
            // Convert to lowercase and remove extra spaces
            var normalized = Regex.Replace(question.ToLower(), @"\s+", " ").Trim();
            return normalized;
        }

        private bool IsPossibleLowStockQuery(string normalizedQuestion)
        {
            var keywords = new[] { "low stock", "low inventory", "running low", "need restock", "out of stock", "below threshold", "low on", "running short" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }

        private bool IsPossibleMostSoldQuery(string normalizedQuestion)
        {
            var keywords = new[] { "most sold", "best sell", "top sell", "most popular", "sell the most", "sold the most", "best seller", "highest selling", "what sell" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }

        private bool IsPossibleLowestSellingQuery(string normalizedQuestion)
        {
            var keywords = new[] { "lowest sell", "least sell", "slowest sell", "slow sell", "barely sell", "minimum sell", "lowest demand" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }

        private bool IsPossibleTotalSalesQuery(string normalizedQuestion)
        {
            var keywords = new[] { "total sales", "total revenue", "how much sell", "how much revenue", "total amount", "overall sales" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }

        private bool IsPossibleTotalInventoryQuery(string normalizedQuestion)
        {
            var keywords = new[] { "total inventory", "total stock", "how much inventory", "inventory total", "stock quantity", "total quantity" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }

        private bool IsPossibleTopSupplierQuery(string normalizedQuestion)
        {
            var keywords = new[] { "top supplier", "best supplier", "supplier most", "largest supplier", "main supplier", "highest supplier" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }

        private bool IsPossibleRecentSalesQuery(string normalizedQuestion)
        {
            var keywords = new[] { "recent sales", "latest sales", "recent order", "last sales", "new sales" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }

        private bool IsPossibleHighestRevenueQuery(string normalizedQuestion)
        {
            var keywords = new[] { "highest revenue", "revenue product", "top revenue", "most revenue", "best revenue" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }

        private bool IsPossibleRestockQuery(string normalizedQuestion)
        {
            var keywords = new[] { "restock", "replenish", "reorder", "should order", "need order" };
            return keywords.Any(k => normalizedQuestion.Contains(k));
        }
    }
}
