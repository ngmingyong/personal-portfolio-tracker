using System.Net.Http;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using PersonalPortfolioTracker.DTOs.Response;
using PersonalPortfolioTracker.Integrations.Interfaces;

namespace PersonalPortfolioTracker.Integrations.Implementations
{
    public class StockInfoHandler: IStockInfoHandler
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        private readonly TimeSpan cacheLifespan = TimeSpan.FromHours(8);

        private readonly string _stockSearchApiPath = Environment.GetEnvironmentVariable("STOCK_SEARCH_API_PATH") ?? "";
        private readonly string _stockSearchApiToken = Environment.GetEnvironmentVariable("STOCK_SEARCH_API_TOKEN") ?? "";

        public StockInfoHandler(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<StockInfoDto[]?> GetStockInfoAsync(string stockCode, string exchange)
        {

            if (!_cache.TryGetValue(stockCode, out StockInfoDto[]? cachedStockInfo))
            {
                cachedStockInfo = await GetStockInfoFromExternalAPIAsync(stockCode, exchange);

                _cache.Set(stockCode, cachedStockInfo, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheLifespan
                });
            }

            return cachedStockInfo;
        }

        private async Task<StockInfoDto[]?> GetStockInfoFromExternalAPIAsync(string stockCode, string exchange)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_stockSearchApiPath}{stockCode}.{exchange}?api_token={_stockSearchApiToken}&fmt=json");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"External API Error {(int)response.StatusCode}: {response.Content}");
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                StockInfoDto[]? result = JsonSerializer.Deserialize<StockInfoDto[]?>(json, options);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to deserialize external API response: {ex.Message}");
                return null;
            }
        }
    }
}