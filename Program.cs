using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using FirewallManager.data;
using System.Linq;
using Microsoft.Data.SqlClient.Server;
using System.Collections.Generic;

namespace FirewallManager
{
    class Program
    {
        static int Main(string[] args)
        {
            Boolean Verbose = true;
            Boolean AddMode = true;
            String  IpAddr = "";

            if (args.Length < 2 || args.Length > 3) return 8;
            var s = args[0].Trim().ToLower();
            if (s.Length>0 &&(string.Compare(s, "add")==0 || string.Compare(s, "del")==0) )
            {
                AddMode = string.Compare(s, "add") == 0 ;
                if (args.Length == 3)
                {
                    s = args[2].Trim().ToLower();
                    if (s.Length == 0 || string.Compare(s, "-s") != 0) return 8;
                    Verbose = false;
                }
                try
                {
                    IpAddr = args[1].Trim();
                    MainAsync(IpAddr, AddMode, Verbose).Wait();
                    if (Verbose)
                    {
                        if (AddMode) Console.WriteLine(IpAddr + " blocked OK");
                        else Console.WriteLine(IpAddr + " unblocked OK");
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    if (Verbose) Console.WriteLine(ex.Message);
                    return 1;
                }
                
            }
            return 1;
        }

        static async Task MainAsync(String Ipa, Boolean AddMode, Boolean Verbose)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                            .AddJsonFile("appsettings.json", false)
                            .Build();

            var conn   = Configuration.GetConnectionString("DataConnection");
            var server = Configuration["ServerName"];

            {
              var whitelist = new List<string>();
              Configuration.GetSection("WhiteList").Bind(whitelist);
              if (AddMode && whitelist!=null && whitelist.Count>0 && whitelist.Contains(Ipa))
              {
                  throw new Exception(Ipa + " is on the whitelist, you can not block it");
              }
            }

            if (conn != null && !string.IsNullOrWhiteSpace(conn))
            {
                // check if IP Address is already in black list
                var context = new MyContext(conn, server);
                Rule q  = context.Rules
                     .Where(s => s.Server == server && s.Ipaddr == Ipa)
                     .FirstOrDefault<Rule>();
                if (AddMode) { 
                  if (q==null) {
                    // Get last DB inserted ID
                    int? intIdt = context.Rules.Max(s => (int?)s.Id);
                    if (intIdt == null) intIdt = 1;
                    else intIdt++;
                    String rulename = String.Format("FirewallMan Block-{0}", intIdt);
                    try
                    {
                        FirewallConf firewall = new FirewallConf();
                        firewall.AddBlockRule(rulename, Ipa);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    context.Add(new Rule() { Server = server, Ipaddr = Ipa, RuleName = rulename, Processed = true });
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                  }
                  else throw new Exception(Ipa+" is already in the block list");
                }
                else
                {
                  if (q == null) throw new Exception(Ipa + " is not on the block list");
                  else  
                  {
                        // Get rule's name
                        String rulename = q.RuleName;
                        if (rulename.Trim().Length==0) throw new Exception(Ipa + " Rulename is empty");
                        else try
                        {
                            FirewallConf firewall = new FirewallConf();
                            firewall.DeleteRule(rulename);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        context.Rules.Remove(q);
                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            else throw new Exception("db connection is not defined");
        }
    }
}