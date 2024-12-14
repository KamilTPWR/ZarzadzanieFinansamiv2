namespace ZarzadzanieFinansami;

public static class Core
{
    public static int NumberOfRows = Constants.NUMBEROFROWS;
    public static int Page = 1;
    public static int PagesNumber()
    {
        return (int)Math.Ceiling((double)DbUtility.GetNumberOfTransactions() / NumberOfRows);
    }
}