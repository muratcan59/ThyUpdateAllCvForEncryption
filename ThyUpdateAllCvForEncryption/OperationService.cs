using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThyUpdateAllCvForEncryption
{
    internal class OperationService
    {
        public void UpdateAllCv()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, true);
            IConfiguration config = builder.Build();
            var connectionString = config.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var allCv = connection.Query($"SELECT HOBI,DERNEK,ENGELLI_ACIKLAMA," +
                $" HOBI_ENCRYPT_MI,DERNEK_ENCRYPT_MI,ENGELLI_ACIKLAMA_ENCRYPT_MI,NUMARA FROM [dbo].[CV] WITH (NOLOCK)" +
                $" WHERE (HOBI IS NOT NULL AND HOBI_ENCRYPT_MI = 0) OR" +
                $" (DERNEK IS NOT NULL AND DERNEK_ENCRYPT_MI = 0) OR" +
                $" (ENGELLI_ACIKLAMA IS NOT NULL AND ENGELLI_ACIKLAMA_ENCRYPT_MI = 0)");
            Console.ForegroundColor = ConsoleColor.Green;
            if (allCv != null)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                foreach (var item in allCv)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(item.HOBI) && !item.HOBI_ENCRYPT_MI)
                        {
                            var encryptionServiceHobi = EncryptionService.Encryption(item.HOBI);
                            if (encryptionServiceHobi != null)
                            {
                                connection.Query("UPDATE [dbo].[CV] SET HOBI=@Hobi ,HOBI_ENCRYPT_MI=@HobiEncryptMi WHERE NUMARA=@Numara", new { Hobi = encryptionServiceHobi, HobiEncryptMi = true, Numara = item.NUMARA });
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"{item.NUMARA} 'lı kişinin cv'sinde ki HOBI satırı encrypt edildi. ");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"{item.NUMARA} 'lı kişinin cv'sinde ki HOBI satırı encrypt edilemedi. ");
                            }
                        }
                        if (!string.IsNullOrEmpty(item.DERNEK) && !item.DERNEK_ENCRYPT_MI)
                        {
                            var encryptionServiceDernek = EncryptionService.Encryption(item.DERNEK);
                            if (encryptionServiceDernek != null)
                            {

                                connection.Query("UPDATE [dbo].[CV] SET DERNEK=@Dernek ,DERNEK_ENCRYPT_MI=@DernekEncryptMi WHERE NUMARA=@Numara", new { Dernek = encryptionServiceDernek, DernekEncryptMi = true, Numara = item.NUMARA });
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"{item.NUMARA} 'lı kişinin cv'sinde ki DERNEK satırı encrypt edildi. ");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"{item.NUMARA} 'lı kişinin cv'sinde ki DERNEK satırı encrypt edilemedi. ");
                            }
                        }
                        if (!string.IsNullOrEmpty(item.ENGELLI_ACIKLAMA) && !item.ENGELLI_ACIKLAMA_ENCRYPT_MI)
                        {
                            var encryptionServiceEngelli = EncryptionService.Encryption(item.ENGELLI_ACIKLAMA);
                            if (encryptionServiceEngelli != null)
                            {
                                connection.Query("UPDATE [dbo].[CV] SET ENGELLI_ACIKLAMA=@EngelliAciklama ,ENGELLI_ACIKLAMA_ENCRYPT_MI=@EngelliAciklamaEncryptMi WHERE NUMARA=@Numara", new { EngelliAciklama = encryptionServiceEngelli, EngelliAciklamaEncryptMi = true, Numara = item.NUMARA }); ;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"{item.NUMARA} 'lı kişinin cv'sinde ki ENGELLI_ACIKLAMA satırı encrypt edildi. ");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"{item.NUMARA} 'lı kişinin cv'sinde ki ENGELLI_ACIKLAMA satırı encrypt edilemedi. ");
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                    }
                }
                
                timer.Stop();
                Console.WriteLine($"{timer.Elapsed.Minutes} dakika {timer.Elapsed.Seconds} saniyede  tamamlandı.");
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Encrypt edilecek veri yok.");
                connection.Close();
            }

        }
    }
}
