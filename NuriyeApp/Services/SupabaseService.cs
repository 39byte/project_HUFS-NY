using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NuriyeApp.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Supabase.Realtime.PostgresChanges;

namespace NuriyeApp.Services
{
    public class SupabaseService
    {
        private static SupabaseService? _instance;
        public static SupabaseService Instance => _instance ??= new SupabaseService();

        private Supabase.Client? _client;

        public async Task<Supabase.Client> GetClientAsync()
        {
            if (_client != null) return _client;

            var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            using var stream = File.OpenRead(configPath);
            using var doc = await JsonDocument.ParseAsync(stream);
            var sb = doc.RootElement.GetProperty("Supabase");
            var url = sb.GetProperty("Url").GetString()!;
            var key = sb.GetProperty("Key").GetString()!;

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };
            _client = new Supabase.Client(url, key, options);
            await _client.InitializeAsync();
            return _client;
        }

        public async Task<List<Rental>> GetRentalsAsync()
        {
            var client = await GetClientAsync();
            var result = await client.From<Rental>()
                .Select("*")
                .Order("id", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();
            return result.Models;
        }

        public async Task<List<Rental>> GetPendingRentalsAsync()
        {
            var client = await GetClientAsync();
            var result = await client.From<Rental>()
                .Select("*")
                .Filter("상태", Supabase.Postgrest.Constants.Operator.Equals, "대기")
                .Order("id", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();
            return result.Models;
        }

        public async Task<List<Rental>> GetOngoingRentalsAsync()
        {
            var client = await GetClientAsync();
            var result = await client.From<Rental>()
                .Select("*")
                .Filter("상태", Supabase.Postgrest.Constants.Operator.Equals, "확정")
                .Order("id", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();
            return result.Models;
        }

        public async Task UpdateRentalStatusAsync(int id, string status, string staff, string remarks, string? actualReturnDate = null)
        {
            var client = await GetClientAsync();
            if (actualReturnDate != null)
            {
                await client.From<Rental>()
                    .Where(r => r.Id == id)
                    .Set(r => r.Status, status)
                    .Set(r => r.Staff, staff)
                    .Set(r => r.Remarks, remarks)
                    .Set(r => r.ActualReturnDate, actualReturnDate)
                    .Update();
            }
            else
            {
                await client.From<Rental>()
                    .Where(r => r.Id == id)
                    .Set(r => r.Status, status)
                    .Set(r => r.Staff, staff)
                    .Set(r => r.Remarks, remarks)
                    .Update();
            }
        }

        public async Task<List<InventoryItem>> GetInventoryAsync()
        {
            var client = await GetClientAsync();
            var result = await client.From<InventoryItem>().Select("*").Get();
            return result.Models;
        }

        public async Task UpsertInventoryAsync(List<InventoryItem> items)
        {
            var client = await GetClientAsync();
            await client.From<InventoryItem>().Upsert(items);
        }

        public async Task<string> GetSettingAsync(string key)
        {
            var client = await GetClientAsync();
            var result = await client.From<SettingModel>()
                .Select("*")
                .Filter("key", Supabase.Postgrest.Constants.Operator.Equals, key)
                .Get();
            return result.Models.Count > 0 ? result.Models[0].Value : "";
        }

        public async Task UpdateSettingAsync(string key, string value)
        {
            var client = await GetClientAsync();
            await client.From<SettingModel>()
                .Where(s => s.Key == key)
                .Set(s => s.Value, value)
                .Update();
        }

        public async Task<List<Rental>> GetActiveRentalsAsync()
        {
            var all = await GetRentalsAsync();
            return all.FindAll(r => r.Status == "확정" || r.Status == "대여중");
        }

        public async Task SubmitRentalAsync(Rental rental)
        {
            var client = await GetClientAsync();
            await client.From<Rental>().Insert(rental);
        }

        public async Task<bool> CheckConflictAsync(string equipment, string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out var newStart) ||
                !DateTime.TryParse(endDate, out var newEnd))
                return false;

            var all = await GetRentalsAsync();
            foreach (var r in all)
            {
                if (r.Status != "대기" && r.Status != "확정") continue;
                if (!r.Equipment.Contains(equipment) && !equipment.Contains(r.Equipment)) continue;
                if (!DateTime.TryParse(r.StartDate, out var rs) ||
                    !DateTime.TryParse(r.EndDate, out var re)) continue;
                if (newStart <= re && newEnd >= rs) return true;
            }
            return false;
        }

        public async Task SubscribeToNewRentalsAsync(Action<Rental> onNewRental)
        {
            var client = await GetClientAsync();
            var channel = client.Realtime.Channel("realtime", "public", "Rentals");
            channel.Register(new PostgresChangesOptions("public", "Rentals"));
            channel.AddPostgresChangeHandler(
                PostgresChangesOptions.ListenType.Inserts,
                (_, change) =>
                {
                    var rental = change.Model<Rental>();
                    if (rental != null) onNewRental(rental);
                });
            await channel.Subscribe();
        }
    }

    [Table("Settings")]
    public class SettingModel : BaseModel
    {
        [Column("key")]
        public string Key { get; set; } = "";

        [Column("value")]
        public string Value { get; set; } = "";
    }
}
