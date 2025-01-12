namespace FurryFriends.Core.ClientAggregate;

public class Species
{

  public int Id { get; set; }
  public string Name { get; set; } = default!;
  public string Description { get; set; } = default!;

  public virtual ICollection<Breed> Breeds { get; set; } = default!;

  private Species()  {  }

  private Species(string name, string description)
  {
    Name = name;
    Description = description;
  }

  public static Species Create(string name, string description)
  {
    return new Species(name, description);
  }
}
