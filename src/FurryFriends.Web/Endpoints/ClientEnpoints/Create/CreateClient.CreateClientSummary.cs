using FurryFriends.Web.Endpoints.ClientEnpoints.Create;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Create;

public class CreateClientSummary : Summary<CreateClient>
{
    public CreateClientSummary()
    {
        Summary = "Creates a new client";
        Description = """
        Creates a new client with the provided details.<br/><br/>
        <b>Client Type:</b><br/>
        • Regular (1)<br/>
        • Premium (2)<br/>
        • Corporate (3)<br/><br/>
        <b>Referral Source:</b><br/>
        • None (0)<br/>
        • Website (1)<br/>
        • ExistingClient (2)<br/>
        • SocialMedia (3)<br/>
        • SearchEngine (4)<br/>
        • Veterinarian (5)<br/>
        • LocalAdvertising (6)<br/>
        • Other (7)
        """;
        
        Response<CreateClientReponse>(200, "ok response with body");
        Response<ErrorResponse>(400, "validation failure");
        Response(404, "account not found");
    }
}