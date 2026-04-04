
namespace BackupEditorN.Services;

public class CronValidator
{
    private static readonly (int Min, int Max)[] ranges =
    {

        (0, 59), //minuta
        (0, 23), //hodina
        (1, 31), //den
        (1, 12), //mesic
        (1, 7)  //den v tydnu

    };
    public bool IsValid(string cronString)
    {
        if (string.IsNullOrWhiteSpace(cronString)) return false;
        string[] parts = cronString.Trim().Split(' ');
        
        if (parts.Length != 5) return false;
        
        for (int i = 0; i < 5; i++)
        {
            if (parts[i] == "*") continue;

            if (parts[i].Contains("-"))
            {
                string[] range = parts[i].Split('-');
                if (range.Length != 2) return false;
                if (!int.TryParse(range[0], out int from)) return false;
                if (!int.TryParse(range[1], out int to)) return false;
                if (from < ranges[i].Min || to > ranges[i].Max) return false;
            }

            else
            {
                if (!int.TryParse(parts[i], out int value)) return false;
            }
        }
        
        return true;
    }

    // public string? GetNextOccurence(string cronString)
    // {
    //     if (!IsValid(cronString)) return null;
    //
    //     string[] parts = cronString.Trim().Split(' ');
    //     DateTime dt = DateTime.Now.AddMinutes(1);
    //     dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
    //
    //     for (int i = 0; i < 60 * 24 * 366; i++ , dt = dt.AddMinutes(1))
    //
    // }
}