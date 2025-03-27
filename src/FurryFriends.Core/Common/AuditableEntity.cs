﻿﻿﻿﻿﻿﻿﻿﻿﻿namespace FurryFriends.Core.Common;

public abstract class AuditableEntity<T>: EntityBase, Ardalis.SharedKernel.IAggregateRoot
{
  public new T Id { get; set; } = default!;
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  protected AuditableEntity()
  {
    CreatedAt = DateTime.Now;
    UpdatedAt = DateTime.Now;
  }
}
