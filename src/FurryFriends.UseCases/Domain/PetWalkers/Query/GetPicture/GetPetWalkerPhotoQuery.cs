using FurryFriends.UseCases.Services.PictureService;
using Mediator;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetPicture;

public record GetPetWalkerPhotoQuery(Guid PetWalkerId) : IQuery<Result<DetailPictureDto>>;
