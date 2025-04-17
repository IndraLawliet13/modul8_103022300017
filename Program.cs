using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

//{
//    "lang": "en",
//    "transfer": {
//        "threshold": "25000000",
//        "low_fee": "6500",
//        "high_fee": "15000"
//    },
//    "methods": ["RTO", "(real-time)", "SKN", "RTGS", "BI", "FAST"],
//    "confirmation": {
//        "en": "yes",
//        "id": "ya"
//    }
//}

public class TransferConfig
{
    [JsonPropertyName("threshold")]
    public string Threshold { get; set; }
    [JsonPropertyName("low_fee")]
    public string LowFee { get; set; }
    [JsonPropertyName("high_fee")]
    public string HighFee { get; set; }
}

public class AppConfig
{
    [JsonPropertyName("lang")]
    public string Lang { get; set; }
    [JsonPropertyName("transfer")]
    public TransferConfig Transfer { get; set; }
    [JsonPropertyName("methods")]
    public List<string> Methods { get; set; }
    [JsonPropertyName("confirmation")]
    public Dictionary<string, string> Confirmation { get; set; }
    public static AppConfig ReadConfigFile(string filePath)
    {
        try
        {
            string jsonString = File.ReadAllText(filePath);
            AppConfig config = JsonSerializer.Deserialize<AppConfig>(jsonString);
            return config ?? CreateDefaultConfig(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading config file: {ex.Message}");
            return CreateDefaultConfig(filePath);
        }
    }
    public static AppConfig CreateDefaultConfig(string filePath)
    {
        var config = new AppConfig();
        config.Lang = "en";
        config.Transfer = new TransferConfig
        {
            Threshold = "25000000",
            LowFee = "6500",
            HighFee = "15000"
        };
        config.Methods = new List<string> { "RTO", "(real-time)", "SKN", "RTGS", "BI", "FAST" };
        config.Confirmation = new Dictionary<string, string>
        {
            { "en", "yes" },
            { "id", "ya" }
        };
        config.SaveConfigFile(filePath);
        return config;
    }
    public void SaveConfigFile(string filePath)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving config file: {ex.Message}");
        }
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        int Fee = 0;
        string jsonFilePath = "bank_transfer_config.json";
        AppConfig config = AppConfig.ReadConfigFile(jsonFilePath);
        if (config.Lang == "en")
        {
            Console.Write("Please insert the amount of money to transfer: ");
        }
        else if (config.Lang == "id")
        {
            Console.Write("Masukkan jumlah uang yang akan di - transfer: ");
        }
        string amountStr = Console.ReadLine();
        if (int.TryParse(amountStr, out int amount) && int.TryParse(config.Transfer.Threshold, out int threshold) && amount <= threshold)
        {
            Fee = int.Parse(config.Transfer.LowFee);
        }
        else
        {
            Fee = int.Parse(config.Transfer.HighFee);
        }
        if (config.Lang == "id")
        {
            Console.WriteLine($"Biaya Transfer = {Fee}\nTotal Biaya = {Fee + amount}\nPilih metode transfer: ");
        }
        else if (config.Lang == "en")
        {
            Console.WriteLine($"Transfer Fee = {Fee}\nTotal Amount = {Fee+amount}\nSelect payment method: ");
        }
        int no = 1;
        foreach (var method in config.Methods)
        {
            Console.WriteLine(no + ". " + method);
            no++;
        }
        if (config.Lang == "id")
        {
            Console.Write("Pilih metode transfer: ");
        }
        else if (config.Lang == "en")
        {
            Console.Write("Select payment method: ");
        }
        string methodStr = Console.ReadLine();
        int.TryParse(methodStr, out int methodIndex);
        if (methodIndex > 0 && methodIndex <= config.Methods.Count)
        {
            string selectedMethod = config.Methods[methodIndex - 1];
            if (config.Lang == "id")
            {
                Console.Write($"Ketik '{config.Confirmation["id"]}' untuk mengkonfirmasi transaksi: ");
                string confirmation = Console.ReadLine();
                if (confirmation == config.Confirmation["id"])
                {
                    Console.WriteLine($"Transaksi berhasil menggunakan metode {selectedMethod}.");
                }
                else
                {
                    Console.WriteLine("Konfirmasi gagal.");
                }
            }
            else if (config.Lang == "en")
            {
                Console.Write($"Please type '{config.Confirmation["en"]}' to confirm the transaction: ");
                string confirmation = Console.ReadLine();
                if (confirmation == config.Confirmation["en"])
                {
                    Console.WriteLine($"Transaction successful using {selectedMethod} method.");
                }
                else
                {
                    Console.WriteLine("Confirmation failed.");
                }
            }
        }else
        {
            if (config.Lang == "id")
            {
                Console.WriteLine("Pilihan tidak valid.");
            }
            else if (config.Lang == "en")
            {
                Console.WriteLine("Invalid choice.");
            }
        }
    }
}