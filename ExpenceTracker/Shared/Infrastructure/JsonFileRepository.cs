using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExpenceTracker.Shared.Domain;

namespace ExpenceTracker.Shared.Infrastructure
{
    public abstract class JsonFileRepository<T> where T : Entity
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        protected JsonFileRepository(string filePath)
        {
            _filePath = filePath;
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        protected async Task<List<T>> ReadAllAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<T>();
                }

                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<T>();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected async Task WriteAllAsync(List<T> items)
        {
            await _semaphore.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await FileAtomicWriter.WriteAllTextAsync(_filePath, json);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
