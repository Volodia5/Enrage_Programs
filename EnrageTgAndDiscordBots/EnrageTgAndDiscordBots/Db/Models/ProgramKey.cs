using System;
using System.Collections.Generic;

namespace EnrageTgAndDiscordBots.Models;

public partial class ProgramKey
{
    public string Key { get; set; } = null!;

    public DateTime ExpiredTime { get; set; }
}
