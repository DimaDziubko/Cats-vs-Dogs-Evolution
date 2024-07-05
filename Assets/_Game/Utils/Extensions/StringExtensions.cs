namespace Assets._Game.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string FixJsonArrays(this string jsonData)
        {
            return jsonData.Replace("\"*[", "[").Replace("]*\"", "]");
        }
        
    }
}