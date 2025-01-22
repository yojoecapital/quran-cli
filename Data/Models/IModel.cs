namespace QuranCli.Data.Models
{
    public interface IModel
    {
        public static abstract void CreateTable();
        public void Insert();

    }
}