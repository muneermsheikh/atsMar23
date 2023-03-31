namespace core.Entities.MasterEntities
{
     public class Category: BaseEntity
    {
            public Category(int id, string name)
          {
               Id = id;
               Name = name;
          }
          public Category(string name)
          {
               Name = name;
          }

          public Category()
          {
          }

          public string Name { get; set; }
    }
}