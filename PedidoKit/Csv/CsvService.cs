using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using PedidoKit.Models;

namespace PedidoKit.Csv
{
    public class CsvService
    {
        private readonly string _filePath;
        private readonly CsvConfiguration _config;

        public CsvService(string filePath)
        {
            _filePath = filePath;
            _config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                Encoding = System.Text.Encoding.UTF8,
                HasHeaderRecord = true,
            };

            if (!File.Exists(_filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
                File.WriteAllText(_filePath, $"`{nameof(Kit).ToUpper()},{nameof(Kit.Descricao).ToUpper()},{nameof(Kit.Pecas).ToUpper()},{nameof(Kit.Total).ToUpper()}\n");
            }
        }

        public List<Kit> GetAll()
        {
            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, _config);
            var kits = csv.GetRecords<Kit>().ToList();
            for (int i = 0; i < kits.Count; i++)
                kits[i].Id = i + 1;
            return kits;
        }

        public Kit? GetById(int id)
        {
            var kits = GetAll();
            return kits.FirstOrDefault(k => k.Id == id);
        }

        public void Add(Kit newKit)
        {
            var kits = GetAll();
            kits.Add(newKit);
            SaveAll(kits);
        }

        public void Update(int id, Kit updatedKit)
        {
            var kits = GetAll();
            var index = kits.FindIndex(k => k.Id == id);
            if (index == -1)
                throw new Exception("Kit não encontrado");

            kits[index].Nome = updatedKit.Nome;
            kits[index].Descricao = updatedKit.Descricao;
            kits[index].Pecas = updatedKit.Pecas;
            kits[index].Total = updatedKit.Total;
            SaveAll(kits);
        }

        public void Delete(int id)
        {
            var kits = GetAll();
            var kit = kits.FirstOrDefault(k => k.Id == id);
            if (kit == null)
                throw new Exception("Kit não encontrado");

            kits.Remove(kit);
            SaveAll(kits);
        }

        private void SaveAll(List<Kit> kits)
        {
            using var writer = new StreamWriter(_filePath);
            using var csv = new CsvWriter(writer, _config);
            csv.WriteHeader<Kit>();
            csv.NextRecord();

            foreach (var kit in kits)
            {
                csv.WriteField(kit.Nome);
                csv.WriteField(kit.Descricao);
                csv.WriteField(kit.Pecas);
                csv.WriteField(kit.Total);
                csv.NextRecord();
            }
        }
    }
}