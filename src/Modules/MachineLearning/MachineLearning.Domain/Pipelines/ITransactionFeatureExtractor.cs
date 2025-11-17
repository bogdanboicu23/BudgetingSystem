using System;
using System.Threading.Tasks;
using MachineLearning.Domain.Entities;

namespace MachineLearning.Domain.Pipelines;

public interface ITransactionFeatureExtractor
{
    Task<TransactionFeatures> ExtractFeaturesAsync(RawTransactionData rawTransaction);
}

public class RawTransactionData
{
    public Guid Id { get; set; }
    public string Description { get; set; } = "";
    public string MerchantName { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public string? MerchantCategory { get; set; }
}