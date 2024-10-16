﻿using System.Collections.Generic;
using _Game.Core.Configs.Models;

namespace _Game.Core.Configs.Repositories.Age
{
    public interface IAgeConfigRepository
    {
        AgeConfig GetAgeConfig(int ageId);
        float GetAgePrice(int ageId);
    }
}