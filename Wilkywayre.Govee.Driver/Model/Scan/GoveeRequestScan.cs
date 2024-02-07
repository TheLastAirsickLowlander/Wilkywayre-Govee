namespace Wilkywayre.Govee.Driver.Model;

public class GoveeRequestScan 
{
    public string cmd => "scan";
    public AccountTopic data { get; set; } = new AccountTopic();
}
public class AccountTopic
{
    public string account_topic => "reserve";
}
