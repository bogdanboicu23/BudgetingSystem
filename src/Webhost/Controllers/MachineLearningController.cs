using MachineLearning.Domain.Entities;
using MachineLearning.Domain.Pipelines;
using MachineLearning.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Webhost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineLearningController : ControllerBase
{
    private readonly ITransactionCategorizationService _categorizationService;

    public MachineLearningController(ITransactionCategorizationService categorizationService)
    {
        _categorizationService = categorizationService;
    }

    /// <summary>
    /// Categorize a single transaction using ML algorithms
    /// </summary>
    [HttpPost("categorize")]
    public async Task<ActionResult<TransactionCategorization>> CategorizeTransaction(
        [FromBody] CategorizeTransactionRequest request)
    {
        try
        {
            var rawTransaction = new RawTransactionData
            {
                Id = request.TransactionId,
                Description = request.Description,
                MerchantName = request.MerchantName,
                Amount = request.Amount,
                Date = request.Date,
                Location = request.Location,
                MerchantCategory = request.MerchantCategory
            };

            var result = await _categorizationService.CategorizeTransactionAsync(rawTransaction);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Categorize multiple transactions in batch
    /// </summary>
    [HttpPost("categorize-batch")]
    public async Task<ActionResult<List<TransactionCategorization>>> CategorizeTransactionsBatch(
        [FromBody] BatchCategorizeRequest request)
    {
        try
        {
            var rawTransactions = request.Transactions.Select(t => new RawTransactionData
            {
                Id = t.TransactionId,
                Description = t.Description,
                MerchantName = t.MerchantName,
                Amount = t.Amount,
                Date = t.Date,
                Location = t.Location,
                MerchantCategory = t.MerchantCategory
            });

            var results = await _categorizationService.CategorizeTransactionsBatchAsync(rawTransactions);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Provide user feedback to improve ML models
    /// </summary>
    [HttpPost("feedback")]
    public async Task<ActionResult> ProvideFeedback([FromBody] FeedbackRequest request)
    {
        try
        {
            await _categorizationService.ProvideFeedbackAsync(
                request.TransactionId,
                request.CorrectCategory,
                request.CorrectSubcategory);

            return Ok(new { Message = "Feedback received successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get category suggestions based on partial text
    /// </summary>
    [HttpGet("suggest-categories")]
    public async Task<ActionResult<List<string>>> GetSuggestedCategories([FromQuery] string query)
    {
        if (string.IsNullOrEmpty(query))
            return BadRequest("Query parameter is required");

        var suggestions = await _categorizationService.GetSuggestedCategoriesAsync(query);
        return Ok(suggestions);
    }

    /// <summary>
    /// Train ML models with user-provided data
    /// </summary>
    [HttpPost("train")]
    public async Task<ActionResult> TrainModels([FromBody] TrainModelsRequest request)
    {
        try
        {
            var trainingData = request.TrainingData.Select(t => new TransactionTrainingData
            {
                Features = new TransactionFeatures
                {
                    Description = t.Description,
                    MerchantName = t.MerchantName,
                    Amount = t.Amount,
                    Date = t.Date
                },
                ActualCategory = t.Category,
                ActualSubcategory = t.Subcategory,
                ActualType = t.Type
            });

            await _categorizationService.TrainModelsAsync(trainingData);
            return Ok(new { Message = "Models trained successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Evaluate model performance
    /// </summary>
    [HttpPost("evaluate")]
    public async Task<ActionResult<ModelPerformanceReport>> EvaluateModels(
        [FromBody] EvaluateModelsRequest request)
    {
        try
        {
            var testData = request.TestData.Select(t => new TransactionTrainingData
            {
                Features = new TransactionFeatures
                {
                    Description = t.Description,
                    MerchantName = t.MerchantName,
                    Amount = t.Amount,
                    Date = t.Date
                },
                ActualCategory = t.Category,
                ActualSubcategory = t.Subcategory,
                ActualType = t.Type
            });

            var report = await _categorizationService.EvaluateModelsAsync(testData);
            return Ok(report);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get available categories and subcategories
    /// </summary>
    [HttpGet("categories")]
    public ActionResult<Dictionary<string, List<string>>> GetAvailableCategories()
    {
        return Ok(PredefinedCategories.Categories);
    }
}

// DTOs
public class CategorizeTransactionRequest
{
    public Guid TransactionId { get; set; }
    public string Description { get; set; } = "";
    public string MerchantName { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public string? MerchantCategory { get; set; }
}

public class BatchCategorizeRequest
{
    public List<CategorizeTransactionRequest> Transactions { get; set; } = new();
}

public class FeedbackRequest
{
    public Guid TransactionId { get; set; }
    public string CorrectCategory { get; set; } = "";
    public string CorrectSubcategory { get; set; } = "";
}

public class TrainModelsRequest
{
    public List<TrainingDataItem> TrainingData { get; set; } = new();
}

public class EvaluateModelsRequest
{
    public List<TrainingDataItem> TestData { get; set; } = new();
}

public class TrainingDataItem
{
    public string Description { get; set; } = "";
    public string MerchantName { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; } = "";
    public string Subcategory { get; set; } = "";
    public TransactionType Type { get; set; }
}