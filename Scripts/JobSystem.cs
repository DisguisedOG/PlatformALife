using Godot;
using System;
using System.Collections.Generic;

public static class JobSystem
{
    private static readonly Dictionary<string, List<string>> DefaultJobSkills = new()
    {
        { "Warrior", new List<string> { "Shield Bash", "Heavy Strike", "Tūwaewae Stance" } },
        { "Archer", new List<string> { "Precision Shot", "Rapid Fire", "Eagle Eye" } },
        { "Bandit", new List<string> { "Shadow Step", "Poison Trap", "Gold Swipe" } },
        { "Healer", new List<string> { "Regen Pulse", "Mana Flow", "Aether Ward" } }
    };

    private static readonly string[] DefaultBeneficiaries = new[]
    {
        "Warrior",
        "Archer",
        "Bandit",
        "Healer"
    };

    private const int RonLockoutDays = 30;

    public static void InitializeJobData(SaveData data)
    {
        if (data.JobXP == null)
        {
            data.JobXP = new Dictionary<string, int>();
        }

        if (data.JobLevels == null)
        {
            data.JobLevels = new Dictionary<string, int>();
        }

        if (data.JobSkills == null)
        {
            data.JobSkills = new Dictionary<string, List<string>>();
        }

        foreach (string job in DefaultBeneficiaries)
        {
            if (!data.JobXP.ContainsKey(job))
            {
                data.JobXP[job] = 0;
            }

            if (!data.JobLevels.ContainsKey(job))
            {
                data.JobLevels[job] = 1;
            }

            if (!data.JobSkills.ContainsKey(job))
            {
                data.JobSkills[job] = new List<string>(DefaultJobSkills[job]);
            }
        }

        if (string.IsNullOrEmpty(data.ActiveBeneficiary) || data.ActiveBeneficiary == "None")
        {
            data.ActiveBeneficiary = DefaultBeneficiaries[0];
        }

        if (string.IsNullOrEmpty(data.ActiveJob) || data.ActiveJob == "None")
        {
            data.ActiveJob = data.ActiveBeneficiary;
        }
    }

    public static int GainJobXP(SaveData data, string job, int xp)
    {
        InitializeJobData(data);

        if (!data.JobXP.ContainsKey(job))
        {
            data.JobXP[job] = 0;
            data.JobLevels[job] = 1;
            data.JobSkills[job] = DefaultJobSkills.ContainsKey(job)
                ? new List<string>(DefaultJobSkills[job])
                : new List<string>();
        }

        data.JobXP[job] += xp;
        int level = LevelForXP(data.JobXP[job]);
        if (level > data.JobLevels[job])
        {
            data.JobLevels[job] = level;
            AddJobSkillsForLevel(data, job, level);
        }

        return data.JobLevels[job];
    }

    public static int GetJobLevel(SaveData data, string job)
    {
        InitializeJobData(data);
        return data.JobLevels.ContainsKey(job) ? data.JobLevels[job] : 1;
    }

    public static bool CanCreateRon(SaveData data)
    {
        if (!data.IsRonCreated)
        {
            return true;
        }

        if (string.IsNullOrEmpty(data.RonCreationDate))
        {
            return true;
        }

        if (DateTime.TryParse(data.RonCreationDate, out DateTime created))
        {
            return (DateTime.UtcNow - created).TotalDays >= RonLockoutDays;
        }

        return true;
    }

    public static void RecordRonCreation(SaveData data)
    {
        data.IsRonCreated = true;
        data.RonCreationDate = DateTime.UtcNow.ToString("o");
    }

    private static int LevelForXP(int xp)
    {
        int level = 1;
        while (xp >= XPForLevel(level + 1))
        {
            level++;
        }
        return level;
    }

    private static int XPForLevel(int level)
    {
        return 100 * level * level;
    }

    private static void AddJobSkillsForLevel(SaveData data, string job, int level)
    {
        if (!data.JobSkills.ContainsKey(job))
        {
            data.JobSkills[job] = new List<string>();
        }

        if (level == 10)
        {
            data.JobSkills[job].Add("Tier 1 Mastery");
        }
        else if (level == 30)
        {
            data.JobSkills[job].Add("Tier 2 Mastery");
        }
        else if (level == 70)
        {
            data.JobSkills[job].Add("Tier 3 Mastery");
        }
    }
}
