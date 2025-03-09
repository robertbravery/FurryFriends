﻿using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Get;

public class GetClientByEmailResponse(ClientRecord? data, bool success = true, string message = "Success", List<string>? errors = null)
  : ResponseBase<ClientRecord>(data, success, message, errors)
{
}
