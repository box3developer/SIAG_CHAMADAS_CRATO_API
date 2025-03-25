namespace PATINHAS_RFID_API.utils
{
    public static class ListaUtils
    {
        public static bool TemItens<T>(this List<T> lista)
        {
            if (lista == null || lista.Count == 0)
                return false;

            return true;
        }
    }
}
