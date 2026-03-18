using FurryFriends.Core.TimeslotAggregate;

namespace FurryFriends.Core.Entities;

// This file provides type aliases to make TimeslotAggregate entities 
// available in the Entities namespace for backward compatibility
public class Timeslot : Core.TimeslotAggregate.Timeslot { }
public class WorkingHours : Core.TimeslotAggregate.WorkingHours { }
public class TravelBuffer : Core.TimeslotAggregate.TravelBuffer { }
public class CustomTimeRequest : Core.TimeslotAggregate.CustomTimeRequest { }