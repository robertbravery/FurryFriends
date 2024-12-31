```mermaid
classDiagram
    class User {
        Guid Id
        string Name
        string Email
        PhoneNumber PhoneNumber
        Address Address
        string Description
        Photo BioPhoto
        List~Photo~ PetPhotos
        List~ServiceOffered~ ServicesOffered
        Schedule Availability
        List~ExperienceEntry~ Experience
        List~Certification~ Certifications
        List~Testimonial~ Testimonials
        List~string~ AreasServed
        InsuranceDetails InsuranceDetails
    }
    class Photo {
        string Url
        string? Caption
    }
    class PhoneNumber {
        string Value
    }
    class Address {
        string Street
        string City
        string Postcode
        string Country
    }
    class ServiceOffered {
        ServiceType Type
        decimal HourlyRate
    }
    class ServiceType {
        DogWalking
        PetSitting
        HomeVisits
        // ...
    }
    class Schedule {
        List~TimeSlot~ TimeSlots
    }
    class TimeSlot {
        DayOfWeek Day
        TimeRange Time
    }
    class TimeRange{
        TimeSpan Start
        TimeSpan End
    }
    class ExperienceEntry {
        string AnimalType
        int YearsOfExperience
        string Description
    }
    class Certification {
        string Name
        string IssuingOrganization
        DateTime? ExpiryDate
    }
    class Testimonial {
        string AuthorName
        string Content
        int? Rating
    }
    class InsuranceDetails {
        string Provider
        string PolicyNumber
        DateTime ExpiryDate
    }

    User "1" *-- "1" PhoneNumber
    User "1" *-- "1" Address
    User "1" *-- "1" BioPhoto
    User "1" *-- "*" PetPhotos
    User "1" *-- "*" ServiceOffered
    User "1" *-- "1" Schedule
    Schedule "1" *-- "*" TimeSlot
    TimeSlot "1" *-- "1" TimeRange
    User "1" *-- "*" ExperienceEntry
    User "1" *-- "*" Certification
    User "1" *-- "*" Testimonial
    User "1" *-- "1" InsuranceDetails
    ServiceOffered "*" -- "1" ServiceType
    ```