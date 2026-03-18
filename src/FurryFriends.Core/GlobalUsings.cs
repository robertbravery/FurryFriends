global using Ardalis.GuardClauses;
global using Ardalis.Result;
global using Ardalis.SharedKernel;
global using Ardalis.SmartEnum;
global using Ardalis.Specification;
global using MediatR;
global using Microsoft.Extensions.Logging;
global using FurryFriends.Core.Extensions;

// Re-export TimeslotAggregate types for backward compatibility
global using Timeslot = FurryFriends.Core.TimeslotAggregate.Timeslot;
global using WorkingHours = FurryFriends.Core.TimeslotAggregate.WorkingHours;
global using TravelBuffer = FurryFriends.Core.TimeslotAggregate.TravelBuffer;
global using CustomTimeRequest = FurryFriends.Core.TimeslotAggregate.CustomTimeRequest;
