namespace FurryFriends.Core.ClientAggregate;
public class Breed
{

  public int Id { get; set; }
  public string Name { get; set; } = default!;
  public string Description { get; set; } = default!;
  public int SpeciesId { get; set; }
  public virtual Species Species { get; set; } = default!;
  public virtual ICollection<Pet> Pets { get; set; } = default!;

  private Breed() { }

  private Breed(string name, string description)
  {
    Name = name;
    Description = description;
  }

  public static Breed Create(string name, string description)
  {
    return new Breed(name, description);
  }
}
