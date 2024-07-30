using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Interactors.Helpers
{
    public class ConfigValidHelper
    {
        public static bool ValidateConfigurationJson(string pathSchema, string pathConfig)
        {
            JSchema schema = JSchema.Parse(File.ReadAllText(pathSchema));
            JObject jsonObject = JObject.Parse(File.ReadAllText(pathConfig));
            if (!jsonObject.IsValid(schema))
            {
                Log.Warning("The configuration file was not found or it is incorrect. The default configuration is used.");
                return false;
            }
            return true;
        }
    }


}