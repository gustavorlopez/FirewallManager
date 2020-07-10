using System;
using System.Collections.Generic;
using System.Text;
using NetFwTypeLib;

namespace FirewallManager
{
    public class FirewallConf
    {
        public int AddBlockRule (String RuleName, String RemoteAddress)
        {
            return AddRule(RuleName, "Bloqueo atacantes RDP y SQL", RemoteAddress, "*",
                    NET_FW_ACTION_.NET_FW_ACTION_BLOCK, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN, 
                    NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY);
        }
        public int AddRule(String name, String Description, String RemoteAdresses, String LocalPort,
                  NET_FW_ACTION_ Action, NET_FW_RULE_DIRECTION_ Direction, 
                  NET_FW_IP_PROTOCOL_ Protocole, String ApplicationName = "FirewallManager")
        {
            Type Policy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2", false);
            INetFwPolicy2 FwPolicy = (INetFwPolicy2)Activator.CreateInstance(Policy2);
            INetFwRules rules = FwPolicy.Rules;
            //Delete if exist to avoid deplicated rules
            DeleteRule(name);
            Type RuleType = Type.GetTypeFromProgID("HNetCfg.FWRule");
            INetFwRule rule = (INetFwRule)Activator.CreateInstance(RuleType);
            rule.Name = name;
            rule.Description = Description;
            rule.Protocol = (int) Protocole;
            if (LocalPort!="*") rule.LocalPorts = LocalPort;
            rule.RemoteAddresses = RemoteAdresses;
            rule.Action = Action;
            rule.Direction = Direction;
            rule.ApplicationName = ApplicationName;
            rule.Grouping = "A1";
            rule.Enabled = true;
            try
            { 
                rules.Add(rule);
                return 0;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public void DeleteRule(String RuleName)
        {
            Type Policy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2", false);
            INetFwPolicy2 FwPolicy = (INetFwPolicy2)Activator.CreateInstance(Policy2);
            INetFwRules rules = FwPolicy.Rules;
            try
            {
              rules.Remove(RuleName);
            }
            catch
            {

            }
        }
    }
}
