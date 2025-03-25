namespace PATINHAS_RFID_API.Util
{
    public static class SqlUtil
    {
        public static string GetStringTratadaWhere(string? dados)
        {
            if (string.IsNullOrWhiteSpace(dados))
                return "";

            return "%" + dados.ToLower().Replace(" ", "%") + "%";
        }
    }
}
