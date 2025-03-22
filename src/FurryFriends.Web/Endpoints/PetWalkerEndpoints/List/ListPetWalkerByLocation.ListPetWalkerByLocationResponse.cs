using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public class ListPetWalkerByLocationResponse(
  List<PetWalkerListResponseDto> rowsData,
  int pageNumber,
  int pageSize,
  int totalCount, string[] hideColumns) : ListResponse<PetWalkerListResponseDto>(rowsData, pageNumber, pageSize, totalCount, hideColumns)
{

}
