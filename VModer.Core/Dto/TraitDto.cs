﻿using VModer.Core.Models.Character;
using VModer.Core.Services;

namespace VModer.Core.Dto;

public sealed class TraitDto
{
    public required string Name { get; set; }
    public required string LocalizedName { get; set; }
    public required string Modifiers { get; set; }
    public required FileOrigin FileOrigin { get; set; }
    public required TraitType Type { get; set; }
}