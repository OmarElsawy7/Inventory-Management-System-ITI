using Microsoft.AspNetCore.Mvc;
using InventorySystem.Services;
using InventorySystem.ViewModels;

namespace InventorySystem.Controllers
{
    public class AIAssistantController : Controller
    {
        private readonly IAIAssistantService _aiService;
        private readonly ILogger<AIAssistantController> _logger;

        public AIAssistantController(IAIAssistantService aiService, ILogger<AIAssistantController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewModel = new AIAssistantViewModel
            {
                Title = "AI Inventory Assistant",
                Description = "Ask questions about your inventory, sales, purchases, products, suppliers, and stock.",
                SampleQuestions = new List<string>
                {
                    "Which products are currently low in stock?",
                    "What are the most sold products?",
                    "Which products sell the least?",
                    "What are the total sales?",
                    "What is our total revenue?",
                    "How much inventory do we currently have?",
                    "Which supplier has supplied the most products?",
                    "What are the recent sales?",
                    "Which products should we restock?",
                    "Which products generated the most revenue?"
                },
                Response = null,
                IsLoading = false
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Query(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter a question.",
                    response = (string?)null
                });
            }

            try
            {
                var response = await _aiService.ProcessQuestionAsync(question);

                return Json(new
                {
                    success = true,
                    message = "Query processed successfully.",
                    response = response,
                    timestamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing AI query: {ex.Message}");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while processing your question.",
                    response = (string?)null,
                    error = ex.Message
                });
            }
        }
    }
}
