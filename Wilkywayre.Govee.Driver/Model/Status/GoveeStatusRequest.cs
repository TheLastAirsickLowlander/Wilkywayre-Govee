using Wilkywayre.Govee.Driver.Interfaces;

namespace Wilkywayre.Govee.Driver.Model.Status;

public class GoveeStatusRequest : IGoveeCommand
{
    public string GetCommand() => GoveeCommands.Status;
}