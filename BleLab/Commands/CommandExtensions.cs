using System.Threading.Tasks;

namespace BleLab.Commands
{
    public static class CommandExtensions
    {
        public static async Task<T> GetTask<T>(this T command) where T : CommandBase
        {
            var task = command.GetStatusAsync();
            await task.ConfigureAwait(false);
            return command;
        }
    }
}