namespace PATINHAS_RFID_API.Util
{
    public static class HandleException
    {
        public static void FkException(Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException.ToString().Contains("violates foreign key constraint"))
                throw new ValidacaoException("Não é possivel excluir o item selecionado, pois o mesmo contém vinculo com outros itens!");

            throw ex;
        }
    }
}
