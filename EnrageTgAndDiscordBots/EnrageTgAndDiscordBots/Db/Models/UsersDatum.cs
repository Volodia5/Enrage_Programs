using System;
using System.Collections.Generic;

namespace EnrageTgAndDiscordBots.Models;

public partial class UsersDatum
{
    public int Id { get; set; }

    public string PlayerName { get; set; } = null!;

    public int PlayerRating { get; set; }

    public int PlayerPosition { get; set; }

    public string PlayerTgNick { get; set; } = null!;

    public long ChatId { get; set; }

    public string? PlayerDescription { get; set; }
}
