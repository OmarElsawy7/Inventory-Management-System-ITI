namespace InventorySystem.ViewModels
{
    public class AIAssistantViewModel
    {
        public string Title { get; set; } = "AI Inventory Assistant";
        public string Description { get; set; } = "Ask questions about your inventory, sales, purchases, products, suppliers, and stock.";
        public List<string> SampleQuestions { get; set; } = new();
        public string? Response { get; set; }
        public string? LastQuestion { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
        public bool IsLoading { get; set; }
    }
}
