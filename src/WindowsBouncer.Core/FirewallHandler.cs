using Microsoft.Extensions.Logging;
using NetFwTypeLib;

namespace WindowsBouncer.Core
{
    public class FirewallHandler : Handler
    {
        private static INetFwPolicy2 _firewallPolicy;
        private readonly ILogger<FirewallHandler> _logger;

        public static INetFwPolicy2 FirewallPolicy 
        { 
            get
            {
                if (_firewallPolicy == null)
                {
                    _firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                }
                return _firewallPolicy;
            }
        }

        public FirewallHandler(ILogger<FirewallHandler> logger)
        {
            _logger = logger;
        }

        public override IList<SecurityEvent> Handle(IList<SecurityEvent> securityEventList)
        {
            var rule = GetFirewallRule() ?? CreateFirewallRule();
            var distinctIPs = securityEventList.DistinctBy(d => d.Ip).Select(d => d.Ip).ToList();
            var existingBlockedIps = new List<string>();

            if (rule.RemoteAddresses != "*")
            {
                existingBlockedIps = rule.RemoteAddresses.Split(',').Select(a => a.Replace("/255.255.255.255", "")).ToList();
            }

            foreach (var ip in distinctIPs)
            {
                if (rule.RemoteAddresses.Equals("*"))
                {
                    rule.RemoteAddresses = $"{ip}/255.255.255.255";
                    rule.Enabled = true;
                    _logger.LogInformation($"Firewall rule is enabled and entry with IP {ip} is added to the firewall rule.");
                }
                else
                {
                    if (existingBlockedIps.Contains(ip))
                    {
                        _logger.LogInformation($"Entry with IP {ip} is already added to the firewall rule, skipping.");
                        securityEventList.Remove(securityEventList.First(e => e.Ip == ip));
                    }
                    else
                    {
                        rule.RemoteAddresses += $",{ip}/255.255.255.255";
                        _logger.LogInformation($"Entry with IP {ip} is added to the firewall rule.");
                    }
                }
            }
            
            return base.Handle(securityEventList);
        }

        private INetFwRule GetFirewallRule()
        {
            return FirewallPolicy.Rules.Cast<INetFwRule>().FirstOrDefault(r => r.Name.Equals(Constants.FIREWALLRULE));
        }

        private INetFwRule CreateFirewallRule()
        {
            var firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            firewallRule.Description = "This rule is managed by the WindowsBouncer security service to block malicious login attempts from outside.";
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallRule.Enabled = false;
            firewallRule.InterfaceTypes = "All";
            firewallRule.Name = Constants.FIREWALLRULE;
            FirewallPolicy.Rules.Add(firewallRule);
            return firewallRule;
        }
    }
}
