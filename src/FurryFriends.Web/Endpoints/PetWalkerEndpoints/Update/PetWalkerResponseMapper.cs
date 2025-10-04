using Ardalis.Result;
using FastEndpoints;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePetWalker;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Update;

public class PetWalkerResponseMapper : IMapper
{
    public UpdatePetWalkerResponse FromEntity(PetWalker petWalker)
    {
        return new UpdatePetWalkerResponse
        {
            Id = petWalker.Id
        };
    }
}
