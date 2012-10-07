using Dazzle.Storage;

namespace Dazzle.Database.Tests
{
    public abstract class with_database_having_2_rows : SpecificationContext
    {
        public override void Given()
        {
            this.db.ExecuteQuery("update users set firstname = 'jane', lastname = 'tompson' where key = 1"); 
            this.db.ExecuteQuery("update users set firstname = 'bob', lastname = 'smith' where key = 2");
        }
    }
}