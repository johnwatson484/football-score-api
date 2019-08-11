using Microsoft.EntityFrameworkCore;

namespace FootballScoreAPI.Data
{
    public class DbInitializer
    {
        public static void Initialize(FootballScoreContext context)
        {
            context.Database.Migrate();
        }
    }
}
