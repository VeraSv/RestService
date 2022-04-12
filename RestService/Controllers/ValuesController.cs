using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestApi.Models;
using System.Net.Http;
using System.Configuration;
using System.Data.SqlClient;
using System.Json;
using Microsoft.SqlServer.Management.Smo;


namespace RestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {
        }
        private static readonly HttpClient client = new HttpClient();

        [HttpPost]
        public async Task<string> Post(DataModel data)
        {
            if (data != null)
            {
                try
                {
                    var stringTask = client.GetStringAsync(data.Url);

                    var msg = await stringTask;

                    JsonValue v = JsonValue.Parse(msg);

                    var settings = ConfigurationManager.AppSettings;
                    var allKeys = settings.Keys;
                    foreach (var key in allKeys)
                    {
                        if (v.ContainsKey(key.ToString().ToLower()))
                        {
                            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"];
                            SqlConnection connection = new SqlConnection(connectionString.ToString());
                            var dbExist = new Server(connection.DataSource).Databases.Contains("ApiDatabase");
                           
                            try
                            {
                                if(dbExist == false)
                                {
                                    SqlCommand command = new SqlCommand("CREATE DATABASE ApiDatabase", connection);
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    connection.Open();

                                    try
                                    {
                                        SqlCommand command = new SqlCommand("use ApiDatabase create table PostegrySql (Id int IDENTITY(1,1) PRIMARY KEY , Name varchar(255), Value varchar(255))", connection);
                                        command.ExecuteNonQuery();
                                    }
                                    catch (System.Exception e)
                                    {
                                        continue;

                                    }
                                    finally {
                                       
                                        var value = v[key.ToString().ToLower()];
                                        SqlCommand Command = new SqlCommand("use ApiDatabase insert into PostegrySql (Name, Value) values('" + key.ToString() + "', '" + value.ToString() + "')", connection);
                                        Command.ExecuteNonQuery();
                                    }
                                   
                                }
                            }
                            catch (System.Exception e)
                            {
                                return (e.Message);
                            }
                        }
                    }
                   return ("Ok");
                }
                catch (Exception e)
                {
                    return (e.Message);
                }
            }
            else return ("Uri is empty");
        }
    }
}
