using FurryFriends.UseCases.Services.PictureService;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetPicture;

public record GetPetWalkerPhotoQuery(Guid PetWalkerId) : IQuery<Result<DetailPictureDto>>;
