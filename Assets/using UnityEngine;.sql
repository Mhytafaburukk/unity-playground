using UnityEngine;
using System.Data;
using System.Data.SqlClient;
using System;

public class SqlHandler : MonoBehaviour
{

    private string connectionString = "Server=localhost,1433;Database=UnityGameDB;User ID=sa;Password=dockermBk_052064!;TrustServerCertificate=True";

    void Start()
    {
        SavePlayerData("Cengiz", 1500);
    }

    public void SavePlayerData(string pName, int pScore)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                Debug.Log("SQL Bağlantısı Başarılı!");
                string query = "INSERT INTO Players (PlayerName, Score) VALUES (@name, @score)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // SQL Injection önlemek için parametre kullanıyoruz
                    cmd.Parameters.AddWithValue("@name", pName);
                    cmd.Parameters.AddWithValue("@score", pScore);

                    cmd.ExecuteNonQuery();
                    Debug.Log("Veri başarıyla kaydedildi!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SQL Hatası: " + e.Message);
            }
        }
    }
}