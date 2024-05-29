using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_API;
using System.Collections.ObjectModel;
using System.Windows.Shapes;

namespace ClassLibrary.APINetwork
{
    public class DataLog
    {
        private static SqliteConnection connection;

        private static SqliteCommand command;

        private static Dialogservice dialogService;

        private const string Paths = @"Data\roots.sqlite";
        public DataLog()
        {
            connection = new SqliteConnection();
            command = new SqliteCommand();
            dialogService = new Dialogservice();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            try
            {
                connection = new SqliteConnection("Data Source = " + Paths);
                connection.Open();

                const string sqlcommand = "CREATE TABLE IF NOT EXISTS Root_Json(Root_Cca3 varchar(10) PRIMARY KEY, json_data TEXT);";
                command = new SqliteCommand(sqlcommand, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage(e.Message, "Erro");
            }

            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public static Response SaveData(ObservableCollection<Root> roots)
        {
            if (roots == null)

                return new Response
                {
                    Success = false,
                    Message = "Não existe lista de Países!!"
                };

            try
            {

                connection = new SqliteConnection("Data Source = " + Paths);

                connection.Open();


                foreach (var root in roots)
                {
                    const string sql = "INSERT INTO Root_Json (Root_Cca3, json_data) VALUES (@cca3, @json)"; ;
                    
                    command = new SqliteCommand(sql, connection);

                    command.Parameters.Add("@cca3", SqliteType.Text, 3).Value = root.CCA3;
                    command.Parameters.Add("@json", SqliteType.Text).Value = JsonConvert.SerializeObject(root);

                    command.ExecuteNonQuery();

                }

                return new Response
                {
                    Success = true,
                    Message = "Inserido na DataBase, com sucesso!",
                };
            }

            catch (Exception e)
            {
                dialogService.ShowMessage(e.Message, "Erro");
            }

            return new Response
            {
                Success = true,
                Message = "Inserido na DataBase, com sucesso"
            };
        }


        public static Response GetData()
        {
            try
            {
                connection = new SqliteConnection("Data Source =" + Paths);
                connection.Open();

                const string sql = "select json_data FROM Root_Json";

                command = new SqliteCommand(sql, connection);

                SqliteDataReader reader = command.ExecuteReader();

                var separar = "[";
                while (reader.Read())
                {
                    {
                        separar += new string((string)reader["json_data"] + ",");
                    }
                }
                separar += "]";

                if (separar.Length > 0)
                {
                    var paises = JsonConvert.DeserializeObject<ObservableCollection<Root>>(separar);
                    return new Response
                    {
                        Success = true,
                        Message = "Dados lidos com sucesso.",
                        Result = paises
                    };
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessage(e.Message, "Erro");
                return null;
            }
            return new Response
            {
                Success = true,
                Message = "Erro"
            };
        }

        public static Response DeleteData()
        {
            try
            {
                string sql = "DELETE FROM Root_Json";
                command = new SqliteCommand(sql, connection);
                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                dialogService.ShowMessage(e.Message, "Erro");
            }
            return new Response
            {
                Success = true,
                Message = "Erro"
            };
        }

        public async Task<Response> DownloadFlags(ObservableCollection<Root> List, IProgress<int> progress)
        {
            string flagsFolder = @"Flags\Bandeiras.sqlite";

            if (!Directory.Exists(flagsFolder))
            {
                Directory.CreateDirectory(flagsFolder);
            }


            try
            {
                string[] flagsInFolder = Directory.GetFiles(flagsFolder);

                int flagsDownloaded = 0;

                var httpClient = new HttpClient();

                foreach (var lista in List)
                {
                    string flagFile = $"{lista.CCA3}.png";
                    string filePath = System.IO.Path.Combine(flagsFolder, flagFile);

                    if (!File.Exists(filePath))
                    {
                        var stream = await httpClient.GetStreamAsync(lista.Flags.Png);
                        using (var fileStream = File.Create(filePath))
                        {
                            stream.CopyTo(fileStream);
                            flagsDownloaded++;

                            int percentageComplete = flagsDownloaded * 100 / (List.Count - flagsInFolder.Length);

                            progress.Report(percentageComplete);

                        }
                    }
                    lista.Flags.LocalImage = Directory.GetCurrentDirectory() + @"/Bandeiras.sqlite/" + $"{lista.CCA3}.png";
                }

                if (flagsDownloaded > 0)
                {
                    return new Response
                    {
                        Success = true,
                        Message = $"Download concluído: {flagsDownloaded}"
                    };
                }
                else
                {
                    return new Response
                    {
                        Success = true,
                        Message = "Bandeiras já guardadas."
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
