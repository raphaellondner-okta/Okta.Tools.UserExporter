using Okta.Core;
using Okta.Core.Clients;
using Okta.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Okta.Tools.UserExporter
{
    class Program
    {

        static FileStream fsOutput = null;
        static StreamWriter sw = null;

        static void Main(string[] args)
        {
            string strFileName = ConfigurationManager.AppSettings["OutputFileName"];
            string strOktaUrl = ConfigurationManager.AppSettings["OktaUrl"];
            string strOktaApiKey = ConfigurationManager.AppSettings["OktaApiKey"];

            if (string.IsNullOrEmpty(strFileName))
            {
                string strFileSuffix = DateTime.Now.ToString("yyyyMMdd-hhmmss");
                strFileName = string.Format("OktaUsers_{0}.csv", strFileSuffix);
            }

            string strFilePath = strFileName;

            try
            {
                if (!strFilePath.StartsWith(".\\"))
                {
                    strFilePath = string.Format(".\\{0}", strFilePath);
                }

                fsOutput = new FileStream(strFilePath, FileMode.Create);
                sw = new StreamWriter(fsOutput);

                OktaClient oktaClient = new OktaClient(strOktaApiKey, new Uri(strOktaUrl));
                UsersClient usersClient = oktaClient.GetUsersClient();
                Uri nextPage = null;
                PagedResults<User> users;
                string headerLine = "Id,Login,Status,Created,Activated, Last Login Date, Last Updated Date, Password Changed Date, Status Changed, First Name,Last Name,Email,Secondary Email,Mobile Phone";
                sw.WriteLine(headerLine);

                do
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    users = usersClient.GetList(pageSize: 100, nextPage: nextPage);
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    Console.WriteLine("Getting 100 users in {0} s", ts.TotalSeconds);

                    stopWatch.Reset();
                    stopWatch.Start();
                    foreach (var user in users.Results)
                    {
                        string line = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\"", user.Id, user.Profile.Login, user.Status, user.Created, user.Activated, user.LastLogin, user.LastUpdated, user.PasswordChanged, user.StatusChanged, user.Profile.FirstName, user.Profile.LastName, user.Profile.Email, user.Profile.SecondaryEmail, user.Profile.MobilePhone);
                        List<string> unmappedProperties = user.Profile.GetUnmappedPropertyNames();
                        StringBuilder sb = new StringBuilder();
                        foreach (string unmappedProperty in unmappedProperties)
                        {
                            string sPropValue = user.Profile.GetProperty(unmappedProperty);
                            sPropValue = sPropValue.Replace("\r\n", "");
                            sPropValue = sPropValue.Replace("\"", "\"\"");
                            sb.Append(",\"");
                            sb.Append(sPropValue);
                            sb.Append("\"");
                           
                        }
                        line += sb.ToString();
                        sw.WriteLine(line);
                        sw.Flush();

                    }
                    stopWatch.Stop();
                    ts = stopWatch.Elapsed;
                    Console.WriteLine("Writing 100 users to the file in {0} s", ts.TotalSeconds);

                    nextPage = users.NextPage;
                }
                while (!users.IsLastPage);
                Console.WriteLine("All your users were exported to the file. Please press Enter to exit.");
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("An error occurred: {0}", ex.ToString()));
                Console.ReadLine();
            }
            finally
            {
                if (sw != null)
                    sw.Close();

                if (fsOutput != null)
                    fsOutput.Close();
            }

        }
    }
}
