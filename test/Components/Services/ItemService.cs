using test.Components.Data;

namespace test.Components.Services
{
    public class ItemService
    {
        private readonly HttpClient _httpClient;

        public ItemService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Item>> GetitemsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7174/api/item/allitems");

                if (response.IsSuccessStatusCode)
                {
                    var items = await response.Content.ReadFromJsonAsync<List<Item>>();

                    if (items == null)
                    {
                        Console.WriteLine("No items found in the response.");
                        return new List<Item>();
                    }

                    return items;
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                    return new List<Item>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Request error: {httpEx.Message}");
                return new List<Item>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return new List<Item>();
            }
        }
        public async Task<string> AddItemAsync(Item item)
        {
            if (item == null)
            {
                Console.WriteLine("Error: Item parameter is null.");
                return "Error: Item cannot be null.";
            }

            try
            {
                Console.WriteLine("Sending request to API...");

                
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7174/api/Item/postitem", item);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API request succeeded.");
                    return "Item submitted successfully!";
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API request failed. StatusCode: {response.StatusCode}, Error: {errorDetails}");
                    return $"Error: {response.StatusCode} - {errorDetails}";
                }
            }
            catch (HttpRequestException httpRequestEx)
            {
                Console.WriteLine($"Network Error: {httpRequestEx.Message}");
                return $"Network Error: {httpRequestEx.Message}";
            }
            catch (TaskCanceledException taskCanceledEx)
            {
                Console.WriteLine($"Request Timeout: {taskCanceledEx.Message}");
                return $"Request Timeout: {taskCanceledEx.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return $"An unexpected error occurred: {ex.Message}";
            }
            finally
            {
                Console.WriteLine("AddItemAsync method execution completed.");
            }
        }
    }
}