﻿using Ardalis.SharedKernel;
using FurryFriends.UseCases.Services.PictureService;
using Microsoft.AspNetCore.Http;
using System;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePhoto;

public class UpdatePhotoCommand : ICommand<Result<DetailedPhotoDto>>
{
  public required Guid PetWalkerId { get; set; }
  public required Guid PhotoId { get; set; }
  public required IFormFile File { get; set; }
  public string? Description { get; set; }
}
